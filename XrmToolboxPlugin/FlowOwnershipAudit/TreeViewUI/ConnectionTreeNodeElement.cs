using System;
using System.Collections.Generic;
using FlowOwnershipAudit.TreeViewUIElements;

namespace FlowOwnershipAudit.TreeViewUI
{
    internal class ConnectionTreeNodeElement : TreeNodeElementBase
    {
        internal DirectoryTreeNode _parentNodeElement;

        public string ConnectionName { get; }

        public string EnvironmentId { get; }

        // right now we dont have any child objects, but we could have them in the future
        internal override IEnumerable<TreeNodeElementBase> ChildObjects => throw new NotImplementedException();

        internal override TreeNodeElementBase Parent => _parentNodeElement;

        internal MigrationStatus MigrationStatus { get; set; }
        public ConnectionTreeNodeElement(Action<NodeUpdateObject> updateNodeUiDelegate,
                                                    DirectoryTreeNode parentNodeElement,
                                                  string connectionName,
                                                  string environmentId) : base(updateNodeUiDelegate)
        {
            _parentNodeElement = parentNodeElement;
            ConnectionName = connectionName;
            EnvironmentId = environmentId;

            _parentNodeElement.ObservableChildNodes.Add(this);

            // ctor has been called, this means we need to call the update method to display the flow in the UI. This needs to be done after properties have been initialized!
            updateNodeUiDelegate(new NodeUpdateObject(this)
            {
                ParentNodeId = (Parent != null) ? Parent.ElementId.ToString() : null,
                NodeText = ConnectionName,
                UpdateReason = UpdateReason.AddedToList
            });
        }
    }
}