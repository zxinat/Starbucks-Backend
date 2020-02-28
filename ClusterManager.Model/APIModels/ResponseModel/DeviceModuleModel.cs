using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.APIModels.ResponseModel
{
    public class DeviceModuleModel
    {
        public string id { get; set; }
        public string targetCondition { get; set; }
        public string createdTimeUtc { get; set; }
        public string lastUpdatedTimeUtc { get; set; }
        public int priority { get; set; }
    }

}
