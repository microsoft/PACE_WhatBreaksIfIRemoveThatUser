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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.btnStartQueries = new System.Windows.Forms.Button();
            this.btnReassign = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlParameters = new System.Windows.Forms.Panel();
            this.tbSelectedEnvironments = new System.Windows.Forms.TextBox();
            this.lblSelectedEnvironments_description = new System.Windows.Forms.Label();
            this.btnSelectEnvironments = new System.Windows.Forms.Button();
            this.pbMain = new System.Windows.Forms.ProgressBar();
            this.tbTargetUserEmail = new XrmToolBox.Controls.TextBoxWithPlaceholder();
            this.cbCheckConnection = new System.Windows.Forms.CheckBox();
            this.cbCheckFlowOwners = new System.Windows.Forms.CheckBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tvTreeview = new CustomTreeViewControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lvComponentCount = new System.Windows.Forms.ListView();
            this.rtbSidepanel = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lbDebugOutput = new System.Windows.Forms.ListBox();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.toolStripMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlParameters.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.pnlMain.SuspendLayout();
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
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStripMenu.Size = new System.Drawing.Size(2142, 34);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(59, 29);
            this.tsbClose.Text = "Close";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 34);
            // 
            // tsbResetTool
            // 
            this.tsbResetTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbResetTool.Image = ((System.Drawing.Image)(resources.GetObject("tsbResetTool.Image")));
            this.tsbResetTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResetTool.Name = "tsbResetTool";
            this.tsbResetTool.Size = new System.Drawing.Size(107, 29);
            this.tsbResetTool.Text = "Restart tool";
            this.tsbResetTool.ToolTipText = "Click to reset the plugin to the initial state:\r\nRemoves the selected environment" +
    "s, target user, log output and all components that have been " +
    "loaded.";
            this.tsbResetTool.Click += new System.EventHandler(this.tsbResetTool_Click);
            // 
            // tsbHelp
            // 
            this.tsbHelp.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelp.Image")));
            this.tsbHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelp.Name = "tsbHelp";
            this.tsbHelp.Size = new System.Drawing.Size(77, 29);
            this.tsbHelp.Text = "Help";
            this.tsbHelp.Click += new System.EventHandler(this.tsbHelp_Click);
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Enabled = false;
            this.btnExportToExcel.Location = new System.Drawing.Point(321, 423);
            this.btnExportToExcel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(190, 35);
            this.btnExportToExcel.TabIndex = 31;
            this.btnExportToExcel.Text = "Export to Excel";
            this.toolTip1.SetToolTip(this.btnExportToExcel, "Export this information to Excel.");
            this.btnExportToExcel.UseVisualStyleBackColor = true;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // btnStartQueries
            // 
            this.btnStartQueries.Enabled = false;
            this.btnStartQueries.Location = new System.Drawing.Point(162, 378);
            this.btnStartQueries.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStartQueries.Name = "btnStartQueries";
            this.btnStartQueries.Size = new System.Drawing.Size(190, 35);
            this.btnStartQueries.TabIndex = 16;
            this.btnStartQueries.Text = "Load Data";
            this.toolTip1.SetToolTip(this.btnStartQueries, "Query the selected environments for components that are owne" +
        "d by a specific user.");
            this.btnStartQueries.UseVisualStyleBackColor = true;
            this.btnStartQueries.Click += new System.EventHandler(this.btnStartQueries_Click);
            // 
            // btnReassign
            // 
            this.btnReassign.Enabled = false;
            this.btnReassign.Location = new System.Drawing.Point(15, 423);
            this.btnReassign.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReassign.Name = "btnReassign";
            this.btnReassign.Size = new System.Drawing.Size(190, 35);
            this.btnReassign.TabIndex = 35;
            this.btnReassign.Text = "Reassign selected";
            this.toolTip1.SetToolTip(this.btnReassign, "Open Dialog to reassign flows to another user.");
            this.btnReassign.UseVisualStyleBackColor = true;
            this.btnReassign.Click += new System.EventHandler(this.btnReassign_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlParameters);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(2134, 1127);
            this.splitContainer1.SplitterDistance = 547;
            this.splitContainer1.TabIndex = 16;
            // 
            // pnlParameters
            // 
            this.pnlParameters.AutoSize = true;
            this.pnlParameters.Controls.Add(this.btnReassign);
            this.pnlParameters.Controls.Add(this.tbSelectedEnvironments);
            this.pnlParameters.Controls.Add(this.lblSelectedEnvironments_description);
            this.pnlParameters.Controls.Add(this.btnSelectEnvironments);
            this.pnlParameters.Controls.Add(this.btnStartQueries);
            this.pnlParameters.Controls.Add(this.btnExportToExcel);
            this.pnlParameters.Controls.Add(this.pbMain);
            this.pnlParameters.Controls.Add(this.tbTargetUserEmail);
            this.pnlParameters.Controls.Add(this.cbCheckConnection);
            this.pnlParameters.Controls.Add(this.cbCheckFlowOwners);
            this.pnlParameters.Controls.Add(this.lblWarning);
            this.pnlParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlParameters.Location = new System.Drawing.Point(0, 0);
            this.pnlParameters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlParameters.Name = "pnlParameters";
            this.pnlParameters.Size = new System.Drawing.Size(547, 1127);
            this.pnlParameters.TabIndex = 13;
            // 
            // tbSelectedEnvironments
            // 
            this.tbSelectedEnvironments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSelectedEnvironments.CausesValidation = false;
            this.tbSelectedEnvironments.Location = new System.Drawing.Point(183, 54);
            this.tbSelectedEnvironments.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSelectedEnvironments.Name = "tbSelectedEnvironments";
            this.tbSelectedEnvironments.ReadOnly = true;
            this.tbSelectedEnvironments.Size = new System.Drawing.Size(64, 26);
            this.tbSelectedEnvironments.TabIndex = 34;
            this.tbSelectedEnvironments.Text = "none";
            this.tbSelectedEnvironments.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblSelectedEnvironments_description
            // 
            this.lblSelectedEnvironments_description.AutoSize = true;
            this.lblSelectedEnvironments_description.Location = new System.Drawing.Point(3, 57);
            this.lblSelectedEnvironments_description.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSelectedEnvironments_description.Name = "lblSelectedEnvironments_description";
            this.lblSelectedEnvironments_description.Size = new System.Drawing.Size(172, 20);
            this.lblSelectedEnvironments_description.TabIndex = 33;
            this.lblSelectedEnvironments_description.Text = "selected environments:";
            // 
            // btnSelectEnvironments
            // 
            this.btnSelectEnvironments.Location = new System.Drawing.Point(4, 5);
            this.btnSelectEnvironments.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSelectEnvironments.Name = "btnSelectEnvironments";
            this.btnSelectEnvironments.Size = new System.Drawing.Size(254, 42);
            this.btnSelectEnvironments.TabIndex = 32;
            this.btnSelectEnvironments.Text = "Select Environments";
            this.btnSelectEnvironments.UseVisualStyleBackColor = true;
            this.btnSelectEnvironments.Click += new System.EventHandler(this.btnSelectEnvironments_Click);
            // 
            // pbMain
            // 
            this.pbMain.Enabled = false;
            this.pbMain.Location = new System.Drawing.Point(6, 491);
            this.pbMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(507, 35);
            this.pbMain.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbMain.TabIndex = 30;
            // 
            // tbTargetUserEmail
            // 
            this.tbTargetUserEmail.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tbTargetUserEmail.Location = new System.Drawing.Point(15, 122);
            this.tbTargetUserEmail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbTargetUserEmail.Name = "tbTargetUserEmail";
            this.tbTargetUserEmail.Placeholder = null;
            this.tbTargetUserEmail.Size = new System.Drawing.Size(409, 26);
            this.tbTargetUserEmail.TabIndex = 29;
            this.tbTargetUserEmail.Tag = true;
            this.tbTargetUserEmail.TextChanged += new System.EventHandler(this.tbTargetUserEmail_TextChanged);
            // 
            // cbCheckConnectionReferences
            // 
            this.cbCheckConnection.AutoSize = true;
            this.cbCheckConnection.Checked = true;
            this.cbCheckConnection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCheckConnection.Location = new System.Drawing.Point(15, 212);
            this.cbCheckConnection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbCheckConnection.Name = "cbCheckConnections";
            this.cbCheckConnection.Size = new System.Drawing.Size(252, 24);
            this.cbCheckConnection.TabIndex = 28;
            this.cbCheckConnection.Text = "Check Connections";
            this.cbCheckConnection.UseVisualStyleBackColor = true;
            // 
            // cbCheckFlowOwners
            // 
            this.cbCheckFlowOwners.AutoSize = true;
            this.cbCheckFlowOwners.Checked = true;
            this.cbCheckFlowOwners.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCheckFlowOwners.Location = new System.Drawing.Point(15, 177);
            this.cbCheckFlowOwners.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbCheckFlowOwners.Name = "cbCheckFlowOwners";
            this.cbCheckFlowOwners.Size = new System.Drawing.Size(196, 24);
            this.cbCheckFlowOwners.TabIndex = 27;
            this.cbCheckFlowOwners.Text = "Check Flow Ownership";
            this.cbCheckFlowOwners.UseVisualStyleBackColor = true;
            // 
            // lblWarning
            // 
            this.lblWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.Location = new System.Drawing.Point(10, 595);
            this.lblWarning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(501, 372);
            this.lblWarning.TabIndex = 26;
            this.lblWarning.Text = resources.GetString("lblWarning.Text");
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1583, 1127);
            this.tabControl1.TabIndex = 16;
            this.tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer2);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Size = new System.Drawing.Size(1575, 1094);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Overview";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(4, 5);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tvTreeview);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer2.Size = new System.Drawing.Size(1567, 1084);
            this.splitContainer2.SplitterDistance = 674;
            this.splitContainer2.TabIndex = 3;
            // 
            // tvTreeview
            // 
            this.tvTreeview.CheckBoxes = true;
            this.tvTreeview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvTreeview.Location = new System.Drawing.Point(0, 0);
            this.tvTreeview.Margin = new System.Windows.Forms.Padding(0);
            this.tvTreeview.Name = "tvTreeview";
            this.tvTreeview.ShowNodeToolTips = true;
            this.tvTreeview.Size = new System.Drawing.Size(674, 1084);
            this.tvTreeview.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lvComponentCount, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rtbSidepanel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(889, 1084);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // lvComponentCount
            // 
            this.lvComponentCount.AutoArrange = false;
            this.lvComponentCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvComponentCount.HideSelection = false;
            this.lvComponentCount.Location = new System.Drawing.Point(3, 3);
            this.lvComponentCount.Name = "lvComponentCount";
            this.lvComponentCount.Size = new System.Drawing.Size(883, 394);
            this.lvComponentCount.TabIndex = 3;
            this.lvComponentCount.UseCompatibleStateImageBehavior = false;
            // 
            // rtbSidepanel
            // 
            this.rtbSidepanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rtbSidepanel.CausesValidation = false;
            this.rtbSidepanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbSidepanel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbSidepanel.Location = new System.Drawing.Point(3, 403);
            this.rtbSidepanel.Name = "rtbSidepanel";
            this.rtbSidepanel.ReadOnly = true;
            this.rtbSidepanel.Size = new System.Drawing.Size(883, 678);
            this.rtbSidepanel.TabIndex = 2;
            this.rtbSidepanel.Text = "";
            this.rtbSidepanel.Enabled = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lbDebugOutput);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Size = new System.Drawing.Size(1575, 1094);
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
            this.lbDebugOutput.ItemHeight = 20;
            this.lbDebugOutput.Location = new System.Drawing.Point(4, 5);
            this.lbDebugOutput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lbDebugOutput.MinimumSize = new System.Drawing.Size(749, 2);
            this.lbDebugOutput.Name = "lbDebugOutput";
            this.lbDebugOutput.ScrollAlwaysVisible = true;
            this.lbDebugOutput.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbDebugOutput.Size = new System.Drawing.Size(1567, 1084);
            this.lbDebugOutput.TabIndex = 11;
            this.lbDebugOutput.TabStop = false;
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.Controls.Add(this.splitContainer1);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 34);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlMain.Size = new System.Drawing.Size(2142, 1137);
            this.pnlMain.TabIndex = 14;
            // 
            // FlowOwnershipAuditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.toolStripMenu);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FlowOwnershipAuditControl";
            this.Size = new System.Drawing.Size(2142, 1171);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlParameters.ResumeLayout(false);
            this.pnlParameters.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripButton tsbResetTool;
        private System.Windows.Forms.ToolStripButton tsbHelp;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlParameters;
        private System.Windows.Forms.Button btnReassign;
        private System.Windows.Forms.TextBox tbSelectedEnvironments;
        private System.Windows.Forms.Label lblSelectedEnvironments_description;
        private System.Windows.Forms.Button btnSelectEnvironments;
        private System.Windows.Forms.Button btnStartQueries;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.ProgressBar pbMain;
        private XrmToolBox.Controls.TextBoxWithPlaceholder tbTargetUserEmail;
        private System.Windows.Forms.CheckBox cbCheckConnection;
        private System.Windows.Forms.CheckBox cbCheckFlowOwners;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox lbDebugOutput;
        private System.Windows.Forms.Panel pnlMain;
        private CustomTreeViewControl tvTreeview;
        private System.Windows.Forms.RichTextBox rtbSidepanel;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView lvComponentCount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
