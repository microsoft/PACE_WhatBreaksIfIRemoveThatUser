namespace FlowOwnershipAudit
{
    partial class ReAssignForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbTargetUserEmail = new XrmToolBox.Controls.TextBoxWithPlaceholder();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReassign = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbTargetUserEmail
            // 
            this.tbTargetUserEmail.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tbTargetUserEmail.Location = new System.Drawing.Point(194, 50);
            this.tbTargetUserEmail.Name = "tbTargetUserEmail";
            this.tbTargetUserEmail.Placeholder = null;
            this.tbTargetUserEmail.Size = new System.Drawing.Size(274, 20);
            this.tbTargetUserEmail.TabIndex = 30;
            this.tbTargetUserEmail.Tag = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(115, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Target User";
            // 
            // btnReassign
            // 
            this.btnReassign.Location = new System.Drawing.Point(437, 92);
            this.btnReassign.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnReassign.Name = "btnReassign";
            this.btnReassign.Size = new System.Drawing.Size(68, 29);
            this.btnReassign.TabIndex = 32;
            this.btnReassign.Text = "Reassign";
            this.btnReassign.UseVisualStyleBackColor = true;
            this.btnReassign.Click += new System.EventHandler(this.btnReassign_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(509, 92);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 29);
            this.btnCancel.TabIndex = 33;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Location = new System.Drawing.Point(13, 13);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(355, 13);
            this.lblHeader.TabIndex = 34;
            this.lblHeader.Text = "Enter the id of the user that you want to reassign the {0} selected flows to.";
            // 
            // ReAssignForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(587, 132);
            this.ControlBox = false;
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnReassign);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbTargetUserEmail);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReAssignForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select target user";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ReAssignForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XrmToolBox.Controls.TextBoxWithPlaceholder tbTargetUserEmail;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReassign;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblHeader;
    }
}