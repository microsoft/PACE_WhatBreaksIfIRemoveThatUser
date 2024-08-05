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
        public static async Task<string> GetUserIdFromGraph(string accesstoken, string user, Action<string, object[]> LogInfo)
        {
            string graphApiVersion = "1.6";
            string filter = $"startswith(userPrincipalName,'{user}') or startswith(displayName,'{user}')";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
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
                    LogInfo($"An error occurred: {response.StatusCode}", new object[] { });
                    return null;
                }
            }
        }
        #endregion

        #region PowerApps calls
        public static async Task<EnvironmentList> GetAllEnvironmentsInTenantAsync(string accesstoken, Action<ProgressChangedEventArgs> ProgressChanged)
        {
            string apiversion = "2016-11-01";
            EnvironmentList environmentsList = new EnvironmentList();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
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
        public static async Task<EnvironmentList> GetAllFlowPermissions(string userId, string targetEnvironmentId, EnvironmentList environmentList, string accesstoken, Action<ProgressChangedEventArgs> ProgressChanged)
        {
            string flowEndpoint = "https://api.flow.microsoft.com";
            string apiVersion = "2016-11-01";

            await Task.WhenAll(environmentList.value.Select(async environment =>
            {
                foreach (var flow in environment.flows)
                {
                    FlowPermissionList flowPermissionList = new FlowPermissionList();

                    // Call the API for each environment
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
                        string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/environments/{environment.name}/flows/{flow.name}/permissions?api-version={apiVersion}";
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            flowPermissionList = JsonConvert.DeserializeObject<FlowPermissionList>(responseContent);
                            flow.permissions = flowPermissionList.value;
                        }
                        else
                        {
                            // Handle the error here
                        }
                    }

                    //Report owner found to the UI?
                    //TODO - WIP
                    foreach (var permission in flow.permissions)
                    {
                        if (permission.properties.roleName == "Owner" &&
                                        permission.properties.principal.id == userId)
                        {
                            var returnObj = new
                            {
                                FlowName = flow.properties.displayName,
                                FlowId = flow.name,
                                EnvironmentId = environment.name,
                                EnvironmentName = environment.properties.displayName,
                            };

                            ProgressChanged(new ProgressChangedEventArgs(70, returnObj));
                        }
                    }
                }
            }));

            return environmentList;
        }

        public static async Task<EnvironmentList> GetAllFlows(string userId, string targetEnvironmentId, EnvironmentList environments, string accesstoken)
        {
            string flowEndpoint = "https://api.flow.microsoft.com";
            string apiVersion = "2016-11-01";

            foreach (var environment in environments.value)
            {
                FlowList flowList = new FlowList();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
                    string url = $"{flowEndpoint}/providers/Microsoft.ProcessSimple/scopes/admin/environments/{environment.name}/v2/flows?api-version={apiVersion}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        flowList = JsonConvert.DeserializeObject<FlowList>(responseContent);
                    }
                    else
                    {
                        // Handle the error here
                    }
                }

                environment.flows = flowList.value;
            }

            return environments;
        }
        #endregion

        private static void GetAllConnectionReferencesOwnedByUserInEnvironment(string userId, string environmentId, Action<ProgressChangedEventArgs> ProgressChanged)
        {
            // auth

            // call api

            // report progress for every flow that is returned by the API
            for (int i = 0; i < 10; i++)
            {
                // wait 2 seconds for demo purposes
                System.Threading.Thread.Sleep(2000);

                var returnObj = new { Index = i, FlowName = $"ConnectionReference_XYZ{i}" };

                // report progress
                ProgressChanged(new ProgressChangedEventArgs(i * 10, returnObj));
            }
        }
    }
}
