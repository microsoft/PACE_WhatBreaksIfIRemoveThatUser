using System;
using System.Collections.Generic;
using WhatBreaksIf.DTO;
using WhatBreaksIf.TreeViewUIElements;

namespace WhatBreaksIf.TreeViewUI
{
    // implementation of TreeNodeElementBase, this one is used to display Environments in the treeview
    internal class EnvironmentTreeNodeElement : TreeNodeElementBase
    {
        public string EnvironmentName { get; set; }

        public string EnvironmentId { get; set; }

        public EnvironmentTreeNodeElement(Action<NodeUpdateObject> updateNodeUi, string environmentName, string environmentId) : base(updateNodeUi)
        {
            EnvironmentName = environmentName;
            EnvironmentId = environmentId;

            // constructor for the environment tree node was called, update the UI to display it. This needs to happen after the backing fields of the properties have been set!
            updateNodeUi(new NodeUpdateObject()
            {
                TreeNodeElement = this,
                NodeText = EnvironmentName,
                UpdateReason = UpdateReason.AddedToList
            });
        }

        internal List<TreeNodeElementBase> EnvironmentNodeElements { get; } = new List<TreeNodeElementBase>();

        internal override TreeNodeElementBase Parent => null; // this is the top level node

        internal override IEnumerable<TreeNodeElementBase> ChildObjects => EnvironmentNodeElements;
    }
}