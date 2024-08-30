namespace FlowOwnershipAudit
{
    partial class FlowOwnershipAuditControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowOwnershipAuditControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbResetTool = new System.Windows.Forms.ToolStripButton();
            this.tsbHelp = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tlpOverviewTab = new System.Windows.Forms.TableLayoutPanel();
            this.tvTreeview = new CustomTreeViewControl();
            this.rtbSidepanel = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lbDebugOutput = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnReassign = new System.Windows.Forms.Button();
            this.tbSelectedEnvironments = new System.Windows.Forms.TextBox();
            this.lblSelectedEnvironments_description = new System.Windows.Forms.Label();
            this.btnSelectEnvironments = new System.Windows.Forms.Button();
            this.btnStartQueries = new System.Windows.Forms.Button();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.pbMain = new System.Windows.Forms.ProgressBar();
            this.tbTargetUserEmail = new XrmToolBox.Controls.TextBoxWithPlaceholder();
            this.cbCheckConnectionReferences = new System.Windows.Forms.CheckBox();
            this.cbCheckFlowOwners = new System.Windows.Forms.CheckBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tlpOverviewTab.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.tsbResetTool,
            this.tsbHelp});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripMenu.Size = new System.Drawing.Size(1428, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(40, 28);
            this.tsbClose.Text = "Close";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // tsbResetTool
            // 
            this.tsbResetTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbResetTool.Image = ((System.Drawing.Image)(resources.GetObject("tsbResetTool.Image")));
            this.tsbResetTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResetTool.Name = "tsbResetTool";
            this.tsbResetTool.Size = new System.Drawing.Size(71, 28);
            this.tsbResetTool.Text = "Restart tool";
            this.tsbResetTool.ToolTipText = "Click to reset the plugin to the initial state:\r\nRemoves the selected environment" +
    "s, target user, log output and all Flows + Connection References that have been " +
    "loaded.";
            this.tsbResetTool.Click += new System.EventHandler(this.tsbResetTool_Click);
            // 
            // tsbHelp
            // 
            this.tsbHelp.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelp.Image")));
            this.tsbHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelp.Name = "tsbHelp";
            this.tsbHelp.Size = new System.Drawing.Size(60, 28);
            this.tsbHelp.Text = "Help";
            this.tsbHelp.Click += new System.EventHandler(this.tsbHelp_Click);
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 31);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1428, 730);
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
            this.tabControl1.Size = new System.Drawing.Size(1067, 718);
            this.tabControl1.TabIndex = 15;
            this.tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tlpOverviewTab);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(1059, 692);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Overview";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tlpOverviewTab
            // 
            this.tlpOverviewTab.ColumnCount = 2;
            this.tlpOverviewTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tlpOverviewTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlpOverviewTab.Controls.Add(this.tvTreeview, 0, 0);
            this.tlpOverviewTab.Controls.Add(this.rtbSidepanel, 1, 0);
            this.tlpOverviewTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpOverviewTab.Location = new System.Drawing.Point(3, 3);
            this.tlpOverviewTab.Name = "tlpOverviewTab";
            this.tlpOverviewTab.RowCount = 1;
            this.tlpOverviewTab.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOverviewTab.Size = new System.Drawing.Size(1053, 686);
            this.tlpOverviewTab.TabIndex = 2;
            // 
            // tvTreeview
            // 
            this.tvTreeview.CheckBoxes = true;
            this.tvTreeview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvTreeview.Location = new System.Drawing.Point(0, 0);
            this.tvTreeview.Margin = new System.Windows.Forms.Padding(0);
            this.tvTreeview.Name = "tvTreeview";
            this.tvTreeview.ShowNodeToolTips = true;
            this.tvTreeview.Size = new System.Drawing.Size(631, 686);
            this.tvTreeview.TabIndex = 1;
            this.tvTreeview.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // rtbSidepanel
            // 
            this.rtbSidepanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rtbSidepanel.CausesValidation = false;
            this.rtbSidepanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbSidepanel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbSidepanel.Location = new System.Drawing.Point(634, 3);
            this.rtbSidepanel.Name = "rtbSidepanel";
            this.rtbSidepanel.ReadOnly = true;
            this.rtbSidepanel.Size = new System.Drawing.Size(416, 680);
            this.rtbSidepanel.TabIndex = 2;
            this.rtbSidepanel.Text = "";
            this.rtbSidepanel.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbSidepanel_LinkClicked);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lbDebugOutput);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(1059, 700);
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
            this.lbDebugOutput.Size = new System.Drawing.Size(1053, 694);
            this.lbDebugOutput.TabIndex = 11;
            this.lbDebugOutput.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnReassign);
            this.panel1.Controls.Add(this.tbSelectedEnvironments);
            this.panel1.Controls.Add(this.lblSelectedEnvironments_description);
            this.panel1.Controls.Add(this.btnSelectEnvironments);
            this.panel1.Controls.Add(this.btnStartQueries);
            this.panel1.Controls.Add(this.btnExportToExcel);
            this.panel1.Controls.Add(this.pbMain);
            this.panel1.Controls.Add(this.tbTargetUserEmail);
            this.panel1.Controls.Add(this.cbCheckConnectionReferences);
            this.panel1.Controls.Add(this.cbCheckFlowOwners);
            this.panel1.Controls.Add(this.lblWarning);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(6, 6);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel1.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(344, 718);
            this.panel1.TabIndex = 12;
            // 
            // btnReassign
            // 
            this.btnReassign.Enabled = false;
            this.btnReassign.Location = new System.Drawing.Point(10, 275);
            this.btnReassign.Name = "btnReassign";
            this.btnReassign.Size = new System.Drawing.Size(127, 23);
            this.btnReassign.TabIndex = 35;
            this.btnReassign.Text = "Reassign selected";
            this.toolTip1.SetToolTip(this.btnReassign, "Open Dialog to reassign flows to another user.");
            this.btnReassign.UseVisualStyleBackColor = true;
            this.btnReassign.Click += new System.EventHandler(this.btnReassign_Click);
            // 
            // tbSelectedEnvironments
            // 
            this.tbSelectedEnvironments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSelectedEnvironments.CausesValidation = false;
            this.tbSelectedEnvironments.Location = new System.Drawing.Point(122, 35);
            this.tbSelectedEnvironments.Name = "tbSelectedEnvironments";
            this.tbSelectedEnvironments.ReadOnly = true;
            this.tbSelectedEnvironments.Size = new System.Drawing.Size(43, 20);
            this.tbSelectedEnvironments.TabIndex = 34;
            this.tbSelectedEnvironments.Text = "none";
            this.tbSelectedEnvironments.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblSelectedEnvironments_description
            // 
            this.lblSelectedEnvironments_description.AutoSize = true;
            this.lblSelectedEnvironments_description.Location = new System.Drawing.Point(2, 37);
            this.lblSelectedEnvironments_description.Name = "lblSelectedEnvironments_description";
            this.lblSelectedEnvironments_description.Size = new System.Drawing.Size(116, 13);
            this.lblSelectedEnvironments_description.TabIndex = 33;
            this.lblSelectedEnvironments_description.Text = "selected environments:";
            // 
            // btnSelectEnvironments
            // 
            this.btnSelectEnvironments.Location = new System.Drawing.Point(3, 3);
            this.btnSelectEnvironments.Name = "btnSelectEnvironments";
            this.btnSelectEnvironments.Size = new System.Drawing.Size(169, 27);
            this.btnSelectEnvironments.TabIndex = 32;
            this.btnSelectEnvironments.Text = "Select Environments";
            this.btnSelectEnvironments.UseVisualStyleBackColor = true;
            this.btnSelectEnvironments.Click += new System.EventHandler(this.btnSelectEnvironments_Click);
            // 
            // btnStartQueries
            // 
            this.btnStartQueries.Enabled = false;
            this.btnStartQueries.Location = new System.Drawing.Point(108, 246);
            this.btnStartQueries.Name = "btnStartQueries";
            this.btnStartQueries.Size = new System.Drawing.Size(127, 23);
            this.btnStartQueries.TabIndex = 16;
            this.btnStartQueries.Text = "Load Data";
            this.toolTip1.SetToolTip(this.btnStartQueries, "Query the selected environments for flows and connection references that are owne" +
        "d by a specific user.");
            this.btnStartQueries.UseVisualStyleBackColor = true;
            this.btnStartQueries.Click += new System.EventHandler(this.btnStartQueries_Click);
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Enabled = false;
            this.btnExportToExcel.Location = new System.Drawing.Point(214, 275);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(127, 23);
            this.btnExportToExcel.TabIndex = 31;
            this.btnExportToExcel.Text = "Export to Excel";
            this.toolTip1.SetToolTip(this.btnExportToExcel, "Export this information to Excel.");
            this.btnExportToExcel.UseVisualStyleBackColor = true;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // pbMain
            // 
            this.pbMain.Enabled = false;
            this.pbMain.Location = new System.Drawing.Point(4, 319);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(338, 23);
            this.pbMain.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbMain.TabIndex = 30;
            // 
            // tbTargetUserEmail
            // 
            this.tbTargetUserEmail.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tbTargetUserEmail.Location = new System.Drawing.Point(10, 79);
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
            this.cbCheckConnectionReferences.Location = new System.Drawing.Point(10, 138);
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
            this.cbCheckFlowOwners.Location = new System.Drawing.Point(10, 115);
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
            // FlowOwnershipAuditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "FlowOwnershipAuditControl";
            this.Size = new System.Drawing.Size(1428, 761);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tlpOverviewTab.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnStartQueries;
        private XrmToolBox.Controls.TextBoxWithPlaceholder tbTargetUserEmail;
        private System.Windows.Forms.CheckBox cbCheckConnectionReferences;
        private System.Windows.Forms.CheckBox cbCheckFlowOwners;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox lbDebugOutput;
        private CustomTreeViewControl tvTreeview;
        private System.Windows.Forms.ProgressBar pbMain;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.Button btnSelectEnvironments;
        private System.Windows.Forms.Label lblSelectedEnvironments_description;
        private System.Windows.Forms.TextBox tbSelectedEnvironments;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripButton tsbResetTool;
        private System.Windows.Forms.TableLayoutPanel tlpOverviewTab;
        private System.Windows.Forms.RichTextBox rtbSidepanel;
        private System.Windows.Forms.ToolStripButton tsbHelp;
        private System.Windows.Forms.Button btnReassign;
    }
}
