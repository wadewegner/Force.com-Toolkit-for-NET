using Salesforce.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesforce.Common.Models;

namespace Salesforce.Force.FunctionalTests.Models
{
    public class AtrributedAccount : Account, IAttributedObject
    {
        public ObjectAttributes Attributes { get; set; }

        public AtrributedAccount()
        {
            Attributes = new ObjectAttributes()
            {
                Type = "Account"
            };

        }


    }
}
