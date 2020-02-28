using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ClusterManager.Model;
using ClusterManager.Model.ResponseModel;
using Newtonsoft.Json;
using ClusterManager.Dto.Infrastructures;
using System.Net.Http.Headers;
using ClusterManager.Model.ViewModels;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Management.IotHub;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Management;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Management.ContainerService.Fluent;
using ClusterManager.Model.ViewModels.RequestModel;
using ClusterManager.Model.APIModels.ResponseModel;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
namespace ClusterManager.Dto
{
    public class IoTHubResourceDto:IIoTHubResourceDto
    {
        readonly IHttpClientFactory _clientFactory;
        public IoTHubResourceDto(IHttpClientFactory clientFactory)
        {
            this._clientFactory = clientFactory;
        }
        public async Task<IoTHubResourceModel> ListBySubId(string SubId,string access_token)

        {
            //List<KeyValuePair<string, string>> vals = new List<KeyValuePair<string, string>>();

            //vals.Add(new KeyValuePair<string, string>("subscriptionId", SubId)); 
            string url = string.Format("https://management.chinacloudapi.cn/subscriptions/{0}" +
                "/providers/Microsoft.Devices/IotHubs?api-version=2018-04-01", SubId);
            Model.ResponseModel.IoTHubResourceModel objs =null;
            using(HttpClient httpClient = new HttpClient())
            {
                //httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                //HttpContent content = new FormUrlEncodedContent(vals);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",access_token);
                HttpResponseMessage hrm = httpClient.GetAsync(url).Result;
                if (hrm.IsSuccessStatusCode)
                {
                    string data = await hrm.Content.ReadAsStringAsync();
                    objs = JsonConvert.DeserializeObject<IoTHubResourceModel>(data);
                    //await DataOperations(authenticationResponse);
                }
                else
                {
                    Console.WriteLine("Error." + hrm.ReasonPhrase);
                }
            }
            return objs;
        }
        public async Task<string> CreateOrUpdate(string subid,IoTHubModel ioTHubModel,string resourceGoupNname ,string access_token)
        {
            string url = string.Format("{0}/resourceGroups/{1}/providers/Microsoft.Devices/IotHubs/" +
                "{2}?api-version=2018-04-01",subid, resourceGoupNname, ioTHubModel.name);
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(ioTHubModel);
            request.Content= new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return response.ReasonPhrase;
            }
            else
            {
                return response.ReasonPhrase;
            }
            /*string url = string.Format("https://management.chinacloudapi.cn/subscriptions/" +
                "{0}/resourceGroups/{1}/" +
                "providers/Microsoft.Devices/IotHubs/" +
                "{2}?api-version=2018-04-01",subid,ioTHubModel.resourceGroupName,ioTHubModel.iothubName);
            using (HttpClient httpClient = new HttpClient())
            {
                //httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                string requestbody = JsonConvert.SerializeObject(ioTHubModel);
                var content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                HttpResponseMessage hrm =  httpClient.PutAsync(url,content).Result;
                if (hrm.IsSuccessStatusCode)
                {
                    return "success";
                }
                else
                {
                    Console.WriteLine("Error." + hrm.ReasonPhrase);
                    return "error";
                }
            }*/
            
        }
        public async Task<string> DeleteIotHub(string subid, string resourceGroupName, string resourceName,string access_token)
        {
            string url = string.Format("{0}/resourceGroups/{1}/providers/Microsoft.Devices" +
                "/IotHubs/{2}?api-version=2018-04-01", subid, resourceGroupName, resourceName);
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return "success";
            }
            else
            {
                return "error";
            }
        }
        public async Task<IoTHubInfoModel> GetIoTHubInfo(string subid,string resourceGroupName,string resourceName,string access_token)
        {
            string url = string.Format("{0}/resourceGroups/{1}/providers/Microsoft.Devices/IotHubs/" +
                "{2}?api-version=2018-04-01", subid, resourceGroupName, resourceName);
            var request= new HttpRequestMessage(HttpMethod.Get, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();
            IoTHubInfoModel ioTHubInfo = null;
            if (response.IsSuccessStatusCode)
            {
                ioTHubInfo = JsonConvert.DeserializeObject<IoTHubInfoModel>(data);
            }
            else
            {
                Console.WriteLine("Error." + response.ReasonPhrase);
            }
            return ioTHubInfo;
        }
        public async Task<IoTHubKeys> GetIoTHubKeys(string subid,string resourceGroupName,string resourceName,string access_token)
        {
            string url =string.Format( "{0}/resourceGroups/{1}/providers/Microsoft.Devices/IotHubs/" +
                "{2}/listkeys?api-version=2018-04-01",subid,resourceGroupName,resourceName);
            var request= new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();
            IoTHubKeys ioTHubKeys = null;
            if (response.IsSuccessStatusCode)
            {
                ioTHubKeys = JsonConvert.DeserializeObject<IoTHubKeys>(data);
            }
            else
            {
                Console.WriteLine("Error." + response.ReasonPhrase);
            }
            return ioTHubKeys;

        }
        public async Task<IoTHubKey> GetIoTHubKeyForKeyName(string subid,string resourceGroupName,string resourceName,string keyName,string access_token)
        {
            string url = string.Format("{0}/resourceGroups/{1}/providers/Microsoft.Devices/IotHubs/{2}/IotHubKeys/{3}/listkeys?api-version=2018-04-01",
                subid,resourceGroupName,resourceName,keyName);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();
            IoTHubKey ioTHubKey = null;
            if (response.IsSuccessStatusCode)
            {
                ioTHubKey = JsonConvert.DeserializeObject<IoTHubKey>(data);
            }
            else
            {
                Console.WriteLine("Error." + response.ReasonPhrase);
            }
            return ioTHubKey;
        }
        public async Task<string> CreateDevice(AccessPolicyModel accessPolicyModel, string deviceId,bool isIotEdge)
        {
            IoTHubDevice ioTHubDevice = new IoTHubDevice(accessPolicyModel);
            return await ioTHubDevice.CreateDevice(deviceId,isIotEdge);
        }
        public async Task<DeviceInfoModel> GetDeviceInfo(AccessPolicyModel accessPolicyModel,string deviceId,string access_token)
        {
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel
            {
                apiVersion = "2018-08-30-preview",
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                hostName = accessPolicyModel.HostName,
                requestPath = string.Format("/devices/{0}", deviceId)
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/get";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                TModel<DeviceInfoModel> job = JsonConvert.DeserializeObject<TModel<DeviceInfoModel>>(result);
                return job.body;
            }
            else
            {
                return null; 
            }
        }
        public async Task<object> SendMessageToDevice(AccessPolicyModel accessPolicyModel, SendMessageModel sendMessageModel,string access_token)
        {
            Newtonsoft.Json.Linq.JObject requestBody = new Newtonsoft.Json.Linq.JObject
            {
                { "hostName",accessPolicyModel.HostName},
                {"owner",accessPolicyModel.SharedAccessKeyName },
                {"key",accessPolicyModel.SharedAccessKey },
                {"deviceID",sendMessageModel.deviceId },
                {"body",sendMessageModel.body },
                {"properties",JsonConvert.SerializeObject(sendMessageModel.properties) }
            };
            string url = "https://main.iothub.ext.azure.cn/api/Service/SendMessage/";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestBody);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return response.StatusCode;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
        public async Task<object> InvokeMethod(AccessPolicyModel accessPolicyModel,DirectMethodModel directMethodModel,string access_token)
        {
            Newtonsoft.Json.Linq.JObject requestBody = new Newtonsoft.Json.Linq.JObject
            {
                { "hostName",accessPolicyModel.HostName},
                {"owner",accessPolicyModel.SharedAccessKeyName },
                {"key",accessPolicyModel.SharedAccessKey },
                {"deviceID",directMethodModel.deviceId },
                {"moduleID",null },
                {"methodName",directMethodModel.methodName },
                {"connectionTimeout",directMethodModel.connectionTimeout },
                {"responseTimeout",directMethodModel.responseTimeout },
                {"payload",directMethodModel.payload }
            };
            string url = "https://main.iothub.ext.azure.cn/api/Service/InvokeMethod/";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestBody);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return response.StatusCode;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
        public async Task<DeviceInfoModel> UpdateDeviceInfo(UpdateDeviceViewModel updateDeviceViewModel,AccessPolicyModel accessPolicyModel,string deviceId,string access_token)
        {
            DeviceInfoModel deviceInfoModel = await GetDeviceInfo(accessPolicyModel, deviceId, access_token);
            deviceInfoModel.status = updateDeviceViewModel.status;
            deviceInfoModel.authentication.symmetricKey.primaryKey = updateDeviceViewModel.primaryKey;
            //deviceInfoModel.authentication.symmetricKey.secondaryKey = updateDeviceViewModel.secondaryKey;
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel
            {
                apiVersion = "2018-08-30-preview",
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                etag = deviceInfoModel.etag,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                hostName = accessPolicyModel.HostName,
                requestBody = JsonConvert.SerializeObject(deviceInfoModel),
                requestPath = string.Format("/devices/{0}", deviceId)
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/put";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                TModel<DeviceInfoModel> job = JsonConvert.DeserializeObject<TModel<DeviceInfoModel>>(result);
                return job.body;
            }
            else
            {
                return null;
            }
        }
        public IQuery ListDevices(int maxCount,AccessPolicyModel accessPolicyModel)
        {
            IoTHubDevice ioTHubDevice = new IoTHubDevice(accessPolicyModel);
            return ioTHubDevice.ListDevices(maxCount);
        }
        public async Task<object> GetIotEdgeDevices(AccessPolicyModel accessPolicyModel,string access_token)
        {
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel()
            {
                hostName = accessPolicyModel.HostName,
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                apiVersion = "2018-08-30-preview",
                requestPath="/devices/query",
                requestBody= "{\"query\":\"SELECT deviceId as deviceId, status as status, " +
                "lastActivityTime as lastActivityTime, statusUpdatedTime as statusUpdateTime, " +
                "authenticationType as authenticationType, cloudToDeviceMessageCount as cloudToDeviceMessageCount, " +
                "deviceScope as deviceScope FROM devices WHERE (NOT IS_DEFINED(capabilities.iotEdge) " +
                "OR capabilities.iotEdge=true)\"}",
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/post";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result =await  response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return result;
            }
            else
            {
                return response.ReasonPhrase;
            }

        }
        public async Task<object> GetIotDevices(AccessPolicyModel accessPolicyModel, string access_token)
        {
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel()
            {
                hostName = accessPolicyModel.HostName,
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                apiVersion = "2018-08-30-preview",
                requestPath = "/devices/query",
                requestBody = "{\"query\":\"SELECT deviceId as deviceId, status as status, " +
                "lastActivityTime as lastActivityTime, statusUpdatedTime as statusUpdateTime, " +
                "authenticationType as authenticationType, cloudToDeviceMessageCount as cloudToDeviceMessageCount, " +
                "deviceScope as deviceScope FROM devices WHERE (NOT IS_DEFINED(capabilities.iotEdge) " +
                "OR capabilities.iotEdge=false)\"}",
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/post";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return result;
            }
            else
            {
                return response.ReasonPhrase;
            }

        }
        public async Task<ConcurrentBag<Device>> GetDevicesAsync( AccessPolicyModel accessPolicyModel)
        {
            IoTHubDevice ioTHubDevice = new IoTHubDevice(accessPolicyModel);
            return await ioTHubDevice.GetDevices();
        }
        public async Task<string> DeleteDevice(string deviceId,AccessPolicyModel accessPolicyModel)
        {
            IoTHubDevice iothubdevice = new IoTHubDevice(accessPolicyModel);
            return await iothubdevice.DeleteDevice(deviceId);

        }
        public string GetDeviceKey(string deviceId,AccessPolicyModel accessPolicyModel)
        {
            IoTHubDevice ioTHubDevice = new IoTHubDevice(accessPolicyModel);
            return ioTHubDevice.GetDeviceKey(deviceId);
        }
        public async Task<Twin> GetDeviceTwin(string deviceId, AccessPolicyModel accessPolicyModel,string access_token)
        {
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel()
            {
                hostName = accessPolicyModel.HostName,
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                apiVersion = "2018-08-30-preview",
                requestPath = string.Format("/twins/{0}",deviceId),
                requestBody = null,
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/get";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<DeviceTwinResponseModel>(result).body;
            }
            else
            {
                return null;
            }
            //IoTHubDevice ioTHubDevice = new IoTHubDevice(accessPolicyModel);
            //return ioTHubDevice.GetDeviceTwin(deviceId);
        }
        public async Task<string> UpdateDeviceTwin(string deviceId, Twin twin,AccessPolicyModel accessPolicyModel,string access_token)
        {
            //IoTHubDevice ioTHubDevice = new IoTHubDevice(accessPolicyModel);
            //return ioTHubDevice.UpdateDeviceTwin(deviceId, twin);
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel()
            {
                hostName = accessPolicyModel.HostName,
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                apiVersion = "2018-08-30-preview",
                requestPath = string.Format("/twins/{0}",deviceId),
                requestBody = JsonConvert.SerializeObject(twin),
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/patch";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return result;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
        public async Task<object> GetIoTEdgeDeviceDeployment(AccessPolicyModel accessPolicyModel, string access_token)
        {
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel()
            {
                hostName = accessPolicyModel.HostName,
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                apiVersion = "2018-06-30",
                requestPath = "/configurations/"
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/get";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                Newtonsoft.Json.Linq.JObject job = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(result);
                return job;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
        public async Task<object> GetDeviceModules(string deviceId, AccessPolicyModel accessPolicyModel, string access_token)
        {
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel
            {
                apiVersion = "2018-08-30-preview",
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                hostName = accessPolicyModel.HostName,
                requestPath = string.Format("/devices/{0}/modules", deviceId)
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/get";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                Newtonsoft.Json.Linq.JObject job = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(result);
                //TModel<DeviceModouleModel> job = JsonConvert.DeserializeObject<TModel<DeviceModouleModel>>(result);
                return job;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
        //根据ModuleID获取IoTEdge设备中的Module Twin Information , 采用 SasToken认证，返回的Module Twin Information使用T model接受
        public async Task<T> GetModuleTwinInfoById<T>(string hubName,string deviceId,string moduleId,string sasToken)
        {
            string url = string.Format("https://{0}.azure-devices.cn/twins/{1}/modules/{2}?api-version=2018-06-30",
                hubName,deviceId,moduleId);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", sasToken);
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            T moduleTwin ;
            if(response.IsSuccessStatusCode)
            {
                moduleTwin = JsonConvert.DeserializeObject<T>(result);
            }
            else
            {
                moduleTwin = default(T);
            }
            return moduleTwin;
        }

        //获取IoTEdge设备的默认ModuleContent
        public async Task<Content>GetDefaultDeviceModuleContent(string resourceName,string deviceId,string sasToken)
        {
            EdgeAgentModuleTwinModel edgeAgentModuleTwin = await GetModuleTwinInfoById<EdgeAgentModuleTwinModel>(resourceName, deviceId, "$edgeAgent", sasToken);
            EdgeHubModuleTwinModel edgeHubModuleTwin = await GetModuleTwinInfoById<EdgeHubModuleTwinModel>(resourceName, deviceId, "$edgeHub", sasToken);
            Content content = new Content()
            {
                modulesContent=
                {
                    _edgeAgent =
                    {
                        agentpropertiesdesired =
                        {
                            modules=edgeAgentModuleTwin.properties.desired.modules,
                            runtime=edgeAgentModuleTwin.properties.desired.runtime,
                            schemaVersion=edgeAgentModuleTwin.properties.desired.schemaVersion,
                            systemModules=edgeAgentModuleTwin.properties.desired.systemModules,
                        }
                    },
                    _edgeHub=
                    {
                        edgehubpropertiesdesired=
                        {
                            routes=edgeHubModuleTwin.properties.desired.routes
                        }
                    },
                }
            };
            return content;
            
        }

        public async Task<object> CreateDevcieDeployment(AccessPolicyModel accessPolicyModel,string DeploymentId, object DeploymentData,string access_token)
        {
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel()
            {
                hostName = accessPolicyModel.HostName,
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                apiVersion = "2018-08-30-preview",
                requestPath = string.Format("/configurations/{0}", DeploymentId),
                requestBody = JsonConvert.SerializeObject(DeploymentData)
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/put";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                Newtonsoft.Json.Linq.JObject job = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(result);
                return job;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
        public async Task<object> DeleteDeployment(AccessPolicyModel accessPolicyModel,string deploymentId,string access_token)
        {
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel()
            {
                hostName = accessPolicyModel.HostName,
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                apiVersion = "2018-06-30",
                requestPath = string.Format("/configurations/{0}", deploymentId)
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/delete";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JObject job = (JObject)JsonConvert.DeserializeObject(result);
                return job;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }
        
        public async Task<ResponseModel<DeviceModouleModel>> ListDeviceDeployment(string deviceId, AccessPolicyModel accessPolicyModel, string access_token)
        {
            RequestDeviceModel requestDeviceModel = new RequestDeviceModel
            {
                apiVersion = "2018-08-30-preview",
                authorizationPolicyKey = accessPolicyModel.SharedAccessKey,
                authorizationPolicyName = accessPolicyModel.SharedAccessKeyName,
                hostName = accessPolicyModel.HostName,
                requestPath = string.Format("/devices/{0}/modules", deviceId)
            };
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/get";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            string requestbody = JsonConvert.SerializeObject(requestDeviceModel);
            request.Content = new StringContent(requestbody, UnicodeEncoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                DeviceModouleModel job = JsonConvert.DeserializeObject<DeviceModouleModel>(result);
                //TModel<DeviceModouleModel> job = JsonConvert.DeserializeObject<TModel<DeviceModouleModel>>(result);
                return new ResponseModel<DeviceModouleModel>(response.ReasonPhrase,job);
            }
            else
            {
                return new ResponseModel<DeviceModouleModel>(response.ReasonPhrase, null) ;
            }
        }

        public async Task<List<IoTHubInsightMetricModel>> GetAllIoTHubInsightMetric(string access_token)
        {
            string url = "https://monitoring.hosting.azureportal.chinacloudapi.cn/monitoring/Content/iframe/metrics.app/def/DeviceIothubs.json";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();
            List<IoTHubInsightMetricModel> result = null;
            if (response.IsSuccessStatusCode)
            {
                result = JsonConvert.DeserializeObject<List<IoTHubInsightMetricModel>>(data);
            }
            else
            {
                Console.WriteLine(response.ReasonPhrase);
            }
            return result;
        }
        public async Task<InsightResponseModel> GetIoTHubInsight(string subid, string resourceGroupName, string resourceName, string access_token, InsightModel insightModel)
        { 
            string url = string.Format("{0}/resourceGroups/{1}/providers/Microsoft.Devices/IotHubs/" +
                "{2}/providers/microsoft.Insights/metrics?" +
                "timespan={3}&interval={4}&metricnames={5}&aggregation={6}&metricNamespace=microsoft.devices%2Fiothubs" +
                "&autoadjusttimegrain=true&validatedimensions=false&api-version=2019-07-01", 
                subid, resourceGroupName, resourceName,insightModel.timespan,insightModel.interval,insightModel.localizedValue,insightModel.aggregation);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var client = this._clientFactory.CreateClient("chinacloudapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();
            InsightResponseModel insight = null;
            if (response.IsSuccessStatusCode)
            {
                insight = JsonConvert.DeserializeObject<InsightResponseModel>(data);
            }
            else
            {
                Console.WriteLine("Error." + response.ReasonPhrase);
            }
            return insight;
        }

    }
    public class IoTHubDevice
    {
        readonly string connectionstring;
        RegistryManager registryManager;
        AccessPolicyModel _accessPolicyModel;
        public IoTHubDevice(AccessPolicyModel accessPolicyDeviceModel)
        {
            this.connectionstring = string.Format("HostName={0};SharedAccessKeyName={1};SharedAccessKey={2}",
                accessPolicyDeviceModel.HostName,
                accessPolicyDeviceModel.SharedAccessKeyName,
                accessPolicyDeviceModel.SharedAccessKey);
            this.registryManager= RegistryManager.CreateFromConnectionString(connectionstring);
            this._accessPolicyModel = accessPolicyDeviceModel;
        }
        public async Task<string> CreateDevice(string deviceId,bool isIotEdge)
        {
            string result = null; 
            try
            {
                result= await AddDeviceAsync(deviceId,isIotEdge);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            return result;
        }
        public async Task<string> AddDeviceAsync(string deviceId,bool isIotEdge)
        {
            Device device;
            string result = null ;
            //newdevice.Capabilities.IotEdge = isIotEdge;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
                device.Capabilities.IotEdge = isIotEdge;
                device = await registryManager.UpdateDeviceAsync(device);
                result = "success";
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
                result = "Failed";
            }
            return result;
        }
        /*public async Task<string> DeleteDeviceAsync()
        {
            Device device;
            string result;
            string deviceId = this._createDeviceModel.DeviceId;
            try
            {
                device = await registryManager.RemoveDeviceAsync();
            }
            catch(DeviceNotFoundException)
            { }

        }*/
        public IQuery ListDevices(int maxCount)
        {
            IQuery devices = registryManager.CreateQuery("select * from devices", maxCount);

            return devices;
        }
        public async Task<ConcurrentBag<Device>> GetDevices()
        {
            RegistryStatistics registryStatistics = await registryManager.GetRegistryStatisticsAsync();
            int maxCount = (int)registryStatistics.TotalDeviceCount;
            IEnumerable<Device> tempDevices = await registryManager.GetDevicesAsync(maxCount);
            List<string> IotEdgeDeviceIdList = null;
            List<string> IotDeviceIdList = null;
            foreach (var d in tempDevices)
            {
                //var device = await registryManager.GetDeviceAsync(d.Id);
                if (d.Capabilities.IotEdge == true)
                {
                    IotEdgeDeviceIdList.Add(d.Id);
                }
                else
                {
                    IotDeviceIdList.Add(d.Id);
                }
                
            }
            return null;
        }
        public async Task<string> DeleteDevice(string deviceId)
        {
            string result=null;
            RegistryStatistics registryStatistics = await registryManager.GetRegistryStatisticsAsync();
            IEnumerable<Device> devices = await registryManager.GetDevicesAsync((Int32)registryStatistics.TotalDeviceCount);
            bool flag = false;
            foreach (var obj in devices)
            {
                if (obj.Id == deviceId)
                {
                    await registryManager.RemoveDeviceAsync(obj);
                    flag = true;
                }
            }
            if (flag)
            {
                result = "success";
            }
            else
            {
                result = "error";
            }
            return result;
        }
        public string GetDeviceKey(string deviceId)
        {
            var device= registryManager.GetDeviceAsync(deviceId);
            string PrimaryKey= device.Result.Authentication.SymmetricKey.PrimaryKey;
            string SecondaryKey= device.Result.Authentication.SymmetricKey.SecondaryKey;
            string PrimaryConnectionString = string.Format("HostName:{0};DeviceId:{1};SharedAccessKey:{2}",
                this._accessPolicyModel.HostName,deviceId,PrimaryKey);
            string SecondaryConnectionString = string.Format("HostName:{0};DeviceId:{1};SharedAccessKey:{2}",
                this._accessPolicyModel.HostName, deviceId, SecondaryKey);
            return  string.Format("\"deviceId\":{0},\"PrimaryKey\":{1},\"SecondaryKey\":{2},\"PrimaryConnectionString\"" +
                ":{3},\"SecondaryConnectionString\":{4}",
                deviceId,PrimaryKey,SecondaryKey,PrimaryConnectionString,SecondaryConnectionString);
        }
        public Task<Twin> GetDeviceTwin(string deviceId)
        {
            Task<Twin> twin = registryManager.GetTwinAsync(deviceId);
            return twin;
        }
        
       /* public Task<Twin> UpdateDeviceTwin(string deviceId,Twin twin)
        {
            //TwinJsonConverter twinJsonConverter=new TwinJsonConverter();
            //JsonReader reader = null;
            //JsonSerializer jsonSerializer = null;
            //Twin twin = twinJsonConverter.ReadJson(reader, Twin ,jsonTwinPatch, jsonSerializer);
            //Task<Twin> updatetwin= registryManager.UpdateTwinAsync(deviceId, twin, etag);
            string url = "https://main.iothub.ext.azure.cn/api/dataPlane/patch";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            return null;
        }*/
    }

}
