using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Utility
{
    public class DateTransform
    {
        public string UTC2CCT (string utcTime)
        {
            //2020-02-24T09:55:00Z => 2020-02-24 17:55:00Z
            DateTime date = Convert.ToDateTime(utcTime);
            return date.ToLocalTime().ToString("u");

        }
    }
}
