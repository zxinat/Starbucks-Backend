using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model
{
    public class CreateAKSModel
    {
        public string name { get; set; }
        public string location { get; set; }
        public JObject tags { get; set; }
        public AKSProperties Properties { get; set; }

    }
    public class AgentPoolProfilesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string vmSize { get; set; }
        public string osType { get; set; }

    }

    public class NetworkProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public string loadBalancerSku { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string outboundType { get; set; }
    }

    public class ServicePrincipalProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public string clientId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secret { get; set; }
    }

    public class AddonProfiles
    {
        public OmsAgent omsagent { get; set; }
    }
    public class OmsAgent
    {
        public Config config { get; set; }
        public bool enabled { get; set; }
    }

    public class Config
    {
        public string logAnalyticsWorkspaceResourceID { get; set; }
    }

    public class AKSProperties
    {
        /// <summary>
        /// 
        /// </summary>
        public string kubernetesVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dnsPrefix { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AgentPoolProfilesItem> agentPoolProfiles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public NetworkProfile networkProfile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ServicePrincipalProfile servicePrincipalProfile { get; set; }

        public AddonProfiles addonProfiles { get; set; }

        public bool enableRBAC { get; set; }
    }
}
