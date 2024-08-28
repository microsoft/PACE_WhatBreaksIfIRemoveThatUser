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
                    string originalOwnerId = GetSystemUserIdFromDataverse(_selectedNodes.FirstOrDefault().Key, _orginalOwner).Result;
                    string targetOwnerId = GetSystemUserIdFromDataverse(_selectedNodes.FirstOrDefault().Key, targetOwner).Result;

                    //TODO: Parallel.foreach
                    foreach (KeyValuePair<string, List<TreeNode>> environmentFlows in _selectedNodes)
                    {
                        foreach (var node in environmentFlows.Value)
                        {
                            if (node.Tag is FlowTreeNodeElement)
                            {
                                // get the flow object from the node
                                Flow flow = ((FlowTreeNodeElement)node.Tag).Flow;
                                // set the owner of the flow to the target user
                                await SetWorkflowOwnerAsync(environmentFlows.Key, flow.properties.workflowEntityId, targetOwnerId);
                                // grant access to the original user
                                await GrantAccessAsync(environmentFlows.Key, flow.properties.workflowEntityId, originalOwnerId);
                                //TODO: Update flow object

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
            };

            bgw.RunWorkerAsync();
        }
    }
}
