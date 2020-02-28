using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.APIModels.ResponseModel
{
    public class EdgeHubModuleTwinModel
    {
        public string devicsId { get; set; }
        public string moduleId { get; set; }
        public string etag { get; set; }
        public string deviceEtag { get; set; }
        public string status { get; set; }
        public string statusUpdateTime { get; set; }
        public string connectionState { get; set; }
        public string lastActivityTime { get; set; }
        public int cloudToDeviceMessageCount { get; set; }
        public string authenticationType { get; set; }
        public x509Thumbprint x509Thumbprint { get; set; }
        public int version { get; set; }
        public HubProperty properties { get; set; }

    }
    public class HubProperty
    {
        public Edgehubpropertiesdesired desired { get; set; }
        public EdgehubPropertiesReported reported { get; set; }
    }
    public class EdgehubPropertiesReported
    {
        public string schemaVersion { get; set; }
        public EdgeHubPropertiesReportedVersion version { get; set; }
        public int lastDesiredVersion { get; set; }
        public JObject clients { get; set; }
    }
    public class EdgeHubPropertiesReportedVersion
    {
        public string version { get; set; }
        public string build { get; set; }
        public string commit { get; set; }
    }
    public class HubReportedClient
    {
        public string status { get; set; }
        public string lastConnectedTimeUtc { get; set; }
        public string lastDisconnectedTimeUtc { get; set; }
    }
}
