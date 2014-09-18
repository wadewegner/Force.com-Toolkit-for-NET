using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using NUnit.Framework;
using Salesforce.Chatter.Models;
using Salesforce.Common;

namespace Salesforce.Chatter.FunctionalTests
{
    [TestFixture]
    public class ChatterClientTests
    {
        private static readonly string TokenRequestEndpointUrl = ConfigurationManager.AppSettings["TokenRequestEndpointUrl"];
        private static readonly string SecurityToken = ConfigurationManager.AppSettings["SecurityToken"];
        private static readonly string ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static readonly string Username = ConfigurationManager.AppSettings["Username"];
        private static readonly string Password = ConfigurationManager.AppSettings["Password"] + SecurityToken;

        private string _userAgent;
        private AuthenticationClient _auth;
        private ChatterClient _chatterClient;

        [TestFixtureSetUp]
        public void Init()
        {
            _userAgent = "chatter-toolkit-dotnet";
            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password, _userAgent, TokenRequestEndpointUrl).Wait();

            _chatterClient = new ChatterClient(_auth.InstanceUrl, _auth.AccessToken, _auth.ApiVersion);
        }

        [Test]
        public void Chatter_IsNotNull()
        {
            Assert.IsNotNull(_chatterClient);
        }

        [Test]
        public async void Chatter_Feeds_IsNotNull()
        {
            var feeds = await _chatterClient.FeedsAsync<object>();

            Assert.IsNotNull(feeds);
        }

        [Test]
        public async void Chatter_Users_Me_IsNotNull()
        {
            var me = await _chatterClient.MeAsync<UserDetail>();

            Assert.IsNotNull(me);
        }

        [Test]
        public async void Chatter_Users_Me_Id_IsNotNull()
        {
            var me = await _chatterClient.MeAsync<UserDetail>();

            Assert.IsNotNull(me.id);
        }

        [Test]
        public async void Chatter_PostFeedItem()
        {
            var feedItem = await postFeedItem(_chatterClient);
            Assert.IsNotNull(feedItem);
        }

        [Test]
        public async void Chatter_Add_Comment()
        {
            var feedItem = await postFeedItem(_chatterClient);
            var feedId = feedItem.id;

            var messageSegment = new MessageSegmentInput
            {
                text = "Comment testing 1, 2, 3",
                type = "Text"
            };

            var body = new MessageBodyInput { messageSegments = new List<MessageSegmentInput> { messageSegment } };
            var commentInput = new FeedItemInput
            {
                attachment = null,
                body = body
            };

            var comment = await _chatterClient.PostFeedItemCommentAsync<Comment>(commentInput, feedId);
            Assert.IsNotNull(comment);
        }

        [Test]
        public async void Chatter_Add_Comment_With_Mention_IsNotNull()
        {
            var feedItem = await postFeedItem(_chatterClient);
            var feedId = feedItem.id;

            var me = await _chatterClient.MeAsync<UserDetail>();
            var meId = me.id;

            var messageSegment1 = new MessageSegmentInput
            {
                id = meId,
                type = "Mention",
            };

            var messageSegment2 = new MessageSegmentInput
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
            var commentInput = new FeedItemInput
            {
                attachment = null,
                body = body
            };

            var comment = await _chatterClient.PostFeedItemCommentAsync<Comment>(commentInput, feedId);
            Assert.IsNotNull(comment);
        }

        [Test]
        public async void Chatter_Like_FeedItem_IsNotNull()
        {
            var feedItem = await postFeedItem(_chatterClient);
            var feedId = feedItem.id;

            var liked = await _chatterClient.LikeFeedItemAsync<Like>(feedId);

            Assert.IsNotNull(liked);
        }

        [Test]
        public async void Chatter_Share_FeedItem_IsNotNull()
        {
            var feedItem = await postFeedItem(_chatterClient);
            var feedId = feedItem.id;

            var me = await _chatterClient.MeAsync<UserDetail>();
            var meId = me.id;

            var sharedFeedItem = await _chatterClient.ShareFeedItemAsync<FeedItem>(feedId, meId);

            Assert.IsNotNull(sharedFeedItem);
        }

        [Test]
        public async void Chatter_Get_My_News_Feed_IsNotNull()
        {
            var myNewsFeeds = await _chatterClient.GetMyNewsFeedAsync<FeedItemPage>();

            Assert.IsNotNull(myNewsFeeds);
        }

        [Test]
        public async void Chatter_Get_My_News_Feed_WithQuery_IsNotNull()
        {
            var myNewsFeeds = await _chatterClient.GetMyNewsFeedAsync<FeedItemPage>("wade");

            Assert.IsNotNull(myNewsFeeds);
        }

        [Test]
        public async void Chatter_Get_Groups_IsNotNull()
        {
            var groups = await _chatterClient.GetGroupsAsync<GroupPage>();

            Assert.IsNotNull(groups);
        }
        
        //TODO: Create a test that creates a chatter group and adds to the feed so that it never fails
        [Test]
        public async void Chatter_Get_Group_News_Feed_IsNotNull()
        {
            var groups = await _chatterClient.GetGroupsAsync<GroupPage>();
            var groupId = groups.groups[0].id;
            var groupFeed = await _chatterClient.GetGroupFeedAsync<FeedItemPage>(groupId);

            Assert.IsNotNull(groupFeed);
        }

        [Test]
        public async void Chatter_Get_Topics_IsNotNull()
        {
            var topics = await _chatterClient.GetTopicsAsync<TopicCollection>();

            Assert.IsNotNull(topics);
        }

        [Test]
        public async void Chatter_Get_Users_IsNotNull()
        {
            var users = await _chatterClient.GetUsersAsync<UserPage>();

            Assert.IsNotNull(users);
        }

        #region private functions
        private async Task<FeedItem> postFeedItem(ChatterClient chatter)
        {
            var me = await chatter.MeAsync<UserDetail>();
            var id = me.id;

            var messageSegment = new MessageSegmentInput
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
