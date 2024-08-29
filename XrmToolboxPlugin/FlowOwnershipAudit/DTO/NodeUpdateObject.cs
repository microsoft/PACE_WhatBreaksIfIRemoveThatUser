using FlowOwnershipAudit.TreeViewUIElements;

namespace FlowOwnershipAudit.DTO
{
    // This object is only used to transfer data to update the UI, it is not meant to hold any data on itself
    internal class NodeUpdateObject
    {
        internal UpdateReason UpdateReason { get; set; }
        internal string NodeId { get { return TreeNodeElement.ElementId; } }
        internal string ParentNodeId { get; set; }
        internal string NodeText { get; set; }
        internal TreeNodeElementBase TreeNodeElement { get; set; }
        public string ToolTipText { get; set; }
    }

    internal enum UpdateReason
    {
        AddedToList,
        RemovedFromList,
        DetailsAdded,
        MigrationFailed,
        MigrationSucceeded
    }
}