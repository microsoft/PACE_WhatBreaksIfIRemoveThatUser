using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowOwnershipAudit.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ConnectionParameters
    {
        public string sku { get; set; }
        public string username { get; set; }
        public string storageaccount { get; set; }

        [JsonProperty("token:grantType")]
        public string tokengrantType { get; set; }

        [JsonProperty("token:clientId")]
        public string tokenclientId { get; set; }

        [JsonProperty("token:TenantId")]
        public string tokenTenantId { get; set; }
    }

    public class ConnectionParametersSet
    {
        public string name { get; set; }
        public ConnectionReferencesValues values { get; set; }
    }

    public class ConnectionReferencesCreatedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string email { get; set; }
        public string type { get; set; }
        public string tenantId { get; set; }
        public string userPrincipalName { get; set; }
    }

    public class ConnectionReferencesEnvironment
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class ConnectionReferencesError
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class ConnectionReferencesProperties
    {
        public string apiId { get; set; }
        public string displayName { get; set; }
        public string iconUri { get; set; }
        public List<Status> statuses { get; set; }
        public ConnectionParameters connectionParameters { get; set; }
        public int keywordsRemaining { get; set; }
        public ConnectionReferencesCreatedBy createdBy { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime lastModifiedTime { get; set; }
        public ConnectionReferencesEnvironment environment { get; set; }
        public bool allowSharing { get; set; }
        public ConnectionParametersSet connectionParametersSet { get; set; }
        public DateTime? expirationTime { get; set; }
        public List<TestLink> testLinks { get; set; }
        public string accountName { get; set; }
    }

    public class ConnectionReferencesList
    {
        public List<ConnectionReference> value { get; set; }
    }

    public class Status
    {
        public string status { get; set; }
        public string target { get; set; }
        public ConnectionReferencesError error { get; set; }
    }

    public class TestLink
    {
        public string requestUri { get; set; }
        public string method { get; set; }
    }

    public class ConnectionReference
    {
        public string name { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public ConnectionReferencesProperties properties { get; set; }
        public bool isOwnedByX { get; set; }
    }

    public class ConnectionReferencesValues
    {
    }


}
