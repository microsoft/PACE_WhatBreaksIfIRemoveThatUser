using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WhatBreaksIf
{
    public partial class EnvironmentSelector : Form
    {
        private List<(string ColumnName, Func<Model.Environment, object> ValueLookup, Func<Model.Environment, string> DisplayStringLookup)> _columnMappingForenvironment;

        public List<Model.Environment> SelectedEnvironments
        {
            get
            {
                var selectedItems = listView1.GetSelectedItems();
                return selectedItems;
            }
        }

        public EnvironmentSelector(List<Model.Environment> availableEnvironments)
        {
            _columnMappingForenvironment = new List<(
                string ColumnName,
                Func<Model.Environment, object> ValueLookup,
                Func<Model.Environment, string> DisplayStringLookup)>
            {
                ("Name", LookupValue, LookupString)
            };

            InitializeComponent();

            listView1.AddRange(availableEnvironments);
        }

        private object LookupValue(Model.Environment environment)
        {
            // there might be a better object to return
            return environment.name;
        }

        private string LookupString(Model.Environment environment)
        {
            return environment.properties.displayName;
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
