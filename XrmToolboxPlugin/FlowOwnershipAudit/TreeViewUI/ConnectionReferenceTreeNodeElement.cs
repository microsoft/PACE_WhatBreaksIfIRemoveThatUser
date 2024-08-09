using System;
using System.Collections.Generic;
using WhatBreaksIf.DTO;
using WhatBreaksIf.TreeViewUIElements;

namespace WhatBreaksIf.TreeViewUI
{
    internal class ConnectionReferenceTreeNodeElement : TreeNodeElementBase
    {
        public ConnectionReferenceTreeNodeElement(Action<NodeUpdateObject> updateNodeUiDelegate,
                                                    DirectoryTreeNode parentNodeElement,
                                                  string connectionReferenceName,
                                                  string environmentId) : base(updateNodeUiDelegate)
        {
            Parent = parentNodeElement;
            ConnectionReferenceName = connectionReferenceName;
            EnvironmentId = environmentId;

            // ctor has been called, this means we need to call the update method to display the flow in the UI. This needs to be done after properties have been initialized!
            updateNodeUiDelegate(new NodeUpdateObject()
            {
                TreeNodeElement = this,
                ParentNodeId = (Parent != null) ? Parent.ElementId.ToString() : null,
                NodeText = ConnectionReferenceName,
                UpdateReason = UpdateReason.AddedToList
            });
        }

        public string ConnectionReferenceName { get; }
        public string EnvironmentId { get; }

        // right now we dont have any child objects, but we could have them in the future
        internal override IEnumerable<TreeNodeElementBase> ChildObjects => throw new NotImplementedException();

        internal override TreeNodeElementBase Parent { get; }
    }
}