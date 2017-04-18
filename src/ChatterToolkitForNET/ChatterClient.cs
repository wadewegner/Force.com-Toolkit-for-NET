using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Salesforce.Chatter.Models;
using Salesforce.Common;

namespace Salesforce.Chatter
{
    public class ChatterClient : IChatterClient, IDisposable
    {
        private readonly IJsonHttpClient _jsonHttpClient;
        private readonly JsonHttpClient _fullJsonHttpClient;
	    private readonly string _itemsOrElements;

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion)
            : this(instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            _jsonHttpClient = _fullJsonHttpClient = new JsonHttpClient(instanceUrl, apiVersion, accessToken, httpClient);

            // A change in endpoint for feed item was introduced in v31 of the API.
            _itemsOrElements = float.Parse(_fullJsonHttpClient.GetApiVersion().Substring(1)) > 30 ? "feed-elements" : "feed-items";
        }

        public Task<T> FeedsAsync<T>(CancellationToken token)
        {
            return _jsonHttpClient.HttpGetAsync<T>("chatter/feeds", token);
        }

        public Task<T> MeAsync<T>(CancellationToken token)
        {
            return _jsonHttpClient.HttpGetAsync<T>("chatter/users/me", token);
        }

        public Task<T> PostFeedItemAsync<T>(FeedItemInput feedItemInput, string userId, CancellationToken token)
        {
            // Feed items not available post v30.0
            if (float.Parse(_fullJsonHttpClient.GetApiVersion().Substring(1)) > 30.0)
            {
                return _jsonHttpClient.HttpPostAsync<T>(feedItemInput, "chatter/feed-elements", token);
            }

            return _jsonHttpClient.HttpPostAsync<T>(feedItemInput, string.Format("chatter/feeds/news/{0}/{1}", userId, _itemsOrElements), token);
        }

        public Task<T> PostFeedItemToObjectAsync<T>(ObjectFeedItemInput envelope, CancellationToken token)
        {
            return _jsonHttpClient.HttpPostAsync<T>(envelope, "chatter/feed-elements/", token);
        }

        public Task<T> PostFeedItemWithAttachmentAsync<T>(ObjectFeedItemInput envelope, byte[] fileContents, string fileName, CancellationToken token)
        {
            return _jsonHttpClient.HttpBinaryDataPostAsync<T>("chatter/feed-elements/", envelope, fileContents, "feedElementFileUpload", fileName, token);
        }

        public Task<T> PostFeedItemCommentAsync<T>(FeedItemInput envelope, string feedId, CancellationToken token)
        {
            if (float.Parse(_fullJsonHttpClient.GetApiVersion().Substring(1)) > 30.0)
            {
                return _jsonHttpClient.HttpPostAsync<T>(envelope, string.Format("chatter/{0}/{1}/capabilities/comments/items", _itemsOrElements, feedId), token);
            }

            return _jsonHttpClient.HttpPostAsync<T>(envelope, string.Format("chatter/{0}/{1}/comments", _itemsOrElements, feedId), token);
        }

        public Task<T> LikeFeedItemAsync<T>(string feedId, CancellationToken token)
        {
            if (float.Parse(_fullJsonHttpClient.GetApiVersion().Substring(1)) > 30.0)
            {
                return _jsonHttpClient.HttpPostAsync<T>(null, string.Format("chatter/{0}/{1}/capabilities/chatter-likes/items", _itemsOrElements, feedId), token);
            }

            return _jsonHttpClient.HttpPostAsync<T>(null, string.Format("chatter/{0}/{1}/likes", _itemsOrElements, feedId), token);
        }

        public Task<T> ShareFeedItemAsync<T>(string feedId, string userId, CancellationToken token)
        {
            var sharedFeedItem = new SharedFeedItemInput { SubjectId = userId };

            if (float.Parse(_fullJsonHttpClient.GetApiVersion().Substring(1)) > 30.0)
            {
                sharedFeedItem.OriginalFeedElementId = feedId;
                return _jsonHttpClient.HttpPostAsync<T>(sharedFeedItem, "chatter/feed-elements", token);
            }

            sharedFeedItem.OriginalFeedItemId = feedId;
            return _jsonHttpClient.HttpPostAsync<T>(sharedFeedItem, string.Format("chatter/feeds/user-profile/{0}/{1}", userId, _itemsOrElements), token);
        }

        public Task<T> GetMyNewsFeedAsync<T>(string query = "", CancellationToken token = default(CancellationToken))
        {
            var url = string.Format("chatter/feeds/news/me/{0}", _itemsOrElements);

            if (!string.IsNullOrEmpty(query))
                url += string.Format("?q={0}", query);

            return _jsonHttpClient.HttpGetAsync<T>(url, token);
        }

        public Task<T> GetGroupsAsync<T>(CancellationToken token)
        {
            return _jsonHttpClient.HttpGetAsync<T>("chatter/groups", token);
        }

        public Task<T> GetGroupFeedAsync<T>(string groupId, CancellationToken token)
        {
            return _jsonHttpClient.HttpGetAsync<T>(string.Format("chatter/feeds/record/{0}/{1}", _itemsOrElements, groupId), token);
        }

        public Task<T> GetUsersAsync<T>(CancellationToken token)
        {
            return _jsonHttpClient.HttpGetAsync<T>("chatter/users", token);
        }

        public Task<T> GetTopicsAsync<T>(CancellationToken token)
        {
            return _jsonHttpClient.HttpGetAsync<T>("connect/topics", token);
        }

        public void Dispose()
        {
            _jsonHttpClient.Dispose();
        }
    }
}
