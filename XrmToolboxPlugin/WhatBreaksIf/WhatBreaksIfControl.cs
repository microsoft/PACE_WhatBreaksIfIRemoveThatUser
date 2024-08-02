using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

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
                            GetAllFlowsOwnedByUserInEnvironment(targetUser, targetEnvironmentId, (progress) => worker.ReportProgress(progress.ProgressPercentage, progress.UserState));
                        },
                        ProgressChanged = e =>
                        {
                            // TODO: Display the flow that was retrieved and update progressbar

                            // e.UserState is the object that was returned by the API
                            // e.ProgressPercentage is the progress of the query - probably useless right now because we do not have progress reporting implemented and it will be difficult since we run multithreaded for several environemnts

                            // todo: maybe get rid of the dynamic and use a typed object
                            dynamic flowObj = e.UserState;
                            string flowName = flowObj.FlowName;
                            string flowId = flowObj.FlowId;
                            string environmentId = flowObj.EnvironmentId;

                            // create treenodeelement
                            new FlowTreeNodeElement(UpdateNode,
                                                    parentNodeElement: null,
                                                    flowName: flowName,
                                                    flowId: flowId,
                                                    environmentId: environmentId,
                                                    environmentName: "EnvironmentName");

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
                var returnObj = new { 
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

        // This object is only used to transfer data to update the UI, it is not meant to hold any data on itself
        internal class NodeUpdateObject
        {
            internal UpdateReason UpdateReason { get; set; }
            internal string NodeId { get { return TreeNodeElement.ElementId; } }
            internal string ParentNodeId { get; set; }
            internal string NodeText { get; set; }
            internal TreeNodeElementBase TreeNodeElement { get; set; }
        }

        internal enum UpdateReason
        {
            AddedToList,
            RemovedFromList,
            DetailsAdded
        }

        // this base class is used so we can display different types of objects in the treeview. Abstract because we enforce typed implementations
        internal abstract class TreeNodeElementBase
        {
            private readonly Action<NodeUpdateObject> updateNodeUi;

            public string ElementId { get; set; }

            internal abstract IEnumerable<TreeNodeElementBase> ChildObjects { get; }

            internal abstract TreeNodeElementBase Parent { get; }

            public TreeNodeElementBase(Action<NodeUpdateObject> updateNodeUi)
            {
                this.updateNodeUi = updateNodeUi;
            }
        }

        // implementation of TreeNodeElementBase, this one is used to display Environments in the treeview
        internal class EnvironmentTreeNodeElement : TreeNodeElementBase
        {
            public string EnvironmentName { get; set; }

            public string EnvironmentId { get; set; }

            public EnvironmentTreeNodeElement(Action<NodeUpdateObject> updateNodeUi, string environmentName, string environmentId) : base(updateNodeUi)
            {
                EnvironmentName = environmentName;
                EnvironmentId = environmentId;

                // constructor for the environment tree node was called, update the UI to display it. This needs to happen after the backing fields of the properties have been set!
                updateNodeUi(new NodeUpdateObject()
                {
                    TreeNodeElement = this,
                    NodeText = EnvironmentName,
                    UpdateReason = UpdateReason.AddedToList
                });
            }

            internal List<TreeNodeElementBase> EnvironmentNodeElements { get;} = new List<TreeNodeElementBase>();

            internal override TreeNodeElementBase Parent => null; // this is the top level node

            internal override IEnumerable<TreeNodeElementBase> ChildObjects => EnvironmentNodeElements;
        }

        // one implementation of TreeNodeElementBase, this one is used to display Flows in the treeview
        internal class FlowTreeNodeElement : EnvironmentTreeNodeElement
        {
            internal TreeNodeElementBase _parentNodeElement;

            public string FlowName { get; set; }

            public string FlowId { get; set; }

            public Uri FlowUri { get => new Uri($"https://make.powerautomate.com/environments{EnvironmentId}/solutions/~preferred/flows/{FlowId})"); }

            public FlowTreeNodeElement(Action<NodeUpdateObject> updateNodeUiDelegate,
                                      TreeNodeElementBase parentNodeElement,
                                      string flowName,
                                      string flowId,
                                      string environmentId,
                                      string environmentName) : base(updateNodeUiDelegate, environmentName, environmentId)
            {
                // ctor has been called, this means we need to call the update method to display the flow in the UI
                // TODO Implement logic for updating object that already exist

                updateNodeUiDelegate(new NodeUpdateObject()
                {
                    TreeNodeElement = this,
                    ParentNodeId = (parentNodeElement != null) ? _parentNodeElement.ElementId.ToString() : null,
                    NodeText = FlowName,
                    UpdateReason = UpdateReason.AddedToList
                });
                _parentNodeElement = parentNodeElement;
            }

            // right now we dont have any child objects, but we could have them in the future, for example to show connection references that sit under a flow
            internal override IEnumerable<TreeNodeElementBase> ChildObjects => throw new NotImplementedException();

            internal override TreeNodeElementBase Parent => _parentNodeElement;
        }

        // TODO: Implement ConnectionReferenceTreeNodeElement
    }
}