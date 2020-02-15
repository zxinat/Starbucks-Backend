using ClusterManager.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.Core.Infrastructures
{
    public interface IMonitorBus
    {
        Task<object> LogQuery(string email, AKSExcuteLogQuery logQuery);
        Task<object> GetAKSCPUMemoryInsight(string email,AKSInsightsQueryParams queryParams);
        Task<object> GetAKSNodeCountInsight(string email, AKSInsightsQueryParams queryParams);
        Task<object> GetAKSPodCountInsight(string email, AKSInsightsQueryParams queryParams);
        Task<object> ListLogQuery(string email, string subid, string resourceGroupName,
            string resourceName);

    }
}
