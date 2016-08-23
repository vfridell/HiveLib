using Noobot.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBot
{
    class HivebotConfiguration : ConfigurationBase
    {
        public HivebotConfiguration()
        {
            UseMiddleware<HiveGameMiddleware>();
            UsePlugin<HiveGamePlugin>();
        }

    }
}
