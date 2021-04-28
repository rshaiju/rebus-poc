using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.ServiceProvider;
using System;
using System.IO;
using System.Reflection;

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


        private static void ConfigureServices(IServiceCollection serviceCollection)
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
                          ).AutoRegisterHandlersFromAssembly(Assembly.GetExecutingAssembly())
                .BuildServiceProvider();

            serviceProvider.UseRebus();
        }
    }
}
