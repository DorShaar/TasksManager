using Microsoft.Extensions.DependencyInjection;
using TaskData.IDsProducer;
using TaskData.Notes;
using TaskData.TasksGroups;
using TaskData.TaskStatus;

namespace TaskData
{
    public static class TaskerDataExtensions
    {
        public static void UseTaskerDataEntities(this IServiceCollection services)
        {
            services.AddSingleton<IIDProducer, IDProducer>();
            services.AddSingleton<INoteFactory, NoteFactory>();
            services.AddSingleton<ITasksGroupFactory, TaskGroupFactory>();
            services.AddSingleton<ITaskStatusHistory, TaskStatusHistory>();
        }
    }
}