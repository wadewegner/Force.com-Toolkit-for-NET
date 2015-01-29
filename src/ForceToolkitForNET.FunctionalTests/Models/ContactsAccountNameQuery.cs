namespace Salesforce.Force.FunctionalTests.Models.QueryTest
{
    public class Contact
    {
        public string AccountId { get; set; }
        public Account Account { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string MobilePhone { get; set; }
    }
}
