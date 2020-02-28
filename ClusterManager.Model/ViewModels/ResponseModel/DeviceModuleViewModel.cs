using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.ViewModels.ResponseModel
{
    public class DeviceModuleViewModel
    {
        public string id { get; set; }
        public string createdTimeUtc { get; set; }
        public int priority { get; set; }
        public systemMetrics systemMetrics { get; set; }
    }
    public class systemMetrics
    {
        public results results { get; set; }
    }
    public class results
    {
        public int appliedCount { get; set; }
        public int reportedSuccessfulCount { get; set; }
        public int reportedFailedCount { get; set; }
        public int targetedCount { get; set; }
    }
}
