using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClusterManager.Core.Infrastructures;
using ClusterManager.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClusterManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        private readonly IMonitorBus _monitorBus;
        public MonitorController(IMonitorBus monitorBus)
        {
            _monitorBus = monitorBus;
        }
        //Azure日志服务
        [HttpPost("LogAnalytic/{email}/{subid}/{resourceGroupName}/{resourceName}/LogQuery")]
        public async Task<object> LogQuery(string email,[FromBody] AKSExcuteLogQuery logQuery)
        {
            return await _monitorBus.LogQuery(email, logQuery);
        }
        [HttpGet("LogAnalytic/{email}/{subid}/{resourceGroupName}/{resourceName}/ListLogQuery")]
        public async Task<object> ListLogQuery(string email, string subid, string resourceGroupName, string resourceName)
        {
            return await _monitorBus.ListLogQuery(email, subid, resourceGroupName, resourceName);
        }

        //Azure 见解
        [HttpPost("Insights/{email}/{subid}/GetAKSCPUMemoryInsight")]
        public async Task<object> GetAKSCPUMemoryInsight(string email,[FromBody] AKSInsightsQueryParams queryParams)
        {
            return await _monitorBus.GetAKSCPUMemoryInsight(email,queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/GetAKSCPUInsight")]
        public object GetAKSCPUInsight(string email, [FromBody] AKSInsightsQueryParams queryParams)
        {
            return _monitorBus.GetAKSCPUOrMemoryInsight(email, "cpu",queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/GetAKSMemoryInsight")]
        public object GetAKSMemoryInsight(string email, [FromBody] AKSInsightsQueryParams queryParams)
        {
            return _monitorBus.GetAKSCPUOrMemoryInsight(email, "memory", queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/GetAKSNodeCountInsight")]
        public async Task<object> GetAKSNodeCountInsight(string email, [FromBody] AKSInsightsQueryParams queryParams)
        {
            return await _monitorBus.GetAKSNodeCountInsight(email, queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/GetAKSPodCountInsight")]
        public async Task<object> GetAKSPodCountInsight(string email, [FromBody] AKSInsightsQueryParams queryParams)
        {
            return await _monitorBus.GetAKSPodCountInsight(email, queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/GetAKSRunStatusInsight")]
        public async Task<object> GetAKSRunStatusInsight(string email,string subid,[FromBody] AKSRunningStatusQuery queryParams)
        {
            return await _monitorBus.GetAKSRunStatusInsight(email, queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/ListAllNodeInsight")]
        public async Task<object> ListAllNodeInsight(string email,[FromBody] ListAllNodeQueryParams queryParams)
        {
            return await _monitorBus.ListAllNodeInsight(email, queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/GetNodeInfo")]
        public async Task<object> GetNodeInfo(string email,[FromBody] NodeInfoQueryParams queryParams)
        {
            return await _monitorBus.GetNodeInfo(email, queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/{nodeName}/ListAKSNodeContainerInsight")]
        public async Task<object> ListAKSNodeContainerInsight(string email,[FromBody] AKSNodeContainersQueryParams queryParams)
        {
            return await _monitorBus.ListAKSNodeContainerInsight(email, queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/{podName}/GetPodInfoInsight")]
        public async Task<object> GetPodInfoInsight(string email,[FromBody] AKSPodInfoInsightQueryParams queryParams)
        {
            return await _monitorBus.GetPodInfoInsight(email, queryParams);
        }
        [HttpPost("Insights/{email}/{subid}/{nodeName}/GetNodeContainerInfoInsight")]
        public async Task<object> GetNodeContainerInfoInsight(string email,[FromBody] AKSNodeContainerInfoQueryParams queryParams)
        {
            return await _monitorBus.GetNodeContainerInfoInsight(email, queryParams);
        }
    }
}