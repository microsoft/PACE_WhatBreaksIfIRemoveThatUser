using System;
using System.Collections.Generic;
using FlowOwnershipAudit.Model;
using FlowOwnershipAudit.TreeViewUIElements;

namespace FlowOwnershipAudit.TreeViewUI
{
    // one implementation of TreeNodeElementBase, this one is used to display Flows in the treeview. Parent is always the environment 
    internal class FlowTreeNodeElement : TreeNodeElementBase
    {
        internal DirectoryTreeNode _parentNodeElement;

        public Flow Flow { get; }

        public Uri FlowUri { get => new Uri($"https://make.powerautomate.com/environments/{Flow.properties.environment.name}/solutions/~preferred/flows/{Flow.name}"); }

        // right now we dont have any child objects, but we could have them in the future, for example to show connection references that sit under a flow
        internal override IEnumerable<TreeNodeElementBase> ChildObjects => throw new NotImplementedException();

        internal override TreeNodeElementBase Parent => _parentNodeElement;

        public FlowTreeNodeElement(Action<NodeUpdateObject> updateNodeUiDelegate,
                                  DirectoryTreeNode parentNodeElement,
                                  Flow flow
                                 ) : base(updateNodeUiDelegate)
        {

            _parentNodeElement = parentNodeElement;
            Flow = flow;

            _parentNodeElement.ObservableChildNodes.Add(this);
            
            updateNodeUiDelegate(new NodeUpdateObject(this)
            {
                ParentNodeId = (parentNodeElement != null) ? _parentNodeElement.ElementId.ToString() : null,
                NodeText = flow.properties.displayName,
                UpdateReason = UpdateReason.AddedToList
            });
        }
    }
}