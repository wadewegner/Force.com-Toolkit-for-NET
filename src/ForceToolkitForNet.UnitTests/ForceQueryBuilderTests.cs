using System;
using NUnit.Framework;
using Salesforce.Common.Attributes;

namespace Salesforce.Force.UnitTests
{
    class Account
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public int CustomerNumber__c { get; set; }
    }

    class Opportunity
    {
        public string Name { get; set; }

        [SubEntity]
        public Account Account { get; set; }
    }


    [TestFixture]
    public class ForceQueryBuilderTests
    {
        [Test]
        public void SimpleObjectQueriesForAllProperties()
        {
            var query = ForceQueryBuilder.DeriveQuery<Account>("Account", "123");
            Assert.AreEqual("SELECT Name, Phone, CustomerNumber__c FROM Account WHERE Id = '123'", query);
        }

        [Test]
        public void QueryObjectWithNestedObject()
        {
            var query = ForceQueryBuilder.DeriveQuery<Opportunity>("Opportunity", "321");
            Assert.AreEqual("SELECT Name, Account.Name, Account.Phone, Account.CustomerNumber__c FROM Opportunity WHERE Id = '321'", query);
        }
    }
}
