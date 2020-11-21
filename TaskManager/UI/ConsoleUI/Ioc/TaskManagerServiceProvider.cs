using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Tasker.Options;
using Tasker.TaskerVariables;
using Tasker.TaskerWorkers;
using UI.ConsolePrinter;

namespace Composition
{
    public class TaskManagerServiceProvider : ITaskManagerServiceProvider
    {
        private readonly ServiceProvider mServiceProvider;

        public TaskManagerServiceProvider()
        {
            mServiceProvider = CreateServiceProvider();
        }

        private ServiceProvider CreateServiceProvider()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(typeof(ConsolePrinter));

            RegisterWorkers(serviceCollection);

            RegisterLogger(serviceCollection);

            AddConfiguration(serviceCollection);

            serviceCollection.AddHttpClient(TaskerConsts.HttpClientName, (serviceProvider, client) =>
            {
                IOptionsMonitor<TaskerConfiguration> configuration =
                    serviceProvider.GetRequiredService<IOptionsMonitor<TaskerConfiguration>>();

                client.BaseAddress = new Uri(configuration.CurrentValue.ServerAddress);
            });

            return serviceCollection.BuildServiceProvider();
        }

        private void RegisterWorkers(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(TasksProvider));
            serviceCollection.AddSingleton(typeof(TasksCreator));
            serviceCollection.AddSingleton(typeof(TasksRemover));
            serviceCollection.AddSingleton(typeof(TasksChanger));
            serviceCollection.AddSingleton(typeof(NotesOpener));
        }

        private void RegisterLogger(ServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        }

        private void AddConfiguration(IServiceCollection services)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            const string configFileName = "TaskerConfiguration.yaml";
            configurationBuilder.AddYamlFile(Path.Combine("Options", configFileName), optional: false);

            IConfiguration configuration = configurationBuilder.Build();

            // Binds between IConfiguration to given configurtaion.
            services.Configure<TaskerConfiguration>(configuration);
            services.AddOptions();
        }

        public object GetService(Type serviceType)
        {
            return mServiceProvider.GetRequiredService(serviceType);
        }

        public void Dispose()
        {
            mServiceProvider.Dispose();
        }
    }
}