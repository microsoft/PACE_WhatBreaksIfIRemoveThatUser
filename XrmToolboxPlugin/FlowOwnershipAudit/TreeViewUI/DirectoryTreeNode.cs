using System;
using System.Collections.Generic;
using FlowOwnershipAudit.TreeViewUIElements;

namespace FlowOwnershipAudit.TreeViewUI
{
    /// <summary>
    /// This is a tree node element that is used to organize the child nodes under an environment. It is used to read either "flows" or "conenction references" now. 
    /// This node is not supposed to hold any data itself, just references to the child nodes.
    /// </summary>
    internal class DirectoryTreeNode : TreeNodeElementBase
    {
        private readonly string directoryName;
        public EnvironmentTreeNodeElement parentNodeElement;
        internal List<TreeNodeElementBase> childNodeElements { get; } = new List<TreeNodeElementBase>();

        public DirectoryTreeNode(Action<NodeUpdateObject> updateNodeUi,
                                 string directoryName,
                                 EnvironmentTreeNodeElement parentNodeElement) : base(updateNodeUi)
        {
            this.directoryName = directoryName;
            this.parentNodeElement = parentNodeElement;

            // ctor was called this means we have to add this node to the treeview 
            updateNodeUi(new NodeUpdateObject()
            {
                TreeNodeElement = this,
                NodeText = directoryName,
                UpdateReason = UpdateReason.AddedToList,
                ParentNodeId = (parentNodeElement != null) ? parentNodeElement.ElementId : null
            });
        }


        internal override IEnumerable<TreeNodeElementBase> ChildObjects => childNodeElements;

        internal override TreeNodeElementBase Parent => parentNodeElement;

        internal string DirectoryName => directoryName;
    }
}