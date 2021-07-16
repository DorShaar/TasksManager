using Microsoft.Extensions.DependencyInjection;
using TaskData.IDsProducer;
using TaskData.TasksGroups;
using TaskData.TasksGroups.Producers;
using TaskData.TaskStatus;
using TaskData.WorkTasks.Producers;

namespace TaskData.Ioc
{
    public static class TaskerDataExtensions
    {
        public static IServiceCollection UseTaskerDataEntities(this IServiceCollection services)
        {
            services.AddSingleton<IIDProducer, IDProducer>()
                    .AddSingleton<ITasksGroupFactory, TaskGroupFactory>()
                    .AddSingleton<ITaskStatusHistory, TaskStatusHistory>();

            return services;
        }

        public static IServiceCollection RegisterRegularWorkTaskProducer(this IServiceCollection services)
        {
            services.AddSingleton<IWorkTaskProducer, WorkTaskProducer>();

            return services;
        }

        public static IServiceCollection RegisterRegularTasksGroupProducer(this IServiceCollection services)
        {
            services.AddSingleton<ITasksGroupProducer, TasksGroupProducer>();

            return services;
        }
    }
}