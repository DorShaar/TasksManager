using Database.Contracts;
using Database.JsonService;
using Logger;
using Logger.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using TaskData.Contracts;

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

               return serviceCollection.BuildServiceProvider();
          }

          public object GetService(Type serviceType)
          {
               return mServiceProvider.GetService(serviceType);
          }
     }
}