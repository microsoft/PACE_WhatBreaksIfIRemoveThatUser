using FlowOwnershipAudit.Model;
using FlowOwnershipAudit.TreeViewUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using static FlowOwnershipAudit.API;

namespace FlowOwnershipAudit
{
    public partial class ReAssignForm : Form
    {
        private Dictionary<string, List<TreeNode>> _selectedNodes { get; set; }
        private string _orginalOwner { get; set; }

        public ReAssignForm(Dictionary<string, List<TreeNode>> nodes, string orginalOwner)
        {
            InitializeComponent();
            this._selectedNodes = nodes;
            this._orginalOwner = orginalOwner;
        }

        private void ReAssignForm_Load(object sender, EventArgs e)
        {

        }

        private void btnReassign_Click(object sender, EventArgs e)
        {
            var targetOwner = tbTargetUserEmail.Text;
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += async (obj, arg) =>
            {
                // get the user id from the graph api. This should be the same for all environments so we dont need to do it in the parallel loop
                try
                {
                    string originalOwnerId = GetSystemUserIdFromDataverse(_selectedNodes.FirstOrDefault().Key, _orginalOwner);
                    string targetOwnerId = GetSystemUserIdFromDataverse(_selectedNodes.FirstOrDefault().Key, targetOwner);

                    //TODO: Parallel.foreach
                    foreach (KeyValuePair<string, List<TreeNode>> environmentFlows in _selectedNodes)
                    {
                        foreach (var node in environmentFlows.Value)
                        {
                            if (node.Tag is FlowTreeNodeElement)
                            {
                                // Get the flow object from the node
                                Flow flow = ((FlowTreeNodeElement)node.Tag).Flow;
                                // Set the owner of the flow to the target user
                                SetWorkflowOwnerAsync(environmentFlows.Key, flow.properties.workflowEntityId, targetOwnerId);
                                // Grant access to the original user
                                GrantAccessAsync(environmentFlows.Key, flow.properties.workflowEntityId, originalOwnerId);
                                // Update flow object
                                GetFlowDetails(flow);
                                GetFlowPermissons(flow);
                                //TODO: Update the treeview

                                //TODO: Report Progress on the UI. Progressbar or something?
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception($"Error getting user id for ", ex);
                }

                //var parallelResult = Parallel.ForEach(
                //    source: targetEnvironments,
                //    parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 10 },
                //    body: currentTargetEnvironment =>
                //    {
                        
                //    });
                //arg.Result = parallelResult;
                // TODO: error handling within individual tasks
            };
            bgw.RunWorkerCompleted += (obj, arg) =>
            {
                // worker completed. This means that the main operation is done, all environments have been queried and the treeview has been built
                this.Close();
            };

            bgw.RunWorkerAsync();
        }
    }
}
