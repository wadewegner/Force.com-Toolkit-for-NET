using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Models
{
    public class AuthTokenWithRefresh : AuthToken
    {
        public string refresh_token;
    }
}
