using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatBreaksIf.Model
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
