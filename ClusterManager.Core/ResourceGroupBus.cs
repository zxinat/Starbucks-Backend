using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ClusterManager.Core.Infrastructures;
using ClusterManager.Model.ResponseModel;
using ClusterManager.Dto;
using ClusterManager.Dto.Infrastructures;
using Microsoft.Extensions.Configuration;
using ClusterManager.Model;
using ClusterManager.Dao.Infrastructures;
using Microsoft.Extensions.Options;

namespace ClusterManager.Core
{
    public class ResourceGroupBus:IResourceGroupBus
    {
        private readonly IResourceGroupDto _resourceGroupDto;
        private readonly IConfiguration _configuration;
        private readonly ITokenDto _tokenDto;
        private readonly IOptions<TokenResourceModel> _tokenResource;
        //private readonly string subid;
        private readonly ISubscriptionDto _subscriptionDto;
        private readonly ServicePrinciple servicePrinciple;
        //public string ManageResource = "https://management.chinacloudapi.cn";
        //public string LogAnalyResource = "https://api.loganalytics.azure.cn";
        public ResourceGroupBus(IResourceGroupDto resourceGroupDto,
            IConfiguration configuration,
            ITokenDto tokenDto,
            ISubscriptionDto subscriptionDto,
            IAccountDao accountDao,
            IOptions<TokenResourceModel> tokenResource)
        {

            this._resourceGroupDto = resourceGroupDto;
            this._configuration = configuration;
            this._tokenDto = tokenDto;
            this._subscriptionDto = subscriptionDto;
            _tokenResource = tokenResource;
            //this.subid = "6273fbea-8a11-498b-8218-02b6f4398e12";
        }
        public async Task<ResourceGroupModel> GetAllResourceGroup(string email,string subid)
        {
            //string subid = this._configuration["accountsetting:subscriptionId"]; 
            //string subid = "6273fbea-8a11-498b-8218-02b6f4398e12";
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            ResourceGroupModel resourceGroup = await this._resourceGroupDto.GetAllResource(subid ,access_token);
            return resourceGroup;
        }
        public async Task<string> CreateOrUpdate(string email,string subid,string resourceName,string location)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._resourceGroupDto.CreateOrUpdate(subid, resourceName,location,access_token);
        }
        public async Task<string> ListResource(string email,string subid,string resourceGroupName)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._resourceGroupDto.ListResource(subid, resourceGroupName, access_token);
        }
        public async Task<List<SubscriptionModel>> GetSubsciptions(string email)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._subscriptionDto.GetSubscriptions(access_token);
        }
        public async Task<string> GetSubscriptionNameById(string email,string subid)
        {
            string access_token = _tokenDto.GetTokenString(email, _tokenResource.Value.manage);
            return await this._subscriptionDto.GetSubscriptionNameById(subid, access_token);
        }

    }
}
