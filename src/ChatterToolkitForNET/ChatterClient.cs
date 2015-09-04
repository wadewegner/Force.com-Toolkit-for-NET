using System;
using System.Net.Http;
using System.Threading.Tasks;
using Salesforce.Chatter.Models;
using Salesforce.Common;

namespace Salesforce.Chatter
{
    public class ChatterClient : IChatterClient, IDisposable
    {
        private JsonHttpClient _jsonHttpClient;

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion) 
            : this (instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            _jsonHttpClient = new JsonHttpClient(instanceUrl, apiVersion, accessToken, httpClient);
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
            return _jsonHttpClient.HttpPostAsync<T>(feedItemInput, string.Format("chatter/feeds/news/{0}/feed-items", userId));
        }

        public Task<T> PostFeedItemCommentAsync<T>(FeedItemInput envelope, string feedId)
        {
            return _jsonHttpClient.HttpPostAsync<T>(envelope, string.Format("chatter/feed-items/{0}/comments", feedId));
        }

        public Task<T> LikeFeedItemAsync<T>(string feedId)
        {
            return _jsonHttpClient.HttpPostAsync<T>(null, string.Format("chatter/feed-items/{0}/likes", feedId));
        }

        public Task<T> ShareFeedItemAsync<T>(string feedId, string userId)
        {
            var sharedFeedItem = new SharedFeedItemInput {OriginalFeedItemId = feedId};

            return _jsonHttpClient.HttpPostAsync<T>(sharedFeedItem, string.Format("chatter/feeds/user-profile/{0}/feed-items", userId));
        }

        public Task<T> GetMyNewsFeedAsync<T>(string query = "")
        {
            var url = "chatter/feeds/news/me/feed-items";

            if (!string.IsNullOrEmpty(query))
                url += string.Format("?q={0}",query);

            return _jsonHttpClient.HttpGetAsync<T>(url);
        }

        public Task<T> GetGroupsAsync<T>()
        {
            return _jsonHttpClient.HttpGetAsync<T>("chatter/groups");
        }

        public Task<T> GetGroupFeedAsync<T>(string groupId)
        {
            return _jsonHttpClient.HttpGetAsync<T>(string.Format("chatter/feeds/record/{0}/feed-items", groupId));
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
