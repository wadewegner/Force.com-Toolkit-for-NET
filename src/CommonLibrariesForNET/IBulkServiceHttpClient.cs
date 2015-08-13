using System.Threading.Tasks;

namespace Salesforce.Common
{
    public interface IBulkServiceHttpClient
    {
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
    }
}
