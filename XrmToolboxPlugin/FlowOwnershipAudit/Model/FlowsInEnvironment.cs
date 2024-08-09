using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatBreaksIf.Model
{
    public class Creator
    {
        public string tenantId { get; set; }
        public string objectId { get; set; }
        public string userId { get; set; }
        public string userType { get; set; }
    }

    public class FlowEnvironment
    {
        public string name { get; set; }
        public string type { get; set; }
        public string id { get; set; }
    }

    public class FlowProperties
    {
        public string apiId { get; set; }
        public string displayName { get; set; }
        public string state { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime lastModifiedTime { get; set; }
        public string flowSuspensionReason { get; set; }
        public Environment environment { get; set; }
        public Creator creator { get; set; }
        public bool flowFailureAlertSubscribed { get; set; }
        public string workflowEntityId { get; set; }
        public string workflowUniqueId { get; set; }
        public bool isManaged { get; set; }
    }

    public class FlowList
    {
        public List<Flow> value { get; set; }
        public string nextLink { get; set; }
    }

    public class Flow
    {
        public string name { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public FlowProperties properties { get; set; }
        public List<FlowPermission> permissions { get; set; }
    }



}
