using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.ViewModels.ResponseModel
{
    public class AKSInsightDataViewModel
    {
        public string ClusterName { get; set; }
        public string CounterName { get; set; }
        public double Min { get; set; }
        public double Avg { get; set; }
        public double Max { get; set; }
        public double P50 { get; set; }
        public double P90 { get; set; }
        public double P95 { get; set; }
        public List<string> list_TimeGenerated { get; set; }
        public List<double> list_Min { get; set; }
        public List<double> list_Avg { get; set; }
        public List<double> list_Max { get; set; }
        public List<double> list_P50 { get; set; }
        public List<double> list_P90 { get; set; }
        public List<double> list_P95 { get; set; }
    }
}
