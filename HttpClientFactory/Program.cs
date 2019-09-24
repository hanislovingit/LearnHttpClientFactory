using System;
using System.Threading.Tasks;
using HttpClientFactory.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace HttpClientFactory
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // create a new ServiceCollection 
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            // create a new ServiceProvider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // For demo purposes: overall catch-all to log any exception that might 
            // happen to the console & wait for key input afterwards so we can easily 
            // inspect the issue.  
            try
            {
                // Run our IntegrationService containing all samples and
                // await this call to ensure the application doesn't 
                // prematurely exit.
                await serviceProvider.GetService<IIntegrationService>().Run();
            }
            catch (Exception generalException)
            {
                // log the exception
                var logger = serviceProvider.GetService<ILogger<Program>>();
                logger.LogError(generalException,
                    "An exception happened while running the integration service.");
            }

            Console.ReadKey();
        }


        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // add loggers
            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole(); // By default the lifetime of the logging service is set to Singleton.
            }).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Information);


            // register the integration service on our container with a 
            // scoped lifetime

            // For the CRUD demos
            //serviceCollection.AddScoped<IIntegrationService, CRUDService>();

            // For the Stream demos
            serviceCollection.AddScoped<IIntegrationService, StreamService>();
        }
    }
}
