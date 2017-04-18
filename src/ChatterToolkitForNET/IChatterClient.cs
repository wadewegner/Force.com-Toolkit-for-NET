using System.Threading;
using System.Threading.Tasks;
using Salesforce.Chatter.Models;

namespace Salesforce.Chatter
{
    public interface IChatterClient
    {
        Task<T> FeedsAsync<T>(CancellationToken token = default(CancellationToken));
        Task<T> MeAsync<T>(CancellationToken token = default(CancellationToken));
        Task<T> PostFeedItemAsync<T>(FeedItemInput feedItemInput, string userId, CancellationToken token = default(CancellationToken));
        Task<T> PostFeedItemCommentAsync<T>(FeedItemInput envelope, string feedId, CancellationToken token = default(CancellationToken));
        Task<T> LikeFeedItemAsync<T>(string feedId, CancellationToken token = default(CancellationToken));
        Task<T> ShareFeedItemAsync<T>(string feedId, string userId, CancellationToken token = default(CancellationToken));
        Task<T> GetMyNewsFeedAsync<T>(string query = "", CancellationToken token = default(CancellationToken));
        Task<T> GetGroupsAsync<T>(CancellationToken token = default(CancellationToken));
        Task<T> GetGroupFeedAsync<T>(string groupId, CancellationToken token = default(CancellationToken));
	    Task<T> GetUsersAsync<T>(CancellationToken token = default(CancellationToken));
	    Task<T> GetTopicsAsync<T>(CancellationToken token = default(CancellationToken));
    }
}