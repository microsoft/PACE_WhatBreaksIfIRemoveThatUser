using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlowOwnershipAudit.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AllowedOperation
    {
        public Type type { get; set; }
    }

    public class Cluster
    {
        public string category { get; set; }
        public string number { get; set; }
        public string uriSuffix { get; set; }
        public string geoShortName { get; set; }
        public string environment { get; set; }
    }

    public class CreatedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string email { get; set; }
        public string type { get; set; }
        public string tenantId { get; set; }
        public string userPrincipalName { get; set; }
    }

    public class DisallowedOperation
    {
        public Type type { get; set; }
        public Reason reason { get; set; }
    }

    public class GovernanceConfiguration
    {
        public string protectionLevel { get; set; }
    }

    public class LastModifiedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string email { get; set; }
        public string type { get; set; }
        public string tenantId { get; set; }
        public string userPrincipalName { get; set; }
    }

    public class LifecycleOperationsEnforcement
    {
        public List<AllowedOperation> allowedOperations { get; set; }
        public List<DisallowedOperation> disallowedOperations { get; set; }
    }

    public class LinkedEnvironmentMetadata
    {
        public string type { get; set; }
        public string resourceId { get; set; }
        public string friendlyName { get; set; }
        public string uniqueName { get; set; }
        public string domainName { get; set; }
        public string version { get; set; }
        public string instanceUrl { get; set; }
        public string instanceApiUrl { get; set; }
        public int baseLanguage { get; set; }
        public string instanceState { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime modifiedTime { get; set; }
        public string hostNameSuffix { get; set; }
        public string bapSolutionId { get; set; }
        public List<string> creationTemplates { get; set; }
        public string securityGroupId { get; set; }
        public string webApiVersion { get; set; }
        public string backgroundOperationsState { get; set; }
        public string scaleGroup { get; set; }
        public string platformSku { get; set; }
        public string schemaType { get; set; }
    }

    public class Management
    {
        public string id { get; set; }
    }

    public class NotificationMetadata
    {
        public string state { get; set; }
        public string branding { get; set; }
    }

    public class Properties
    {
        public string tenantId { get; set; }
        public string azureRegionHint { get; set; }
        public string displayName { get; set; }
        public DateTime createdTime { get; set; }
        public CreatedBy createdBy { get; set; }
        public DateTime lastModifiedTime { get; set; }
        public string provisioningState { get; set; }
        public string creationType { get; set; }
        public string environmentSku { get; set; }
        public string environmentType { get; set; }
        public bool isDefault { get; set; }
        public RuntimeEndpoints runtimeEndpoints { get; set; }
        public LinkedEnvironmentMetadata linkedEnvironmentMetadata { get; set; }
        public string trialScenarioType { get; set; }
        public NotificationMetadata notificationMetadata { get; set; }
        public string retentionPeriod { get; set; }
        public States states { get; set; }
        public UpdateCadence updateCadence { get; set; }
        public RetentionDetails retentionDetails { get; set; }
        public ProtectionStatus protectionStatus { get; set; }
        public Cluster cluster { get; set; }
        public List<object> connectedGroups { get; set; }
        public LifecycleOperationsEnforcement lifecycleOperationsEnforcement { get; set; }
        public GovernanceConfiguration governanceConfiguration { get; set; }
        public bool bingChatEnabled { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public string description { get; set; }
    }

    public class ProtectionStatus
    {
        public string keyManagedBy { get; set; }
    }

    public class Reason
    {
        public string message { get; set; }
        public string type { get; set; }
    }

    public class RequestedBy
    {
        public string displayName { get; set; }
        public string type { get; set; }
    }

    public class RetentionDetails
    {
        public string retentionPeriod { get; set; }
        public DateTime backupsAvailableFromDateTime { get; set; }
    }

    public class EnvironmentList
    {
        public List<Environment> value { get; set; }
    }

    public class Runtime
    {
        public string runtimeReasonCode { get; set; }
        public RequestedBy requestedBy { get; set; }
        public string id { get; set; }
    }

    public class RuntimeEndpoints
    {
        [JsonProperty("microsoft.BusinessAppPlatform")]
        public string microsoftBusinessAppPlatform { get; set; }

        [JsonProperty("microsoft.CommonDataModel")]
        public string microsoftCommonDataModel { get; set; }

        [JsonProperty("microsoft.PowerApps")]
        public string microsoftPowerApps { get; set; }

        [JsonProperty("microsoft.PowerAppsAdvisor")]
        public string microsoftPowerAppsAdvisor { get; set; }

        [JsonProperty("microsoft.PowerVirtualAgents")]
        public string microsoftPowerVirtualAgents { get; set; }

        [JsonProperty("microsoft.ApiManagement")]
        public string microsoftApiManagement { get; set; }

        [JsonProperty("microsoft.Flow")]
        public string microsoftFlow { get; set; }
    }

    public class States
    {
        public Management management { get; set; }
        public Runtime runtime { get; set; }
    }

    public class Type
    {
        public string id { get; set; }
    }

    public class UpdateCadence
    {
        public string id { get; set; }
    }

    public class Environment
    {
        public string id { get; set; }
        public string type { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public Properties properties { get; set; }
        public List<Flow> flows { get; set; } = new List<Flow>();
        public List<Connection> connections { get; set; } = new List<Connection>();
    }
}
