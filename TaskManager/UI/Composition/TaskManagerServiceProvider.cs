﻿using Database.Configuration;
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
using System.IO;

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

            // Register logger.
            serviceCollection.AddSingleton<ILogger, ConsoleLogger>();

            // Register object serializer.
            serviceCollection.AddSingleton<IObjectSerializer, JsonSerializerWrapper>();

            RegisterTaskDataEntities(serviceCollection);

            RegisterDatabaseEntities(serviceCollection);

            // Register TaskManager service.
            serviceCollection.AddSingleton<ITaskManager, TaskManager.TaskManager>();

            return serviceCollection.BuildServiceProvider();
        }

        private void RegisterTaskDataEntities(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<INoteBuilder, NoteBuilder>();
            serviceCollection.AddSingleton<INotesSubjectBuilder, NotesSubjectBuilder>();
            serviceCollection.AddSingleton<ITask, Task>();
            serviceCollection.AddSingleton<ITaskGroup, TaskGroup>();
            serviceCollection.AddSingleton<ITaskGroupBuilder, TaskGroupBuilder>();
        }

        private void RegisterDatabaseEntities(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILocalRepository<ITaskGroup>, Database<ITaskGroup>>();

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
    }
}