using System;
using System.Collections.Generic;
using FlowOwnershipAudit.DTO;
using FlowOwnershipAudit.TreeViewUIElements;

namespace FlowOwnershipAudit.TreeViewUI
{
    // one implementation of TreeNodeElementBase, this one is used to display Flows in the treeview. Parent is always the environment 
    internal class FlowTreeNodeElement : TreeNodeElementBase
    {
        internal DirectoryTreeNode _parentNodeElement;

        public string FlowName { get; }

        public string FlowId { get; }

        public string EnvironmentId { get; }

        public Uri FlowUri { get => new Uri($"https://make.powerautomate.com/environments/{EnvironmentId}/solutions/~preferred/flows/{FlowId}"); }

        public FlowTreeNodeElement(Action<NodeUpdateObject> updateNodeUiDelegate,
                                  DirectoryTreeNode parentNodeElement,
                                  string flowName,
                                  string flowId,
                                  string environmentId
                                 ) : base(updateNodeUiDelegate)
        {
            // ctor has been called, this means we need to call the update method to display the flow in the UI
            // TODO Implement logic for updating object that already exist

            _parentNodeElement = parentNodeElement;
            FlowName = flowName;
            FlowId = flowId;
            EnvironmentId = environmentId;
            updateNodeUiDelegate(new NodeUpdateObject()
            {
                TreeNodeElement = this,
                ParentNodeId = (parentNodeElement != null) ? _parentNodeElement.ElementId.ToString() : null,
                NodeText = FlowName,
                UpdateReason = UpdateReason.AddedToList
            });
        }

        // right now we dont have any child objects, but we could have them in the future, for example to show connection references that sit under a flow
        internal override IEnumerable<TreeNodeElementBase> ChildObjects => throw new NotImplementedException();

        internal override TreeNodeElementBase Parent => _parentNodeElement;
    }
}