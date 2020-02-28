using ClusterManager.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.Core.Infrastructures
{
    public interface IAKSBus
    {
        Task<object> ListAllAKS(string email,string subid);
        Task<object> GetAKSInfo(string email,string subid,string resourceGroup, string AKSName);

        Task<object> ListK8sVersion(string email, string subid);
        Task<object> ListWorkspace(string email, string subid);

        Task<object> CreateAKS(string email,string subid,string resourceGroupName, CreateAKSModel createAKSModel);
        Task<object> DeleteAKS(string email, string subid, string resourceGroupName, string resourceName);
    }
}
