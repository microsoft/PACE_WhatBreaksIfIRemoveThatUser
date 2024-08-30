using System;
using System.Collections.Generic;
using FlowOwnershipAudit.TreeViewUIElements;

namespace FlowOwnershipAudit.TreeViewUI
{
    internal class ConnectionReferenceTreeNodeElement : TreeNodeElementBase
    {
        internal DirectoryTreeNode _parentNodeElement;

        public string ConnectionReferenceName { get; }

        public string EnvironmentId { get; }

        // right now we dont have any child objects, but we could have them in the future
        internal override IEnumerable<TreeNodeElementBase> ChildObjects => throw new NotImplementedException();

        internal override TreeNodeElementBase Parent => _parentNodeElement;

        public ConnectionReferenceTreeNodeElement(Action<NodeUpdateObject> updateNodeUiDelegate,
                                                    DirectoryTreeNode parentNodeElement,
                                                  string connectionReferenceName,
                                                  string environmentId) : base(updateNodeUiDelegate)
        {
            _parentNodeElement = parentNodeElement;
            ConnectionReferenceName = connectionReferenceName;
            EnvironmentId = environmentId;

            _parentNodeElement.ObservableChildNodes.Add(this);

            // ctor has been called, this means we need to call the update method to display the flow in the UI. This needs to be done after properties have been initialized!
            updateNodeUiDelegate(new NodeUpdateObject(this)
            {
                ParentNodeId = (Parent != null) ? Parent.ElementId.ToString() : null,
                NodeText = ConnectionReferenceName,
                UpdateReason = UpdateReason.AddedToList
            });
        }
    }
}