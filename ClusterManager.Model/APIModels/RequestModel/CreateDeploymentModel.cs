using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ClusterManager.Model.APIModels.ResponseModel
{
    public class CreateDeploymentModel
    {
        public string id { get; set; } = "";
        public JObject labels { get; set; } = new Newtonsoft.Json.Linq.JObject();
        public int priority { get; set; }
        public string targetCondition { get; set; } = "";
        public Content content { get; set; } = new Content();
        public Metrics metrics { get; set; } = new Metrics();
        //[DefaultValue("*")]
        public string etag { get; set; } = "*";
    }
    public class Content
    {
        public ModulesContent modulesContent { get; set; } = new ModulesContent();
    }
    public class ModulesContent
    {
        [JsonProperty(PropertyName = "$edgeAgent")]
        public _EdgeAgent _edgeAgent { get; set; } = new _EdgeAgent();
        [JsonProperty(PropertyName = "$edgeHub")]
        public _EdgeHub _edgeHub { get; set; } = new _EdgeHub();
    }
    public class _EdgeAgent
    {
        [JsonProperty(PropertyName = "properties.desired")]
        public Agentpropertiesdesired agentpropertiesdesired { get; set; } = new Agentpropertiesdesired();
    }
    public class Agentpropertiesdesired
    {
        public JObject modules { get; set; } = new Newtonsoft.Json.Linq.JObject();
        public Runtime runtime { get; set; } = new Runtime();
        public string schemaVersion { get; set; } = "1.0";
        public SystemModules systemModules { get; set; } = new SystemModules();
    }
    public class modules
    {

    }
    public class Module
    {
        public modulesettings settings { get; set; } = new modulesettings();
        public string type { get; set; } = "docker";
        public string status { get; set; } = "running";
        public string restartPolicy { get; set; } = "always";
        public string version { get; set; } = "1.0";
    }
    public class modulesettings
    {
        public string image { get; set; }
        public string createOptions { get; set; }
    }
    public class Runtime
    {
        public Settings settings { get; set; } = new Settings();
        public string type { get; set; } = "docker";
    }
    public class Settings
    {
        public string minDockerVersion { get; set; } = "v1.25";
        public JObject registryCredentials { get; set; } = new JObject();
        public string loggingOptions { get; set; } = "";
    }
    public class SystemModules
    {
        public EdgeAgent edgeAgent { get; set; } = new EdgeAgent();
        public EdgeHub edgeHub { get; set; } = new EdgeHub();
    }
    public class EdgeAgent
    {
        public AgentSettings settings { get; set; } = new AgentSettings();
        public string type { get; set; } = "docker";
        public string status { get; set; }
        public string restartPolicy { get; set; }
    }
    public class AgentSettings
    {
        public string image { get; set; } = "mcr.microsoft.com/azureiotedge-agent:1.0";
        public string createOptions { get; set; } = "";
    }
    public class HubSettings
    {
        public string image { get; set; } = "mcr.microsoft.com/azureiotedge-hub:1.0";
        public string createOptions { get; set; } = "{\"HostConfig\":{\"PortBindings\":{\"8883/tcp\":[{\"HostPort\":\"8883\"}],\"5671/tcp\":[{\"HostPort\":\"5671\"}],\"443/tcp\":[{\"HostPort\":\"443\"}]}}}";
    }

    public class EdgeHub
    {
        public HubSettings settings { get; set; } = new HubSettings();
        public string type { get; set; } = "docker";
        public string status { get; set; } = "running";
        public string restartPolicy { get; set; } = "always";
        public JObject env { get; set; }
    }

    
    public class _EdgeHub
    {
        [JsonProperty(PropertyName = "properties.desired")]
        public Edgehubpropertiesdesired edgehubpropertiesdesired { get; set; } = new Edgehubpropertiesdesired();
    }
    public class Edgehubpropertiesdesired
    {
        public JObject routes { get; set; } = new Newtonsoft.Json.Linq.JObject();
        public string schemaVersion { get; set; } = "1.0";
        public StoreAndForwardConfiguration storeAndForwardConfiguration { get; set; } = new StoreAndForwardConfiguration();
    }
    public class StoreAndForwardConfiguration
    {
        public int timeToLiveSecs { get; set; } = 7200;
    }
    public class Metrics
    {
        public JObject queries { get; set; } = new Newtonsoft.Json.Linq.JObject();
        public JObject results { get; set; } = new Newtonsoft.Json.Linq.JObject();
    }
}
