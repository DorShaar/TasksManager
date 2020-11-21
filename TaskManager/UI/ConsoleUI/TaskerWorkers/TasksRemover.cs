using Tasker.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tasker.TaskerVariables;
using Tasker.Resources;
using Tasker.Communication;
using Tasker.Extensions;

namespace Tasker.TaskerWorkers
{
    public class TasksRemover
    {
        private readonly HttpClient mHttpClient;
        private readonly ILogger<TasksRemover> mLogger;

        public TasksRemover(IHttpClientFactory httpClientFactory, ILogger<TasksRemover> logger)
        {
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));

            mHttpClient = httpClientFactory.CreateClient(TaskerConsts.HttpClientName);
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> RemoveObject(CommandLineOptions.RemoveOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task, group)");
                return 1;
            }

            try
            {
                switch (options.ObjectType.ToLower())
                {
                    case "task":
                    case "tasks":
                        return await RemoveTask(options.ObjectId).ConfigureAwait(false);

                    case "group":
                    case "groups":
                        return await RemoveTaskGroup(options.ObjectId).ConfigureAwait(false);

                    default:
                        mLogger.LogError("No valid object type given (task, group)");
                        return 1;
                }
            }
            catch (HttpRequestException ex)
            {
                mLogger.LogCritical(ex, $"Could not connect to server at {mHttpClient.BaseAddress}, please check server is up");
                return 1;
            }
            catch (Exception ex)
            {
                mLogger.LogCritical(ex, "Operation failed");
                return 1;
            }
        }

        private async Task<int> RemoveTask(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given to remove");
                return 1;
            }

            Uri taskUri = TaskerUris.WorkTasksUri.CombineRelative(taskId);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, taskUri);

            WorkTaskResource workTaskResource = await HttpMessageRequester.SendHttpRequestMessage<WorkTaskResource>(
                mHttpClient, httpRequestMessage, mLogger).ConfigureAwait(false);

            if (workTaskResource != null)
                mLogger.LogInformation($"Delete task id {workTaskResource.TaskId}. Description: {workTaskResource.Description}");

            return 0;
        }

        private async Task<int> RemoveTaskGroup(string taskGroup)
        {
            if (string.IsNullOrEmpty(taskGroup))
            {
                mLogger.LogError("No group name or group id given");
                return 1;
            }

            Uri groupUri = TaskerUris.TasksGroupUri.CombineRelative(taskGroup);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, groupUri);

            TasksGroupResource tasksGroupResource = await HttpMessageRequester.SendHttpRequestMessage<TasksGroupResource>(
                mHttpClient, httpRequestMessage, mLogger).ConfigureAwait(false);

            if (tasksGroupResource != null)
                mLogger.LogInformation($"Deleted group id {tasksGroupResource.GroupId} {tasksGroupResource.GroupName}");

            return 0;
        }
    }
}