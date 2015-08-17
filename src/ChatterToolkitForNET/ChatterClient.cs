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
        private String itemsOrElements = "feed-items";

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion) 
            : this (instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public ChatterClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            _serviceHttpClient = new ServiceHttpClient(instanceUrl, apiVersion, accessToken, httpClient);
            // A change in endpoint for feed item was introduced in v31 of the API.
            if (float.Parse(_serviceHttpClient._apiVersion.Substring(1)) > 30)
            {
               itemsOrElements = "feed-elements";
            }
            else
            {
                itemsOrElements = "feed-items";
            }

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
            // Feed items not available post v30.0
            if (float.Parse(_serviceHttpClient._apiVersion.Substring(1)) > 30.0)
            {
                return _serviceHttpClient.HttpPostAsync<T>(feedItemInput, "chatter/feed-elements");
            }
            else
            {
                return _serviceHttpClient.HttpPostAsync<T>(feedItemInput, string.Format("chatter/feeds/news/{0}/{1}", userId, itemsOrElements));
            }
        }

        public Task<T> PostFeedItemCommentAsync<T>(FeedItemInput envelope, string feedId)
        {
            if (float.Parse(_serviceHttpClient._apiVersion.Substring(1)) > 30.0)
            {
                return _serviceHttpClient.HttpPostAsync<T>(envelope, string.Format("chatter/{0}/{1}/capabilities/comments/items", itemsOrElements, feedId));
            }
            else
            {
                return _serviceHttpClient.HttpPostAsync<T>(envelope, string.Format("chatter/{0}/{1}/comments", itemsOrElements, feedId));
            }
        }

        public Task<T> LikeFeedItemAsync<T>(string feedId)
        {
            if (float.Parse(_serviceHttpClient._apiVersion.Substring(1))> 30.0)
            {
                return _serviceHttpClient.HttpPostAsync<T>(null, string.Format("chatter/{0}/{1}/capabilities/chatter-likes/items", itemsOrElements, feedId));
            }
            else
            {
                return _serviceHttpClient.HttpPostAsync<T>(null, string.Format("chatter/{0}/{1}/likes", itemsOrElements, feedId));
            }
        }

        public Task<T> ShareFeedItemAsync<T>(string feedId, string userId)
        {
            var sharedFeedItem = new SharedFeedItemInput {
                SubjectId = userId
            };
            if (float.Parse(_serviceHttpClient._apiVersion.Substring(1)) > 30.0)
            {
                sharedFeedItem.OriginalFeedElementId = feedId;
                return _serviceHttpClient.HttpPostAsync<T>(sharedFeedItem, "chatter/feed-elements");
            }
            else
            {
                sharedFeedItem.OriginalFeedItemId = feedId;
                return _serviceHttpClient.HttpPostAsync<T>(sharedFeedItem, string.Format("chatter/feeds/user-profile/{0}/{1}", userId, itemsOrElements));
            }
        }

        public Task<T> GetMyNewsFeedAsync<T>(string query = "")
        {
            var url = string.Format("chatter/feeds/news/me/{0}", itemsOrElements);

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
            return _serviceHttpClient.HttpGetAsync<T>(string.Format("chatter/feeds/record/{0}/{1}", itemsOrElements, groupId));
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
