using ClusterManager.Dto.Infrastructures;
using ClusterManager.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.Dto
{
    public class MonitorDto:IMonitorDto
    {
        readonly IHttpClientFactory _clientFactory;
        public MonitorDto(IHttpClientFactory clientFactory)
        {
            this._clientFactory = clientFactory;
        }

        //Azure日志服务
        public async Task<object> ExcuteLogQuery(AKSExcuteLogQuery logquery,string access_token)
        {
            //timespan="P1D"
            //string query = string.Format("set query_take_max_records=10001;set truncationmaxsize=67108864;InsightsMetrics| limit 50");
            CreateQueryString createQuery = new CreateQueryString();
            var requestmodel = new JObject
            {
                { "query",createQuery.ExcuteAKSLogQuery(logquery)}
            };
            string requestbody = JsonConvert.SerializeObject(requestmodel);
            string url = string.Format("/v1/subscriptions/{0}/resourceGroups/{1}/providers/" +
                "Microsoft.ContainerService/managedClusters/{2}/query?scope=hierarchy&timespan={3}",
                logquery.subid, logquery.resourceGroupName, logquery.resourceName, logquery.timespan);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient("LogAnalyResource");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if(response.IsSuccessStatusCode)
            {
                JObject job = (JObject)JsonConvert.DeserializeObject(result);
                return job;
            }
            else
            {
                return response.ReasonPhrase;
            }

        }
        public async Task<object> ListLogQuery(string subid,string resourceGroupName,
            string resourceName,string access_token)
        {
            string url = string.Format("/subscriptions/{0}/resourceGroups/{1}/providers" +
                "/Microsoft.ContainerService/managedClusters/{2}/providers/microsoft.insights" +
                "/logs/metadata?api-version=2018-03-01-preview&scope=hierarchy",
                subid,resourceGroupName,resourceName);
            var request= new HttpRequestMessage(HttpMethod.Get, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JObject job = (JObject)JsonConvert.DeserializeObject(result);
                return job;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }

        //日志历史记录
        public async Task<object> GetLogHistory()
        {
            return null;
        }

        //见解服务
        public async Task<object>GetAKSCPUMemoryInsight(AKSInsightsQueryParams queryParams ,string access_token)
        {
            CreateQueryString createQueryString = new CreateQueryString();
            var requestmodel = new JObject
            {
                { "query",createQueryString.AKSCPUMemoryInsightsQuery(queryParams)}
            };
            string requestbody = JsonConvert.SerializeObject(requestmodel);
            string url = string.Format("/subscriptions/{0}/resourcegroups/defaultresourcegroup-east2/providers/" +
                "microsoft.operationalinsights/workspaces/defaultworkspace-{0}-east2/query?api-version=2017-10-01",
                queryParams.subid);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JObject job = (JObject)JsonConvert.DeserializeObject(result);
                return job;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
        public async Task<object> GetAKSNodeCountInsight(AKSInsightsQueryParams queryParams, string access_token)
        {
            CreateQueryString createQueryString = new CreateQueryString();
            var requestmodel = new JObject
            {
                { "query",createQueryString.AKSNodeCountQuery(queryParams)}
            };
            string requestbody = JsonConvert.SerializeObject(requestmodel);
            string url = string.Format("/subscriptions/{0}/resourcegroups/defaultresourcegroup-east2/providers/" +
                "microsoft.operationalinsights/workspaces/defaultworkspace-{0}-east2/query?api-version=2017-10-01",
                queryParams.subid);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JObject job = (JObject)JsonConvert.DeserializeObject(result);
                return job;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
        public async Task<object> GetAKSPodCountInsight(AKSInsightsQueryParams queryParams, string access_token)
        {
            CreateQueryString createQueryString = new CreateQueryString();
            var requestmodel = new JObject
            {
                { "query",createQueryString.AKSPodCountQuery(queryParams)}
            };
            string requestbody = JsonConvert.SerializeObject(requestmodel);
            string url = string.Format("/subscriptions/{0}/resourcegroups/defaultresourcegroup-east2/providers/" +
                "microsoft.operationalinsights/workspaces/defaultworkspace-{0}-east2/query?api-version=2017-10-01",
                queryParams.subid);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JObject job = (JObject)JsonConvert.DeserializeObject(result);
                return job;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
    }


}
