using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WhatBreaksIf.DTO;
using WhatBreaksIf.Model;
using WhatBreaksIf.TreeViewUI;
using WhatBreaksIf.TreeViewUIElements;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using static WhatBreaksIf.API;
using Environment = System.Environment;

namespace WhatBreaksIf﻿
{
    public partial class WhatBreaksIfControl : PluginControlBase, INoConnectionRequired, IAboutPlugin﻿, IGitHubPlugin
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
                    if (_flowsQueryCompleted && connectionRefsQueryCompleted)
                    {
                        EnvironmentQueriesCompleted?.Invoke(this, new EventArgs());
                    }
                }
            }
            public bool connectionRefsQueryCompleted﻿
            {
                get => _connectionRefsQueryCompleted;
                set
                {
                    _connectionRefsQueryCompleted = value;

                    // raise event and inform that all queries have been completed if necessary﻿
                    if (_flowsQueryCompleted && connectionRefsQueryCompleted)
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
            {  }

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
                    // check whether all environments in this collection have finished both flow and connection reference queries﻿
                    if (this.All(x => x.Value.flowsQueryCompleted && x.Value.connectionRefsQueryCompleted))
                    {
                        AllEnvironmentsQueriesCompleted(this, new EventArgs());
                    }
                }
            }
        }

        // these delegates are used to update the UI from a different thread﻿
        private delegate void _updateLogWindowDelegate(string msg, params object[] args);
        private delegate void _updateTreeNodeDelegate(NodeUpdateObject nodeUpdateObject);

        // this list will contain the target environments that we query as well as the status of their respective queries﻿
        // todo watch out for thread safety issues﻿
        private readonly EnvironmentCollection targetEnvironments = new EnvironmentCollection();

        private Settings mySettings;

        private readonly string sidePanelDefaultText = "Select environments and click start, then select any node to see the results here.";

        public string RepositoryName => "PACE_WhatBreaksIfIRemoveThatUser";

        public string UserName => "microsoft";

        public WhatBreaksIfControl()
        {
            InitializeComponent();

            // subscribe to the event that tells us that all queries have been completed﻿
            targetEnvironments.AllEnvironmentsQueriesCompleted += AllEnvironmentQueriesCompleted;

            // subscribe to the event that the underlying list of target environments has changed﻿
            targetEnvironments.CollectionChanged += TargetEnvironments_CollectionChanged;
        }

        /// <summary>﻿
        /// This event occurs when the plugin is closed﻿
        /// </summary>﻿
        /// <param name="sender"></param>﻿
        /// <param name="e"></param>﻿
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings﻿
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            LogInfo("Hello world :) ");

            // Loads or creates the settings for the plugin﻿
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

            // for some reason the designer keeps deleting this default text...﻿
            tbTargetUserEmail.Text = "Please enter the target user email address";
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
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            bool checkConnectionReferences = cbCheckConnectionReferences.Checked;

            // disable controls﻿
            tbTargetUserEmail.Enabled = false;
            cbCheckFlowOwners.Enabled = false;
            cbCheckConnectionReferences.Enabled = false;
            btnStartQueries.Enabled = false;
            btnSelectEnvironments.Enabled = false;

            // clear sidepanel just in case
            rtbSidepanel.Text = sidePanelDefaultText;

            // this might be not the first time that the user clicks the button, so we need to clean up﻿
            treeView1.Nodes.Clear();

            LogInfo($"Will search the following for {targetUser}:" +
                $" Flow Ownership: {(cbCheckFlowOwners.Checked ? "yes" : "no")}" +
                $" Connection References: {(cbCheckConnectionReferences.Checked ? "yes" : "no")}" +
                $" ...");

            LogInfo($"Will query {targetEnvironments.Count()} environments");

            List<EnvironmentTreeNodeElement> environmentTreeNodes = new List<EnvironmentTreeNodeElement>();
            List<DirectoryTreeNode> directoryTreeNodes = new List<DirectoryTreeNode>();

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += (obj, arg) =>
            {
                var parallelResult = Parallel.ForEach(
                    source: targetEnvironments,
                    parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 10 },
                    body: currentTargetEnvironment =>
                    {
                        // this is the foreach that is running in parallel for each environment﻿
                        // create environment node for the current environment﻿
                        EnvironmentTreeNodeElement environmentNode = new EnvironmentTreeNodeElement(UpdateNode, currentTargetEnvironment.Key.properties.displayName, currentTargetEnvironment.Key.name);
                        environmentTreeNodes.Add(environmentNode);

                        var userTask = GetUserIdFromGraph(targetUser);
                        var userid = userTask.Result;

                        LogInfo($"Looking for {targetUser} with id {userid} in {currentTargetEnvironment.Key.name}");

                        LogInfo($"Processing environment {currentTargetEnvironment.Key.name}");

                        if (checkFlowOwners)
                        {
                            // create a directory node that holds the references to the flows so we know where in the UI to place them﻿
                            var flowDirectoryNode = new DirectoryTreeNode(UpdateNode, "Flows", environmentNode);
                            directoryTreeNodes.Add(flowDirectoryNode);

                            AddFlowsToEnvironment(
                                targetEnvironment: currentTargetEnvironment.Key);

                            AddFlowPermissionsToEnvironment(
                                userId: userid,
                                targetEnvironment: currentTargetEnvironment.Key,
                                ProgressChanged: (flowObj) =>
                                {
                                    // todo: maybe get rid of the dynamic and use a typed object﻿
                                    dynamic flowObjDyn = flowObj;
                                    string flowName = flowObjDyn.FlowName;
                                    string flowId = flowObjDyn.FlowId;
                                    string environmentId = flowObjDyn.EnvironmentId;
                                    string environmentName = flowObjDyn.EnvironmentName;

                                    // create treenodeelement﻿
                                    new FlowTreeNodeElement(UpdateNode,
                                                           parentNodeElement: flowDirectoryNode,
                                                           flowName: flowName,
                                                           flowId: flowId,
                                                           environmentId: environmentId﻿
                                                           );
                                });
                            currentTargetEnvironment.Value.flowsQueryCompleted = true;
                        }
                        else
                        {
                            // mark as completed if we are not checking for flow owners
                            currentTargetEnvironment.Value.flowsQueryCompleted = true;
                        }

                        if (checkConnectionReferences)
                        {
                            // create a directory node that holds the references to the connectionreferences so we know where in the UI to place them﻿
                            var connectionReferencesDirectoryNode = new DirectoryTreeNode(UpdateNode, "Connection References", environmentNode);
                            directoryTreeNodes.Add(connectionReferencesDirectoryNode);

                            AddConnectionReferencesToEnvironment(
                                userId: userid,
                                targetEnvironment: currentTargetEnvironment.Key,
                                ProgressChanged: (ConnrectionReferenceObj) =>
                                {
                                    dynamic connectionReferenceObj = ConnrectionReferenceObj;
                                    string connectionReferenceName = connectionReferenceObj.ConnectionReferenceName;
                                    string environmentId = connectionReferenceObj.EnvironmentId;

                                    // create treenodeelement﻿
                                    new ConnectionReferenceTreeNodeElement(UpdateNode,
                                                           parentNodeElement: connectionReferencesDirectoryNode,
                                                           connectionReferenceName: connectionReferenceName,
                                                           environmentId: environmentId﻿
                                                           );
                                });

                            currentTargetEnvironment.Value.connectionRefsQueryCompleted = true;
                        }
                        else
                        {
                            // mark as completed if we are not checking for connection references
                            currentTargetEnvironment.Value.connectionRefsQueryCompleted = true;
                        }
                    });
            };

            bgw.RunWorkerAsync();
            // --- careful, all the stuff above runs async, everything below here will run immediately ----﻿
        }

        private void AllEnvironmentQueriesCompleted(object sender, EventArgs e)
        {
            // invoke if necessary - this event will likely be called from a background thread﻿
            if (treeView1.InvokeRequired)
            {
                treeView1.Invoke(new EventHandler(AllEnvironmentQueriesCompleted), sender, e);
            }
            else
            {
                LogInfo("All queries have been completed.");
                pbMain.Style = ProgressBarStyle.Continuous;
                btnExportToExcel.Enabled = true;
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
                LogInfo("Exporting to {0}", saveFileDialog1.FileName);

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
            treeView1.Nodes.Clear();

            // progressbar﻿
            pbMain.Style = ProgressBarStyle.Continuous;

            // log output box﻿
            lbDebugOutput.Items.Clear();

            // sidepanel
            rtbSidepanel.Text = sidePanelDefaultText;
        }

        /// <summary>﻿
        /// This event occurs when the connection has been updated in XrmToolBox﻿
        ﻿/// </summary>﻿
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        private void UpdateNode(NodeUpdateObject nodeUpdateObject)
        {
            // because this might be called from a different thread﻿
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
                                // create a new top level node﻿
                                var createNode = new TreeNode﻿
                                {
                                    Name = nodeUpdateObject.NodeId,
                                    Text = nodeUpdateObject.NodeText,
                                    ForeColor = System.Drawing.Color.Black,
                                    Tag = nodeUpdateObject.TreeNodeElement,
                                    //ToolTipText = "n/a",﻿
                                    Checked = true
                                };
                                treeView1.Nodes.Add(createNode);
                                createNode.Expand();

                            }
                            else
                            {
                                // add under an existing node﻿
                                var createNode = new TreeNode﻿
                                {
                                    Name = nodeUpdateObject.NodeId,
                                    Text = nodeUpdateObject.NodeText,
                                    ForeColor = System.Drawing.Color.Black,
                                    Tag = nodeUpdateObject.TreeNodeElement,
                                    //ToolTipText = "n/a",﻿
                                    Checked = true
                                };
                                parentNode.Nodes.Add(createNode);
                                parentNode.Expand();
                            }
                            break;

                        // this is used so we can update nodes in the UI that are already there with additional details﻿
                        case UpdateReason.DetailsAdded:
                            var updateNode = treeView1.Nodes.Find(nodeUpdateObject.NodeId, true).FirstOrDefault();
                            updateNode.ForeColor = System.Drawing.Color.Black;
                            updateNode.Tag = nodeUpdateObject.TreeNodeElement;
                            //updateNode.ToolTipText = "n/a.";﻿
                            updateNode.Checked = true;

                            break;

                        case UpdateReason.RemovedFromList:
                            // not implemented﻿
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
            MessageBox.Show("");
        }

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
                    var sidepanelText = $"Flow Name: {flowNode.FlowName}{Environment.NewLine}" +
                        $"Flow Id: {flowNode.FlowId}{Environment.NewLine}" +
                        $"Flow URI: {flowNode.FlowUri}{Environment.NewLine}" +
                        $"Environment Id: {flowNode.EnvironmentId}";

                    // set the sidepanel text
                    rtbSidepanel.Text = sidepanelText;
                }
                else if (e.Node.Tag is ConnectionReferenceTreeNodeElement)
                {
                    var connectionReferenceNode = e.Node.Tag as ConnectionReferenceTreeNodeElement;

                    // get the connection reference details and display in the sidepanel
                    var sidepanelText = $"Connection Reference Name: {connectionReferenceNode.ConnectionReferenceName}{Environment.NewLine}" +
                        $"Environment Id: {connectionReferenceNode.EnvironmentId}{Environment.NewLine}";

                    // set the sidepanel text
                    rtbSidepanel.Text = sidepanelText;
                }
            }
        }

        private void tsbHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                text: "For help and to open a bug report, please click below to open GitHub.",
                caption: "Help",
                buttons: MessageBoxButtons.OK,
                icon: MessageBoxIcon.Information,
                defaultButton: MessageBoxDefaultButton.Button1,
                options: 0,
                helpFilePath: "https://github.com/microsoft/PACE_WhatBreaksIfIRemoveThatUser",
                keyword: "GitHub");
        }
    }
}