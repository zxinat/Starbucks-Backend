using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.APIModels.ResponseModel
{
    public class AKSInsightDataModel
    {
        public List<table> tables { get; set; }
    }
    public class table
    {
        public string name { get; set; }
        public List<column> columns { get; set; }
        public List<List<object>> rows { get; set; }
        
    }
    public class column
    {
        public string name { get; set; }
        public string type { get; set; }
    }
}
