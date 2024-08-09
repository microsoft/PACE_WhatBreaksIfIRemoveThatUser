using System.Windows.Forms;

namespace WhatBreaksIfPlugin
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void llGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/microsoft/PACE_WhatBreaksIfIRemoveThatUser");
        }
    }
}
