using ClusterManager.Core.Infrastructures;
using ClusterManager.Dao.Infrastructures;
using ClusterManager.Dto.Infrastructures;
using ClusterManager.Model;
using ClusterManager.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.Core
{
    public class MonitorBus:IMonitorBus
    {
        private readonly IMonitorDto _monitorDto;
        private readonly ITokenDto _tokenDto;
        private readonly IAccountDao _accountDao;
        private readonly IOptions<TokenResourceModel> _tokenResource;
        //public string LogAnalyResource = "https://api.loganalytics.azure.cn";
        // apilog="https://api.loganalytics.azure.cn"
        // portallog="https://portal.loganalytics.azure.cn"
        public MonitorBus(IMonitorDto monitorDto,ITokenDto tokenDto, 
            IAccountDao accountDao,IOptions<TokenResourceModel> tokenResource)
        {
            _monitorDto = monitorDto;
            _tokenDto = tokenDto;
            _accountDao = accountDao;
            _tokenResource = tokenResource;
        }
        public async Task<object> LogQuery(string email,AKSExcuteLogQuery logQuery)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.apilog);
            return await _monitorDto.ExcuteLogQuery(logQuery,access_token);
        }
        public async Task<object> ListLogQuery(string email, string subid, string resourceGroupName,
            string resourceName)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.apilog);
            return await _monitorDto.ListLogQuery(subid, resourceGroupName, resourceName, access_token);
        }


        public async Task<object> GetAKSCPUMemoryInsight(string email,AKSInsightsQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.apilog);
            return await _monitorDto.GetAKSCPUMemoryInsight(queryParams, access_token);
        }
        public async Task<object> GetAKSNodeCountInsight(string email, AKSInsightsQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.apilog);
            return await _monitorDto.GetAKSNodeCountInsight(queryParams, access_token);
        }
        public async Task<object> GetAKSPodCountInsight(string email, AKSInsightsQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.apilog);
            return await _monitorDto.GetAKSPodCountInsight(queryParams, access_token);
        }
    }
}
