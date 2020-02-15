using ClusterManager.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.Dto.Infrastructures
{
    public interface IMonitorDto
    {
        Task<object> ExcuteLogQuery(AKSExcuteLogQuery LogQuery, string access_token);
        Task<object> ListLogQuery(string subid, string resourceGroupName,
            string resourceName, string access_token);
        Task<object> GetAKSCPUMemoryInsight(AKSInsightsQueryParams queryParams, string access_token);
        Task<object> GetAKSNodeCountInsight(AKSInsightsQueryParams queryParams, string access_token);
        Task<object> GetAKSPodCountInsight(AKSInsightsQueryParams queryParams, string access_token);


    }
}
