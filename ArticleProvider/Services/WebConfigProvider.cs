using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ArticleProvider.Services
{
    public class WebConfigProvider : IConfigurationProvider
    {
        public int GetMaximumLikesPerDay()
        {
            try
            {
                return Int32.Parse(WebConfigurationManager.AppSettings["MaximumLikesInADayPerUser"]);
            }
            catch
            {
                return 0;
            }
        }
    }
}