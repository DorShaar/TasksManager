using Database.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using UI.ConsolePrinter;
using ObjectSerializer.JsonService;
using TaskData.TasksGroups;
using Databases;
using TaskManagers;
using TaskData;

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

            serviceCollection.AddSingleton<ITaskManager, TaskManager>();

            serviceCollection.AddSingleton(typeof(ConsolePrinter));

            serviceCollection.UseJsonObjectSerializer();

            serviceCollection.UseTaskerDataEntities();

            RegisterDatabaseEntities(serviceCollection);

            RegisterLogger(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private void RegisterDatabaseEntities(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILocalRepository<ITasksGroup>, Databases.Database>();

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            // Adds YAML settings later
            configurationBuilder.AddYamlFile(@"config\Config.yaml", optional: false);

            IConfiguration configuration = configurationBuilder.Build();

            // Binds between IConfiguration to DatabaseLocalConfigurtaion.
            serviceCollection.Configure<DatabaseLocalConfigurtaion>(configuration);
            serviceCollection.AddOptions();
        }

        private void RegisterLogger(ServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
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