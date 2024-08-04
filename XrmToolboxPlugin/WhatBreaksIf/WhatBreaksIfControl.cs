using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Identity.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using WhatBreaksIf.DTO;
using WhatBreaksIf.Model;
using WhatBreaksIf.TreeViewUI;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using static WhatBreaksIf.API;

namespace WhatBreaksIf
{
    public partial class WhatBreaksIfControl : PluginControlBase, IGitHubPlugin, IAboutPlugin, IHelpPlugin
    {

        #region Fields

        // these delegates are used to update the UI from a different thread
        private delegate void _updateLogWindowDelegate(string msg, params object[] args);
        private delegate void _updateTreeNodeDelegate(NodeUpdateObject nodeUpdateObject);

        private Settings mySettings;

        #endregion

        #region GitHub implementation

        public string RepositoryName => "PACE_WhatBreaksIfIRemoveThatUser";

        public string UserName => "microsoft";

        #endregion

        #region IHelpPluginImplementation

        public string HelpUrl => "https://github.com/microsoft/PACE_WhatBreaksIfIRemoveThatUser";

        #endregion

        # region IAbout implementation

        public void ShowAboutDialog()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ctor

        public WhatBreaksIfControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            LogInfo("Hello world :) ");

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

            // check if the user connected to an environment when opening the plugin. If not, ask him to connect to one now.
            if (ConnectionDetail == null)
            {
                MessageBox.Show("Please connect to an environment first.", "No environment connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // call the service.execute method to force connecting to an environment now
                Service.Execute(new WhoAmIRequest());
            }

            // create environment node
            var envNode = new EnvironmentTreeNodeElement(UpdateNode, ConnectionDetail.ServerName, ConnectionDetail.EnvironmentId);
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void btnStartQueries_Click(object sender, EventArgs eventArgs)
        {
            LogInfo("Starting....");

            var targetUser = tbTargetUserEmail.Text;
            bool checkFlowOwners = cbCheckFlowOwners.Checked;
            bool checkConnectionReferences = cbCheckConnectionReferences.Checked;

            // disable controls
            tbTargetUserEmail.Enabled = false;
            cbCheckFlowOwners.Enabled = false;
            cbCheckConnectionReferences.Enabled = false;
            btnStartQueries.Enabled = false;

            LogInfo($"Will search the following for {targetUser}:" +
                $" Flow Ownership: {(cbCheckFlowOwners.Checked ? "yes" : "no")}" +
                $" Connection References: {(cbCheckConnectionReferences.Checked ? "yes" : "no")}" +
                $" ...");


            // get all selected environments
            LogInfo("Getting all selected environments....");
            List<string> targetEnvironments = new List<string>();

            LogInfo($"Will query {targetEnvironments.Count} environments");

            // always add the current environment from the plugin connection
            // TODO: Implement plugin that can handle multiple connections
            targetEnvironments.Add(ConnectionDetail.EnvironmentId);

            // do this foreach of the environments
            foreach (var targetEnvironmentId in targetEnvironments)
            {
                LogInfo($"Processing environment {0}", targetEnvironmentId);
                if (checkFlowOwners)
                {
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Getting Flow Ownership",
                        Work = (worker, args) =>
                        {
                            var AuthGraphTask = API.AuthenticateAsync(AuthType.Graph);
                            AuthenticationResult auth = AuthGraphTask.Result;

                            worker.ReportProgress(10, "Authentication PowerApps Complete");

                            //LogInfo("Authentication PowerApps Complete.",new object[] { });

                            var userTask = API.GetUserIdFromGraph(auth.AccessToken, "laurenva@partner.eursc.eu", LogInfo);
                            string userid = userTask.Result;

                            var AuthPATask = API.AuthenticateAsync(AuthType.PowerApps);
                            auth = AuthPATask.Result;

                            var EnvironmentsTask = API.GetAllEnvironmentsInTenantAsync(auth.AccessToken, LogInfo);
                            EnvironmentList environments = EnvironmentsTask.Result;

                            worker.ReportProgress(20, "Got all environments in tenant");

                            //LogInfo("Got all environments in tenant.");

                            var AuthFlowTask = API.AuthenticateAsync(AuthType.Flow);
                            auth = AuthFlowTask.Result;

                            worker.ReportProgress(30, "Authentication Flow Complete");

                            //LogInfo("Authentication Flow Complete.");


                            var flowsTask = API.GetAllFlows("", "", environments, auth.AccessToken);
                            environments = flowsTask.Result;

                            worker.ReportProgress(40, "Get All Flows Completed");

                            //LogInfo("Get All Flows Completed.");

                            var flowPermissionsTask = API.GetAllFlowPermissions("", "", environments, auth.AccessToken);
                            environments = flowPermissionsTask.Result;

                            worker.ReportProgress(50, "Get All Flows Permissions Completed");

                            //LogInfo("Get All Flows Permissions Completed.");

                            // Filter the environments based on the conditions
                            List<Model.Environment> filteredEnvironments = new List<Model.Environment>();
                            foreach (var environment in environments.value)
                            {
                                var flows = environment.flows.Where(flow =>
                                    flow.permissions.Any(permission =>
                                        permission.properties.roleName == "Owner" &&
                                        permission.properties.principal.id == userid
                                    )
                                ).ToList();
                                if (flows.Count > 0)
                                {
                                    filteredEnvironments.Add(environment);
                                    environment.flows = flows;
                                }
                            }


                            worker.ReportProgress(50, "Get All Flows Permissions Completed");
                            //GetAllFlowsOwnedByUserInEnvironment(targetUser, targetEnvironmentId, (progress) => worker.ReportProgress(progress.ProgressPercentage, progress.UserState));
                        },
                        ProgressChanged = e =>
                        {
                            // TODO: Display the flow that was retrieved and update progressbar

                            // e.UserState is the object that was returned by the API
                            // e.ProgressPercentage is the progress of the query - probably useless right now because we do not have progress reporting implemented and it will be difficult since we run multithreaded for several environemnts

                            // todo: maybe get rid of the dynamic and use a typed object
                            //dynamic flowObj = e.UserState;
                            //string flowName = flowObj.FlowName;
                            //string flowId = flowObj.FlowId;
                            //string environmentId = flowObj.EnvironmentId;

                            // create treenodeelement
                            /* FlowTreeNodeElement(UpdateNode,
                                                    parentNodeElement: null,
                                                    flowName: flowName,
                                                    flowId: flowId,
                                                    environmentId: environmentId,
                                                    environmentName: "EnvironmentName");*/

                        },
                        PostWorkCallBack = (args) =>
                        {
                            if (args.Error != null)
                            {
                                MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            var result = args.Result;

                            // all the flows should already be displayed in the UI since we report continuously on them

                            LogInfo("Finished Flow Ownership query.");
                        },
                        AsyncArgument = null,
                        // Progress information panel size
                        MessageWidth = 340,
                        MessageHeight = 150
                    });
                }

                if (checkConnectionReferences)
                {
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Getting Connection References",
                        Work = (worker, args) =>
                        {
                            GetAllConnectionReferencesOwnedByUserInEnvironment(targetUser, targetEnvironmentId, (progress) => worker.ReportProgress(progress.ProgressPercentage, progress.UserState));
                        },
                        ProgressChanged = e =>
                        {
                            // TODO: Display the flow that was retrieved and update progressbar
                        },
                        PostWorkCallBack = (args) =>
                        {
                            if (args.Error != null)
                            {
                                MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            var result = args.Result;

                            // all the connectionreferences should already be displayed in the UI since we report continuously on them

                            LogInfo("Finished Connection References query.");
                        },
                        AsyncArgument = null,
                        // Progress information panel size
                        MessageWidth = 340,
                        MessageHeight = 150
                    });
                }
            }

            // --- careful, all the stuff above runs async, so this will run before the queries are done ----
        }

        private void tbTargetUserEmail_TextChanged(object sender, EventArgs e)
        {
            // enable the button if the text is not empty
            btnStartQueries.Enabled = !string.IsNullOrEmpty(tbTargetUserEmail.Text);
        }

        #endregion

        #region Methods

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }


        /// <summary>
        /// Base level implementation on how to get Flows owned by a user. 
        /// This will be called async later but this method is not supposed to handle async code itself
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ProgressChanged">Event handler that will be called each time the progress changes</param>
        private void GetAllFlowsOwnedByUserInEnvironment(string userId, string targetEnvironmentId, Action<ProgressChangedEventArgs> ProgressChanged)
        {
            // auth

            // call api

            // report progress for every flow that is returned by the API
            for (int i = 0; i < 10; i++)
            {
                // wait 2 seconds for demo purposes
                System.Threading.Thread.Sleep(2000);


                // call api

                // get flow informnation 
                var returnObj = new
                {
                    Index = i,
                    FlowName = $"Flow_XYZ{i}",
                    FlowId = i.ToString(),
                    EnvironmentId = targetEnvironmentId,
                };

                // report progress
                ProgressChanged(new ProgressChangedEventArgs(i * 10, returnObj));
            }
        }

        private void GetAllConnectionReferencesOwnedByUserInEnvironment(string userId, string environmentId, Action<ProgressChangedEventArgs> ProgressChanged)
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

        private void UpdateNode(NodeUpdateObject nodeUpdateObject)
        {
            // because this might be called from a different thread
            if (treeView1.InvokeRequired)
            {
                treeView1.Invoke(new _updateTreeNodeDelegate(UpdateNode), nodeUpdateObject);
            }
            else
            {
                try
                {
                    switch (nodeUpdateObject.UpdateReason)
                    {
                        case UpdateReason.AddedToList:
                            var parentNode = treeView1.Nodes.Find(nodeUpdateObject.ParentNodeId, true).FirstOrDefault();
                            if (parentNode == null)
                            {
                                // create a new top level node
                                var createNode = new TreeNode
                                {
                                    Name = nodeUpdateObject.NodeId,
                                    Text = nodeUpdateObject.NodeText,
                                    ForeColor = System.Drawing.Color.Black,
                                    Tag = nodeUpdateObject.TreeNodeElement,
                                    //ToolTipText = "n/a",
                                    Checked = true
                                };
                                treeView1.Nodes.Add(createNode);
                                createNode.Expand();

                            }
                            else
                            {
                                // add under an existing node
                                var createNode = new TreeNode
                                {
                                    Name = nodeUpdateObject.NodeId,
                                    Text = nodeUpdateObject.NodeText,
                                    ForeColor = System.Drawing.Color.Black,
                                    Tag = nodeUpdateObject.TreeNodeElement,
                                    //ToolTipText = "n/a",
                                    Checked = true
                                };
                                parentNode.Nodes.Add(createNode);
                                parentNode.Expand();
                            }
                            break;

                        // this is used so we can update nodes in the UI that are already there with additional details
                        case UpdateReason.DetailsAdded:
                            var updateNode = treeView1.Nodes.Find(nodeUpdateObject.NodeId, true).FirstOrDefault();
                            updateNode.ForeColor = System.Drawing.Color.Black;
                            updateNode.Tag = nodeUpdateObject.TreeNodeElement;
                            //updateNode.ToolTipText = "n/a.";
                            updateNode.Checked = true;

                            break;

                        case UpdateReason.RemovedFromList:
                            // not implemented
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    LogError("Exception building the TreeView. This is not critical but weird.");
                    LogError(ex.Message + " " + ex.StackTrace);
                }
            }
        }

        #endregion

        #region overrides

        private new void LogInfo(string text, params object[] args)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(LogInfo);
                lbDebugOutput.Invoke(update, text);
            }
            else
            {
                base.LogInfo(text, args);
                lbDebugOutput.Items.Add(text);
                if (lbDebugOutput.Items.Count > 1000)
                {
                    lbDebugOutput.Items.RemoveAt(0); // remove first line
                }
                // Make sure the last item is made visible
                lbDebugOutput.SelectedIndex = lbDebugOutput.Items.Count - 1;
                lbDebugOutput.ClearSelected();
            }
        }

        private new void LogError(string text, params object[] args)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(LogError);
                lbDebugOutput.Invoke(update, text);
            }
            else
            {
                base.LogError(text, args);
                lbDebugOutput.Items.Add(text);
                if (lbDebugOutput.Items.Count > 1000)
                {
                    lbDebugOutput.Items.RemoveAt(0); // remove first line
                }
                // Make sure the last item is made visible
                lbDebugOutput.SelectedIndex = lbDebugOutput.Items.Count - 1;
                lbDebugOutput.ClearSelected();
            }
        }

        private new void LogWarning(string text, params object[] args)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(LogWarning);
                lbDebugOutput.Invoke(update, text);
            }
            else
            {
                base.LogWarning(text, args);
                lbDebugOutput.Items.Add(text);
                if (lbDebugOutput.Items.Count > 1000)
                {
                    lbDebugOutput.Items.RemoveAt(0); // remove first line
                }
                // Make sure the last item is made visible
                lbDebugOutput.SelectedIndex = lbDebugOutput.Items.Count - 1;
                lbDebugOutput.ClearSelected();
            }
        }

        #endregion


        // TODO: Implement ConnectionReferenceTreeNodeElement
    }
}