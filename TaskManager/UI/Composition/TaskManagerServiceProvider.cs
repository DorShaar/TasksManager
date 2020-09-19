using Database.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using UI.ConsolePrinter;
using ObjectSerializer.JsonService;
using TaskData.Notes;
using TaskData.TasksGroups;
using Databases;
using TaskManagers;
using TaskData.IDsProducer;
using TaskData.TaskStatus;

namespace Composition
{
    public class TaskManagerServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider mServiceProvider;

        public TaskManagerServiceProvider()
        {
            mServiceProvider = CreateServiceProvider();
        }

        private IServiceProvider CreateServiceProvider()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IObjectSerializer, JsonSerializerWrapper>();

            serviceCollection.AddSingleton<ITaskManager, TaskManager>();

            serviceCollection.AddSingleton(typeof(ConsolePrinter));

            RegisterTaskDataEntities(serviceCollection);

            RegisterDatabaseEntities(serviceCollection);

            RegisterLogger(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private void RegisterTaskDataEntities(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IIDProducer, IDProducer>();
            serviceCollection.AddSingleton<INoteFactory, NoteFactory>();
            serviceCollection.AddSingleton<ITasksGroupFactory, TaskGroupFactory>();
            serviceCollection.AddSingleton<ITaskStatusHistory, TaskStatusHistory>();
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
    }
}