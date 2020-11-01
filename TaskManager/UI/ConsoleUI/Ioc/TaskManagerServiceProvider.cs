using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using UI.ConsolePrinter;
using ObjectSerializer.JsonService;
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

            serviceCollection.AddSingleton(typeof(ConsolePrinter));

            serviceCollection.UseJsonObjectSerializer();

            serviceCollection.UseTaskerDataEntities();

            RegisterLogger(serviceCollection);

            return serviceCollection.BuildServiceProvider();
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