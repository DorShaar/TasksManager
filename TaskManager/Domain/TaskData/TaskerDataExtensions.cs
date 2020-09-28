using Microsoft.Extensions.DependencyInjection;
using TaskData.IDsProducer;
using TaskData.Notes;
using TaskData.TasksGroups;
using TaskData.TaskStatus;

namespace TaskData
{
    public static class TaskerDataExtensions
    {
        public static IServiceCollection UseTaskerDataEntities(this IServiceCollection services)
        {
            services.AddSingleton<IIDProducer, IDProducer>()
                    .AddSingleton<INoteFactory, NoteFactory>()
                    .AddSingleton<ITasksGroupFactory, TaskGroupFactory>()
                    .AddSingleton<ITaskStatusHistory, TaskStatusHistory>();

            return services;
        }
    }
}