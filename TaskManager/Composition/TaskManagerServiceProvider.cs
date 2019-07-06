using Database.Configuration;
using Database.Contracts;
using Database.JsonService;
using Logger;
using Logger.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using TaskData;
using TaskData.Contracts;
using TaskManager.Contracts;

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

               RegisterTaskDataEntities(serviceCollection);

               RegisterDatabaseEntities(serviceCollection);

               // Register TaskManager service.
               serviceCollection.AddSingleton<ITaskManager, TaskManager.TaskManager>();

               return serviceCollection.BuildServiceProvider();
          }

          private void RegisterTaskDataEntities(ServiceCollection serviceCollection)
          {
               serviceCollection.AddSingleton<ITask, Task>();
               serviceCollection.AddSingleton<ITaskGroup, TaskGroup>();
          }

          private void RegisterDatabaseEntities(ServiceCollection serviceCollection)
          {
               serviceCollection.AddSingleton<IRepository<ITaskGroup>, Database<ITaskGroup>>();
               serviceCollection.AddSingleton<IConfiguration, Configuration>();
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