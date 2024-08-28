using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FlowOwnershipAudit.Model;
using Parallel = System.Threading.Tasks.Parallel;
using System.Text;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Activities.Statements;

namespace FlowOwnershipAudit
{
    public static class API
    {
        static IPublicClientApplication app = null;

        public enum AuthType
        {
            PowerApps,
            Flow,
            Graph,
            Dataverse
        }

        #region Authentication
        public static async Task<AuthenticationResult> AuthenticateAsync(AuthType authType, string dataverseURI = "")
        {
            string[] scopes = null;

            switch (authType)
            {
                case AuthType.PowerApps:
                    scopes = new[] { "https://service.powerapps.com//.default" };
                    break;
                case AuthType.Flow:
                    scopes = new[] { "https://service.flow.microsoft.com//.default" };
                    break;
                case AuthType.Graph:
                    scopes = new[] { "https://graph.windows.net//.default" };
                    break;
                case AuthType.Dataverse:
                    scopes = new[] { $"{dataverseURI}/.default" };
                    break;
                default:
                    return null;
            }
            if (app == null)
            {
                app = PublicClientApplicationBuilder.Create("1950a258-227b-4e31-a9cf-717495945fc2")
                .WithDefaultRedirectUri()
                .Build();
            }
            var accounts = await app.GetAccountsAsync();
            AuthenticationResult result;
            try
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                // Handle the exception, e.g., prompt the user to sign in
                result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }

            return result;
        }
        #endregion

        #region Graph calls
        public static async Task<string> GetUserIdFromGraph(string user)
        {
            string graphApiVersion = "1.6";
            string filter = $"startswith(userPrincipalName,'{user}') or startswith(displayName,'{user}')";

            var auth = await AuthenticateAsync(AuthType.Graph);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                string url = $"https://graph.windows.net/myorganization/users?$filter={filter}&api-version={graphApiVersion}";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dynamic users = JsonConvert.DeserializeObject(responseContent);
                    string objectId = users.value[0].objectId;
                    return objectId;
                }
                else
                {
                    // Handle the error here
                    //LogInfo($"An error occurred: {response.StatusCode}", new object[] { });
                    return null;
                }
            }
        }
        #endregion

        #region PowerApps calls
        public static async Task<EnvironmentList> GetAllEnvironmentsInTenantAsync(Action<ProgressChangedEventArgs> ProgressChanged)
        {
            string apiversion = "2016-11-01";
            EnvironmentList environmentsList = new EnvironmentList();

            var auth = await AuthenticateAsync(AuthType.PowerApps);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                string url = "https://api.bap.microsoft.com/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments?api-version=" + apiversion;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    environmentsList = JsonConvert.DeserializeObject<EnvironmentList>(responseContent);
                }
                else
                {
                    // Handle the error here
                }
            }
            foreach (var environment in environmentsList.value)
            {
                var returnObj = new
                {
                    FlowName = string.Empty,
                    FlowId = string.Empty,
                    EnvironmentId = environment.name,
                    EnvironmentName = environment.properties.displayName,
                };

                ProgressChanged(new ProgressChangedEventArgs(10, returnObj));
            }

            return environmentsList;
        }

        public static void AddConnectionReferencesToEnvironment(string userId, Model.Environment targetEnvironment, Action<object> ProgressChanged)
        {
            string powerAppsEndpoint = "https://api.powerapps.com";
            string apiVersion = "2016-11-01";

            ConnectionReferencesList connectionReferencesList = new ConnectionReferencesList();

            var auth = AuthenticateAsync(AuthType.PowerApps).Result;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                //string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/scopes/admin/environments/{targetEnvironment.name}/v2/flows?api-version={apiVersion}";
                string url = $"{powerAppsEndpoint}/providers/Microsoft.PowerApps/scopes/admin/environments/{targetEnvironment.name}/connections?api-version={apiVersion}";
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    connectionReferencesList = JsonConvert.DeserializeObject<ConnectionReferencesList>(responseContent);
                }
                else
                {
                    // Handle the error here
                }
            }

            //Report to UI
            foreach (var connectionReference in connectionReferencesList.value)
            {
                if (connectionReference.properties.createdBy.id == userId)
                {
                    var returnObj = new
                    {
                        ConnectionReferenceName = connectionReference.name,
                        ConnectionReferenceId = connectionReference.name,
                        EnvironmentId = targetEnvironment.name,
                        EnvironmentName = targetEnvironment.properties.displayName,
                    };

                    ProgressChanged(returnObj);
                }
            }

            // attach flowlist to the current targetenvironment
            targetEnvironment.connectionReferences = connectionReferencesList.value;
        }
        #endregion

        #region Flow Calls
        /// <summary>
        /// Works through the targetEnvironment and adds the flow permissions under the list of flows in the property Environment.Flows
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetEnvironment"></param>
        /// <param name="accesstoken"></param>
        /// <param name="ProgressChanged"></param>
        /// <returns></returns>
        public static void AddFlowPermissionsToEnvironment(string userId, Model.Environment targetEnvironment, Action<object> ProgressChanged)
        {
            string flowEndpoint = "https://api.flow.microsoft.com";
            string apiVersion = "2016-11-01";

            if (targetEnvironment.flows != null)
            {
                Parallel.ForEach(
                    source: targetEnvironment.flows,
                    parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 5 },
                    body: async flow =>
                    {
                        //var auth = AuthenticateAsync(AuthType.Flow).Result;

                        //FlowPermissionList flowPermissionList = new FlowPermissionList();

                        //// Call the API for each environment
                        //using (HttpClient client = new HttpClient())
                        //{
                        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                        //    string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/environments/{targetEnvironment.name}/flows/{flow.name}/permissions?api-version={apiVersion}";
                        //    HttpResponseMessage response = client.GetAsync(url).Result;
                        //    if (response.IsSuccessStatusCode)
                        //    {
                        //        string responseContent = response.Content.ReadAsStringAsync().Result;
                        //        flowPermissionList = JsonConvert.DeserializeObject<FlowPermissionList>(responseContent);
                        //        flow.permissions = flowPermissionList.value;
                        //    }
                        //    else
                        //    {
                        //        // Handle the error here
                        //    }
                        //}
                        GetFlowPermissons(flow);

                        if (flow.permissions != null)
                        {
                            foreach (var permission in flow.permissions)
                            {
                                if (permission.properties.roleName == "Owner" &&
                                                permission.properties.principal.id == userId)
                                {
                                    ProgressChanged(flow);
                                }
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// Adds the flows to the environment object
        /// </summary>
        /// <param name="targetEnvironment"></param>
        public static void AddFlowsToEnvironment(Model.Environment targetEnvironment)
        {
            string flowEndpoint = "https://api.flow.microsoft.com";
            string apiVersion = "2016-11-01";

            FlowList flowList = new FlowList();

            var auth = AuthenticateAsync(AuthType.Flow).Result;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/scopes/admin/environments/{targetEnvironment.name}/v2/flows?api-version={apiVersion}";
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    flowList = JsonConvert.DeserializeObject<FlowList>(responseContent);
                }
                else
                {
                    // Handle the error here
                }
            }

            // attach flowlist to the current targetenvironment
            targetEnvironment.flows = flowList.value;
        }

        public static void GetFlowPermissons(Flow flow)
        {
            string flowEndpoint = "https://api.flow.microsoft.com";
            string apiVersion = "2016-11-01";

            var auth = AuthenticateAsync(AuthType.Flow).Result;

            FlowPermissionList flowPermissionList = new FlowPermissionList();

            // Call the API for each environment
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                //string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/environments/{targetEnvironment.name}/flows/{flow.name}/permissions?api-version={apiVersion}";
                string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/environments/{flow.properties.environment.name}/flows/{flow.name}/permissions?api-version={apiVersion}";
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    flowPermissionList = JsonConvert.DeserializeObject<FlowPermissionList>(responseContent);
                    flow.permissions = flowPermissionList.value;
                }
                else
                {
                    // Handle the error here
                }
            }
        }

        public static void GetFlowDetails(Flow flow)
        {
            //"https://{flowEndpoint}/providers/Microsoft.ProcessSimple/scopes/admin/environments/{environment}/flows/{flowName}?api-version={apiVersion}"
            string flowEndpoint = "https://api.flow.microsoft.com";
            string apiVersion = "2016-11-01";

            var auth = AuthenticateAsync(AuthType.Flow).Result;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/scopes/admin/environments/{flow.properties.environment.name}/flows/{flow.name}?api-version={apiVersion}";
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    flow = JsonConvert.DeserializeObject<Flow>(responseContent);
                }
                else
                {
                    // Handle the error here
                }
            }
        }

        #endregion

        #region Dataverse Calls
        /// <summary>
        /// This method provides a way to grant access to a workflow in the Dataverse environment using the provided owner ID and workflow ID.
        /// </summary>
        /// <param name="environmentUrl"></param>
        /// <param name="ownerId"></param>
        /// <param name="workflowId"></param>
        /// <returns></returns>
        public static void GrantAccessAsync(string environmentUrl, string workflowId, string ownerId)
        {
            // Authenticate async against dataverse with an environmenturl
            var auth = AuthenticateAsync(AuthType.Dataverse, environmentUrl).Result;

            // Execute a grantaccessrequest over the rest api of the dataverse instance
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                string url = $"{environmentUrl}/api/data/v9.2/GrantAccess";

                // Create the request body
                var requestBody = new
                {
                    Target = new Dictionary<string, object>
                    {
                        { "workflowid", workflowId },
                        { "@odata.type", "Microsoft.Dynamics.CRM.workflow" }
                    },
                    PrincipalAccess = new
                    {
                        Principal = new Dictionary<string, object>
                        {
                            { "ownerid", ownerId },
                            { "@odata.type", "Microsoft.Dynamics.CRM.systemuser" }
                        },

                        AccessMask = "ReadAccess,WriteAccess,AppendAccess,AppendToAccess,CreateAccess,DeleteAccess,ShareAccess,AssignAccess"
                    }
                };

                // Convert the request body to JSON
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send the grantaccessrequest
                HttpResponseMessage response = client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    // Access granted successfully
                }
                else
                {
                    // Handle the error here
                    string trace = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"Error granting access: {response.ReasonPhrase} {trace}");
                }
            }
        }

        /// <summary>
        /// Sets the owner of a workflow in the Dataverse environment.
        /// </summary>
        /// <param name="workflowId">The ID of the workflow.</param>
        /// <param name="targetOwnerId">The ID of the owner.</param>
        /// <param name="environmentUrl">The URL of the Dataverse environment.</param>
        public static void SetWorkflowOwnerAsync(string environmentUrl, string workflowId, string targetOwnerId)
        {
            var auth = AuthenticateAsync(AuthType.Dataverse, environmentUrl).Result;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(environmentUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                var method = "PATCH";
                var httpVerb = new HttpMethod(method);

                var requestBody = new Dictionary<string, object>
                        {
                            { "ownerid@odata.bind", $"/systemusers({targetOwnerId})" }
                        };

                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var httpRequestMessage =
                    new HttpRequestMessage(httpVerb, $"/api/data/v9.1/workflows({workflowId})")
                    {
                        Content = content
                    };

                var response = client.SendAsync(httpRequestMessage).Result;
                if (response.IsSuccessStatusCode)
                {
                    // Owner set successfully
                }
                else
                {
                    // Handle the error here
                    Console.WriteLine($"Error setting owner: {response.ReasonPhrase}");
                }
            }
        }

        /// <summary>
        /// Retrieves the system user ID from Dataverse based on the provided domain name.
        /// </summary>
        /// <param name="environmentUrl">The URL of the Dataverse environment.</param>
        /// <param name="domainname">The domain name of the user.</param>
        /// <returns>The system user ID if found, otherwise String.Empty</returns>
        public static string GetSystemUserIdFromDataverse(string environmentUrl, string domainname)
        {
            try
            {
                var auth = AuthenticateAsync(AuthType.Dataverse, environmentUrl).Result;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                    string url = $"{environmentUrl}/api/data/v9.1/systemusers?$filter=domainname eq '{domainname}' or internalemailaddress eq '{domainname}'&$select=systemuserid";

                    HttpResponseMessage response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        dynamic users = JsonConvert.DeserializeObject(responseContent);
                        if (users.value.Count > 0)
                        {
                            string systemUserId = users.value[0].systemuserid;
                            return systemUserId;
                        }
                        else
                        {
                            // Handle the case where no users are found
                            return string.Empty;
                        }
                    }
                    else
                    {
                        // Handle the error here
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
                throw;
            }
        }
        #endregion
    }
}
