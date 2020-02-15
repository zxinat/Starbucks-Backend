using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Utility
{
    public class CreateQueryString
    {
        public CreateQueryString()
        {

        }
        public string AKSCPUMemoryInsightsQuery(AKSInsightsQueryParams queryParams)
        {
            string AKSInsightsquery = string.Format(
            "let endDateTime = datetime('{0}');" +
            "let startDateTime = datetime('{1}');" +
            "let trendBinSize = {2};" +
            "let MaxListSize = 1000;" +
            "let rawData = KubeNodeInventory " +
            "|where TimeGenerated<endDateTime " +
            "|where TimeGenerated >= startDateTime " +
            "|where ClusterId =~ '/subscriptions/{3}/resourceGroups/{4}/providers/Microsoft.ContainerService/managedClusters/{5}' " +
            "|extend ClusterName = iff(ClusterId contains '/Microsoft.ContainerService/' or ClusterId contains '/microsoft.kubernetes/', ClusterName, iff(ClusterId contains '/resourceGroups/', split(ClusterName, '/')[4], ClusterName)) " +
            "|distinct ClusterName, Computer    " +
            "|join hint.strategy=shuffle" +
            "(Perf" +
            "   | where TimeGenerated < endDateTime" +
            "   | where TimeGenerated >= startDateTime" +
            "   | where ObjectName == 'K8SNode' " +
            "   | where CounterName == 'cpuCapacityNanoCores' or CounterName == 'memoryCapacityBytes' " +
            "   | summarize LimitValue = max(CounterValue) by Computer, CounterName, bin(TimeGenerated, trendBinSize)" +
            "   | project Computer, CounterName = iif(CounterName == 'cpuCapacityNanoCores', 'cpu', 'memory'), CapacityStartTime = TimeGenerated, CapacityEndTime = TimeGenerated + trendBinSize, LimitValue" +
            ") on Computer    " +
            "| join kind = inner hint.strategy=shuffle" +
            "(Perf" +
            "   | where TimeGenerated < endDateTime + trendBinSize" +
            "   | where TimeGenerated >= startDateTime - trendBinSize" +
            "   | where ObjectName == 'K8SNode'        " +
            "   | where CounterName == 'cpuUsageNanoCores' or CounterName == 'memoryRssBytes'        " +
            "   | project Computer, CounterName = iif(CounterName == 'cpuUsageNanoCores', 'cpu', 'memory'), UsageValue = CounterValue, TimeGenerated" +
            ") on Computer, CounterName    " +
            "| where TimeGenerated >= CapacityStartTime and TimeGenerated<CapacityEndTime    " +
            "| project ClusterName, Computer, CounterName, TimeGenerated, UsagePercent = UsageValue* 100.0 / LimitValue;" +
            "let rawDataCached = materialize(rawData);" +
            "rawDataCached    " +
            "| summarize Min = min(UsagePercent), Avg = avg(UsagePercent), Max = max(UsagePercent), P50 = percentiles(UsagePercent, 50, 90, 95)             by bin(TimeGenerated, trendBinSize), ClusterName, CounterName    " +
            "| sort by TimeGenerated asc    " +
            "| project ClusterName, CounterName, TimeGenerated, Min, Avg, Max, P50, P90 = percentile_UsagePercent_90, P95 = percentile_UsagePercent_95" +
            "| summarize makelist(TimeGenerated, MaxListSize),                makelist(Min, MaxListSize),                makelist(Avg, MaxListSize),                makelist(Max, MaxListSize),                makelist(P50, MaxListSize),                makelist(P90, MaxListSize),                makelist(P95, MaxListSize) by ClusterName, CounterName    " +
            "| join" +
            "(rawDataCached" +
            "   | summarize Min = min(UsagePercent), Avg = avg(UsagePercent), Max = max(UsagePercent), P50 = percentiles(UsagePercent, 50, 90, 95) by ClusterName, CounterName" +
            ") on ClusterName, CounterName    " +
            "| project ClusterName, CounterName, Min, Avg, Max, P50, P90 = percentile_UsagePercent_90, P95 = percentile_UsagePercent_95, list_TimeGenerated, list_Min, list_Avg, list_Max, list_P50, list_P90, list_P95",
            queryParams.endDateTime, queryParams.startDateTime, queryParams.trendBinSize, queryParams.subid, queryParams.resourceGroupName, queryParams.resourceName);
            return AKSInsightsquery;
        }
        public string AKSNodeCountQuery(AKSInsightsQueryParams queryParams)
        {
            string query = string.Format(
                "let endDateTime = datetime('{0}');" +
                "let startDateTime = datetime('{1}');" +
                "let trendBinSize = {2};    " +
                "let maxListSize = 1000;" +
                "let rawData =    KubeNodeInventory  " +
                "| where TimeGenerated < endDateTime  " +
                "| where TimeGenerated >= startDateTime " +
                "| where ClusterId =~ '/subscriptions/{3}/resourceGroups/{4}/providers/Microsoft.ContainerService/managedClusters/{5}'        " +
                "| extend ClusterName = iff(ClusterId contains '/Microsoft.ContainerService/' or ClusterId contains '/microsoft.kubernetes/', ClusterName,\tiff(ClusterId contains '/resourceGroups/', split(ClusterName, '/')[4], ClusterName))    " +
                "| distinct ClusterName, TimeGenerated    " +
                "| summarize ClusterSnapshotCount = count() by Timestamp = bin(TimeGenerated, trendBinSize), ClusterName    " +
                "| join hint.strategy=broadcast " +
                "( KubeNodeInventory  " +
                "   | where TimeGenerated < endDateTime " +
                "   | where TimeGenerated >= startDateTime  " +
                "   | where ClusterId =~ '/subscriptions/{3}/resourceGroups/{4}/providers/Microsoft.ContainerService/managedClusters/{5}'                " +
                "   | extend ClusterName = iff(ClusterId contains '/Microsoft.ContainerService/' or ClusterId contains '/microsoft.kubernetes/', ClusterName,\t    iff(ClusterId contains '/resourceGroups/', split(ClusterName, '/')[4], ClusterName)) " +
                "   | summarize TotalCount = count(), ReadyCount = sumif(1, Status contains ('Ready'))   by ClusterName, Timestamp = bin(TimeGenerated, trendBinSize) " +
                "   | extend NotReadyCount = TotalCount - ReadyCount    " +
                ") on ClusterName, Timestamp    " +
                "| project ClusterName, Timestamp,  TotalCount = todouble(TotalCount) / ClusterSnapshotCount,   ReadyCount = todouble(ReadyCount) / ClusterSnapshotCount,   NotReadyCount = todouble(NotReadyCount) / ClusterSnapshotCount; " +
                "let rawDataCached = materialize(rawData);    " +
                "rawDataCached    " +
                "| order by Timestamp asc    " +
                "| summarize makelist(Timestamp, maxListSize),  makelist(TotalCount, maxListSize),   makelist(ReadyCount, maxListSize),   makelist(NotReadyCount, maxListSize)   by ClusterName " +
                "| join " +
                "( rawDataCached " +
                "   | summarize Avg_TotalCount = avg(TotalCount), Avg_ReadyCount = avg(ReadyCount), Avg_NotReadyCount = avg(NotReadyCount) by ClusterName    ) on ClusterName " +
                "   | project ClusterName, Avg_TotalCount, Avg_ReadyCount, Avg_NotReadyCount, list_Timestamp, list_TotalCount, list_ReadyCount, list_NotReadyCount  ",
                queryParams.endDateTime,queryParams.startDateTime, queryParams.trendBinSize, queryParams.subid, queryParams.resourceGroupName, queryParams.resourceName);

            return query;
        }
        public string AKSPodCountQuery(AKSInsightsQueryParams queryParams)
        {
            string query = string.Format(
                "let endDateTime = datetime('{0}');" +
                "let startDateTime = datetime('{1}');" +
                "let trendBinSize = {2};" +
                "let maxListSize = 1000;" +
                "let rawData =    KubePodInventory    " +
                "| where TimeGenerated < endDateTime    " +
                "| where TimeGenerated >= startDateTime    " +
                "| where ClusterId =~ '/subscriptions/{3}/resourceGroups/{4}/providers/Microsoft.ContainerService/managedClusters/{5}'    " +
                "| extend ClusterName = iff(ClusterId contains '/Microsoft.ContainerService/' or ClusterId contains '/microsoft.kubernetes/', ClusterName,\tiff(ClusterId contains '/resourceGroups/', split(ClusterName, '/')[4], ClusterName))    " +
                "| distinct ClusterName, TimeGenerated    " +
                "| summarize ClusterSnapshotCount = count() by bin(TimeGenerated, trendBinSize), ClusterName    " +
                "| join hint.strategy=broadcast " +
                "(        KubePodInventory        " +
                "   | where TimeGenerated < endDateTime        " +
                "   | where TimeGenerated >= startDateTime        " +
                "   | where ClusterId =~ '/subscriptions/{3}/resourceGroups/{4}/providers/Microsoft.ContainerService/managedClusters/{5}' " +
                "   | extend ClusterName = iff(ClusterId contains '/Microsoft.ContainerService/' or ClusterId contains '/microsoft.kubernetes/', ClusterName,\t      iff(ClusterId contains '/resourceGroups/', split(ClusterName, '/')[4], ClusterName))        " +
                "   | distinct ClusterName, Computer, PodUid, TimeGenerated, PodStatus        " +
                "   | summarize TotalCount = count(),  PendingCount = sumif(1, PodStatus =~ 'Pending'),  RunningCount = sumif(1, PodStatus =~ 'Running'),  SucceededCount = sumif(1, PodStatus =~ 'Succeeded'),  FailedCount = sumif(1, PodStatus =~ 'Failed')   by ClusterName, bin(TimeGenerated, trendBinSize)   " +
                ") on ClusterName, TimeGenerated    " +
                "| extend UnknownCount = TotalCount - PendingCount - RunningCount - SucceededCount - FailedCount    " +
                "| project ClusterName, Timestamp = TimeGenerated, TotalCount = todouble(TotalCount) / ClusterSnapshotCount, PendingCount = todouble(PendingCount) / ClusterSnapshotCount,  RunningCount = todouble(RunningCount) / ClusterSnapshotCount,  SucceededCount = todouble(SucceededCount) / ClusterSnapshotCount,  FailedCount = todouble(FailedCount) / ClusterSnapshotCount,  UnknownCount = todouble(UnknownCount) / ClusterSnapshotCount;   " +
                "let rawDataCached = materialize(rawData);  " +
                "rawDataCached    " +
                "| order by Timestamp asc    " +
                "| summarize makelist(Timestamp, maxListSize),  makelist(TotalCount, maxListSize),  makelist(PendingCount, maxListSize),   makelist(RunningCount, maxListSize),  makelist(SucceededCount, maxListSize),   makelist(FailedCount, maxListSize),   makelist(UnknownCount, maxListSize)  by ClusterName    " +
                "| join" +
                "(        rawDataCached        " +
                "   | summarize Avg_TotalCount = avg(TotalCount), Avg_PendingCount = avg(PendingCount), Avg_RunningCount = avg(RunningCount), Avg_SucceededCount = avg(SucceededCount), Avg_FailedCount = avg(FailedCount), Avg_UnknownCount = avg(UnknownCount) by ClusterName    " +
                ") on ClusterName    " +
                "| project ClusterName, Avg_TotalCount, Avg_PendingCount, Avg_RunningCount, Avg_SucceededCount, Avg_FailedCount, Avg_UnknownCount, list_Timestamp, list_TotalCount, list_PendingCount, list_RunningCount, list_SucceededCount, list_FailedCount, list_UnknownCount    ",
                queryParams.endDateTime, queryParams.startDateTime, queryParams.trendBinSize, queryParams.subid, queryParams.resourceGroupName, queryParams.resourceName);

            return query;
        }
        public string ExcuteAKSLogQuery(AKSExcuteLogQuery logquery)
        {
            string query = string.Format("set query_take_max_records = 10001; set truncationmaxsize = 67108864; {0}", logquery.queryCommand);
            return query;
        }
    }
    public class AKSInsightsQueryParams
    {
        // 当前时间  2020-02-14T06:27:06.981Z
        public string endDateTime { get; set; }
        // 开始时间  2020-02-14T00:25:00.000Z
        public string startDateTime { get; set; }
        // 时间粒度 3m
        public string trendBinSize { get; set; }
        // 订阅ID  6273fbea-8a11-498b-8218-02b6f4398e12
        public string subid { get; set; }
        // 资源组 Starbucks
        public string resourceGroupName { get; set; }
        // 资源 demos
        public string resourceName { get; set; }
    }
    public class AKSExcuteLogQuery
    {
        public string subid { get; set; }
        public string resourceGroupName { get; set; }
        public string resourceName { get; set; }
        public string queryCommand { get; set; }
        public string timespan { get; set; }
    }
}
