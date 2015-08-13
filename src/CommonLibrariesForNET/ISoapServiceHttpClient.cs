using System.Threading.Tasks;

namespace Salesforce.Common
{
    public interface ISoapServiceHttpClient
    {
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
    }
}
