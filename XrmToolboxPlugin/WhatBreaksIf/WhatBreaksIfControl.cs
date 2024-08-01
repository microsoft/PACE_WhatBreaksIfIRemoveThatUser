using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace WhatBreaksIf
{
    public partial class WhatBreaksIfControl : PluginControlBase, IGitHubPlugin, IAboutPlugin, IHelpPlugin
    {

        #region Fields

        private delegate void _updateLogWindowDelegate(string msg, params object[] args);

        #endregion

        #region GitHub implementation

        public string RepositoryName => "PACE_WhatBreaksIfIRemoveThatUser";

        public string UserName => "microsoft";

        #endregion

        #region IHelpPluginImplementation

        public string HelpUrl => "https://github.com/microsoft/PACE_WhatBreaksIfIRemoveThatUser";

        #endregion

        # region IAbout implementation

        public void ShowAboutDialog()
        {
            throw new NotImplementedException();
        }

        #endregion

        private Settings mySettings;

        #region ctor

        public WhatBreaksIfControl()
        {
            InitializeComponent();
        }

        #endregion

        private void GetAccounts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting accounts",
                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(new QueryExpression("account")
                    {
                        TopCount = 50
                    });
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        MessageBox.Show($"Found {result.Entities.Count} accounts");
                    }
                }
            });
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        #region Events

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            LogInfo("Hello world :) ");

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }


        #endregion


        #region Helpers


        #endregion


        #region Methods

        /// <summary>
        /// Base level implementation on how to get Flows owned by a user. 
        /// This will be called async later but this method is not supposed to handle async code itself
        /// </summary>
        /// <param name="userId"></param>
        private object GetAllFlowsOwnedByUser(string userId)
        {
            // wait 5 seconds for demo purposes
            System.Threading.Thread.Sleep(5000);

            // auth

            // call api

            // transform response

            // report back

            return "We just pretend we have something here";
        }

        private object GetAllConnectionReferencesOwnedByUser(string userId)
        {
            // wait 10 seconds for demo purposes
            System.Threading.Thread.Sleep(10000);

            // auth

            // call api

            // transform response

            // report back

            return "We just pretend we have something here";
        }

        #endregion


        #region overrides

        private new void LogInfo(string text, params object[] args)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(LogInfo);
                lbDebugOutput.Invoke(update, text);
            }
            else
            {
                base.LogInfo(text, args);
                lbDebugOutput.Items.Add(text);
                if (lbDebugOutput.Items.Count > 1000)
                {
                    lbDebugOutput.Items.RemoveAt(0); // remove first line
                }
                // Make sure the last item is made visible
                lbDebugOutput.SelectedIndex = lbDebugOutput.Items.Count - 1;
                lbDebugOutput.ClearSelected();
            }
        }

        private new void LogError(string text, params object[] args)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(LogError);
                lbDebugOutput.Invoke(update, text);
            }
            else
            {
                base.LogError(text, args);
                lbDebugOutput.Items.Add(text);
                if (lbDebugOutput.Items.Count > 1000)
                {
                    lbDebugOutput.Items.RemoveAt(0); // remove first line
                }
                // Make sure the last item is made visible
                lbDebugOutput.SelectedIndex = lbDebugOutput.Items.Count - 1;
                lbDebugOutput.ClearSelected();
            }
        }

        private new void LogWarning(string text, params object[] args)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(LogWarning);
                lbDebugOutput.Invoke(update, text);
            }
            else
            {
                base.LogWarning(text, args);
                lbDebugOutput.Items.Add(text);
                if (lbDebugOutput.Items.Count > 1000)
                {
                    lbDebugOutput.Items.RemoveAt(0); // remove first line
                }
                // Make sure the last item is made visible
                lbDebugOutput.SelectedIndex = lbDebugOutput.Items.Count - 1;
                lbDebugOutput.ClearSelected();
            }
        }


        #endregion

        private void btnStartQueries_Click(object sender, EventArgs eventArgs)
        {
            LogInfo("Starting....");

            var targetUser = tbTargetUserEmail.Text;
            bool checkFlowOwners = cbCheckFlowOwners.Checked;
            bool checkConnectionReferences = cbCheckConnectionReferences.Checked;

            // disable controls
            tbTargetUserEmail.Enabled = false;
            cbCheckFlowOwners.Enabled = false;
            cbCheckConnectionReferences.Enabled = false;
            btnStartQueries.Enabled = false;

            LogInfo($"Will search the following for {targetUser}:" +
                $" Flow Ownership: {(cbCheckFlowOwners.Checked ? "yes" : "no")}" +
                $" Connection References: {(cbCheckConnectionReferences.Checked ? "yes" : "no")}" +
                $" ...");

            if (checkFlowOwners)
            {
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Getting Flow Ownership",
                    Work = (worker, args) =>
                    {
                        var result = GetAllFlowsOwnedByUser(targetUser);

                        // TODO: Implement progress reporting
                        worker.ReportProgress(100, result);
                        args.Result = result;
                    },
                    ProgressChanged = e =>
                    {
                        SetWorkingMessage(e.ToString());
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        var result = args.Result;

                        // TODO: do something with the result

                        LogInfo("Finished Flow Ownership query.");
                    },
                    AsyncArgument = null,
                    // Progress information panel size
                    MessageWidth = 340,
                    MessageHeight = 150
                });
            }

            if (checkConnectionReferences)
            {
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Getting Connection References",
                    Work = (worker, args) =>
                    {
                        var result = GetAllConnectionReferencesOwnedByUser(targetUser);

                        // TODO: Implement progress reporting
                        worker.ReportProgress(100, result);
                        args.Result = result;
                    },
                    ProgressChanged = e =>
                    {
                        SetWorkingMessage(e.ToString());
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        var result = args.Result;

                        // TODO: do something with the result

                        LogInfo("Finished Connection References query.");
                    },
                    AsyncArgument = null,
                    // Progress information panel size
                    MessageWidth = 340,
                    MessageHeight = 150
                });
            }

            // --- careful, all the stuff above runs async, so this will run before the queries are done ----
        }

        private void tbTargetUserEmail_TextChanged(object sender, EventArgs e)
        {
            // enable the button if the text is not empty
            btnStartQueries.Enabled = !string.IsNullOrEmpty(tbTargetUserEmail.Text);
        }
    }
}