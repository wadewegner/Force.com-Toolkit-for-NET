using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using NUnit.Framework;
using Salesforce.Chatter.Models;
using Salesforce.Common;

namespace Salesforce.Chatter.FunctionalTests
{
    public class ChatterClientTests
    {
        private static string _tokenRequestEndpointUrl = ConfigurationSettings.AppSettings["TokenRequestEndpointUrl"];
        private static string _securityToken = ConfigurationSettings.AppSettings["SecurityToken"];
        private static string _consumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationSettings.AppSettings["Username"];
        private static string _password = ConfigurationSettings.AppSettings["Password"] + _securityToken;

        public async Task<ChatterClient> GetChatterClient()
        {
            const string userAgent = "chatter-toolkit-dotnet";

            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, userAgent, _tokenRequestEndpointUrl);

            var client = new ChatterClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
            return client;
        }

        [Test]
        public async void Chatter_IsNotNull()
        {
            var chatter = await GetChatterClient();

            Assert.IsNotNull(chatter);
        }

        [Test]
        public async void Chatter_Feeds_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var feeds = await chatter.FeedsAsync<object>();

            Assert.IsNotNull(feeds);
        }

        [Test]
        public async void Chatter_Users_Me_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var me = await chatter.MeAsync<UserDetail>();

            Assert.IsNotNull(me);
        }

        [Test]
        public async void Chatter_Users_Me_Id_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var me = await chatter.MeAsync<UserDetail>();

            Assert.IsNotNull(me.id);
        }

        [Test]
        public async void Chatter_PostFeedItem()
        {
            var chatter = await GetChatterClient();
            var feedItem = await postFeedItem(chatter);
            Assert.IsNotNull(feedItem);
        }

        [Test]
        public async void Chatter_Add_Comment()
        {
            var chatter = await GetChatterClient();
            var feedItem = await postFeedItem(chatter);
            var feedId = feedItem.id;

            var messageSegment = new MessageSegmentInput()
            {
                text = "Comment testing 1, 2, 3",
                type = "Text"
            };

            var body = new MessageBodyInput { messageSegments = new List<MessageSegmentInput> { messageSegment } };
            var commentInput = new FeedItemInput()
            {
                attachment = null,
                body = body
            };

            var comment = await chatter.PostFeedItemCommentAsync<Comment>(commentInput, feedId);
            Assert.IsNotNull(comment);
        }

        [Test]
        public async void Chatter_Add_Comment_With_Mention_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var feedItem = await postFeedItem(chatter);
            var feedId = feedItem.id;

            var me = await chatter.MeAsync<UserDetail>();
            var meId = me.id;

            var messageSegment1 = new MessageSegmentInput()
            {
                id = meId,
                type = "Mention",
            };

            var messageSegment2 = new MessageSegmentInput()
            {
                text = "Comment testing 1, 2, 3",
                type = "Text",
            };

            var body = new MessageBodyInput
            {
                messageSegments = new List<MessageSegmentInput>
                {
                    messageSegment1, 
                    messageSegment2
                }
            };
            var commentInput = new FeedItemInput()
            {
                attachment = null,
                body = body
            };

            var comment = await chatter.PostFeedItemCommentAsync<Comment>(commentInput, feedId);
            Assert.IsNotNull(comment);
        }

        [Test]
        public async void Chatter_Like_FeedItem_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var feedItem = await postFeedItem(chatter);
            var feedId = feedItem.id;
            
            var liked = await chatter.LikeFeedItemAsync<Like>(feedId);

            Assert.IsNotNull(liked);
        }

        [Test]
        public async void Chatter_Share_FeedItem_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var feedItem = await postFeedItem(chatter);
            var feedId = feedItem.id;

            var me = await chatter.MeAsync<UserDetail>();
            var meId = me.id;

            var sharedFeedItem = await chatter.ShareFeedItemAsync<FeedItem>(feedId, meId);

            Assert.IsNotNull(sharedFeedItem);
        }

        [Test]
        public async void Chatter_Get_My_News_Feed_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var myNewsFeeds = await chatter.GetMyNewsFeedAsync<FeedItemPage>();

            Assert.IsNotNull(myNewsFeeds);
        }

        [Test]
        public async void Chatter_Get_My_News_Feed_WithQuery_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var myNewsFeeds = await chatter.GetMyNewsFeedAsync<FeedItemPage>("wade");

            Assert.IsNotNull(myNewsFeeds);
        }

        [Test]
        public async void Chatter_Get_Groups_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var groups = await chatter.GetGroupsAsync<GroupPage>();

            Assert.IsNotNull(groups);
        }
        
        [Test]
        public async void Chatter_Get_Group_News_Feed_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var groups = await chatter.GetGroupsAsync<GroupPage>();
            var groupId = groups.groups[0].id;
            var groupFeed = await chatter.GetGroupFeedAsync<FeedItemPage>(groupId);

            Assert.IsNotNull(groupFeed);
        }

        [Test]
        public async void Chatter_Get_Topics_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var topics = await chatter.GetTopicsAsync<TopicCollection>();

            Assert.IsNotNull(topics);
        }

        [Test]
        public async void Chatter_Get_Users_IsNotNull()
        {
            var chatter = await GetChatterClient();
            var users = await chatter.GetUsersAsync<UserPage>();

            Assert.IsNotNull(users);
        }

        #region private functions
        private async Task<FeedItem> postFeedItem(ChatterClient chatter)
        {
            var me = await chatter.MeAsync<UserDetail>();
            var id = me.id;

            var messageSegment = new MessageSegmentInput()
            {
                text = "Testing 1, 2, 3",
                type = "Text"
            };

            var body = new MessageBodyInput { messageSegments = new List<MessageSegmentInput> { messageSegment } };
            var feedItemInput = new FeedItemInput()
            {
                attachment = null,
                body = body
            };

            var feedItem = await chatter.PostFeedItemAsync<FeedItem>(feedItemInput, id);
            return feedItem;
        }
        #endregion

    }
}
