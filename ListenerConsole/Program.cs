using System;
using Microsoft.Extensions.DependencyInjection;
using Rebus.ServiceProvider;
using Rebus.Config;
using Rebus.Bus;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace ListenerConsole
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
            var queueName= configuration["queueName"];

            var serviceProvider = serviceCollection
                .AddRebus((configure, provider) =>
                            configure.Transport(t => t.UseAzureServiceBus(serviceBusConnectionString, queueName))
                          ).AutoRegisterHandlersFromAssembly(Assembly.GetExecutingAssembly())
                .BuildServiceProvider();

            serviceProvider.UseRebus();
        }
    }
}
