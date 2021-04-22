using System;
using Microsoft.Extensions.DependencyInjection;
using Rebus.ServiceProvider;
using Rebus.Config;
using Rebus.Bus;
using System.Threading.Tasks;
using Rebus.Routing.TypeBased;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SenderConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false, true)
               .AddEnvironmentVariables().Build();

            var serviceBusConnectionString = configuration["sb-connectionstring"];
            var queueName = configuration["queueName"];

            var serviceProvider = new ServiceCollection().AddRebus((configure, provider) => 
            configure.Transport(t => t.UseAzureServiceBusAsOneWayClient(serviceBusConnectionString))
            .Routing(r=>r.TypeBased().Map<string>(queueName))
            ).BuildServiceProvider();

            var bus = serviceProvider.GetRequiredService<IBus>();

            Console.WriteLine("Enter the number of messages:");
            int messageCount = 0;
            if(int.TryParse(Console.ReadLine(),out messageCount))
            {
                for(int i=0;i<messageCount;i++)
                {
                    await bus.Send("Hello world");
                }

                Console.WriteLine($"{messageCount} messages sent");
            }

            Console.WriteLine("Finished..");
            Console.ReadKey();
        }
    }
}
