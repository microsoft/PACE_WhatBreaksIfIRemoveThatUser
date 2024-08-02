namespace WhatBreaksIf
{
    partial class WhatBreaksIfControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WhatBreaksIfControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.treeView1 = new PersonalViewMigrationTool.CustomTreeViewControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lbDebugOutput = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbTargetUserEmail = new XrmToolBox.Controls.TextBoxWithPlaceholder();
            this.cbCheckConnectionReferences = new System.Windows.Forms.CheckBox();
            this.cbCheckFlowOwners = new System.Windows.Forms.CheckBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.btnStartQueries = new System.Windows.Forms.Button();
            this.toolStripMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1417, 25);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 22);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1417, 790);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(356, 6);
            this.tabControl1.Name = "tabControl1";
            this.tableLayoutPanel1.SetRowSpan(this.tabControl1, 2);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1055, 778);
            this.tabControl1.TabIndex = 15;
            this.tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.treeView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1047, 752);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Overview";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(1041, 746);
            this.treeView1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lbDebugOutput);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1047, 752);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Log Output";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lbDebugOutput
            // 
            this.lbDebugOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbDebugOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbDebugOutput.CausesValidation = false;
            this.lbDebugOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDebugOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDebugOutput.ForeColor = System.Drawing.Color.White;
            this.lbDebugOutput.FormattingEnabled = true;
            this.lbDebugOutput.HorizontalScrollbar = true;
            this.lbDebugOutput.Location = new System.Drawing.Point(3, 3);
            this.lbDebugOutput.MinimumSize = new System.Drawing.Size(500, 2);
            this.lbDebugOutput.Name = "lbDebugOutput";
            this.lbDebugOutput.ScrollAlwaysVisible = true;
            this.lbDebugOutput.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbDebugOutput.Size = new System.Drawing.Size(1041, 746);
            this.lbDebugOutput.TabIndex = 11;
            this.lbDebugOutput.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tbTargetUserEmail);
            this.panel1.Controls.Add(this.cbCheckConnectionReferences);
            this.panel1.Controls.Add(this.cbCheckFlowOwners);
            this.panel1.Controls.Add(this.lblWarning);
            this.panel1.Controls.Add(this.tbStatus);
            this.panel1.Controls.Add(this.btnStartQueries);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(6, 6);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel1.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(344, 778);
            this.panel1.TabIndex = 12;
            // 
            // tbTargetUserEmail
            // 
            this.tbTargetUserEmail.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tbTargetUserEmail.Location = new System.Drawing.Point(10, 22);
            this.tbTargetUserEmail.Name = "tbTargetUserEmail";
            this.tbTargetUserEmail.Placeholder = null;
            this.tbTargetUserEmail.Size = new System.Drawing.Size(274, 20);
            this.tbTargetUserEmail.TabIndex = 29;
            this.tbTargetUserEmail.Tag = true;
            this.tbTargetUserEmail.TextChanged += new System.EventHandler(this.tbTargetUserEmail_TextChanged);
            // 
            // cbCheckConnectionReferences
            // 
            this.cbCheckConnectionReferences.AutoSize = true;
            this.cbCheckConnectionReferences.Checked = true;
            this.cbCheckConnectionReferences.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCheckConnectionReferences.Location = new System.Drawing.Point(10, 85);
            this.cbCheckConnectionReferences.Name = "cbCheckConnectionReferences";
            this.cbCheckConnectionReferences.Size = new System.Drawing.Size(172, 17);
            this.cbCheckConnectionReferences.TabIndex = 28;
            this.cbCheckConnectionReferences.Text = "Check Connection References";
            this.cbCheckConnectionReferences.UseVisualStyleBackColor = true;
            // 
            // cbCheckFlowOwners
            // 
            this.cbCheckFlowOwners.AutoSize = true;
            this.cbCheckFlowOwners.Checked = true;
            this.cbCheckFlowOwners.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCheckFlowOwners.Location = new System.Drawing.Point(10, 62);
            this.cbCheckFlowOwners.Name = "cbCheckFlowOwners";
            this.cbCheckFlowOwners.Size = new System.Drawing.Size(135, 17);
            this.cbCheckFlowOwners.TabIndex = 27;
            this.cbCheckFlowOwners.Text = "Check Flow Ownership";
            this.cbCheckFlowOwners.UseVisualStyleBackColor = true;
            // 
            // lblWarning
            // 
            this.lblWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.Location = new System.Drawing.Point(7, 387);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(334, 242);
            this.lblWarning.TabIndex = 26;
            this.lblWarning.Text = resources.GetString("lblWarning.Text");
            // 
            // tbStatus
            // 
            this.tbStatus.Enabled = false;
            this.tbStatus.Location = new System.Drawing.Point(3, 334);
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.Size = new System.Drawing.Size(338, 20);
            this.tbStatus.TabIndex = 22;
            this.tbStatus.Text = "Not started";
            // 
            // btnStartQueries
            // 
            this.btnStartQueries.Enabled = false;
            this.btnStartQueries.Location = new System.Drawing.Point(95, 255);
            this.btnStartQueries.Name = "btnStartQueries";
            this.btnStartQueries.Size = new System.Drawing.Size(127, 23);
            this.btnStartQueries.TabIndex = 16;
            this.btnStartQueries.Text = "Start";
            this.btnStartQueries.UseVisualStyleBackColor = true;
            this.btnStartQueries.Click += new System.EventHandler(this.btnStartQueries_Click);
            // 
            // WhatBreaksIfControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "WhatBreaksIfControl";
            this.Size = new System.Drawing.Size(1417, 815);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.TextBox tbStatus;
        private System.Windows.Forms.Button btnStartQueries;
        private XrmToolBox.Controls.TextBoxWithPlaceholder tbTargetUserEmail;
        private System.Windows.Forms.CheckBox cbCheckConnectionReferences;
        private System.Windows.Forms.CheckBox cbCheckFlowOwners;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox lbDebugOutput;
        private PersonalViewMigrationTool.CustomTreeViewControl treeView1;
    }
}
