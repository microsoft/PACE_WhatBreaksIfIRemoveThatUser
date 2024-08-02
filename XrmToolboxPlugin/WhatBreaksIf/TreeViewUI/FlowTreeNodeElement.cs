using System;
using System.Collections.Generic;
using WhatBreaksIf.DTO;
using WhatBreaksIf.TreeViewUIElements;

namespace WhatBreaksIf.TreeViewUI
{
    // one implementation of TreeNodeElementBase, this one is used to display Flows in the treeview
    internal class FlowTreeNodeElement : EnvironmentTreeNodeElement
    {
        internal TreeNodeElementBase _parentNodeElement;

        public string FlowName { get; set; }

        public string FlowId { get; set; }

        public Uri FlowUri { get => new Uri($"https://make.powerautomate.com/environments{EnvironmentId}/solutions/~preferred/flows/{FlowId})"); }

        public FlowTreeNodeElement(Action<NodeUpdateObject> updateNodeUiDelegate,
                                  TreeNodeElementBase parentNodeElement,
                                  string flowName,
                                  string flowId,
                                  string environmentId,
                                  string environmentName) : base(updateNodeUiDelegate, environmentName, environmentId)
        {
            // ctor has been called, this means we need to call the update method to display the flow in the UI
            // TODO Implement logic for updating object that already exist

            updateNodeUiDelegate(new NodeUpdateObject()
            {
                TreeNodeElement = this,
                ParentNodeId = (parentNodeElement != null) ? _parentNodeElement.ElementId.ToString() : null,
                NodeText = FlowName,
                UpdateReason = UpdateReason.AddedToList
            });
            _parentNodeElement = parentNodeElement;
        }

        // right now we dont have any child objects, but we could have them in the future, for example to show connection references that sit under a flow
        internal override IEnumerable<TreeNodeElementBase> ChildObjects => throw new NotImplementedException();

        internal override TreeNodeElementBase Parent => _parentNodeElement;
    }
}