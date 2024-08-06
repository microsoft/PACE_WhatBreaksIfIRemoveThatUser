using Microsoft.Identity.Client;
using Newtonsoft.Json;
using NuGet.Protocol;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WhatBreaksIf.Model;

namespace WhatBreaksIf
{

    //Get-AdminFlowOwnerRole
    //"https://{flowEndpoint}/providers/Microsoft.ProcessSimple/scopes/admin/environments/{environment}/flows/{flowName}/permissions?api-version={apiVersion}" `

    //Get Environment (by name)
    //"https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{environmentName}?`$$expandParameter&api-version={apiVersion}"

    //Get Environments (all environments)
    //"https://{bapEndpoint}/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments?`$$expandParameter&api-version={apiVersion}"

    //Get-AdminFlow (by environment, name)
    //"https://{flowEndpoint}/providers/Microsoft.ProcessSimple/scopes/admin/environments/{environment}/flows/{flowName}?api-version={apiVersion}&`$top={top}$($includeDeletedParam)"

    //Get-AdminFlows (by environment)
    //"https://{flowEndpoint}/providers/Microsoft.ProcessSimple/scopes/admin/environments/{environment}/v2/flows?api-version={apiVersion}&`$top={top}$($includeDeletedParam)"

    //Get-UsersOrGroupsFromGraph
    //$GraphApiVersion = "1.6"
    //"https://graph.windows.net/myorganization/users?filter={filter}&api-version={graphApiVersion}"
    //"startswith(userPrincipalName,'$SearchString') or startswith(displayName,'$SearchString')"

    public static class API
    {
        static IPublicClientApplication app = null;

        public enum AuthType
        {
            PowerApps,
            Flow,
            Graph
        }

        #region Authentication
        public static async Task<AuthenticationResult> AuthenticateAsync(AuthType authType)
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
                foreach (var flow in targetEnvironment.flows)
                {
                    var auth = AuthenticateAsync(AuthType.Flow).Result;

                    FlowPermissionList flowPermissionList = new FlowPermissionList();

                    // Call the API for each environment
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                        string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/environments/{targetEnvironment.name}/flows/{flow.name}/permissions?api-version={apiVersion}";
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

                    if (flow.permissions != null)
                    {
                        foreach (var permission in flow.permissions)
                        {
                            if (permission.properties.roleName == "Owner" &&
                                            permission.properties.principal.id == userId)
                            {
                                var returnObj = new
                                {
                                    FlowName = flow.properties.displayName,
                                    FlowId = flow.name,
                                    EnvironmentId = targetEnvironment.name,
                                    EnvironmentName = targetEnvironment.properties.displayName,
                                };

                                ProgressChanged(returnObj);
                            }
                        }
                    }
                }
            }
        }

        public static void AddFlowsToEnvironment(Model.Environment targetEnvironment)
        {
            string flowEndpoint = "https://api.flow.microsoft.com";
            string apiVersion = "2016-11-01";

            FlowList flowList = new FlowList();

            var auth =  AuthenticateAsync(AuthType.Flow).Result;

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
        #endregion

        public static void AddConnectionReferencesToEnvironment(string userId, Model.Environment targetEnvironment, Action<ProgressChangedEventArgs> ProgressChanged)
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

                    ProgressChanged(new ProgressChangedEventArgs(70, returnObj));
                }
            }

            // attach flowlist to the current targetenvironment
            targetEnvironment.connectionReferences = connectionReferencesList.value;
        }
    }
}
