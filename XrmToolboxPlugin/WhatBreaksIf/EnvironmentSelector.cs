using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WhatBreaksIf
{
    public partial class EnvironmentSelector : Form
    {

        public List<Model.Environment> SelectedEnvironments
        {
            get
            {
                var selectedEnvironments = new List<Model.Environment>();
                foreach (ListViewItem item in listView1.CheckedItems)
                {
                    selectedEnvironments.Add((Model.Environment)item.Tag);
                }

                return selectedEnvironments;
            }
        }

        public EnvironmentSelector(List<Model.Environment> availableEnvironments)
        {
            InitializeComponent();

            foreach (var environment in availableEnvironments)
            {
                listView1.Items.Add(new ListViewItem(environment.properties.displayName) { Tag = environment });
            }
            listView1.Sort();
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
