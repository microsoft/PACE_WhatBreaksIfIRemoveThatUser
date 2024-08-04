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
    public partial class WhatBreaksIfControl : PluginControlBase
    {
        // helper class to hold information about the environment and the status of the queries
        public class EnvironmentQueryStatus
        {
            private bool _flowsQueryCompleted = false;
            private bool _connectionRefsQueryCompleted = false;

            public bool flowsQueryCompleted
            {
                get => _flowsQueryCompleted;
                set
                {
                    _flowsQueryCompleted = value;

                    // raise event and inform that all queries have been completed if necessary
                    if (_flowsQueryCompleted && connectionRefsQueryCompleted)
                    {
                        AllEnvironmentQueriesCompleted?.Invoke(this, new EventArgs());
                    }
                }
            }
            public bool connectionRefsQueryCompleted { 
                get => _connectionRefsQueryCompleted;
                set
                {
                    _connectionRefsQueryCompleted = value;

                    // raise event and inform that all queries have been completed if necessary
                    if (_flowsQueryCompleted && connectionRefsQueryCompleted)
                    {
                        AllEnvironmentQueriesCompleted?.Invoke(this, new EventArgs());
                    }
                }
            } 

            public event EventHandler AllEnvironmentQueriesCompleted;
        }

        public class EnvironmentCollection : Dictionary<string, EnvironmentQueryStatus>
        {
            /// <summary>
            /// This Event is thrown when the queries of all environments have been completed.
            /// </summary>
            public event EventHandler AllEnvironmentsQueriesCompleted;

            public EnvironmentCollection() : base()
            { }

            // implement our own add method so we can subscribe to the AllQueriesCompleted event
            public new void Add(string environmentId, EnvironmentQueryStatus targetEnvironment)
            {
                targetEnvironment.AllEnvironmentQueriesCompleted += EnvironmentQueriesCompleted;
                base.Add(environmentId, targetEnvironment);
            }

            private void EnvironmentQueriesCompleted(object sender, EventArgs e)
            {
                // check whether all the environments in this collection are done and if they are, throw the event
                if (this.All(x => x.Value.flowsQueryCompleted && x.Value.connectionRefsQueryCompleted))
                {
                    AllEnvironmentsQueriesCompleted?.Invoke(this, new EventArgs());
                }
            }
        }

        #region Fields

        // these delegates are used to update the UI from a different thread
        private delegate void _updateLogWindowDelegate(string msg, params object[] args);
        private delegate void _updateTreeNodeDelegate(NodeUpdateObject nodeUpdateObject);

        // this list will contain the target environments that we query as well as the status of their respective queries
        // todo watch out for thread safety issues
        private readonly EnvironmentCollection targetEnvironments = new EnvironmentCollection();

        private Settings mySettings;

        #endregion

        #region ctor

        public WhatBreaksIfControl()
        {
            InitializeComponent();

            // subscribe to the event that tells us that all queries have been completed
            targetEnvironments.AllEnvironmentsQueriesCompleted += AllEnvironmentQueriesCompleted;
            
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

        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tbTargetUserEmail_TextChanged(object sender, EventArgs e)
        {
            // enable the button if the text is not empty
            btnStartQueries.Enabled = !string.IsNullOrEmpty(tbTargetUserEmail.Text);
        }

        private void btnStartQueries_Click(object sender, EventArgs eventArgs)
        {
            pbMain.Style = ProgressBarStyle.Marquee;
            //pbMain.MarqueeAnimationSpeed = 30;

            LogInfo("Starting....");

            var targetUser = tbTargetUserEmail.Text;
            bool checkFlowOwners = cbCheckFlowOwners.Checked;
            bool checkConnectionReferences = cbCheckConnectionReferences.Checked;

            // disable controls
            tbTargetUserEmail.Enabled = false;
            cbCheckFlowOwners.Enabled = false;
            cbCheckConnectionReferences.Enabled = false;
            btnStartQueries.Enabled = false;

            // this might be not the first time that the user clicks the button, so we need to clean up
            treeView1.Nodes.Clear();
            targetEnvironments.Clear();

            LogInfo($"Will search the following for {targetUser}:" +
                $" Flow Ownership: {(cbCheckFlowOwners.Checked ? "yes" : "no")}" +
                $" Connection References: {(cbCheckConnectionReferences.Checked ? "yes" : "no")}" +
                $" ...");

            // get all selected environments
            LogInfo("Getting all selected environments....");

            // always add the current environment from the plugin connection
            // TODO: Implement plugin that can handle multiple connections
            targetEnvironments.Add(ConnectionDetail.EnvironmentId, new EnvironmentQueryStatus());

            LogInfo($"Will query {targetEnvironments.Count} environments");

            // do this foreach of the environments
            foreach (var currentTargetEnvironment in targetEnvironments)
            {
                // create environment node for the current environment
                var environmentNode = new EnvironmentTreeNodeElement(UpdateNode, ConnectionDetail.ServerName, ConnectionDetail.EnvironmentId);

                LogInfo($"Processing environment {currentTargetEnvironment.Key}");

                if (checkFlowOwners)
                {
                    // create a directory node that holds the references to the flows so we know where in the UI to place them
                    var flowDirectoryNode = new DirectoryTreeNode(UpdateNode, "Flows", environmentNode);

                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Doing stuff..",
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
                            // todo: maybe get rid of the dynamic and use a typed object
                            //dynamic flowObj = e.UserState;
                            //string flowName = flowObj.FlowName;
                            //string flowId = flowObj.FlowId;
                            //string environmentId = flowObj.EnvironmentId;

                            // create treenodeelement
                            new FlowTreeNodeElement(UpdateNode,
                                                    parentNodeElement: flowDirectoryNode,
                                                    flowName: flowName,
                                                    flowId: flowId,
                                                    environmentId: environmentId
                                                    );
                        },
                        PostWorkCallBack = (args) =>
                        {
                            if (args.Error != null)
                            {
                                MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            // cast args.Result to the currentTargetEnvironment
                            var targetEnvironmentResultObj = (KeyValuePair<string, EnvironmentQueryStatus>)args.Result;
                            LogInfo($"Finished Flow Ownership query for environment {targetEnvironmentResultObj.Key}.");
                            // The UI has been updated continuously while the queries were running and the event handler of the environment collection will handle UI after completion of everything
                        },
                        AsyncArgument = currentTargetEnvironment,
                        // Progress information panel size
                        MessageWidth = 340,
                        MessageHeight = 150
                    });
                }

                if (checkConnectionReferences)
                {
                    // create a directory node that holds the references to the connectionreferences so we know where in the UI to place them
                    var connectionReferencesDirectoryNode = new DirectoryTreeNode(UpdateNode, "Connection References", environmentNode);

                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Doing stuff..",
                        Work = (worker, args) =>
                        {
                            // get the targetEnvironment from the args - since this is running multithreaded, we cannot be sure that currentTargetEnvironment is still the same
                            var targetEnvironment = (KeyValuePair<string, EnvironmentQueryStatus>)args.Argument;

                            GetAllConnectionReferencesOwnedByUserInEnvironment(
                                userId: targetUser, 
                                targetEnvironmentId: currentTargetEnvironment.Key,
                                ProgressChanged: (progress) => worker.ReportProgress(progress.ProgressPercentage, progress.UserState));

                            // set the query as completed after we are done
                            targetEnvironment.Value.connectionRefsQueryCompleted = true;

                            // put the currentTargetEnvironment into the args.Result so we can access it in the PostWorkCallBack
                            args.Result = targetEnvironment;
                        },
                        ProgressChanged = e =>
                        {
                            // todo: maybe get rid of the dynamic and use a typed object
                            dynamic connectionReferenceObj = e.UserState;
                            string connectionReferenceName = connectionReferenceObj.ConnectionReferenceName;
                            string environmentId = connectionReferenceObj.EnvironmentId;

                            // create treenodeelement
                            new ConnectionReferenceTreeNodeElement(UpdateNode,
                                                    parentNodeElement: connectionReferencesDirectoryNode,
                                                    connectionReferenceName: connectionReferenceName,
                                                    environmentId: environmentId
                                                    );
                        },
                        PostWorkCallBack = (args) =>
                        {
                            if (args.Error != null)
                            {
                                MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            // cast args.Result to the currentTargetEnvironment
                            var targetEnvironmentResultObj = (KeyValuePair<string, EnvironmentQueryStatus>)args.Result;
                            LogInfo($"Finished ConnectionReferences query for environment {targetEnvironmentResultObj.Key}.");
                            // The UI has been updated continuously while the queries were running and the event handler of the environment collection will handle UI after completion of everything
                        },
                        AsyncArgument = currentTargetEnvironment,
                        // Progress information panel size
                        MessageWidth = 340,
                        MessageHeight = 150
                    });
                }
            }

            // --- careful, all the stuff above runs async, everything below here will run immediately ----
        }

        private void AllEnvironmentQueriesCompleted(object sender, EventArgs e)
        {
            // invoke if necessary - this event will likely be called from a background thread
            if (treeView1.InvokeRequired)
            {
                treeView1.Invoke(new EventHandler(AllEnvironmentQueriesCompleted), sender, e);
            }
            else
            {
                LogInfo("All queries have been completed.");
                pbMain.Style = ProgressBarStyle.Continuous;
                btnExportToExcel.Enabled = true;
                btnStartQueries.Enabled = true;
            }

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

        private void GetAllConnectionReferencesOwnedByUserInEnvironment(string userId, string targetEnvironmentId, Action<ProgressChangedEventArgs> ProgressChanged)
        {
            // auth

            // call api

            // report progress for every flow that is returned by the API
            for (int i = 0; i < 10; i++)
            {
                // wait 1 second for demo purposes
                System.Threading.Thread.Sleep(1000);

                var returnObj = new { 
                    ConnectionReferenceName = $"ConnectionReference_XYZ{i}",
                    EnvironmentId = targetEnvironmentId
                };

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
    }
}