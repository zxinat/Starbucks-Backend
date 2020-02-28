﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ClusterManager.Model;
using ClusterManager.Model.ResponseModel;
using ClusterManager.Model.ViewModels;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using ClusterManager.Model.ViewModels.RequestModel;
using ClusterManager.Model.APIModels.ResponseModel;

namespace ClusterManager.Core.Infrastructures
{
     public interface IIoTHubResourceBus
    {
        Task<string> GetBySubId(string email,string subid);
        Task<string> CreateOrUpdate(IoTHubModel ioTHubModel, string email,string subid,string resourceGroupName);
        Task<string> DeleteIoTHub(string email,string subid,string resourceGroupName, string resourceName);
        Task<IoTHubInfoModel> GetIoTHubInfo(string email,string subid,string resourceGroupName, string resourceName);
        Task<IoTHubKeys> GetIoTHubKeys(string email,string subid,string resourceGroupName, string resourceName);
        Task<IoTHubKey> GetIoTHubKeyForKeyName(string email, string subid, string resourceGroupName, string resourceName,string keyName);
        Task<string> CreateDevice(AccessPolicyModel createDeviceModel,string deviceId,bool isIotEdge);
        Task<DeviceInfoModel> GetDeviceInfo(string email,string subid,string resourceGroupName, string resourceName, string deviceId);
        Task<DeviceInfoModel> UpdateDeviceInfo(string email, string subid,string resourceGroupName, string resourceName, string deviceId, UpdateDeviceViewModel updateDeviceViewModel);
        Task<object> GetIotEdgeDevices(AccessPolicyModel accessPolicyModel, string email);
        Task<object> GetIotDevices(AccessPolicyModel accessPolicyModel, string email);
        Task<object> SendMessageToDevice(string email, string subid, string resourceGroupName, string resourceName, SendMessageModel sendMessageModel);
        Task<object> InvokeMethod(string email, string subid, string resourceGroupName, string resourceName, DirectMethodModel directMethodModel);
        IQuery ListDevices(int maxCount, AccessPolicyModel createDeviceModel);
        Task<ConcurrentBag<Device>> GetDevicesAsync( AccessPolicyModel accessPolicyModel);
        Task<string> DeleteDevice(string deviceId, AccessPolicyModel accessPolicyModel);
        string GetDeviceKey(string deviceId, AccessPolicyModel accessPolicyModel);
        Task<Twin> GetDeivceTwin(string email, string deviceId, AccessPolicyModel accessPolicyModel);
        //Task<Twin> UpdateDeviceTwin(string deviceId, string jsonTwinPatch, string etag, AccessPolicyModel accessPolicyModel);
        Task<string> UpdateDeviceTwin(string email,string subid,string resourceGroupName, string resourceName, string deviceId, Twin twin);
        Task<object> GetIoTEdgeDeviceDeployment(string email,string subid,string resourceGroupName, string resourceName);
        Task<object> CreateDeviceDeployment(string email, string subid, string resourceGroupName, string resourceName, string DeploymentId, object DeploymentData);
        Task<object> DeleteDeviceDeployment(string email, string subid, string resourceGroupName, string resourceName, string deploymentId);
        Task<object> GetDeviceModules(string email,string subid,string resourceGroupName, string resourceName, string deviceId);
        Task<EdgeAgentModuleTwinModel> GetEdgeAgentModuleTwin(string email, string subid, string resourceGroupName, string resourceName, string deviceId);
        Task<Content> GetDefaultDeviceModuleContent(string email, string subid, string resourceGroupName, string resourceName, string deviceId);


        Task<InsightResponseModel> GetIoTHubInsight(string email,string subid,string resourceGroupName, string resourceName, InsightModel insightModel);
        Task<List<string>> ListInsightAggregationByLocalizeValue(string email, string localizeValue);
        Task<List<Metricname>> ListIoTHubInsightlocalizedValue(string email);
    }
}
