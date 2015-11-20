using System.Threading.Tasks;
using Salesforce.Chatter.Models;
using Salesforce.Common.Models;

namespace Salesforce.Chatter
{
    public interface IChatterClient
    {
        Task<T> FeedsAsync<T>();
        Task<T> MeAsync<T>();
        Task<T> PostFeedItemAsync<T>(FeedItemInput feedItemInput, string userId);
        Task<T> PostFeedItemCommentAsync<T>(FeedItemInput envelope, string feedId);
        Task<T> PostFeedItemToObjectAsync<T>(ObjectFeedItemInput envelope);
        Task<T> PostFeedItemWithAttachmentAsync<T>(ObjectFeedItemInput envelope, byte[] fileContents, string fileName);
        Task<T> LikeFeedItemAsync<T>(string feedId);
        Task<T> ShareFeedItemAsync<T>(string feedId, string userId);
        Task<T> GetMyNewsFeedAsync<T>(string query = "");
        Task<T> GetGroupsAsync<T>();
        Task<T> GetGroupFeedAsync<T>(string groupId);
    }
}