using FlowOwnershipAudit.TreeViewUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FlowOwnershipAudit.TreeViewUIElements
{
    // this base class is used so we can display different types of objects in the treeview. Abstract because we enforce typed implementations
    internal abstract class TreeNodeElementBase 
    {
        internal readonly Action<NodeUpdateObject> updateNodeUi;

        public string ElementId { get; }

        internal abstract IEnumerable<TreeNodeElementBase> ChildObjects { get; }

        internal abstract TreeNodeElementBase Parent { get; }

        public TreeNodeElementBase(Action<NodeUpdateObject> updateNodeUi)
        {
            this.updateNodeUi = updateNodeUi;
            // generate a new guid for the element id
            ElementId = Guid.NewGuid().ToString();
        }
        internal MigrationStatus MigrationStatus { get; set; }
    }
    internal static class TreeNodeExtensions
    {
        internal static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach (var node in c.OfType<TreeNode>())
            {
                yield return node;

                foreach (var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }
    }
}