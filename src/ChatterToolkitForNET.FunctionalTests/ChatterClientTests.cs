using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
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
        private static string _consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationManager.AppSettings["Username"];
        private static string _password = ConfigurationManager.AppSettings["Password"];

        private AuthenticationClient _auth;
        private ChatterClient _chatterClient;
        
        [TestFixtureSetUp]
        public void Init()
        {
            if (string.IsNullOrEmpty(_consumerKey) && string.IsNullOrEmpty(_consumerSecret) && string.IsNullOrEmpty(_username) && string.IsNullOrEmpty(_password))
            {
                _consumerKey = Environment.GetEnvironmentVariable("ConsumerKey");
                _consumerSecret = Environment.GetEnvironmentVariable("ConsumerSecret");
                _username = Environment.GetEnvironmentVariable("Username");
                _password = Environment.GetEnvironmentVariable("Password");
            }

            // Use TLS 1.2 (instead of defaulting to 1.0)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, TokenRequestEndpointUrl).Wait();
            
            _chatterClient = new ChatterClient(_auth.InstanceUrl, _auth.AccessToken, _auth.ApiVersion);
        }

        [Test]
        public void Chatter_IsNotNull()
        {
            Assert.IsNotNull(_chatterClient);
        }

        [Test]
        public async Task Chatter_Feeds_IsNotNull()
        {
            var feeds = await _chatterClient.FeedsAsync<object>();

            Assert.IsNotNull(feeds);
        }

        [Test]
        public async Task Chatter_Users_Me_IsNotNull()
        {
            var me = await _chatterClient.MeAsync<UserDetail>();

            Assert.IsNotNull(me);
        }

        [Test]
        public async Task Chatter_Users_Me_Id_IsNotNull()
        {
            var me = await _chatterClient.MeAsync<UserDetail>();

            Assert.IsNotNull(me.id);
        }

        [Test]
        public async Task Chatter_PostFeedItem()
        {
            var feedItem = await postFeedItem(_chatterClient);
            Assert.IsNotNull(feedItem);
        }

        [Test]
        public async Task Chatter_Add_Comment()
        {
            var feedItem = await postFeedItem(_chatterClient);
            var feedId = feedItem.Id;

            var messageSegment = new MessageSegmentInput
            {
                Text = "Comment testing 1, 2, 3",
                Type = "Text"
            };

            var body = new MessageBodyInput { MessageSegments = new List<MessageSegmentInput> { messageSegment } };
            var commentInput = new FeedItemInput
            {
                Attachment = null,
                Body = body
            };

            var comment = await _chatterClient.PostFeedItemCommentAsync<Comment>(commentInput, feedId);
            Assert.IsNotNull(comment);
        }

        [Test]
        public async Task Chatter_Add_Comment_With_Mention_IsNotNull()
        {
            var feedItem = await postFeedItem(_chatterClient);
            var feedId = feedItem.Id;

            var me = await _chatterClient.MeAsync<UserDetail>();
            var meId = me.id;

            var messageSegment1 = new MessageSegmentInput
            {
                Id = meId,
                Type = "Mention",
            };

            var messageSegment2 = new MessageSegmentInput
            {
                Text = "Comment testing 1, 2, 3",
                Type = "Text",
            };

            var body = new MessageBodyInput
            {
                MessageSegments = new List<MessageSegmentInput>
                {
                    messageSegment1, 
                    messageSegment2
                }
            };
            var commentInput = new FeedItemInput
            {
                Attachment = null,
                Body = body
            };

            var comment = await _chatterClient.PostFeedItemCommentAsync<Comment>(commentInput, feedId);
            Assert.IsNotNull(comment);
        }

        [Test]
        public async Task Chatter_Like_FeedItem_IsNotNull()
        {
            var feedItem = await postFeedItem(_chatterClient);
            var feedId = feedItem.Id;

            var liked = await _chatterClient.LikeFeedItemAsync<Like>(feedId);

            Assert.IsNotNull(liked);
        }

        [Test]
        public async Task Chatter_Share_FeedItem_IsNotNull()
        {
            var feedItem = await postFeedItem(_chatterClient);
            var feedId = feedItem.Id;

            var me = await _chatterClient.MeAsync<UserDetail>();
            var meId = me.id;

            var sharedFeedItem = await _chatterClient.ShareFeedItemAsync<FeedItem>(feedId, meId);

            Assert.IsNotNull(sharedFeedItem);
        }

        [Test]
        public async Task Chatter_Get_My_News_Feed_IsNotNull()
        {
            var myNewsFeeds = await _chatterClient.GetMyNewsFeedAsync<FeedItemPage>();

            Assert.IsNotNull(myNewsFeeds);
        }

        [Test]
        public async Task Chatter_Get_My_News_Feed_WithQuery_IsNotNull()
        {
            var myNewsFeeds = await _chatterClient.GetMyNewsFeedAsync<FeedItemPage>("wade");

            Assert.IsNotNull(myNewsFeeds);
        }

        [Test]
        public async Task Chatter_Get_Groups_IsNotNull()
        {
            var groups = await _chatterClient.GetGroupsAsync<GroupPage>();

            Assert.IsNotNull(groups);
        }
        
        [Test]
        public async Task Chatter_Get_Group_News_Feed_IsNotNull()
        {
            var groups = await _chatterClient.GetGroupsAsync<GroupPage>();
            if (groups.Groups.Count > 0)
            {
                var groupId = groups.Groups[0].Id;
                var groupFeed = await _chatterClient.GetGroupFeedAsync<FeedItemPage>(groupId);

                Assert.IsNotNull(groupFeed);
            }
            else
            {
                Assert.AreEqual(0, groups.Groups.Count);
            }
        }

        [Test]
        public async Task Chatter_Get_Topics_IsNotNull()
        {
            var topics = await _chatterClient.GetTopicsAsync<TopicCollection>();

            Assert.IsNotNull(topics);
        }

        [Test]
        public async Task Chatter_Get_Users_IsNotNull()
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
                Text = "Testing 1, 2, 3",
                Type = "Text"
            };

            var body = new MessageBodyInput { MessageSegments = new List<MessageSegmentInput> { messageSegment } };
            var feedItemInput = new FeedItemInput()
            {
                Attachment = null,
                Body = body,
                SubjectId = id,
                FeedElementType = "FeedItem"
            };

            var feedItem = await chatter.PostFeedItemAsync<FeedItem>(feedItemInput, id);
            return feedItem;
        }
        #endregion
    }
}
