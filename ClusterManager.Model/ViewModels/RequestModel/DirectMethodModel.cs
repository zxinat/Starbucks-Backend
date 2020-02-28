using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.ViewModels.RequestModel
{
    public class DirectMethodModel
    {
        public string deviceId { get; set; }
        public string methodName { get; set; }
        public string payload { get; set; }
        public int connectionTimeout { get; set; }
        public int responseTimeout { get; set; }
        
    }
}
