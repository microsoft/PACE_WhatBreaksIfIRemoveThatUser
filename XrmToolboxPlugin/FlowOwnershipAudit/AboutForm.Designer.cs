namespace WhatBreaksIfPlugin
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.btnClose = new System.Windows.Forms.Button();
            this.llGithub = new System.Windows.Forms.LinkLabel();
            this.gbFelix = new System.Windows.Forms.GroupBox();
            this.llFelix = new System.Windows.Forms.LinkLabel();
            this.lblFelix = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.llLaurens = new System.Windows.Forms.LinkLabel();
            this.lblLaurens = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gbFelix.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(574, 314);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 28);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // llGithub
            // 
            this.llGithub.AutoSize = true;
            this.llGithub.Location = new System.Drawing.Point(227, 73);
            this.llGithub.Name = "llGithub";
            this.llGithub.Size = new System.Drawing.Size(48, 16);
            this.llGithub.TabIndex = 2;
            this.llGithub.TabStop = true;
            this.llGithub.Text = "GitHub";
            this.llGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llGithub_LinkClicked);
            // 
            // gbFelix
            // 
            this.gbFelix.Controls.Add(this.llFelix);
            this.gbFelix.Controls.Add(this.lblFelix);
            this.gbFelix.Location = new System.Drawing.Point(109, 158);
            this.gbFelix.Name = "gbFelix";
            this.gbFelix.Size = new System.Drawing.Size(200, 100);
            this.gbFelix.TabIndex = 3;
            this.gbFelix.TabStop = false;
            // 
            // llFelix
            // 
            this.llFelix.AutoSize = true;
            this.llFelix.Location = new System.Drawing.Point(6, 32);
            this.llFelix.Name = "llFelix";
            this.llFelix.Size = new System.Drawing.Size(57, 16);
            this.llFelix.TabIndex = 1;
            this.llFelix.TabStop = true;
            this.llFelix.Text = "LinkedIn";
            this.llFelix.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llFelix_LinkClicked);
            // 
            // lblFelix
            // 
            this.lblFelix.Location = new System.Drawing.Point(0, 0);
            this.lblFelix.Name = "lblFelix";
            this.lblFelix.Size = new System.Drawing.Size(203, 22);
            this.lblFelix.TabIndex = 0;
            this.lblFelix.Text = "Felix Mora";
            this.lblFelix.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.llLaurens);
            this.groupBox1.Controls.Add(this.lblLaurens);
            this.groupBox1.Location = new System.Drawing.Point(389, 158);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // llLaurens
            // 
            this.llLaurens.AutoSize = true;
            this.llLaurens.Location = new System.Drawing.Point(6, 32);
            this.llLaurens.Name = "llLaurens";
            this.llLaurens.Size = new System.Drawing.Size(57, 16);
            this.llLaurens.TabIndex = 1;
            this.llLaurens.TabStop = true;
            this.llLaurens.Text = "LinkedIn";
            this.llLaurens.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llLaurens_LinkClicked);
            // 
            // lblLaurens
            // 
            this.lblLaurens.Location = new System.Drawing.Point(0, 0);
            this.lblLaurens.Name = "lblLaurens";
            this.lblLaurens.Size = new System.Drawing.Size(203, 22);
            this.lblLaurens.TabIndex = 0;
            this.lblLaurens.Text = "Laurens Vandendriessche";
            this.lblLaurens.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(648, 90);
            this.label1.TabIndex = 5;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // AboutForm
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 355);
            this.ControlBox = false;
            this.Controls.Add(this.llGithub);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbFelix);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.gbFelix.ResumeLayout(false);
            this.gbFelix.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.LinkLabel llGithub;
        private System.Windows.Forms.GroupBox gbFelix;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblFelix;
        private System.Windows.Forms.LinkLabel llFelix;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblLaurens;
        private System.Windows.Forms.LinkLabel llLaurens;
        private System.Windows.Forms.Label label1;
    }
}