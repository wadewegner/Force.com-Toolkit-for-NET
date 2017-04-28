using System;
using System.Net;
﻿using Salesforce.Common.Models.Json;

namespace Salesforce.Common
{
    public class ForceAuthException : Exception
    {
        public string[] Fields { get; private set; }
        public HttpStatusCode HttpStatusCode { get; private set; }
        public Error Error { get; private set; }

        public ForceAuthException(string error, string description)
            : this(ParseError(error), description)
        {
        }

        public ForceAuthException(string error, string description, string[] fields) 
            : this(error, description)
        {
            Fields = fields;
        }

        public ForceAuthException(Error error, string description, string[] fields) 
            : this(error, description)
        {
            Fields = fields;
        }

        public ForceAuthException(string error, string description, HttpStatusCode httpStatusCode)
            : this(ParseError(error), description)
        {
            this.HttpStatusCode = httpStatusCode;
        }

        public ForceAuthException(Error error, string description) 
            : base(description)
        {
            Error = error;
            Fields = new string[0];
            HttpStatusCode = new HttpStatusCode();
        }

        private static Error ParseError(string error)
        {
            Error value;
            return Enum.TryParse(error.Replace("_", ""), true, out value) ? value : Error.Unknown;
        }
    }
}