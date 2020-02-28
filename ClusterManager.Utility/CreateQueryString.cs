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
        public string AKSRunStatusQuery(AKSRunningStatusQuery aKSRunningStatusQuery)
        {
            string query =string.Format(
                "let startDateTime = datetime('{0}');        " +
                "KubeHealth         " +
                "| extend ClusterName = ClusterId        " +
                "| where ClusterId =~ '/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.ContainerService/managedClusters/{3}'        " +
                "| where TimeGenerated > startDateTime         " +
                "| summarize arg_max(TimeGenerated, *) by MonitorInstanceId        " +
                "| project TimeGenerated, MonitorTypeId, MonitorInstanceId, TimeFirstObserved, OldState, NewState,        MonitorLabels, MonitorConfig, Details ",
                aKSRunningStatusQuery.startDateTime,aKSRunningStatusQuery.subid,aKSRunningStatusQuery.resourceGroupName,aKSRunningStatusQuery.resourceName);
            return query;
        }
        public string ListAllNodeQuery(ListAllNodeQueryParams queryParams)
        {
            string limitMetricName;
            if(queryParams.usedMetricName== "memoryWorkingSetBytes" || queryParams.usedMetricName== "memoryRssBytes")
            {
                limitMetricName = "memoryCapacityBytes";
            }
            else
            {
                limitMetricName = "cpuUsageNanoCores";
            }
            string query = string.Format(
                "let endDateTime = datetime('{0}'); " +
                "let startDateTime = datetime('{1}'); " +
                "let binSize = {2};" +
                "let limitMetricName = '{3}';" +
                "let usedMetricName = '{4}';" +
                "let materializedNodeInventory = KubeNodeInventory        " +
                "| where TimeGenerated < endDateTime        " +
                "| where TimeGenerated >= startDateTime        " +
                "| project ClusterName, ClusterId, Node = Computer, TimeGenerated, Status,        NodeName = Computer, NodeId = strcat(ClusterId, '/', Computer), Labels                " +
                "| where ClusterId =~ '/subscriptions/{5}/resourceGroups/{6}/providers/Microsoft.ContainerService/managedClusters/{7}'        ;        " +
                "let materializedPerf = Perf        " +
                "| where TimeGenerated < endDateTime        " +
                "| where TimeGenerated >= startDateTime        " +
                "| where ObjectName == 'K8SNode'        " +
                "| extend NodeId = InstanceName;        " +
                "let materializedPodInventory = KubePodInventory        " +
                "| where TimeGenerated < endDateTime        " +
                "| where TimeGenerated >= startDateTime        " +
                "| where isnotempty(ClusterName)        " +
                "| where isnotempty(Namespace)        " +
                "| where ClusterId =~ '/subscriptions/{5}/resourceGroups/{6}/providers/Microsoft.ContainerService/managedClusters/{7}'                ;        " +
                "let inventoryOfCluster = materializedNodeInventory        " +
                "| summarize arg_max(TimeGenerated, Status) by ClusterName, ClusterId, NodeName, NodeId;        " +
                "let labelsByNode = materializedNodeInventory        " +
                "| summarize arg_max(TimeGenerated, Labels) by ClusterName, ClusterId, NodeName, NodeId;        " +
                "let countainerCountByNode = materializedPodInventory        " +
                "| project ContainerName, NodeId = strcat(ClusterId, '/', Computer)        " +
                "| distinct NodeId, ContainerName        " +
                "| summarize ContainerCount = count() by NodeId;        " +
                "let latestUptime = materializedPerf        " +
                "| where CounterName == 'restartTimeEpoch'        " +
                "| summarize arg_max(TimeGenerated, CounterValue) by NodeId        " +
                "| extend UpTimeMs = datetime_diff('Millisecond', endDateTime,        datetime_add('second', toint(CounterValue), make_datetime(1970,1,1)))        " +
                "| project NodeId, UpTimeMs;        " +
                "let latestLimitOfNodes = materializedPerf        " +
                "| where CounterName == limitMetricName        " +
                "| summarize CounterValue = avg(CounterValue) by NodeId        " +
                "| project NodeId, LimitValue = CounterValue;        " +
                "let actualUsageAggregated = materializedPerf        " +
                "| where CounterName == usedMetricName        " +
                "| summarize Aggregation = min(CounterValue) by NodeId        " +
                "| project NodeId, Aggregation;        " +
                "let aggregateTrendsOverTime = materializedPerf        " +
                "| where CounterName == usedMetricName        " +
                "| summarize TrendAggregation = min(CounterValue) by NodeId, bin(TimeGenerated, binSize)        " +
                "| project NodeId, TrendAggregation, TrendDateTime = TimeGenerated;        " +
                "let unscheduledPods = materializedPodInventory        " +
                "| where isempty(Computer)        " +
                "| extend Node = Computer                " +
                "| where isempty(ContainerStatus)        " +
                "| where PodStatus == 'Pending'        " +
                "| order by TimeGenerated desc        " +
                "| take 1        " +
                "| project ClusterName, NodeName = 'unscheduled', LastReceivedDateTime = TimeGenerated, Status = 'unscheduled', ContainerCount = 0, UpTimeMs = '0', Aggregation = '0',LimitValue = '0', ClusterId;" +
                "let scheduledPods = inventoryOfCluster        " +
                "| join kind=leftouter (aggregateTrendsOverTime) on NodeId        " +
                "| extend TrendPoint = pack(\"TrendTime\", TrendDateTime, \"TrendAggregation\", TrendAggregation)        " +
                "| summarize make_list(TrendPoint) by NodeId, NodeName, Status        " +
                "| join kind=leftouter (labelsByNode) on NodeId        " +
                "| join kind=leftouter (countainerCountByNode) on NodeId        " +
                "| join kind=leftouter (latestUptime) on NodeId        " +
                "| join kind=leftouter (latestLimitOfNodes) on NodeId        " +
                "| join kind=leftouter (actualUsageAggregated) on NodeId        " +
                "| project ClusterName, NodeName, ClusterId, list_TrendPoint,LastReceivedDateTime = TimeGenerated,Status,ContainerCount,UpTimeMs, Aggregation, LimitValue,  Labels " +
                "| limit 250;" +
                "union (scheduledPods), (unscheduledPods)" +
                "| project ClusterName, NodeName,        LastReceivedDateTime,Status,ContainerCount,UpTimeMs = UpTimeMs_long,Aggregation = Aggregation_real, LimitValue = LimitValue_real,list_TrendPoint,Labels,ClusterId",
                queryParams.endDateTime,queryParams.startDateTime,queryParams.binSize,limitMetricName,queryParams.usedMetricName,queryParams.subid,queryParams.resourceGroupName,queryParams.resourceName);
            return query;
        }
        public string GetNodeInfo(NodeInfoQueryParams queryParams)
        {
            string query =string.Format(
                "let startDateTime = datetime('{0}');        " +
                "let timeGenerated = datetime('{1}');        " +
                "let EmptyContainerNodeInventory_CLTable = datatable(TimeGenerated: datetime, Computer: string,  DockerVersion_s: string, OperatingSystem_s: string)[];        " +
                "let ContainerNodeInventory_CLTable = union isfuzzy = true EmptyContainerNodeInventory_CLTable, ContainerNodeInventory_CL        " +
                "| where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime        " +
                "| project Computer = tolower(Computer), DockerVersion = DockerVersion_s, OperatingSystem = OperatingSystem_s, TimeGenerated;        " +
                "let EmptyContainerNodeInventoryTable = datatable(TimeGenerated: datetime, Computer: string,  DockerVersion_s: string, OperatingSystem_s: string)[];        " +
                "let KubeNodeInventoryTable = KubeNodeInventory         " +
                "| where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime        " +
                "| where DockerVersion != \"\" and OperatingSystem != \"\"        " +
                "| where Computer =~ '{2}'        " +
                "| summarize arg_max(TimeGenerated, *) by Computer;        " +
                "let ContainerNodeInventoryTable = union isfuzzy = true ContainerNodeInventory_CLTable, EmptyContainerNodeInventoryTable, KubeNodeInventoryTable, ContainerNodeInventory        " +
                "| where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime        " +
                "| where Computer =~ '{2}'        " +
                "| summarize arg_max(TimeGenerated, *) by Computer;        " +
                "KubeNodeInventory        " +
                "| where TimeGenerated <= timeGenerated        " +
                "| project  Computer = tolower(Computer), Status, ClusterName = iff(ClusterId contains '/Microsoft.ContainerService/' or ClusterId contains '/microsoft.kubernetes/', ClusterName,iff(ClusterId contains '/resourceGroups/', split(ClusterName, '/')[4], ClusterName)),Labels, KubeletVersion, KubeProxyVersion,KubernetesEnvironment = iff(isempty(KubernetesProviderID), KubernetesProviderID, split(KubernetesProviderID, ':')[0]), TimeGenerated        " +
                "| where Computer =~ '{2}'        " +
                "| summarize arg_max(TimeGenerated, *) by Computer        " +
                "| join kind = leftouter ContainerNodeInventoryTable on Computer        " +
                "| join kind = leftouter " +
                "   (            Heartbeat            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | project Computer = tolower(Computer), NodeIP = ComputerIP, ComputerEnvironment, TimeGenerated            " +
                "   | where Computer =~ '{2}'            " +
                "   | summarize arg_max(TimeGenerated, *) by Computer        " +
                "   ) on Computer        " +
                "| join kind = leftouter " +
                "   (            ContainerInventory            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | project Computer = tolower(Computer), Image, ImageTag, Repository, TimeGenerated            " +
                "   | where Computer =~ '{2}'            " +
                "   | where Image == 'microsoft/oms' or (Repository == 'microsoft' and Image == 'oms') or (Repository == 'mcr.microsoft.com' and Image contains 'azuremonitor/containerinsights/')            " +
                "   | summarize arg_max(TimeGenerated, *) by Computer        " +
                "   ) on Computer        " +
                "| join kind = leftouter " +
                "   (            InsightsMetrics            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | where Origin =~ 'container.azm.ms/telegraf'            " +
                "   | where Namespace =~ 'disk' or Namespace =~ 'container.azm.ms/disk'            " +
                "   | extend Tags = todynamic(Tags)            " +
                "   | project TimeGenerated, Computer = tolower(tostring(Tags.hostName)), Device = tostring(Tags.device), Path = tostring(Tags.path), DiskMetricName = Name, DiskMetricValue = Val            " +
                "   | where Computer =~ '{2}'            " +
                "   | where (DiskMetricName =~ 'used') or (DiskMetricName =~ 'free') or (DiskMetricName =~ 'used_percent')            " +
                "   | summarize arg_max(TimeGenerated, *) by Computer, Device, Path, DiskMetricName        " +
                "   ) on Computer        " +
                "| project Computer, Status, ClusterName, KubeletVersion, KubeProxyVersion, DockerVersion, OperatingSystem,NodeIP, Labels, ComputerEnvironment = iff(isempty(KubernetesEnvironment), ComputerEnvironment, KubernetesEnvironment), Image, ImageTag, Device, Path, DiskMetricName, DiskMetricValue",
                queryParams.startDateTime, queryParams.endDateTime, queryParams.nodeName);
            return query;
        }
        public string AKSNodeContainersQuery(AKSNodeContainersQueryParams queryParams)
        {
            string metricLimitCounterName;
            if (queryParams.metricUsageCounterName == "memoryWorkingSetBytes" || queryParams.metricUsageCounterName == "memoryRssBytes")
            {
                metricLimitCounterName = "memoryLimitBytes";
            }
            else
            {
                metricLimitCounterName = "cpuLimitNanoCores";
            }
            string query = string.Format(
            "let startDateTime = datetime('{0}');        " +
            "let endDateTime = datetime('{1}');        " +
            "let trendBinSize = {2};        " +
            "let maxResultCount = 10000;        " +
            "let metricUsageCounterName = '{3}';        " +
            "let metricLimitCounterName = '{4}';        " +
            "let KubePodInventoryTable = KubePodInventory        " +
            "| where TimeGenerated >= startDateTime        " +
            "| where TimeGenerated < endDateTime        " +
            "| where isnotempty(ClusterName)        " +
            "| where isnotempty(Namespace)        " +
            "| where isnotempty(Computer)        " +
            "| project TimeGenerated, ClusterId, ClusterName, Namespace, ServiceName,ControllerName, Node = Computer, Pod = Name,ContainerInstance = ContainerName,ContainerID,ReadySinceNow = format_timespan(endDateTime - ContainerCreationTimeStamp , 'ddd.hh:mm:ss.fff'),Restarts = ContainerRestartCount, Status = ContainerStatus,ContainerStatusReason = columnifexists('ContainerStatusReason', ''),ControllerKind = ControllerKind, PodStatus;            " +
            "let startRestart = KubePodInventoryTable           " +
            "| summarize arg_min(TimeGenerated, *) by Node, ContainerInstance            " +
            "| where ClusterId =~ '/subscriptions/{5}/resourceGroups/{6}/providers/Microsoft.ContainerService/managedClusters/{7}'                                    " +
            "| where Node == '{8}'            " +
            "| project Node, ContainerInstance, InstanceName = strcat(ClusterId, '/', ContainerInstance), StartRestart = Restarts;            " +
            "let IdentityTable =  KubePodInventoryTable            " +
            "| summarize arg_max(TimeGenerated, *) by Node, ContainerInstance            " +
            "| where ClusterId =~ '/subscriptions/{5}/resourceGroups/{6}/providers/Microsoft.ContainerService/managedClusters/{7}'                                    " +
            "| where Node == '{8}'            " +
            "| project ClusterName, Namespace, ServiceName, ControllerName, Node, Pod, ContainerInstance,  InstanceName = strcat(ClusterId, '/', ContainerInstance), ContainerID, ReadySinceNow, Restarts,                    Status = iff(Status =~ 'running', 0, iff(Status=~'waiting', 1, iff(Status =~'terminated', 2, 3))),ContainerStatusReason,ControllerKind, Containers = 1, ContainerName = tostring(split(ContainerInstance, '/')[1]), PodStatus,                    LastPodInventoryTimeGenerated = TimeGenerated, ClusterId;        " +
            "let CachedIdentityTable = materialize(IdentityTable);        " +
            "let FilteredPerfTable = Perf        " +
            "| where TimeGenerated >= startDateTime        " +
            "| where TimeGenerated < endDateTime        " +
            "| where ObjectName == 'K8SContainer'        " +
            "| where InstanceName startswith '/subscriptions/{5}/resourceGroups/{6}/providers/Microsoft.ContainerService/managedClusters/{7}'        " +
            "| project Node = Computer, TimeGenerated, CounterName, CounterValue, InstanceName        " +
            "| where Node == '{8}';        " +
            "let CachedFilteredPerfTable = materialize(FilteredPerfTable);        " +
            "let LimitsTable = CachedFilteredPerfTable        " +
            "| where CounterName =~ metricLimitCounterName        " +
            "| summarize arg_max(TimeGenerated, *) by Node, InstanceName        " +
            "| project Node, InstanceName,            LimitsValue = iff(CounterName =~ 'cpuLimitNanoCores', CounterValue/1000000, CounterValue),            TimeGenerated;        " +
            "let MetaDataTable = CachedIdentityTable        " +
            "| join kind=leftouter (                LimitsTable            ) on Node, InstanceName             " +
            "| join kind= leftouter ( startRestart ) on Node, InstanceName            " +
            "| project ClusterName, Namespace, ServiceName, ControllerName, Node, Pod, InstanceName,                        ContainerID, ReadySinceNow, Restarts, LimitsValue, Status,                        ContainerStatusReason = columnifexists('ContainerStatusReason', ''), ControllerKind, Containers,                        ContainerName, ContainerInstance, StartRestart, PodStatus, LastPodInventoryTimeGenerated, ClusterId;            " +
            "let UsagePerfTable = CachedFilteredPerfTable            " +
            "| where CounterName =~ metricUsageCounterName            " +
            "| project TimeGenerated, Node, InstanceName,            CounterValue = iff(CounterName =~ 'cpuUsageNanoCores', CounterValue/1000000, CounterValue);                " +
            "let LastRestartPerfTable = CachedFilteredPerfTable                " +
            "| where CounterName =~ 'restartTimeEpoch'                " +
            "| summarize arg_max(TimeGenerated, *) by Node, InstanceName                " +
            "| project Node, InstanceName, UpTime = CounterValue,                LastReported = TimeGenerated;            " +
            "let AggregationTable = UsagePerfTable            " +
            "| summarize  Aggregation = min(CounterValue) by Node, InstanceName            " +
            "| project Node, InstanceName, Aggregation;            " +
            "let TrendTable = UsagePerfTable            " +
            "| summarize TrendAggregation = min(CounterValue)                by bin(TimeGenerated, trendBinSize), Node, InstanceName            " +
            "| project TrendTimeGenerated = TimeGenerated, Node, InstanceName , TrendAggregation            " +
            "| summarize TrendList = makelist(pack(\"timestamp\", TrendTimeGenerated, \"value\", TrendAggregation)) by Node, InstanceName;            " +
            "let containerFinalTable = MetaDataTable            " +
            "| join kind= leftouter( AggregationTable ) on Node, InstanceName            " +
            "| join kind = leftouter (LastRestartPerfTable) on Node, InstanceName            " +
            "| order by Aggregation desc, ContainerName            " +
            "| join kind =  leftouter ( TrendTable) on Node, InstanceName                        " +
            "| project ContainerIdentity = strcat(ContainerName, '|', Pod),  Status,            ContainerStatusReason = columnifexists('ContainerStatusReason', ''),            Aggregation, Node, Restarts, ReadySinceNow,  TrendList = iif(isempty(TrendList), parse_json('[]'), TrendList),            LimitsValue, ControllerName, ControllerKind, ContainerID, Containers,            UpTimeNow = datetime_diff('Millisecond', endDateTime, datetime_add('second', toint(UpTime), make_datetime(1970,1,1))),            ContainerInstance, StartRestart, LastReportedDelta = datetime_diff('Millisecond', endDateTime, LastReported),            PodStatus, InstanceName, Namespace, LastPodInventoryTimeGenerated, ClusterId;" +
            "let unscheduledPods = KubePodInventory        " +
            "| where TimeGenerated < endDateTime        " +
            "| where TimeGenerated >= startDateTime        " +
            "| where isnotempty(ClusterName)        " +
            "| where isnotempty(Namespace)        " +
            "| where isempty(Computer)        " +
            "| extend Node = iff(isnotempty(Computer), Computer, 'unscheduled')        " +
            "| where ClusterId =~ '/subscriptions/{5}/resourceGroups/{6}/providers/Microsoft.ContainerService/managedClusters/{7}'                        " +
            "| where Node == '{8}'        " +
            "| where isempty(ContainerStatus)        " +
            "| where PodStatus == 'Pending'        " +
            "| summarize arg_max(TimeGenerated, *) by Name        " +
            "| project ContainerIdentity = strcat('|', Name ),        Status = iff(ContainerStatus =~ 'running', 0, iff(ContainerStatus=~'waiting', 1, iff(ContainerStatus =~'terminated', 2, 3))),        ContainerStatusReason = columnifexists('ContainerStatusReason', ''),        Aggregation = 0.0, Node, Restarts = toint(0), ReadySinceNow = '',        TrendList = parse_json('[]'),        LimitsValue = 0.0, ControllerName, ControllerKind, ContainerID, Containers = 0, UpTimeNow = tolong(0), ContainerInstance = Name,        StartRestart = toint(0), PodStatus, InstanceName = strcat(ClusterId, '/'),        Namespace, LastPodInventoryTimeGenerated = TimeGenerated;union containerFinalTable, unscheduledPods " +
            "| limit 200",
            queryParams.startDateTime, queryParams.endDateTime, queryParams.trendBinSize, queryParams.metricUsageCounterName,metricLimitCounterName, queryParams.subid, queryParams.resourceGroupName, queryParams.resourceName, queryParams.nodeName);
            return query;

        }
        /*public string AKSAllPodInfoQuery(AKSPodInfoInsightQueryParams queryParams)
        {
            string metricLimitCounterName;
            if(queryParams.metricUsageCounterName== "memoryWorkingSetBytes" || queryParams.metricUsageCounterName == "memoryRssBytes")
            {
                metricLimitCounterName = "memoryLimitBytes";
            }
            else
            {
                metricLimitCounterName = "cpuLimitNanoCores";
            }
            string query = string.Format(
                "let startDateTime = datetime('{0}');" +
                "let endDateTime = datetime('{1}');" +
                "let trendBinSize = 15m;" +
                "let maxResultCount = 10000;" +
                "let metricUsageCounterName = '{2}';" +
                "let metricLimitCounterName = '{3}';" +
                "let KubePodInventoryTable = KubePodInventory  " +
                "| where TimeGenerated >= startDateTime   " +
                "| where TimeGenerated < endDateTime     " +
                "| where isnotempty(ClusterName)   " +
                "| where isnotempty(Namespace)  " +
                "| where isnotempty(Computer)  " +
                "| project TimeGenerated, ClusterId, ClusterName, Namespace, ServiceName,ControllerName, Node = Computer, Pod = Name,ContainerInstance = ContainerName,ContainerID,ReadySinceNow = format_timespan(endDateTime - ContainerCreationTimeStamp , 'ddd.hh:mm:ss.fff'),Restarts = ContainerRestartCount, Status = ContainerStatus,ContainerStatusReason = columnifexists('ContainerStatusReason', ''),ControllerKind = ControllerKind, PodStatus;" +
                "let startRestart = KubePodInventoryTable   " +
                "| summarize arg_min(TimeGenerated, *) by Node, ContainerInstance   " +
                "| where ClusterId =~ '/subscriptions/{4}/resourceGroups/{5}/providers/Microsoft.ContainerService/managedClusters/{6}'  " +
                "| where Node == '{7}'  " +          //  aks-nodepool1-13906345-vmss000000
                "| project Node, ContainerInstance, InstanceName = strcat(ClusterId, '/', ContainerInstance), StartRestart = Restarts;" +
                "let IdentityTable =  KubePodInventoryTable    " +
                "| summarize arg_max(TimeGenerated, *) by Node, ContainerInstance   " +
                "| where ClusterId =~ '/subscriptions/{4}/resourceGroups/{5}/providers/Microsoft.ContainerService/managedClusters/{6}'                                    " +
                "| where Node == '{7}'            " +
                "| project ClusterName, Namespace, ServiceName, ControllerName, Node, Pod, ContainerInstance,InstanceName = strcat(ClusterId, '/', ContainerInstance), ContainerID, ReadySinceNow, Restarts,Status = iff(Status =~ 'running', 0, iff(Status=~'waiting', 1, iff(Status =~'terminated', 2, 3))),ContainerStatusReason,ControllerKind, Containers = 1, ContainerName = tostring(split(ContainerInstance, '/')[1]), PodStatus,LastPodInventoryTimeGenerated = TimeGenerated, ClusterId;" +
                "let CachedIdentityTable = materialize(IdentityTable);        " +
                "let FilteredPerfTable = Perf       " +
                "| where TimeGenerated >= startDateTime        " +
                "| where TimeGenerated < endDateTime        " +
                "| where ObjectName == 'K8SContainer'        " +
                "| where InstanceName startswith '/subscriptions/{4}/resourceGroups/{5}/providers/Microsoft.ContainerService/managedClusters/{6}'        " +
                "| project Node = Computer, TimeGenerated, CounterName, CounterValue, InstanceName" +
                "| where Node == '{7}';" +
                "let CachedFilteredPerfTable = materialize(FilteredPerfTable);       " +
                "let LimitsTable = CachedFilteredPerfTable        " +
                "| where CounterName =~ metricLimitCounterName        " +
                "| summarize arg_max(TimeGenerated, *) by Node, InstanceName        " +
                "| project Node, InstanceName,LimitsValue = iff(CounterName =~ 'cpuLimitNanoCores', CounterValue/1000000, CounterValue),TimeGenerated;" +
                "let MetaDataTable = CachedIdentityTable        " +
                "| join kind=leftouter (                LimitsTable            ) on Node, InstanceName             " +
                "| join kind= leftouter ( startRestart ) on Node, InstanceName            " +
                "| project ClusterName, Namespace, ServiceName, ControllerName, Node, Pod, InstanceName,ContainerID, ReadySinceNow, Restarts, LimitsValue, Status,ContainerStatusReason = columnifexists('ContainerStatusReason', ''), ControllerKind, Containers,ContainerName, ContainerInstance, StartRestart, PodStatus, LastPodInventoryTimeGenerated, ClusterId;" +
                "let UsagePerfTable = CachedFilteredPerfTable            " +
                "| where CounterName =~ metricUsageCounterName            " +
                "| project TimeGenerated, Node, InstanceName,            CounterValue = iff(CounterName =~ 'cpuUsageNanoCores', CounterValue/1000000, CounterValue);" +
                "let LastRestartPerfTable = CachedFilteredPerfTable                " +
                "| where CounterName =~ 'restartTimeEpoch'                " +
                "| summarize arg_max(TimeGenerated, *) by Node, InstanceName                " +
                "| project Node, InstanceName, UpTime = CounterValue,                LastReported = TimeGenerated;" +
                "let AggregationTable = UsagePerfTable            " +
                "| summarize  Aggregation = percentile(CounterValue, 95) by Node, InstanceName            " +
                "| project Node, InstanceName, Aggregation;            " +
                "let TrendTable = UsagePerfTable            " +
                "| summarize TrendAggregation = percentile(CounterValue, 95)                by bin(TimeGenerated, trendBinSize), Node, InstanceName            " +
                "| project TrendTimeGenerated = TimeGenerated, Node, InstanceName , TrendAggregation;            " +
                "let containerFinalTable = MetaDataTable            " +
                "| join kind= leftouter( AggregationTable ) on Node, InstanceName            " +
                "| join kind = leftouter (LastRestartPerfTable) on Node, InstanceName            " +
                "| order by Aggregation desc, ContainerName            " +
                "| join kind =  leftouter ( TrendTable) on Node, InstanceName                        " +
                "| project ContainerIdentity = strcat(ContainerName, '|', Pod),  Status,ContainerStatusReason = columnifexists('ContainerStatusReason', ''),Aggregation, Node, Restarts, ReadySinceNow,  TrendTimeGenerated, TrendAggregation,LimitsValue, ControllerName, ControllerKind, ContainerID, Containers,UpTimeNow = datetime_diff('Millisecond', endDateTime, datetime_add('second', toint(UpTime), make_datetime(1970,1,1))),ContainerInstance, StartRestart, LastReportedDelta = datetime_diff('Millisecond', endDateTime, LastReported),PodStatus, InstanceName, Namespace, LastPodInventoryTimeGenerated, ClusterId;let unscheduledPods = KubePodInventory        " +
                "| where TimeGenerated < endDateTime        " +
                "| where TimeGenerated >= startDateTime        " +
                "| where isnotempty(ClusterName)        " +
                "| where isnotempty(Namespace)        " +
                "| where isempty(Computer)        " +
                "| extend Node = iff(isnotempty(Computer), Computer, 'unscheduled')        " +
                "| where ClusterId =~ '/subscriptions/{4}/resourceGroups/{5}/providers/Microsoft.ContainerService/managedClusters/{6}'                        " +
                "| where Node == '{7}'        " +
                "| where isempty(ContainerStatus)        " +
                "| where PodStatus == 'Pending'        " +
                "| summarize arg_max(TimeGenerated, *) by Name        " +
                "| project ContainerIdentity = strcat('|', Name ),        Status = iff(ContainerStatus =~ 'running', 0, iff(ContainerStatus=~'waiting', 1, iff(ContainerStatus =~'terminated', 2, 3))),        ContainerStatusReason = columnifexists('ContainerStatusReason', ''),        Aggregation = 0.0, Node, Restarts = toint(0), ReadySinceNow = '',        TrendTimeGenerated = todatetime(''), TrendAggregation = 0.0,        LimitsValue = 0.0, ControllerName, ControllerKind, ContainerID, Containers = 0, UpTimeNow = tolong(0), ContainerInstance = Name,        StartRestart = toint(0), PodStatus, InstanceName = strcat(ClusterId, '/'),        Namespace, LastPodInventoryTimeGenerated = TimeGenerated;" +
                "union containerFinalTable, unscheduledPods " +
                "| limit 200",
                queryParams.endDateTime, queryParams.startDateTime, metricLimitCounterName, queryParams.metricUsageCounterName, queryParams.subid, queryParams.resourceGroupName, queryParams.resourceName);
            return query;
        }*/
        public string AKSPodInfoInsightQuery(AKSPodInfoInsightQueryParams queryParams)
        {
            string query = string.Format(
                "let startDateTime = datetime('{0}');        " +
                "let timeGenerated = datetime('{1}');        " +
                "KubePodInventory        " +
                "| where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime        " +
                "| project PodName = Name, PodStatus, PodLabel, PodCreationTimeStamp, PodStartTimestamp = PodStartTime, PodUid, PodIp,         ControllerName, ControllerKind, ContainerName, NodeName = Computer, TimeGenerated        " +
                "| where PodName =~ '{2}' " +
                "| summarize arg_max(TimeGenerated, *) by PodName, ContainerName        " +
                "| join kind = leftouter " +
                "   (            Perf            " +
                "   | where ObjectName == 'K8SContainer'            " +
                "   | where CounterName == 'cpuLimitNanoCores'            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | extend ContainerNameParts = split(InstanceName, '/')            " +
                "   | extend ContainerNamePartCount = array_length(ContainerNameParts)            " +
                "   | extend PodUIDIndex = ContainerNamePartCount - 2, ContainerNameIndex = ContainerNamePartCount - 1            " +
                "   | extend ContainerName = strcat(ContainerNameParts[PodUIDIndex], '/', ContainerNameParts[ContainerNameIndex])            " +
                "   | project ContainerName, CounterName, CPULimit = CounterValue, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by ContainerName, CounterName        " +
                "   ) on ContainerName        " +
                "| join kind = leftouter " +
                "   (            Perf            " +
                "   | where ObjectName == 'K8SContainer'            " +
                "   | where CounterName == 'cpuRequestNanoCores'            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | extend ContainerNameParts = split(InstanceName, '/')            " +
                "   | extend ContainerNamePartCount = array_length(ContainerNameParts)            " +
                "   | extend PodUIDIndex = ContainerNamePartCount - 2, ContainerNameIndex = ContainerNamePartCount - 1            " +
                "   | extend ContainerName = strcat(ContainerNameParts[PodUIDIndex], '/', ContainerNameParts[ContainerNameIndex])            " +
                "   | project ContainerName, CounterName, CPURequest = CounterValue, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by ContainerName, CounterName        " +
                "   ) on ContainerName        " +
                "| join kind = leftouter " +
                "   (            Perf            " +
                "   | where ObjectName == 'K8SContainer'            " +
                "   | where CounterName == 'memoryLimitBytes'            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | extend ContainerNameParts = split(InstanceName, '/')            " +
                "   | extend ContainerNamePartCount = array_length(ContainerNameParts)            " +
                "   | extend PodUIDIndex = ContainerNamePartCount - 2, ContainerNameIndex = ContainerNamePartCount - 1            " +
                "   | extend ContainerName = strcat(ContainerNameParts[PodUIDIndex], '/', ContainerNameParts[ContainerNameIndex])            " +
                "   | project ContainerName, CounterName, MemoryLimit = CounterValue, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by ContainerName, CounterName        " +
                "   ) on ContainerName        " +
                "| join kind = leftouter " +
                "   (            Perf            " +
                "   | where ObjectName == 'K8SContainer'            " +
                "   | where CounterName == 'memoryRequestBytes'            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | extend ContainerNameParts = split(InstanceName, '/')            " +
                "   | extend ContainerNamePartCount = array_length(ContainerNameParts)            " +
                "   | extend PodUIDIndex = ContainerNamePartCount - 2, ContainerNameIndex = ContainerNamePartCount - 1            " +
                "   | extend ContainerName = strcat(ContainerNameParts[PodUIDIndex], '/', ContainerNameParts[ContainerNameIndex])            " +
                "   | project ContainerName, CounterName, MemoryRequest = CounterValue, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by ContainerName, CounterName        " +
                "   ) on ContainerName        " +
                "| join kind = leftouter " +
                "   (            Heartbeat            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | project NodeName = Computer, NodeIP = ComputerIP, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by NodeName        " +
                "   ) on NodeName        " +
                "| project PodName, PodStatus, ControllerName, ControllerKind, PodLabel, PodCreationTimeStamp, PodStartTimestamp,         PodUid, NodeIP, ContainerName, CPULimit, CPURequest, MemoryLimit, MemoryRequest",
                queryParams.startDateTime,queryParams.endDateTime,queryParams.podName);
            return query;
        }
        public string AKSNodeContainerInfoInsight(AKSNodeContainerInfoQueryParams queryParams)
        {
            string query = string.Format(
                "let startDateTime = datetime('{0}');        " +
                "let timeGenerated = datetime('{1}');        " +
                "KubePodInventory        " +
                "| where TimeGenerated == timeGenerated        " +
                "| where Computer =~ '{2}'        " +
                "| where ContainerName =~ '{3}/{4}'        " +
                "| project Computer, Name, ContainerName, ContainerID, ContainerStatus,        ContainerStatusReason = columnifexists('ContainerStatusReason', ''),        PodLabel, ContainerCreationTimeStamp, TimeGenerated        " +
                "| summarize arg_max(TimeGenerated, *) by Computer, Name, ContainerName        " +
                "| join kind = leftouter " +
                "(            ContainerInventory            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | where Computer =~ '{2}'            " +
                "   | project Computer, ContainerID, Image, ImageTag, EnvironmentVar, StartedTime, FinishedTime, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by Computer, ContainerID        " +
                ") on ContainerID        " +
                "| join kind = leftouter " +
                "(            Perf            " +
                "   | where ObjectName == 'K8SContainer'            " +
                "   | where Computer =~ '{2}'            " +
                "   | where CounterName == 'cpuLimitNanoCores'            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | extend ContainerNameParts = split(InstanceName, '/')            " +
                "   | extend ContainerNamePartCount = array_length(ContainerNameParts)            " +
                "   | extend PodUIDIndex = ContainerNamePartCount - 2, ContainerNameIndex = ContainerNamePartCount - 1            " +
                "   | extend ContainerName = strcat(ContainerNameParts[PodUIDIndex], '/', ContainerNameParts[ContainerNameIndex])            " +
                "   | where ContainerName =~ '{3}/{4}'            " +
                "   | project Computer, ContainerName, CounterName, CPULimit = CounterValue, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by Computer, ContainerName, CounterName        " +
                ") on Computer, ContainerName        " +
                "| join kind = leftouter " +
                "   (            Perf            " +
                "      | where ObjectName == 'K8SContainer'            " +
                "   | where Computer =~ '{2}'            " +
                "   | where CounterName == 'cpuRequestNanoCores'            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | extend ContainerNameParts = split(InstanceName, '/')            " +
                "   | extend ContainerNamePartCount = array_length(ContainerNameParts)            " +
                "   | extend PodUIDIndex = ContainerNamePartCount - 2, ContainerNameIndex = ContainerNamePartCount - 1            " +
                "   | extend ContainerName = strcat(ContainerNameParts[PodUIDIndex], '/', ContainerNameParts[ContainerNameIndex])            " +
                "   | where ContainerName =~ '{3}/{4}'            " +
                "   | project Computer, ContainerName, CounterName, CPURequest = CounterValue, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by Computer, ContainerName, CounterName        " +
                ") on Computer, ContainerName        " +
                "| join kind = leftouter " +
                "(            Perf            " +
                "   | where ObjectName == 'K8SContainer'            " +
                "   | where Computer =~ '{2}'            " +
                "   | where CounterName == 'memoryLimitBytes'            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | extend ContainerNameParts = split(InstanceName, '/')            " +
                "   | extend ContainerNamePartCount = array_length(ContainerNameParts)            " +
                "   | extend PodUIDIndex = ContainerNamePartCount - 2, ContainerNameIndex = ContainerNamePartCount - 1            " +
                "   | extend ContainerName = strcat(ContainerNameParts[PodUIDIndex], '/', ContainerNameParts[ContainerNameIndex])            " +
                "   | where ContainerName =~ '{3}/{4}'            " +
                "   | project Computer, ContainerName, CounterName, MemoryLimit = CounterValue, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by Computer, ContainerName, CounterName        " +
                ") on Computer, ContainerName        " +
                "| join kind = leftouter " +
                "(            Perf            " +
                "   | where ObjectName == 'K8SContainer'            " +
                "   | where Computer =~ '{2}'            " +
                "   | where CounterName == 'memoryRequestBytes'            " +
                "   | where TimeGenerated <= timeGenerated and TimeGenerated > startDateTime            " +
                "   | extend ContainerNameParts = split(InstanceName, '/')            " +
                "   | extend ContainerNamePartCount = array_length(ContainerNameParts)            " +
                "   | extend PodUIDIndex = ContainerNamePartCount - 2, ContainerNameIndex = ContainerNamePartCount - 1            " +
                "   | extend ContainerName = strcat(ContainerNameParts[PodUIDIndex], '/', ContainerNameParts[ContainerNameIndex])            " +
                "   | where ContainerName =~ '{3}/{4}'           " +
                "   | project Computer, ContainerName, CounterName, MemoryRequest = CounterValue, TimeGenerated            " +
                "   | summarize arg_max(TimeGenerated, *) by Computer, ContainerName, CounterName        " +
                ") on Computer, ContainerName        " +
                "| project ContainerName , ContainerID, ContainerStatus, ContainerStatusReason, Image, ImageTag, ContainerCreationTimeStamp,         StartedTime, FinishedTime, CPULimit, CPURequest, MemoryLimit, MemoryRequest, EnvironmentVar",
                queryParams.startDateTime,queryParams.endDateTime,queryParams.nodeName,queryParams.podUID,queryParams.containerName);
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
    public class AKSRunningStatusQuery
    {
        public string subid { get; set; }
        public string resourceGroupName { get; set; }
        public string resourceName { get; set; }
        public string timespan { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }
    }
    public class ListAllNodeQueryParams
    {
        public string subid { get; set; }
        public string resourceGroupName { get; set; }
        public string resourceName { get; set; }
        public string timespan { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }
        public string binSize { get; set; }
        public string usedMetricName { get; set; }

    }
    public class NodeInfoQueryParams
    {
        public string subid { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }
        public string nodeName { get; set; }
        public string timespan { get; set; }
    }
    public class AKSNodeContainersQueryParams
    {
        public string subid { get; set; }
        public string resourceGroupName { get; set; }
        public string resourceName { get; set; }
        public string timespan { get; set; }
        public string trendBinSize { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }
        public string metricUsageCounterName { get; set; }
        public string nodeName { get; set; }
    }

    public class AKSPodInfoInsightQueryParams
    {
        public string subid { get; set; }
        public string timespan { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }
        public string podName { get; set; }
    }
    public class AKSNodeContainerInfoQueryParams
    {
        // 2020-02-19T20:15:00.000Z
        public string startDateTime { get; set; }
        // 2020-02-20T02:29:35Z
        public string endDateTime { get; set; }
        // aks-nodepool1-13906345-vmss000000
        public string nodeName { get; set; }
        //0824770a-4e32-11ea-8b35-6efca145a7f1
        public string podUID { get; set; }
        // omsagent
        public string containerName { get; set; }
        // 6273fbea-8a11-498b-8218-02b6f4398e12   
        public string subid { get; set; }
        // 2020-02-19T20:15:00.000Z/2020-02-20T02:30:00.000Z
        public string timespan { get; set; }
    }
}
