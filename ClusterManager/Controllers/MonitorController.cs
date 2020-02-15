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
    }
}