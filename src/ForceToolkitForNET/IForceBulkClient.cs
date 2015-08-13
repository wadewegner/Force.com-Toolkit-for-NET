using System.Threading.Tasks;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force
{
    public interface IForceBulkClient
    {
        Task<JobInfoResult> CreateJobAsync(string objectName, ForceBulkClient.OperationType operationType);
    }
}
