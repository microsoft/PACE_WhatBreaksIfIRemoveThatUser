﻿using Microsoft.Identity.Client;
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
using System.Windows.Interop;
using Newtonsoft.Json.Linq;
using static ScintillaNET.Style;
using Microsoft.Xrm.Sdk.Workflow.Activities;
using System.Dynamic;

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

        public static void AddConnectionsToEnvironment(string userId, Model.Environment targetEnvironment, Action<object> ProgressChanged)
        {
            string powerAppsEndpoint = "https://api.powerapps.com";
            string apiVersion = "2016-11-01";

            ConnectionList connectionList = new ConnectionList();

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
                    connectionList = JsonConvert.DeserializeObject<ConnectionList>(responseContent);
                }
                else
                {
                    // Handle the error here
                }
            }

            //Report to UI
            foreach (var connection in connectionList.value)
            {
                if (connection.properties.createdBy.id == userId)
                {
                    connection.isOwnedByX = true;

                    var returnObj = new
                    {
                        ConnectionName = connection.name,
                        ConnectionId = connection.name,
                        EnvironmentId = targetEnvironment.name,
                        EnvironmentName = targetEnvironment.properties.displayName,
                    };

                    ProgressChanged(returnObj);
                }
            }

            // attach flowlist to the current targetenvironment
            targetEnvironment.connections = connectionList.value;
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
            if (targetEnvironment.flows != null)
            {
                Parallel.ForEach(
                    source: targetEnvironment.flows,
                    parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 5 },
                    body: async flow =>
                    {
                        GetFlowPermissons(flow);

                        if (flow.permissions != null)
                        {
                            foreach (var permission in flow.permissions)
                            {
                                if (permission.properties.roleName == "Owner" &&
                                                permission.properties.principal.id == userId)
                                {
                                    flow.isOwnedByX = true;
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

        public static void GetFlowDetails(string userId, Model.Environment targetEnvironment, Flow flow, Action<object> ProgressChanged = null)
        {
            string flowEndpoint = "https://api.flow.microsoft.com";
            string apiVersion = "2016-11-01";

            var auth = AuthenticateAsync(AuthType.Flow).Result;

            //Get the Flow object that we want to update
            Flow flowToUpdate = targetEnvironment.flows.Single(x => x.name == flow.name);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

                string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/scopes/admin/environments/{flow.properties.environment.name}/flows/{flow.name}?api-version={apiVersion}";
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var flowDynamic = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    flowToUpdate.properties.connectionReferences = new List<ConnectionReference>();

                    foreach (var connectionReferences in flowDynamic.properties.connectionReferences.Children())
                    {
                        foreach (dynamic item in connectionReferences.Children())
                        {
                            if (HasProperty(item, "connectionName"))
                            {
                                dynamic connectionReferenceFromDataverse = GetConnectionReferenceFromDataverse(targetEnvironment.properties.linkedEnvironmentMetadata.instanceUrl, item.connectionName.ToString());

                                ConnectionReference connectionReference = new ConnectionReference()
                                {
                                    connectionName = item.connectionName,
                                    connectionReferenceLogicalName = item.connectionReferenceLogicalName,
                                    displayName = item.displayName,
                                    tier = item.tier,
                                    isOwnedByX = HasProperty(connectionReferenceFromDataverse, "_owninguser_value") ? connectionReferenceFromDataverse._owninguser_value == userId : false,
                                    connectionReferenceId = HasProperty(connectionReferenceFromDataverse, "_owninguser_value") ? connectionReferenceFromDataverse.connectionreferenceid : "",
                                };

                                flowToUpdate.properties.connectionReferences.Add(connectionReference);
                            }
                        }
                    }

                    if (ProgressChanged != null)
                    {
                        ProgressChanged(flowToUpdate);
                    }
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
        public static bool GrantAccess(string environmentUrl, string workflowId, string ownerId)
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
                    return true;
                }
                else
                {
                    // Handle the error here
                    string trace = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"Error granting access: {response.ReasonPhrase} {trace}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Sets the owner of a workflow in the Dataverse environment.
        /// </summary>
        /// <param name="workflowId">The ID of the workflow.</param>
        /// <param name="targetOwnerId">The ID of the owner.</param>
        /// <param name="environmentUrl">The URL of the Dataverse environment.</param>
        public static bool SetWorkflowOwner(string environmentUrl, string workflowId, string targetOwnerId)
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
                    return true;
                }
                else
                {
                    // Handle the error here
                    Console.WriteLine($"Error setting owner: {response.ReasonPhrase}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Retrieves the system user ID from Dataverse based on the provided domain name.
        /// </summary>
        /// <param name="environmentUrl">The URL of the Dataverse environment.</param>
        /// <param name="userIdentifier">The domainname, internalemailaddress or azureactivedirectoryobjectid of the user.</param>
        /// <returns>The system user ID if found, otherwise String.Empty</returns>
        public static string GetSystemUserIdFromDataverse(string environmentUrl, string userIdentifier)
        {
            try
            {
                var auth = AuthenticateAsync(AuthType.Dataverse, environmentUrl).Result;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

                    // Check if userIdentifier is a GUID
                    bool isGuid = Guid.TryParse(userIdentifier, out Guid userGuid);

                    string url;
                    if (isGuid)
                    {
                        url = $"{environmentUrl}/api/data/v9.1/systemusers?$filter=azureactivedirectoryobjectid eq '{userGuid}'&$select=systemuserid";
                    }
                    else
                    {
                        url = $"{environmentUrl}/api/data/v9.1/systemusers?$filter=domainname eq '{userIdentifier}' or internalemailaddress eq '{userIdentifier}'&$select=systemuserid";
                    }

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

        /// <summary>
        /// Sets the owner of a workflow in the Dataverse environment.
        /// </summary>
        /// <param name="connectionReferenceId">The ID of the workflow.</param>
        /// <param name="targetOwnerId">The ID of the owner.</param>
        /// <param name="environmentUrl">The URL of the Dataverse environment.</param>
        public static object GetConnectionReferenceFromDataverse(string environmentUrl, string connectionId)
        {
            try
            {
                var auth = AuthenticateAsync(AuthType.Dataverse, environmentUrl).Result;

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(environmentUrl);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

                    //Get Connection Reference from Dataverse based on the connectionreferencelogicalname
                    dynamic connectionReference = new Object();
                    var url = $"/api/data/v9.1/connectionreferences?$filter=connectionid eq '{connectionId}'";
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        dynamic connectionReferences = JsonConvert.DeserializeObject(responseContent);
                        if (connectionReferences.value.Count > 0)
                        {
                            connectionReference = connectionReferences.value[0];
                            return connectionReference;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }

            return "";
        }

        /// <summary>
        /// Sets the owner of a workflow in the Dataverse environment.
        /// </summary>
        /// <param name="connectionReferenceId">The ID of the workflow.</param>
        /// <param name="targetOwnerId">The ID of the owner.</param>
        /// <param name="environmentUrl">The URL of the Dataverse environment.</param>
        public static bool SetConnectionReferenceOwner(string environmentUrl, ConnectionReference connectionReference, string targetOwner)
        {
            var auth = AuthenticateAsync(AuthType.Dataverse, environmentUrl).Result;

            string targetOwnerId = GetSystemUserIdFromDataverse(environmentUrl, targetOwner);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(environmentUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

                //dynamic connectionReferenceDetails = GetConnectionReferenceOwner(environmentUrl, connectionReferenceLogicalName);
                if (connectionReference.isOwnedByX)
                {
                    string apiUrl = $"{environmentUrl}/api/data/v9.1/Assign";

                    // Create the request body
                    var assignRequest = new
                    {
                        Target = new Dictionary<string, object>
                        {
                            //{ "connectionreferenceid", connectionReference.connectionreferenceid },
                            { "connectionreferenceid", connectionReference.connectionReferenceId },
                            { "@odata.type", "Microsoft.Dynamics.CRM.connectionreference" }
                        },
                        Assignee = new Dictionary<string, object>
                        {
                            { "systemuserid", targetOwnerId },
                            { "@odata.type", "Microsoft.Dynamics.CRM.systemuser" }
                        }
                    };
                    var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(assignRequest), Encoding.UTF8, "application/json");
                    var response = client.PostAsync(apiUrl, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // Owner set successfully
                        return true;
                    }
                    else
                    {
                        // Handle the error here
                        Console.WriteLine($"Error setting owner: {response.ReasonPhrase}");
                        return false;
                    }
                }

                return true;
            }
        }

        #endregion

        #region Helper
        public static bool HasProperty(dynamic obj, string name)
        {
            System.Type objType = obj.GetType();

            if (objType == typeof(ExpandoObject))
            {
                return ((IDictionary<string, object>)obj).ContainsKey(name);
            }

            if (objType == typeof(JObject))
            {
                return ((JObject)obj).ContainsKey(name);
            }

            return objType.GetProperty(name) != null;
        }

        #endregion


    }
}
