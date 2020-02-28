using ClusterManager.Core.Infrastructures;
using ClusterManager.Dao.Infrastructures;
using ClusterManager.Dto.Infrastructures;
using ClusterManager.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.Core
{
    public class AKSBus:IAKSBus
    {
        private readonly IAKSDto _aKSDto;
        private readonly ITokenDto _tokenDto;
        private readonly IAccountDao _accountDao;
        private readonly IOptions<TokenResourceModel> _tokenResource;
        //public string ManageResource = "https://management.chinacloudapi.cn";
        //public string LogAnalyResource = "https://api.loganalytics.azure.cn";
        public AKSBus(IAKSDto aKSDto,ITokenDto tokenDto,IAccountDao accountDao, IOptions<TokenResourceModel> tokenResource)
        {
            this._aKSDto = aKSDto;
            this._tokenDto = tokenDto;
            _accountDao = accountDao;
            _tokenResource = tokenResource;

        }
        public async Task<object> ListAllAKS(string email,string subid)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._aKSDto.ListAllAKS(subid, access_token);
        }
        public async Task<object> GetAKSInfo(string email, string subid,string resourceGroup, string AKSName)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._aKSDto.GetAKSInfo(subid, resourceGroup, AKSName, access_token);
        }
        public async Task<object> ListK8sVersion(string email,string subid)
        {
            string access_token= _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._aKSDto.ListK8sVersion(subid, access_token);
        }
        public async Task<object> ListWorkspace(string email,string subid)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._aKSDto.ListWorkspace(subid, access_token);
        }
        public async Task<object> CreateAKS(string email,string subid,string resourceGroupName, CreateAKSModel createAKSModel)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._aKSDto.CreateAKS(subid, resourceGroupName, createAKSModel,access_token);
        }
        public async Task<object> DeleteAKS(string email,string subid,string resourceGroupName,string resourceName)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._aKSDto.DeleteAKS(subid, resourceGroupName, resourceName, access_token);
        }
    }
}
