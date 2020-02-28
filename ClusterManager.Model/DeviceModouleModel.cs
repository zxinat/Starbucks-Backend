using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model
{
    public class DeviceModouleModel
    {
        public string moduleId { get; set; }
        public string managedBy { get; set; }
        public string deviceId { get; set; }
        public string generationId { get; set; }
        public string etag { get; set; }
        public string connectionState { get; set; }
        public string connectionStateUpdatedTime { get; set; }
        public string lastActivityTime { get; set; }
        public int cloudToDeviceMessageCount { get; set; }
        public authentication authentication { get; set; }
    }
}
