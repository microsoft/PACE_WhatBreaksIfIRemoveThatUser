using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FlowOwnershipAudit
{
    public partial class ReAssignForm : Form
    {
        private List<TreeNode> _selectedNodes { get; set; }

        public ReAssignForm(List<TreeNode> nodes)
        {
            InitializeComponent();
            this._selectedNodes = nodes;
        }

        private void ReAssignForm_Load(object sender, EventArgs e)
        {

        }

        private void btnReassign_Click(object sender, EventArgs e)
        {
            // Display the selected nodes
            foreach (TreeNode node in _selectedNodes)
            {
                MessageBox.Show(node.Text);
            }
        }
    }
}
