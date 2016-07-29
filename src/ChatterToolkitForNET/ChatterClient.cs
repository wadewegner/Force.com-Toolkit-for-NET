using System;
using System.Net.Http;
using System.Threading.Tasks;
using Salesforce.Chatter.Models;
using Salesforce.Common;

namespace Salesforce.Chatter
{
    public class ChatterClient : IChatterClient, IDisposable
    {
        private readonly JsonHttpClient _jsonHttpClient;
	    private readonly string _itemsOrElements;

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion)
            : this(instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            _jsonHttpClient = new JsonHttpClient(instanceUrl, apiVersion, accessToken, httpClient);

            // A change in endpoint for feed item was introduced in v31 of the API.
            _itemsOrElements = float.Parse(_jsonHttpClient.GetApiVersion().Substring(1)) > 30 ? "feed-elements" : "feed-items";
        }

        public Task<T> FeedsAsync<T>()
        {
            return _jsonHttpClient.HttpGetAsync<T>("chatter/feeds");
        }

        public Task<T> MeAsync<T>()
        {
            return _jsonHttpClient.HttpGetAsync<T>("chatter/users/me");
        }

        public Task<T> PostFeedItemAsync<T>(FeedItemInput feedItemInput, string userId)
        {
            // Feed items not available post v30.0
            if (float.Parse(_jsonHttpClient.GetApiVersion().Substring(1)) > 30.0)
            {
                return _jsonHttpClient.HttpPostAsync<T>(feedItemInput, "chatter/feed-elements");
            }

            return _jsonHttpClient.HttpPostAsync<T>(feedItemInput, string.Format("chatter/feeds/news/{0}/{1}", userId, _itemsOrElements));
        }

        public Task<T> PostFeedItemToObjectAsync<T>(ObjectFeedItemInput envelope)
        {
            return _jsonHttpClient.HttpPostAsync<T>(envelope, "chatter/feed-elements/");
        }

        public Task<T> PostFeedItemWithAttachmentAsync<T>(ObjectFeedItemInput envelope, byte[] fileContents, string fileName)
        {
            return _jsonHttpClient.HttpBinaryDataPostAsync<T>("chatter/feed-elements/", envelope, fileContents, "feedElementFileUpload", fileName);
        }

        public Task<T> PostFeedItemCommentAsync<T>(FeedItemInput envelope, string feedId)
        {
            if (float.Parse(_jsonHttpClient.GetApiVersion().Substring(1)) > 30.0)
            {
                return _jsonHttpClient.HttpPostAsync<T>(envelope, string.Format("chatter/{0}/{1}/capabilities/comments/items", _itemsOrElements, feedId));
            }

            return _jsonHttpClient.HttpPostAsync<T>(envelope, string.Format("chatter/{0}/{1}/comments", _itemsOrElements, feedId));
        }

        public Task<T> LikeFeedItemAsync<T>(string feedId)
        {
            if (float.Parse(_jsonHttpClient.GetApiVersion().Substring(1)) > 30.0)
            {
                return _jsonHttpClient.HttpPostAsync<T>(null, string.Format("chatter/{0}/{1}/capabilities/chatter-likes/items", _itemsOrElements, feedId));
            }

            return _jsonHttpClient.HttpPostAsync<T>(null, string.Format("chatter/{0}/{1}/likes", _itemsOrElements, feedId));
        }

        public Task<T> ShareFeedItemAsync<T>(string feedId, string userId)
        {
            var sharedFeedItem = new SharedFeedItemInput { SubjectId = userId };

            if (float.Parse(_jsonHttpClient.GetApiVersion().Substring(1)) > 30.0)
            {
                sharedFeedItem.OriginalFeedElementId = feedId;
                return _jsonHttpClient.HttpPostAsync<T>(sharedFeedItem, "chatter/feed-elements");
            }

            sharedFeedItem.OriginalFeedItemId = feedId;
            return _jsonHttpClient.HttpPostAsync<T>(sharedFeedItem, string.Format("chatter/feeds/user-profile/{0}/{1}", userId, _itemsOrElements));
        }

        public Task<T> GetMyNewsFeedAsync<T>(string query = "")
        {
            var url = string.Format("chatter/feeds/news/me/{0}", _itemsOrElements);

            if (!string.IsNullOrEmpty(query))
                url += string.Format("?q={0}", query);

            return _jsonHttpClient.HttpGetAsync<T>(url);
        }

        public Task<T> GetGroupsAsync<T>()
        {
            return _jsonHttpClient.HttpGetAsync<T>("chatter/groups");
        }

        public Task<T> GetGroupFeedAsync<T>(string groupId)
        {
            return _jsonHttpClient.HttpGetAsync<T>(string.Format("chatter/feeds/record/{0}/{1}", _itemsOrElements, groupId));
        }

        public Task<T> GetUsersAsync<T>()
        {
            return _jsonHttpClient.HttpGetAsync<T>("chatter/users");
        }

        public Task<T> GetTopicsAsync<T>()
        {
            return _jsonHttpClient.HttpGetAsync<T>("connect/topics");
        }

        public void Dispose()
        {
            _jsonHttpClient.Dispose();
        }
    }
}
