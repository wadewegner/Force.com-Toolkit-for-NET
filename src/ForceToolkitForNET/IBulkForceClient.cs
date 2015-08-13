using System.Threading.Tasks;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force
{
    public interface IBulkForceClient
    {
        Task<JobInfoResult> CreateJobAsync(string objectName, BulkForceClient.OperationType operationType);
    }
}
