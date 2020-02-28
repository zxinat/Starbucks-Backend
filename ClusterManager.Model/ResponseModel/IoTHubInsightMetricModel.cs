using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.ResponseModel
{
    public class IoTHubInsightMetricModel
    {
        public Metricname name{ get; set; }
        public string unit { get; set; }
        public string primaryAggregationType { get; set; }
        public List<string> supportedAggregationTypes { get; set; }
        public List<MetricAvailability> metricAvailabilities { get; set; }

    }
    public class MetricAvailability
    {
        public string timeGrain { get; set; }
        public string retention { get; set; }
    }
}
