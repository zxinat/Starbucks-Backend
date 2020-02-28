using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.APIModels.ResponseModel
{
    public class EdgeAgentModuleTwinModel
    {
        public string deviceId { get; set; }
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
        public AgentProperty properties { get; set; }
        public JObject configurations { get; set; } = new JObject();
    }
    public class AgentProperty
    {
        public DesiredProperty desired { get; set; }
    }
    public class DesiredProperty
    {
        public string schemaVersion { get; set; }
        public Runtime runtime { get; set; }
        public SystemModules systemModules { get; set; }
        public JObject modules { get; set; } = new JObject();
    }
}
