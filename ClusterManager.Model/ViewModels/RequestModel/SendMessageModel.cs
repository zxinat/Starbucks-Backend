using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.ViewModels.RequestModel
{
    public class SendMessageModel
    {
        public string deviceId { get; set; }
        public string body { get; set; }
        public List<object> properties { get; set; }
    }
}
