using Database.Configuration;
using Database.Contracts;
using Database;
using Logger;
using Logger.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using TaskData;
using TaskData.Contracts;
using TaskManager.Contracts;
using ObjectSerializer.Contracts;
using Database.JsonService;
using Microsoft.Extensions.Configuration;
using UI.ConsolePrinter;

namespace Composition
{
    public class TaskManagerServiceProvider
    {
        private readonly IServiceProvider mServiceProvider;

        public TaskManagerServiceProvider()
        {
            mServiceProvider = CreateServiceProvider();
        }

        private IServiceProvider CreateServiceProvider()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ILogger, ConsoleLogger>();

            serviceCollection.AddSingleton<IObjectSerializer, JsonSerializerWrapper>();

            RegisterTaskDataEntities(serviceCollection);

            RegisterDatabaseEntities(serviceCollection);

            serviceCollection.AddSingleton<ITaskManager, TaskManager.TaskManager>();

            serviceCollection.AddSingleton(typeof(ConsolePrinter));

            return serviceCollection.BuildServiceProvider();
        }

        private void RegisterTaskDataEntities(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<INoteBuilder, NoteBuilder>();
            serviceCollection.AddSingleton<INotesSubjectBuilder, NotesSubjectBuilder>();
            serviceCollection.AddSingleton<IWorkTask, WorkTask>();
            serviceCollection.AddSingleton<ITasksGroup, TaskGroup>();
            serviceCollection.AddSingleton<ITasksGroupBuilder, TaskGroupBuilder>();
        }

        private void RegisterDatabaseEntities(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILocalRepository<ITasksGroup>, Database.Database>();

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            // Adds YAML settings later
            configurationBuilder.AddYamlFile(@"config\Config.yaml", optional: false);

            IConfiguration configuration = configurationBuilder.Build();

            // Binds between IConfiguration to DatabaseLocalConfigurtaion.
            serviceCollection.Configure<DatabaseLocalConfigurtaion>(configuration);
            serviceCollection.AddOptions();
        }

        public ITaskManager GetTaskManagerService()
        {
            return mServiceProvider.GetService<ITaskManager>();
        }

        public ILogger GetLoggerService()
        {
            return mServiceProvider.GetService<ILogger>();
        }

        public ConsolePrinter GetConsolePrinterService()
        {
            return mServiceProvider.GetService<ConsolePrinter>();
        }
    }
}