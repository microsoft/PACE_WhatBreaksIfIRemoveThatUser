using McTools.Xrm.Connection;
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

        private delegate void _updateLogWindowDelegate(string msg);

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
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

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

        private void CustomLog(string text)
        {
            if (lbDebugOutput.InvokeRequired)
            {
                _updateLogWindowDelegate update = new _updateLogWindowDelegate(CustomLog);
                lbDebugOutput.Invoke(update, text);
            }
            else
            {
                LogInfo(text);
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
    }
}