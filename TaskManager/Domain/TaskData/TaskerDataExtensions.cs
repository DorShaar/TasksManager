using Microsoft.Extensions.DependencyInjection;
using TaskData.IDsProducer;
using TaskData.Notes;
using TaskData.TasksGroups;
using TaskData.TaskStatus;
using TaskData.WorkTasks;

namespace TaskData
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
    }
}