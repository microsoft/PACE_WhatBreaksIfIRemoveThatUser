using FlowOwnershipAudit.TreeViewUI;
using FlowOwnershipAudit.TreeViewUIElements;
using FlowOwnershipAudit;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeViewUI
{
    internal class ConnectionReferenceTreeNodeElement : TreeNodeElementBase
    {
        internal FlowTreeNodeElement _parentNodeElement;

        public string ConnectionReferenceName { get; }

        //public string EnvironmentId { get; }

        // right now we dont have any child objects, but we could have them in the future
        internal override IEnumerable<TreeNodeElementBase> ChildObjects => throw new NotImplementedException();

        internal override TreeNodeElementBase Parent => _parentNodeElement;

        //internal MigrationStatus MigrationStatus { get; set; }
        public ConnectionReferenceTreeNodeElement(Action<NodeUpdateObject> updateNodeUiDelegate,
                                                    FlowTreeNodeElement parentNodeElement,
                                                  string connectionReferenceName) : base(updateNodeUiDelegate)
        {
            _parentNodeElement = parentNodeElement;
            //ConnectionName = connectionName;
            //EnvironmentId = environmentId;

            //_parentNodeElement.ObservableChildNodes.Add(this);

            // ctor has been called, this means we need to call the update method to display the flow in the UI. This needs to be done after properties have been initialized!
            updateNodeUiDelegate(new NodeUpdateObject(this)
            {
                ParentNodeId = (Parent != null) ? Parent.ElementId.ToString() : null,
                NodeText = connectionReferenceName
            });
        }
    }
}
