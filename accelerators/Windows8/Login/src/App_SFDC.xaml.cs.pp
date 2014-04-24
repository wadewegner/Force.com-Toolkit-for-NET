using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace $rootnamespace$
{
    partial class App : Application
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string InstanceUrl { get; set; }
    }
}
