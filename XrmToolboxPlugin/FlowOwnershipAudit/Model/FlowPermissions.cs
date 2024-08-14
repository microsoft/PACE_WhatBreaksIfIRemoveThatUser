using System.Collections.Generic;

namespace FlowOwnershipAudit.Model
{
    public class FlowPermissionPrincipal
    {
        public string id { get; set; }
        public string type { get; set; }
        public string tenantId { get; set; }
    }

    public class FlowPermissionProperties
    {
        public string roleName { get; set; }
        public string permissionType { get; set; }
        public FlowPermissionPrincipal principal { get; set; }
    }

    public class FlowPermissionList
    {
        public List<FlowPermission> value { get; set; }
    }

    public class FlowPermission
    {
        public string name { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public FlowPermissionProperties properties { get; set; }
    }
}
