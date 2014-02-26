//TODO: add license header

using Salesforce.Common.Models;

namespace Salesforce.Common
{
    public class ForceAuthException : ForceException
    {
        public ForceAuthException(string error, string description) : base(error, description)
        {
        }

        public ForceAuthException(string error, string description, string[] fields) : base(error, description, fields)
        {
        }

        public ForceAuthException(Error error, string description, string[] fields) : base(error, description, fields)
        {
        }

        public ForceAuthException(Error error, string description) : base(error, description)
        {
        }
    }
}