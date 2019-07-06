using Database.Contracts;
using Database.JsonService;
using Logger;
using Logger.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using TaskData.Contracts;
using TaskManager;
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

               // Register database.
               serviceCollection.AddSingleton<IRepository<ITaskGroup>, Database<ITaskGroup>>();

               // Register TaskManager service.
               serviceCollection.AddSingleton<ITaskManager, TaskManager.TaskManager>();

               return serviceCollection.BuildServiceProvider();
          }

          public ITaskManager GetTaskManagerService()
          {
               return mServiceProvider.GetService<ITaskManager>();
          }
     }
}