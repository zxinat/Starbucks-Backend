using ClusterManager.Model.ViewModels.ResponseModel;
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
        AKSInsightDataViewModel GetAKSCPUOrMemoryInsight(string email, string type,AKSInsightsQueryParams queryParams);
        Task<object> GetAKSNodeCountInsight(string email, AKSInsightsQueryParams queryParams);
        Task<object> GetAKSPodCountInsight(string email, AKSInsightsQueryParams queryParams);
        Task<object> GetAKSRunStatusInsight(string email, AKSRunningStatusQuery queryParams);

        Task<object> ListAllNodeInsight(string email, ListAllNodeQueryParams queryParams);
        Task<object> GetNodeInfo(string email, NodeInfoQueryParams queryParams);
        Task<object> ListAKSNodeContainerInsight(string email, AKSNodeContainersQueryParams queryParams);
        Task<object> GetPodInfoInsight(string email, AKSPodInfoInsightQueryParams queryParams);
        Task<object> GetNodeContainerInfoInsight(string email, AKSNodeContainerInfoQueryParams queryParams);


        Task<object> ListLogQuery(string email, string subid, string resourceGroupName,
            string resourceName);

    }
}
