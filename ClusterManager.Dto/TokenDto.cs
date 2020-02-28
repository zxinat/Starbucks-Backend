﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClusterManager.Dao.Infrastructures;
using ClusterManager.Dto.Infrastructures;
using ClusterManager.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Security.Cryptography;
using System.Globalization;

namespace ClusterManager.Dto
{
    public class TokenDto:ITokenDto
    {
        private readonly IAccountDao _accountDao;
        private readonly IConfiguration _configuration;
        private IMemoryCache _cache;
        public TokenDto(IConfiguration configuration,IAccountDao accountDao,IMemoryCache cache)
        {
            this._accountDao = accountDao;
            this._configuration = configuration;
            _cache = cache;
        }
        public string GetTokenString(string email,string resource)
        {
            _cache.GetOrCreate(resource, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(55));
                return GetToken(email,resource).Result.access_token;
            });
            return _cache.Get(resource).ToString();
        }
        public async Task<TokenModel>  GetToken(string email,string resource)
        {
            ServicePrinciple servicePrinciple=this._accountDao.GetCurrentService(email);
            //string clientId = this._configuration["accountsetting:clientId"];
            //string clientSecret = this._configuration["accountsetting:clientSecret"];
            //string resource = this._configuration["accountsetting:resource"];
            //string tenantId = this._configuration["accountsetting:tenantId"];
            //string tenantId = "c7917735-5d61-4832-8b54-b11d5f1e7810";
            string tenantId = servicePrinciple.TenantId;
            //string clientSecret = "77b650d9-f8d3-4511-8587-c6c930e05225";
            string clientSecret = servicePrinciple.ClientSecret;
            //string resource = "https://management.chinacloudapi.cn";
            string clientId = servicePrinciple.ClientId;
            //string clientId = "57d1ea2f-7ba4-4d03-936a-036368ff957c";
            List<KeyValuePair<string, string>> vals = new List<KeyValuePair<string, string>>();
            vals.Add(new KeyValuePair<string, string>("client_id", clientId));
            vals.Add(new KeyValuePair<string, string>("resource", resource));
            vals.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            vals.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            string tokenUrl = string.Format("https://login.chinacloudapi.cn/{0}/oauth2/token",tenantId);
            TokenModel TokenResponse = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                HttpContent content = new FormUrlEncodedContent(vals);
                HttpResponseMessage hrm = httpClient.PostAsync(tokenUrl,content).Result;
                
                if (hrm.IsSuccessStatusCode)
                {
                    string data = await hrm.Content.ReadAsStringAsync();
                    TokenResponse = JsonConvert.DeserializeObject<TokenModel>(data);
                    Console.WriteLine(TokenResponse);
                    //await DataOperations(authenticationResponse);
                }
                else
                {
                    Console.WriteLine("Error." + hrm.ReasonPhrase);
                }

            }
            return TokenResponse ;
        }
        public string generateSasToken(string resourceUri,string key,string policyName,int expiryInSeconds=3600)
        {
            TimeSpan fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
            string expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + expiryInSeconds);

            string stringToSign = WebUtility.UrlEncode(resourceUri) + "\n" + expiry;

            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key));
            string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            string token = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}", WebUtility.UrlEncode(resourceUri), WebUtility.UrlEncode(signature), expiry);

            if (!String.IsNullOrEmpty(policyName))
            {
                token += "&skn=" + policyName;
            }

            return token;
        }
        public string getIoTHubSasToken()
        {
            string resourceUri = "Starbucks.azure-devices.cn";
            string key = "yUvHIk8kjJqym7VYRBYJ3v8WLgiSRAo4y5+1fYPj9dk=";
            string policyName = "iothubowner";
            return generateSasToken(resourceUri, key, policyName);
        }
    }
}
