using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using FlowOwnershipAudit.Model;
using FlowOwnershipAudit.TreeViewUI;
using FlowOwnershipAudit.TreeViewUIElements;
using TreeViewUI;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using static FlowOwnershipAudit.API;
using Environment = System.Environment;
using ListViewItem = System.Windows.Forms.ListViewItem;

namespace FlowOwnershipAudit
{
    public partial class FlowOwnershipAuditControl : PluginControlBase, IAboutPlugin﻿, IGitHubPlugin, IHelpPlugin
    {
        // helper class to hold information about the environment and the status of the queries﻿
        public class EnvironmentQueryStatus﻿
        {
            private bool _flowsQueryCompleted = false;
            private bool _connectionRefsQueryCompleted = false;

            public bool flowsQueryCompleted﻿
            {
                get
                {
                    return _flowsQueryCompleted;
                }
                set
                {
                    _flowsQueryCompleted = value;

                    // raise event and inform that all queries have been completed if necessary﻿
                    if (_flowsQueryCompleted && connectionsQueryCompleted)
                    {
                        EnvironmentQueriesCompleted?.Invoke(this, new EventArgs());
                    }
                }
            }
            public bool connectionsQueryCompleted
            {
                get => _connectionRefsQueryCompleted;
                set
                {
                    _connectionRefsQueryCompleted = value;

                    // raise event and inform that all queries have been completed if necessary﻿
                    if (_flowsQueryCompleted && connectionsQueryCompleted)
                    {
                        EnvironmentQueriesCompleted?.Invoke(this, new EventArgs());
                    }
                }
            }

            public event EventHandler EnvironmentQueriesCompleted;
        }
        public class EnvironmentCollection : Dictionary<Model.Environment, EnvironmentQueryStatus>
        {
            /// <summary>﻿
            ﻿/// This Event is thrown when the queries of all environments have been completed.﻿
            ﻿/// </summary>﻿
            public event EventHandler AllEnvironmentsQueriesCompleted;

            /// <summary>﻿
            /// This event is thrown when the underlying dictionary changes﻿
            /// </summary>﻿
            public event EventHandler CollectionChanged;

            public EnvironmentCollection() : base()
            { }

            // these overrrides are necessary to implement our own observable event pattern﻿

            public new void Add(Model.Environment key, EnvironmentQueryStatus value)
            {
                // call base method to add item to the dictionary﻿
                base.Add(key, value);

                // invoke collection changed handlers if there are any﻿
                CollectionChanged?.Invoke(this, new EventArgs());

                // subscribe to the event that tells us that this environment finished processing﻿
                value.EnvironmentQueriesCompleted += EnvironmentQueriesCompleted;
            }

            public new void AddRange(IEnumerable<KeyValuePair<Model.Environment, EnvironmentQueryStatus>> items)
            {
                // call base method to add items to the dictionary﻿
                foreach (var item in items)
                {
                    Add(item.Key, item.Value);
                }
            }

            public new void Clear()
            {
                // call base method to clear the dictionary﻿
                base.Clear();

                // invoke collection changed handlers if there are any﻿
                CollectionChanged?.Invoke(this, new EventArgs());
            }

            public new bool Remove(Model.Environment key)
            {
                // call base method to remove the item from the dictionary﻿
                var result = base.Remove(key);

                // invoke collection changed handlers if there are any﻿
                CollectionChanged?.Invoke(this, new EventArgs());

                // TODO : unsubscribe from the event that tells us that this environment finished processing﻿

                return result;
            }

            private void EnvironmentQueriesCompleted(object sender, EventArgs e)
            {
                // check if handler is present before triggering﻿
                if (AllEnvironmentsQueriesCompleted != null)
                {
                    // check whether all environments in this collection have finished both flow and connection queries﻿
                    if (this.All(x => x.Value.flowsQueryCompleted && x.Value.connectionsQueryCompleted))
                    {
                        AllEnvironmentsQueriesCompleted(this, new EventArgs());
                    }
                }
            }
        }

        // these delegates are used to update the UI from a different thread﻿
        private delegate void _updateLogWindowDelegate(string msg, params object[] args);
        private delegate void _updateTreeNodeDelegate(NodeUpdateObject nodeUpdateObject);
        private delegate void _updateListviewDelegate(ListViewItem listViewItem);

        // this list will contain the target environments that we query as well as the status of their respective queries﻿
        // todo watch out for thread safety issues﻿
        private readonly EnvironmentCollection targetEnvironments = new EnvironmentCollection();

        private readonly string sidePanelDefaultText = "Select environments and click start, then select any node to see the results here.";

        #region Properties

        public string RepositoryName => "PACE_WhatBreaksIfIRemoveThatUser";

        public string UserName => "microsoft";

        public string HelpUrl => "https://github.com/microsoft/PACE_WhatBreaksIfIRemoveThatUser";

        #endregion

        public FlowOwnershipAuditControl()
        {
            InitializeComponent();

            // subscribe to the event that tells us that all queries have been completed﻿
            targetEnvironments.AllEnvironmentsQueriesCompleted += AllEnvironmentQueriesCompleted;

            // subscribe to the event that the underlying list of target environments has changed﻿
            targetEnvironments.CollectionChanged += TargetEnvironments_CollectionChanged;

            // subscribe to the event that the treeview node has been selected
            //tvTreeview.BeforeCheck += treeView1_BeforeCheck;
            tvTreeview.AfterSelect += treeView1_AfterSelect;
            tvTreeview.AfterCheck += treeView1_AfterCheck;
            tvTreeview.DrawNode += treeView1_DrawNode;
            tvTreeview.DrawMode = TreeViewDrawMode.OwnerDrawText;

            //Add Columns to listview control
            lvComponentCount.View = View.Details;
            lvComponentCount.GridLines = true;
            lvComponentCount.Scrollable = true;
            lvComponentCount.Sorting = SortOrder.Ascending;
            lvComponentCount.Columns.Add("Component type", 200, HorizontalAlignment.Left);
            lvComponentCount.Columns.Add("Count", 100, HorizontalAlignment.Left);

        }

        #region Event Handlers

        /// <summary>﻿
        /// This event occurs when the plugin is closed﻿
        /// </summary>﻿
        /// <param name="sender"></param>﻿
        /// <param name="e"></param>﻿
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {

        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            // for some reason the designer keeps deleting this default text...﻿
            tbTargetUserEmail.Text = "Please enter the target user's principal name or object id.";
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void TargetEnvironments_CollectionChanged(object sender, EventArgs e)
        {
            if (!targetEnvironments.Any())
            {
                // there are no environments in our target list. Start and export buttons need to be disabled﻿
                btnExportToExcel.Enabled = false;
                btnStartQueries.Enabled = false;

                btnSelectEnvironments.Enabled = true;
            }
            else
            {
                // the list of targetenvironments is no longer empty, that means we need to enable the start button, but only if the target email adress is already there﻿
                if (!string.IsNullOrEmpty(tbTargetUserEmail.Text))
                {
                    btnStartQueries.Enabled = true;
                }

                // export button needs to be disabled, it will be enabled automatically when all queries have been completed﻿
                btnExportToExcel.Enabled = false;
                tbSelectedEnvironments.Text = targetEnvironments.Count().ToString();
            }
        }

        private void tbTargetUserEmail_TextChanged(object sender, EventArgs e)
        {
            // enable the button if the text is not empty﻿ and if targetenvironments have already been selected 
            if (targetEnvironments.Any())
            {
                if (!string.IsNullOrEmpty(tbTargetUserEmail.Text))
                {
                    btnStartQueries.Enabled = true;
                }
                else
                {
                    btnStartQueries.Enabled = false;
                }
            }
            else
            {
                btnStartQueries.Enabled = false;
            }
        }

        private void btnSelectEnvironments_Click(object sender, EventArgs eventArgs)
        {
            // clear the currently selected environments because we want to show a dialog that allows to user to make a selection﻿
            targetEnvironments.Clear();

            // clear sidepanel just in case
            rtbSidepanel.Text = sidePanelDefaultText;

            // get all selected environments﻿
            LogInfo("Getting all environments....");

            WorkAsync(new WorkAsyncInfo﻿
            {
                Message = "Fetching Environments",
                Work = (worker, args) =>
                {
                    var EnvironmentsTask = GetAllEnvironmentsInTenantAsync((progress) => worker.ReportProgress(progress.ProgressPercentage, progress.UserState));

                    args.Result = EnvironmentsTask.Result;
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {

                        // check if inner exception is MsalClientException
                        if (args.Error.InnerException is Microsoft.Identity.Client.MsalClientException)
                        {
                            var msalClientException = (Microsoft.Identity.Client.MsalClientException)args.Error.InnerException;
                            if (msalClientException.ErrorCode == "authentication_canceled")
                            {
                                // user canceled the login, this is not a problem, we just exit
                                LogError("The user canceled the login screen.");
                                return;
                            }
                        }
                        // something bad happened
                        ShowErrorDialog(args.Error.InnerException, allownewissue: true);
                    }

                    var environmentList = (EnvironmentList)args.Result;

                    using (var environmentSelectorForm = new EnvironmentSelector(environmentList.value))
                    {
                        var dialogResult = environmentSelectorForm.ShowDialog();

                        if (dialogResult == DialogResult.OK)
                        {
                            LogInfo($"Selected {environmentSelectorForm.SelectedEnvironments.Count} environments");
                            foreach (var environment in environmentSelectorForm.SelectedEnvironments)
                            {
                                targetEnvironments.Add(environment, new EnvironmentQueryStatus());
                            }
                            tbSelectedEnvironments.Text = targetEnvironments.Count().ToString();
                            toolTip1.SetToolTip(tbSelectedEnvironments, string.Join(", ", targetEnvironments.Keys.Select(x => x.properties.displayName)));
                        }
                        else
                        {
                            LogInfo("No environments selected, all will be used.");
                            // add all environments to targetEnvironments because the user decided to not filter them﻿
                            foreach (var environment in environmentList.value)
                            {
                                targetEnvironments.Add(environment, new EnvironmentQueryStatus());
                                tbSelectedEnvironments.Text = targetEnvironments.Count().ToString();
                                toolTip1.SetToolTip(tbSelectedEnvironments, string.Join(", ", targetEnvironments.Keys.Select(x => x.properties.displayName)));
                            }
                        }
                    }

                    // re-enable the email field because it might be disabled if the user clicked the restart button before
                    tbTargetUserEmail.Enabled = true;
                },
                MessageWidth = 340,
                MessageHeight = 150
            });
        }

        private void btnStartQueries_Click(object sender, EventArgs eventArgs)
        {
            pbMain.Style = ProgressBarStyle.Marquee;

            LogInfo("Starting....");

            var targetUser = tbTargetUserEmail.Text;
            bool checkFlowOwners = cbCheckFlowOwners.Checked;
            bool checkConnections = cbCheckConnection.Checked;

            // disable controls﻿
            tbTargetUserEmail.Enabled = false;
            cbCheckFlowOwners.Enabled = false;
            cbCheckConnection.Enabled = false;
            btnStartQueries.Enabled = false;
            btnSelectEnvironments.Enabled = false;

            // clear sidepanel just in case
            rtbSidepanel.Text = sidePanelDefaultText;

            // this might be not the first time that the user clicks the button, so we need to clean up﻿
            tvTreeview.Nodes.Clear();

            LogInfo($"Will search the following for {targetUser}:" +
                $" Flow Ownership: {(cbCheckFlowOwners.Checked ? "yes" : "no")}" +
                $" Connection: {(cbCheckConnection.Checked ? "yes" : "no")}" +
                $" ...");

            LogInfo($"Will query {targetEnvironments.Count()} environments");

            List<EnvironmentTreeNodeElement> environmentTreeNodes = new List<EnvironmentTreeNodeElement>();
            List<DirectoryTreeNode> directoryTreeNodes = new List<DirectoryTreeNode>();
            List<FlowTreeNodeElement> flowTreeNodes = new List<FlowTreeNodeElement>();

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += (obj, arg) =>
            {
                Guid userid;

                // get the user id from the graph api. This should be the same for all environments so we dont need to do it in the parallel loop
                try
                {
                    if (!Guid.TryParse(targetUser, out userid))
                    {
                        var userTask = GetUserIdFromGraph(targetUser);
                        userid = Guid.Parse(userTask.Result);
                    }
                }
                catch (Exception ex)
                {
                    // if we cant get the user id, we cant do anything. Log the error and throw it up
                    // this could happen if the user is unable to authenticate or if the user canceled the login screen
                    LogError("An error occurred while trying to get the user id from the graph api.");

                    // exit the bgw, there is nothing we can do
                    return;
                }

                var parallelResult = Parallel.ForEach(
                    source: targetEnvironments,
                    parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 10 },
                    body: currentTargetEnvironment =>
                    {
                        // this is the foreach that is running in parallel for each environment﻿
                        // create environment node for the current environment﻿
                        ListViewGroup group = new ListViewGroup(currentTargetEnvironment.Key.properties.displayName, HorizontalAlignment.Left);
                        EnvironmentTreeNodeElement environmentNode = new EnvironmentTreeNodeElement(UpdateNode, currentTargetEnvironment.Key.properties.displayName, currentTargetEnvironment.Key.name);
                        environmentTreeNodes.Add(environmentNode);

                        LogInfo($"Processing environment {currentTargetEnvironment.Key.name}");
                        LogInfo($"Looking for {targetUser} with id {userid} in {currentTargetEnvironment.Key.name}");

                        if (checkFlowOwners)
                        {
                            // create a directory node that holds the references to the flows so we know where in the UI to place them﻿
                            var flowDirectoryNode = new DirectoryTreeNode(UpdateNode, "Flows", environmentNode);
                            directoryTreeNodes.Add(flowDirectoryNode);

                            AddFlowsToEnvironment(
                                targetEnvironment: currentTargetEnvironment.Key);

                            AddFlowPermissionsToEnvironment(
                                userId: userid.ToString(),
                                targetEnvironment: currentTargetEnvironment.Key,
                                ProgressChanged: (flowObj) =>
                                {
                                    //Flow flow = flowObj as Flow;

                                    FlowTreeNodeElement flowTreeNodeElement = new FlowTreeNodeElement(UpdateNode,
                                                               parentNodeElement: flowDirectoryNode,
                                                               flow: (Flow)flowObj
                                                               )
                                    {
                                        MigrationStatus = MigrationStatus.NotMigratedYet
                                    };

                                    //flowTreeNodes.Add(flowTreeNodeElement);

                                    GetFlowDetails(currentTargetEnvironment.Key, (Flow)flowObj,
                                    ProgressChanged: (flowDetailsObj) =>
                                    {
                                        Flow flowDetails = (Flow)flowDetailsObj;

                                        foreach (var connectionReference in flowDetails.properties.connectionReferences)
                                        {
                                            ConnectionReferenceTreeNodeElement connectionReferenceTreeNodeElement = new ConnectionReferenceTreeNodeElement(UpdateNode,
                                            parentNodeElement: flowTreeNodeElement, //flowTreeNodes.Where(x => x.Flow.name == currentFlow.name).SingleOrDefault(),
                                            connectionReferenceName: connectionReference.connectionReferenceLogicalName);

                                        }
                                    });
                                });

                            currentTargetEnvironment.Value.flowsQueryCompleted = true;

                            ListViewItem Item = new ListViewItem(new string[]
                            {"Flows",
                            currentTargetEnvironment.Key.flows.Where(x=> x.isOwnedByX).Count().ToString()});
                            Item.Group = group;
                            UpdateListView(Item);

                            //I want to count the number of unique connectionreferences in the flows in the list. 
                            //The connection references can be found in the property flow.properties.connectionReferences and the unique identifier to group on is the id field.

                            Item = new ListViewItem(new string[]
                            {"Connection References",
                            currentTargetEnvironment.Key.flows
                            .Where(x=>x.properties != null && x.properties.connectionReferences != null)
                            .SelectMany(flow => flow.properties.connectionReferences)
                            .Select(connectionReference => connectionReference.id)
                            .Distinct()
                            .Count()
                            .ToString()
                            });
                            Item.Group = group;
                            UpdateListView(Item);
                        }
                        else
                        {
                            // mark as completed if we are not checking for flow owners
                            currentTargetEnvironment.Value.flowsQueryCompleted = true;
                        }

                        if (checkConnections)
                        {
                            // create a directory node that holds the references to the connectionreferences so we know where in the UI to place them﻿
                            var connectionsDirectoryNode = new DirectoryTreeNode(UpdateNode, "Connection", environmentNode);
                            directoryTreeNodes.Add(connectionsDirectoryNode);

                            AddConnectionsToEnvironment(
                                userId: userid.ToString(),
                                targetEnvironment: currentTargetEnvironment.Key,
                                ProgressChanged: (ConnectionObj) =>
                                {
                                    dynamic connectionObj = ConnectionObj;
                                    string connectionName = connectionObj.ConnectionName;
                                    string environmentId = connectionObj.EnvironmentId;

                                    // create treenodeelement﻿
                                    new ConnectionTreeNodeElement(UpdateNode,
                                                               parentNodeElement: connectionsDirectoryNode,
                                                               connectionName: connectionName,
                                                               environmentId: environmentId﻿
                                                               );
                                });

                            currentTargetEnvironment.Value.connectionsQueryCompleted = true;

                            ListViewItem Item = new ListViewItem(new string[]
                            {"Connections",
                            currentTargetEnvironment.Key.connections.Where(x=> x.isOwnedByX).Count().ToString()});
                            Item.Group = group;
                            UpdateListView(Item);
                        }
                        else
                        {
                            // mark as completed if we are not checking for connections
                            currentTargetEnvironment.Value.connectionsQueryCompleted = true;
                        }
                    });
                arg.Result = parallelResult;
                // TODO: error handling within individual tasks
            };
            bgw.RunWorkerCompleted += (obj, arg) =>
            {
                // worker completed. This means that the main operation is done, all environments have been queried and the treeview has been built

                // check if the bgw ran into an error
                if (arg.Error != null)
                {
                    LogError("An error occurred during the background worker operation.");
                    LogError(arg.Error.Message);
                    LogError(arg.Error.StackTrace);
                }

                // arg contains the parallelResult. we dont need it for now
                var parallelResult = (ParallelLoopResult)arg.Result;
            };

            bgw.RunWorkerAsync();
            // --- careful, all the stuff above runs async, everything below here will run immediately ----﻿
        }

        private void AllEnvironmentQueriesCompleted(object sender, EventArgs e)
        {
            // TODO if no environments have been loaded, this will never be called and the progressbar keeps on going forever

            // invoke if necessary - this event will likely be called from a background thread﻿
            if (tvTreeview.InvokeRequired)
            {
                tvTreeview.Invoke(new EventHandler(AllEnvironmentQueriesCompleted), sender, e);
            }
            else
            {
                LogInfo("All queries have been completed.");
                pbMain.Style = ProgressBarStyle.Continuous;
                btnExportToExcel.Enabled = true;
                btnReassign.Enabled = true;
            }
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog﻿
            {
                Title = "Save an excel file",
                Filter = "Excel files|*.xlsx"
            };
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.﻿
            if (saveFileDialog1.FileName != "")
            {
                LogInfo($"Exporting to {saveFileDialog1.FileName}");

                using (ExcelExporter exporter = new ExcelExporter(targetEnvironments))
                {
                    exporter.ExportToExcel(saveFileDialog1.FileName);
                }
            }
        }

        private void tsbResetTool_Click(object sender, EventArgs e)
        {
            // reset the tool and start over﻿

            // buttons﻿
            btnExportToExcel.Enabled = false;
            btnStartQueries.Enabled = false;
            btnSelectEnvironments.Enabled = true;

            // textboxes﻿
            tbSelectedEnvironments.Text = "none";
            tbTargetUserEmail.Text = "Please enter the target user email address";

            // targetEnvironments﻿
            targetEnvironments.Clear();

            // treeview﻿
            tvTreeview.Nodes.Clear();

            // progressbar﻿
            pbMain.Style = ProgressBarStyle.Continuous;

            // log output box﻿
            lbDebugOutput.Items.Clear();

            // sidepanel
            rtbSidepanel.Text = sidePanelDefaultText;

            // listview
            lvComponentCount.Clear();
        }

        private void tsbHelp_Click(object sender, EventArgs e)
        {
            using (var f = new AboutForm())
            {
                f.ShowDialog();
            }
        }

        private void rtbSidepanel_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.LinkText);
            Process.Start(sInfo);
        }

        private void btnReassign_Click(object sender, EventArgs e)
        {
            var originalOwner = tbTargetUserEmail.Text;

            // TODO: bad design to run that query just to get the count...
            using (var f = new ReAssignForm(GetSelectedNodes().Count))
            {
                f.ShowDialog();

                if (f.DialogResult == DialogResult.OK)
                {
                    LogInfo($"Reassigning flows to {f.TargetOwner}...");

                    // start the reassignment process in the background
                    ReassignCheckedObjects(f.TargetOwner, GetSelectedNodes());
                }
                else
                {
                    LogInfo("Reassign canceled by the user.");
                }
            }
        }

        /// <summary>
        /// Reassigns the checked flows to the target owner
        /// </summary>
        /// <param name="targetOwnerId"></param>
        /// <exception cref="Exception"></exception>
        private void ReassignCheckedObjects(string targetOwner, IList<TreeNode> selectedNodes)
        {
            // start progressbar
            pbMain.Style = ProgressBarStyle.Marquee;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += (obj, arg) =>
            {
                try
                {
                    // group nodes by environment
                    var groupedNodes = selectedNodes.GroupBy(x => targetEnvironments.Keys
                        .Where(y => y.flows.Any(z => z.name == ((FlowTreeNodeElement)x.Tag).Flow.name))
                        .Select(y => y.properties.linkedEnvironmentMetadata.instanceUrl)
                        .FirstOrDefault());

                    // run reassignment in parallel per environment
                    Parallel.ForEach(groupedNodes, (group) =>
                    {
                        var environmentUrl = group.Key;

                        LogInfo("Get target systemuserid from dataverse");

                        // Get target systemuserid from dataverse
                        string targetOwnerId = GetSystemUserIdFromDataverse(environmentUrl, targetOwner);

                        //Reassign Flows
                        // get flowTreeNodeElement from Tag
                        foreach (var tag in group.Where(x => x.Tag.GetType() == typeof(FlowTreeNodeElement)).Select(x => x.Tag as FlowTreeNodeElement))
                        {
                            // Get the flow object from the node
                            Flow flow = tag.Flow;

                            // Take ownership information from the flow object
                            string originalOwnerId = GetSystemUserIdFromDataverse(environmentUrl, flow.permissions.Where(x => x.properties.roleName == "Owner").FirstOrDefault().properties.principal.id);

                            // Set the owner of the flow to the target user
                            if (!SetWorkflowOwner(environmentUrl, flow.properties.workflowEntityId, targetOwnerId))
                            {
                                // means something went wrong and we need to abort the current flow reassignment
                                tag.updateNodeUi(new NodeUpdateObject(tag)
                                {
                                    UpdateReason = UpdateReason.MigrationFailed,
                                    NodeText = "Unable to reassign this flow to the target user."
                                });
                                tag.MigrationStatus = MigrationStatus.MigrationFailed;
                                return;
                            }
                            // Grant access to the original user
                            if (!GrantAccess(environmentUrl, flow.properties.workflowEntityId, originalOwnerId))
                            {
                                // means something went wrong and we need to abort the current flow reassignment
                                tag.updateNodeUi(new NodeUpdateObject(tag)
                                {
                                    UpdateReason = UpdateReason.MigrationFailed,
                                    NodeText = "Unable to share this flow with the original owner after reassignment."
                                });
                                tag.MigrationStatus = MigrationStatus.MigrationFailed;
                                return;
                            }

                            // Update flow object
                            // TODO?
                            //GetFlowDetails(flow);
                            GetFlowPermissons(flow);

                            tag.updateNodeUi(new NodeUpdateObject(tag)
                            {
                                UpdateReason = UpdateReason.MigrationSucceeded,
                                NodeText = "Migration successful"
                            });
                            tag.MigrationStatus = MigrationStatus.MigrationSuccessful;
                        }

                        //Reassign Connection References
                        // get flowTreeNodeElement from Tag
                        //foreach (var tag in group.Where(x => x.Tag.GetType() == typeof(ConnectionTreeNodeElement)).Select(x => x.Tag as ConnectionTreeNodeElement))
                        //{
                        //    // Get the flow object from the node
                        //    Connection connection = tag.;

                        //    // Set the owner of the flow to the target user
                        //    if (!SetWorkflowOwner(environmentUrl, connectionReference.properties.createdBy.id, targetOwnerId))
                        //    {
                        //        // means something went wrong and we need to abort the current flow reassignment
                        //        tag.updateNodeUi(new NodeUpdateObject(tag)
                        //        {
                        //            UpdateReason = UpdateReason.MigrationFailed,
                        //            NodeText = "Unable to reassign this connection reference to the target user."
                        //        });
                        //        tag.MigrationStatus = MigrationStatus.MigrationFailed;
                        //        return;
                        //    }

                        //    // Update flow object
                        //    //GetFlowDetails(flow);
                        //    //GetFlowPermissons(flow);

                        //    tag.updateNodeUi(new NodeUpdateObject(tag)
                        //    {
                        //        UpdateReason = UpdateReason.MigrationSucceeded,
                        //        NodeText = "Migration successful"
                        //    });
                        //    tag.MigrationStatus = MigrationStatus.MigrationSuccessful;
                        //}

                        LogInfo("Reassigning flows in " + environmentUrl);
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception("Error reassigning");
                }

            };
            bgw.RunWorkerCompleted += (obj, arg) =>
            {
                // stop progressbar
                pbMain.Style = ProgressBarStyle.Continuous;
                LogInfo("Reassigning completed.");
            };

            bgw.RunWorkerAsync();
        }

        #endregion

        #region Methods
        private List<TreeNode> GetSelectedNodes()
        {
            return tvTreeview.Nodes.Descendants().Where(x => x.Checked && x.Tag.GetType() == typeof(FlowTreeNodeElement)).ToList();
        }

        private void UpdateListView(ListViewItem listViewItem)
        {
            if (lvComponentCount.InvokeRequired)
            {
                lvComponentCount.Invoke(new _updateListviewDelegate(UpdateListView), listViewItem);
            }
            else
            {
                lvComponentCount.Items.Add(listViewItem);

                if (!lvComponentCount.Groups.Contains(listViewItem.Group))
                {
                    lvComponentCount.Groups.Add(listViewItem.Group);
                }
            }
        }

        private void UpdateNode(NodeUpdateObject nodeUpdateObject)
        {
            // because this might be called from a different thread﻿
            if (tvTreeview.InvokeRequired)
            {
                tvTreeview.Invoke(new _updateTreeNodeDelegate(UpdateNode), nodeUpdateObject);
            }
            else
            {
                try
                {
                    // this is the target node that needs to be updated
                    var updateNode = tvTreeview.Nodes.Find(nodeUpdateObject.NodeId, true).FirstOrDefault();

                    switch (nodeUpdateObject.UpdateReason)
                    {
                        case UpdateReason.AddedToList:
                            var parentNode = tvTreeview.Nodes.Find(nodeUpdateObject.ParentNodeId, true).FirstOrDefault();
                            if (parentNode == null)
                            {
                                // create a new top level node﻿
                                var createNode = new TreeNode﻿
                                {
                                    Name = nodeUpdateObject.NodeId,
                                    Text = nodeUpdateObject.NodeText,
                                    ForeColor = Color.Black,
                                    Tag = nodeUpdateObject.TreeNodeElement,
                                    NodeFont = new Font(tvTreeview.Font, FontStyle.Regular),
                                    //ToolTipText = "n/a",﻿
                                    Checked = false
                                };
                                tvTreeview.Nodes.Add(createNode);
                                createNode.Expand();
                            }
                            else
                            {
                                // add under an existing node﻿
                                var createNode = new TreeNode﻿
                                {
                                    Name = nodeUpdateObject.NodeId,
                                    Text = nodeUpdateObject.NodeText,
                                    ForeColor = Color.Black,
                                    NodeFont = new Font(tvTreeview.Font, FontStyle.Regular),
                                    Tag = nodeUpdateObject.TreeNodeElement,
                                    //ToolTipText = "n/a",﻿
                                    Checked = false
                                };
                                parentNode.Nodes.Add(createNode);
                                parentNode.Expand();
                            }
                            break;
                        // this is used so we can update nodes in the UI that are already there with additional details﻿
                        case UpdateReason.DetailsAdded:
                            updateNode.ForeColor = Color.Black;
                            updateNode.NodeFont = new Font(tvTreeview.Font, FontStyle.Regular);
                            updateNode.Tag = nodeUpdateObject.TreeNodeElement;
                            updateNode.Text = nodeUpdateObject.NodeText;
                            //updateNode.ToolTipText = "n/a.";﻿
                            updateNode.Checked = false;
                            break;
                        case UpdateReason.RemovedFromList:
                            // not implemented﻿
                            break;
                        case UpdateReason.MigrationSucceeded:
                            updateNode.ForeColor = Color.Green;
                            updateNode.NodeFont = new Font(tvTreeview.Font, FontStyle.Strikeout);
                            updateNode.ToolTipText = nodeUpdateObject.ToolTipText;
                            updateNode.Checked = false;
                            //TODO: does not work, needs to be handled in the draw method. Don't ask me why, it just does not work
                            updateNode.HideCheckBox();
                            break;
                        case UpdateReason.MigrationFailed:
                            updateNode.ForeColor = Color.Red;
                            updateNode.NodeFont = new Font(tvTreeview.Font, FontStyle.Strikeout);
                            updateNode.ToolTipText = nodeUpdateObject.ToolTipText;
                            updateNode.Checked = false;
                            //TODO: does not work, needs to be handled in the draw method. Don't ask me why, it just does not work
                            updateNode.HideCheckBox();
                            break;
                        default:
                            break;
                    }

                    //Sort the Treeview
                    tvTreeview.Sort();

                }
                catch (Exception ex)
                {
                    LogError("Exception building the TreeView. This is not critical but weird.");
                    LogError(ex.Message + " " + ex.StackTrace);
                }
            }
        }

        private new void LogInfo(string text, params object[] args)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(LogInfo);
                lbDebugOutput.Invoke(update, text, args);
            }
            else
            {
                base.LogInfo(text, args);
                lbDebugOutput.Items.Add(text);
                if (lbDebugOutput.Items.Count > 1000)
                {
                    lbDebugOutput.Items.RemoveAt(0); // remove first line﻿
                }
                // Make sure the last item is made visible﻿
                lbDebugOutput.SelectedIndex = lbDebugOutput.Items.Count - 1;
                lbDebugOutput.ClearSelected();
            }
        }

        private new void LogError(string text, params object[] args)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(LogError);
                lbDebugOutput.Invoke(update, text, args);
            }
            else
            {
                base.LogError(text, args);
                lbDebugOutput.Items.Add(text);
                if (lbDebugOutput.Items.Count > 1000)
                {
                    lbDebugOutput.Items.RemoveAt(0); // remove first line﻿
                }
                // Make sure the last item is made visible﻿
                lbDebugOutput.SelectedIndex = lbDebugOutput.Items.Count - 1;
                lbDebugOutput.ClearSelected();
            }
        }

        private new void LogWarning(string text, params object[] args)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(LogWarning);
                lbDebugOutput.Invoke(update, text, args);
            }
            else
            {
                base.LogWarning(text, args);
                lbDebugOutput.Items.Add(text);
                if (lbDebugOutput.Items.Count > 1000)
                {
                    lbDebugOutput.Items.RemoveAt(0); // remove first line﻿
                }
                // Make sure the last item is made visible﻿
                lbDebugOutput.SelectedIndex = lbDebugOutput.Items.Count - 1;
                lbDebugOutput.ClearSelected();
            }
        }

        public void ShowAboutDialog()
        {
            using (var f = new AboutForm())
            {
                f.ShowDialog();
            }
        }

        #endregion

        #region TreeView
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.afterselect?view=windowsdesktop-8.0&redirectedfrom=MSDN
            // This event does not occur when the node is unselected. To detect whether the selection was cleared, you can test the TreeNode.IsSelected property.
            if (e.Node.IsSelected)
            {
                // if this node has no tag, then do nothing
                if (e.Node.Tag == null) return;

                // check which kind of node this is
                if (e.Node.Tag is FlowTreeNodeElement)
                {
                    var flowNode = e.Node.Tag as FlowTreeNodeElement;

                    // get the flow details and display in the sidepanel
                    var sidepanelText = $"Flow Name: {flowNode.Flow.properties.displayName}{Environment.NewLine}" +
                        $"Flow Id: {flowNode.Flow.name}{Environment.NewLine}" +
                        $"Flow URI: {flowNode.FlowUri}{Environment.NewLine}" +
                        $"Environment Id: {flowNode.Flow.properties.environment.name}";

                    // set the sidepanel text
                    rtbSidepanel.Text = sidepanelText;
                }
                else if (e.Node.Tag is ConnectionTreeNodeElement)
                {
                    var connectionNode = e.Node.Tag as ConnectionTreeNodeElement;

                    // get the connection reference details and display in the sidepanel
                    var sidepanelText = $"Connection Name: {connectionNode.ConnectionName}{Environment.NewLine}" +
                        $"Environment Id: {connectionNode.EnvironmentId}{Environment.NewLine}";

                    // set the sidepanel text
                    rtbSidepanel.Text = sidepanelText;
                }
            }
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now} Drawing node {e.Node.Text}");

            if (e.Node.Bounds.Height == 0)
                return;

            var nodeTag = e.Node.Tag as TreeNodeElementBase;

            if (nodeTag != null) // this node has a tag
            {
                //this is a connection node or it is a directory tree node that belongs to a connection reference branch - those should never have a checkbox because we dont migrate them now
                if ((nodeTag.GetType() == typeof(ConnectionTreeNodeElement)
                    || (nodeTag.GetType() == typeof(ConnectionReferenceTreeNodeElement))
                    || (nodeTag.GetType() == typeof(DirectoryTreeNode) && e.Node.Text.IndexOf("Connection References") != -1)))
                {
                    e.Node.HideCheckBox();
                }
                else
                {
                    //this is any other node that has a tag - use the tag content to determine whether a checkbox should be drawn or not
                    //var nodeTag = e.Node.Tag as TreeNodeElementBase;
                    if (nodeTag != null)
                    {
                        // the only valid reason for us to show a checkbox is because an item has not been migrated yet
                        switch (nodeTag.MigrationStatus)
                        {
                            case MigrationStatus.MigrationFailed:
                            case MigrationStatus.MigrationSuccessful:
                                Debug.WriteLine($"{DateTime.Now} Hiding checkbox for {e.Node.Text} because migration state of this item is: {nodeTag.MigrationStatus}");
                                e.Node.HideCheckBox();
                                break;
                            case MigrationStatus.NotMigratedYet:
                                break;
                            default:
                                break;
                        }
                    }
                }

            }

            TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.NodeFont, new Point(e.Node.Bounds.Left + 2, e.Node.Bounds.Top + 2), e.Node.ForeColor);
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // If we do not check this, we end in an infinite loop
            if (e.Action != TreeViewAction.ByMouse && e.Action != TreeViewAction.ByKeyboard)
                return;

            // Handle the event when a checkbox is checked or unchecked
            var checkedNode = e.Node;

            // Update all child nodes
            UpdateChildNodes(checkedNode, checkedNode.Checked);

            // Update parent nodes
            UpdateParentNodes(checkedNode);
        }

        private void UpdateChildNodes(TreeNode node, bool isChecked)
        {
            foreach (TreeNode childNode in node.Nodes)
            {
                childNode.Checked = isChecked;
                UpdateChildNodes(childNode, isChecked);
            }
        }

        private void UpdateParentNodes(TreeNode node)
        {
            if (node.Parent != null)
            {
                bool allSiblingsChecked = true;
                foreach (TreeNode sibling in node.Parent.Nodes)
                {
                    if (!sibling.Checked)
                    {
                        allSiblingsChecked = false;
                        break;
                    }
                }
                node.Parent.Checked = allSiblingsChecked;
                UpdateParentNodes(node.Parent);
            }
        }
        #endregion
    }
}