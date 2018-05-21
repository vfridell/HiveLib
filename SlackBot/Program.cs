using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noobot.Core.Configuration;
using Topshelf;

namespace SlackBot
{
    class Program
    {
        private static readonly IConfigReader ConfigReader = new ConfigReader();

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<NoobotHost>(s =>
                {
                    s.ConstructUsing(name => new NoobotHost(ConfigReader));

                    s.WhenStarted(n =>
                    {
                        n.Start();
                    });

                    s.WhenStopped(n => n.Stop());
                });

                x.RunAsNetworkService();
                x.SetDisplayName("Noobot");
                x.SetServiceName("Noobot");
                x.SetDescription("An extensible Slackbot built in C#");
            });
        }
    }
}
