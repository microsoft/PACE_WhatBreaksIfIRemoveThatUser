using System;
using System.Windows.Forms;

namespace FlowOwnershipAudit
{
    public partial class ReAssignForm : Form
    {
        public string TargetOwner { get; private set; }

        public ReAssignForm()
        {
            InitializeComponent();
        }

        private void ReAssignForm_Load(object sender, EventArgs e)
        {
        }

        private void btnReassign_Click(object sender, EventArgs e)
        {
            TargetOwner = tbTargetUserEmail.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
