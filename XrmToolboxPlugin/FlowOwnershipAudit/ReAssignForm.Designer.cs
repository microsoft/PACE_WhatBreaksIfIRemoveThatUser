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
            this.SuspendLayout();
            // 
            // tbTargetUserEmail
            // 
            this.tbTargetUserEmail.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tbTargetUserEmail.Location = new System.Drawing.Point(285, 53);
            this.tbTargetUserEmail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbTargetUserEmail.Name = "tbTargetUserEmail";
            this.tbTargetUserEmail.Placeholder = null;
            this.tbTargetUserEmail.Size = new System.Drawing.Size(409, 26);
            this.tbTargetUserEmail.TabIndex = 30;
            this.tbTargetUserEmail.Tag = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(166, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 20);
            this.label1.TabIndex = 31;
            this.label1.Text = "Target User";
            // 
            // btnReassign
            // 
            this.btnReassign.Location = new System.Drawing.Point(373, 104);
            this.btnReassign.Name = "btnReassign";
            this.btnReassign.Size = new System.Drawing.Size(90, 31);
            this.btnReassign.TabIndex = 32;
            this.btnReassign.Text = "Reassign";
            this.btnReassign.UseVisualStyleBackColor = true;
            this.btnReassign.Click += new System.EventHandler(this.btnReassign_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(488, 104);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 31);
            this.btnCancel.TabIndex = 33;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ReAssignForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(881, 203);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnReassign);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbTargetUserEmail);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
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
    }
}