using ClusterManager.Core.Infrastructures;
using ClusterManager.Dao.Infrastructures;
using ClusterManager.Dto.Infrastructures;
using ClusterManager.Model;
using ClusterManager.Model.APIModels.ResponseModel;
using ClusterManager.Model.ViewModels.ResponseModel;
using ClusterManager.Utility;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            
            return await _monitorDto.GetAKSCPUMemoryInsight(queryParams, access_token);
        }
        public AKSInsightDataViewModel GetAKSCPUOrMemoryInsight(string email, string type,AKSInsightsQueryParams queryParams)
        {
            int index;
            if(type=="cpu")
            {
                index = 1;
            }
            else
            {
                index = 0;
            }
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            DateTransform dateTransform = new DateTransform();
            AKSInsightDataModel aKSInsightData = (AKSInsightDataModel)_monitorDto.GetAKSCPUMemoryInsight(queryParams, access_token).Result.data;
            AKSInsightDataViewModel aKSInsightDataView = new AKSInsightDataViewModel();
            aKSInsightDataView.ClusterName = (string)aKSInsightData.tables[0].rows[index][0];
            aKSInsightDataView.CounterName = (string)aKSInsightData.tables[0].rows[index][1];
            aKSInsightDataView.Min = (double)aKSInsightData.tables[0].rows[index][2];
            aKSInsightDataView.Avg = (double)aKSInsightData.tables[0].rows[index][3];
            aKSInsightDataView.Max = (double)aKSInsightData.tables[0].rows[index][4];
            aKSInsightDataView.P50 = (double)aKSInsightData.tables[0].rows[index][5];
            aKSInsightDataView.P90 = (double)aKSInsightData.tables[0].rows[index][6];
            aKSInsightDataView.P95 = (double)aKSInsightData.tables[0].rows[index][7];
            string list_TimeGenerated = (string)aKSInsightData.tables[0].rows[index][8];
            List<string> timelist = JsonConvert.DeserializeObject<List<string>>(list_TimeGenerated);
            List<string> newtimelist = new List<string>();
            foreach (string date in timelist)
            {
               newtimelist.Add( dateTransform.UTC2CCT(date));
            }
            aKSInsightDataView.list_TimeGenerated = newtimelist;
            string list_Min = (string)aKSInsightData.tables[0].rows[index][9];
            aKSInsightDataView.list_Min = JsonConvert.DeserializeObject<List<double>>(list_Min);
            string list_Avg = (string)aKSInsightData.tables[0].rows[index][10];
            aKSInsightDataView.list_Avg = JsonConvert.DeserializeObject<List<double>>(list_Avg);
            string list_Max = (string)aKSInsightData.tables[0].rows[index][11];
            aKSInsightDataView.list_Max = JsonConvert.DeserializeObject<List<double>>(list_Max);
            string list_P50 = (string)aKSInsightData.tables[0].rows[index][12];
            aKSInsightDataView.list_P50 = JsonConvert.DeserializeObject<List<double>>(list_P50);
            string list_P90 = (string)aKSInsightData.tables[0].rows[index][13];
            aKSInsightDataView.list_P90 = JsonConvert.DeserializeObject<List<double>>(list_P90);
            string list_P95 = (string)aKSInsightData.tables[0].rows[index][14];
            aKSInsightDataView.list_P95 = JsonConvert.DeserializeObject<List<double>>(list_P95);
            return aKSInsightDataView;
        }
        public async Task<object> GetAKSNodeCountInsight(string email, AKSInsightsQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await _monitorDto.GetAKSNodeCountInsight(queryParams, access_token);
        }
        public async Task<object> GetAKSPodCountInsight(string email, AKSInsightsQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await _monitorDto.GetAKSPodCountInsight(queryParams, access_token);
        }
        public async Task<object> GetAKSRunStatusInsight(string email,AKSRunningStatusQuery queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await _monitorDto.GetAKSRunningStatus(queryParams, access_token);
        }

        public async Task<object> ListAllNodeInsight(string email,ListAllNodeQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await _monitorDto.ListAllNodeInsight(queryParams, access_token);
        }
        public async Task<object> GetNodeInfo(string email,NodeInfoQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await _monitorDto.GetNodeInfo(queryParams, access_token);
        }

        public async Task<object> ListAKSNodeContainerInsight(string email,AKSNodeContainersQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await _monitorDto.ListAKSNodeContainerInsight(queryParams, access_token);
        }
        public async Task<object> GetPodInfoInsight(string email,AKSPodInfoInsightQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await _monitorDto.GetPodInfoInsight( queryParams,access_token);
        }
        public async Task<object> GetNodeContainerInfoInsight(string email,AKSNodeContainerInfoQueryParams queryParams)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await _monitorDto.GetContainerInfoInsight(queryParams, access_token);
        }
    }
}
