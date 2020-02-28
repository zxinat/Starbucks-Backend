using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using ClusterManager.Model;
using Newtonsoft.Json;

namespace ClusterManager.Utility
{
    public class CreateRequestJObject
    {
        public JObject CreateAKSRequestJObject(CreateAKSModel AKSModel)
        {
            var agentPoolProfile = new JObject
            {
                {"name",AKSModel.Properties.agentPoolProfiles[0].name },
                {"count" ,AKSModel.Properties.agentPoolProfiles[0].count},
                {"vmSize" ,AKSModel.Properties.agentPoolProfiles[0].vmSize},
                {"osType",AKSModel.Properties.agentPoolProfiles[0].osType },
                {"type","VirtualMachineScaleSets" }
            };
            var agentPoolProfiles = new JArray();
            agentPoolProfiles.Add(agentPoolProfile);
            var networkProfile = new JObject
            {
                {"loadBalancerSku",AKSModel.Properties.networkProfile.loadBalancerSku },
                {"outboundType","loadBalancer" }
            };
            var servicePrincipalProfile = new JObject
            {
                {"clientId",AKSModel.Properties.servicePrincipalProfile.clientId },
                {"secret" ,AKSModel.Properties.servicePrincipalProfile.secret}
            };
            var config = new JObject
            {
                {"logAnalyticsWorkspaceResourceID",AKSModel.Properties.addonProfiles.omsagent.config.logAnalyticsWorkspaceResourceID }
            };
            var addonProfiles = new JObject
            {
                {"omsagent" ,new JObject
                    {
                        { "config",config },
                        { "enabled", AKSModel.Properties.addonProfiles.omsagent.enabled}
                    }
                }
            };
            var properties = new JObject
            {
                {"kubernetesVersion",AKSModel.Properties.kubernetesVersion },
                {"dnsPrefix",AKSModel.Properties.dnsPrefix },
                {"agentPoolProfiles" ,agentPoolProfiles},
                {"networkProfile" ,networkProfile},
                {"servicePrincipalProfile",servicePrincipalProfile },
                {"addonProfiles" ,addonProfiles},
                {"enableRBAC",AKSModel.Properties.enableRBAC }
            };
            var requestmodel = new JObject
            {
                {"location",AKSModel.location },
                {"tags",AKSModel.tags },
                {"properties", properties}
            };
            return requestmodel;
        }
        public JObject CreateModulesContentRequestJObject()
        {
            var requestBody = new JObject
            {
                { "id",""},
                { "labels",new JObject{"testversion","1" } },
                { "priority",10},
                {"targetCondition","" },
                
            };
            var modulesContent = new JObject
            {
                { "$edgeAgent"},
                { "$egdeHub"}
            };
            return null;   
        }

    }
}
