using System;
using System.Net.Http;
using System.Threading.Tasks;
using Salesforce.Chatter.Models;
using Salesforce.Common;

namespace Salesforce.Chatter
{
    public class ChatterClient : IChatterClient, IDisposable
    {
        private ServiceHttpClient _serviceHttpClient;

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion) 
            : this (instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            _serviceHttpClient = new ServiceHttpClient(instanceUrl, apiVersion, accessToken, httpClient);
        }
        
        public Task<T> FeedsAsync<T>()
        {
            return _serviceHttpClient.HttpGetAsync<T>("chatter/feeds");
        }

        public Task<T> MeAsync<T>()
        {
            return _serviceHttpClient.HttpGetAsync<T>("chatter/users/me");
        }

        public Task<T> PostFeedItemAsync<T>(FeedItemInput feedItemInput, string userId)
        {
            return _serviceHttpClient.HttpPostAsync<T>(feedItemInput, string.Format("chatter/feeds/news/{0}/feed-items", userId));
        }

        public Task<T> PostFeedItemCommentAsync<T>(FeedItemInput envelope, string feedId)
        {
            return _serviceHttpClient.HttpPostAsync<T>(envelope, string.Format("chatter/feed-items/{0}/comments", feedId));
        }

        public Task<T> LikeFeedItemAsync<T>(string feedId)
        {
            return _serviceHttpClient.HttpPostAsync<T>(null, string.Format("chatter/feed-items/{0}/likes", feedId));
        }

        public Task<T> ShareFeedItemAsync<T>(string feedId, string userId)
        {
            var sharedFeedItem = new SharedFeedItemInput {OriginalFeedItemId = feedId};

            return _serviceHttpClient.HttpPostAsync<T>(sharedFeedItem, string.Format("chatter/feeds/user-profile/{0}/feed-items", userId));
        }

        public Task<T> GetMyNewsFeedAsync<T>(string query = "")
        {
            var url = "chatter/feeds/news/me/feed-items";

            if (!string.IsNullOrEmpty(query))
                url += string.Format("?q={0}",query);

            return _serviceHttpClient.HttpGetAsync<T>(url);
        }

        public Task<T> GetGroupsAsync<T>()
        {
            return _serviceHttpClient.HttpGetAsync<T>("chatter/groups");
        }

        public Task<T> GetGroupFeedAsync<T>(string groupId)
        {
            return _serviceHttpClient.HttpGetAsync<T>(string.Format("chatter/feeds/record/{0}/feed-items", groupId));
        }

        public Task<T> GetUsersAsync<T>()
        {
            return _serviceHttpClient.HttpGetAsync<T>("chatter/users");
        }

        public Task<T> GetTopicsAsync<T>()
        {
            return _serviceHttpClient.HttpGetAsync<T>("connect/topics");
        }

        public void Dispose()
        {
            _serviceHttpClient.Dispose();
        }
    }
}
