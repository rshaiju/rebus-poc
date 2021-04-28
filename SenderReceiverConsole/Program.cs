using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SenderReceiverConsole
{
    class Program
    {
        private static void Main(string[] args)
        {
            var host = new HostBuilder()
                 .ConfigureServices((hostContext, services) =>
                 {
                     ConfigureServices(services);
                 }).Build();

            host.RunAsync().Wait();
        }


        private static async Task ConfigureServices(IServiceCollection serviceCollection)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables().Build();

            var serviceBusConnectionString = configuration["sb-connectionstring"];
            var queueName = configuration["queueName"];

            var serviceProvider = serviceCollection
                .AddRebus((configure, provider) =>
                            configure.Transport(t => t.UseAzureServiceBus(serviceBusConnectionString, queueName))
                            .Routing(r => r.TypeBased().Map<string>(queueName))
                          ).AutoRegisterHandlersFromAssembly(Assembly.GetExecutingAssembly())
                .BuildServiceProvider();

            var bus = serviceProvider.GetRequiredService<IBus>();

            Console.WriteLine("Let's send out some messages first so that we can receive");
            Console.WriteLine("Enter the number of messages:");
            int messageCount = 0;
            if (int.TryParse(Console.ReadLine(), out messageCount))
            {
                for (int i = 0; i < messageCount; i++)
                {
                    await bus.Send("Hello world");
                }

                Console.WriteLine($"{messageCount} messages sent");
            }

            serviceProvider.UseRebus();
        }
    }
}
