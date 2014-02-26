using System.Net.Http;
using System.Threading.Tasks;
using Salesforce.Chatter.Models;
using Salesforce.Common;

namespace Salesforce.Chatter
{
    public class ChatterClient : IChatterClient
    {
        private static ServiceHttpClient _serviceHttpClient;
        private static string _userAgent = "common-libraries-dotnet";

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion) 
            : this (instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            _serviceHttpClient = new ServiceHttpClient(instanceUrl, apiVersion, accessToken, _userAgent, httpClient);
        }
        
        public async Task<T> FeedsAsync<T>()
        {
            var feeds = await _serviceHttpClient.HttpGetAsync<T>("chatter/feeds");
            return feeds;
        }

        public async Task<T> MeAsync<T>()
        {
            var me = await _serviceHttpClient.HttpGetAsync<T>("chatter/users/me");
            return me;
        }

        public async Task<T> PostFeedItemAsync<T>(FeedItemInput feedItemInput, string userId)
        {
            var feedItem = await _serviceHttpClient.HttpPostAsync<T>(feedItemInput, string.Format("chatter/feeds/news/{0}/feed-items", userId));
            return feedItem;
        }

        public async Task<T> PostFeedItemCommentAsync<T>(FeedItemInput envelope, string feedId)
        {
            var feedItem = await _serviceHttpClient.HttpPostAsync<T>(envelope, string.Format("chatter/feed-items/{0}/comments", feedId));
            return feedItem;
        }

        public async Task<T> LikeFeedItemAsync<T>(string feedId)
        {
            var like = await _serviceHttpClient.HttpPostAsync<T>(null, string.Format("chatter/feed-items/{0}/likes", feedId));
            return like;
        }

        public async Task<T> ShareFeedItemAsync<T>(string feedId, string userId)
        {
            var sharedFeedItem = new SharedFeedItemInput {originalFeedItemId = feedId};

            var feedItem = await _serviceHttpClient.HttpPostAsync<T>(sharedFeedItem, string.Format("chatter/feeds/user-profile/{0}/feed-items", userId));
            return feedItem;
        }

        public async Task<T> GetMyNewsFeedAsync<T>(string query = "")
        {
            var url = "chatter/feeds/news/me/feed-items";

            if (!string.IsNullOrEmpty(query))
                url += string.Format("?q={0}",query);

            var myNewsFeed = await _serviceHttpClient.HttpGetAsync<T>(url);

            return myNewsFeed;
        }

        public async Task<T> GetGroupsAsync<T>()
        {
            var groups = await _serviceHttpClient.HttpGetAsync<T>("chatter/groups");
            return groups;
        }

        public async Task<T> GetGroupFeedAsync<T>(string groupId)
        {
            var groupFeed = await _serviceHttpClient.HttpGetAsync<T>(string.Format("chatter/feeds/record/{0}/feed-items", groupId));
            return groupFeed;
        }

        public async Task<T> GetUsersAsync<T>()
        {
            var users = await _serviceHttpClient.HttpGetAsync<T>("chatter/users");
            return users;
        }

        public async Task<T> GetTopicsAsync<T>()
        {
            var users = await _serviceHttpClient.HttpGetAsync<T>("connect/topics");
            return users;
        }
    }
}
