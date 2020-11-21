using Tasker.Options;
using Tasker.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Tasker.TaskerVariables;
using Tasker.Communication;
using Tasker.Extensions;

namespace Tasker.TaskerWorkers
{
    public class TasksChanger
    {
        private const string PostMediaType = "application/json";

        private readonly HttpClient mHttpClient;
        private readonly ILogger<TasksChanger> mLogger;

        public TasksChanger(IHttpClientFactory httpClientFactory, ILogger<TasksChanger> logger)
        {
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));

            mHttpClient = httpClientFactory.CreateClient(TaskerConsts.HttpClientName);
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> CloseTask(CommandLineOptions.CloseOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            try
            {
                switch (options.ObjectType.ToLower())
                {
                    case "task":
                    case "tasks":
                        return await ChangeTaskStatus(options.ObjectId, options.Reason, TaskerConsts.ClosedTaskStatus).ConfigureAwait(false);

                    default:
                        mLogger.LogError("No valid object type given (task)");
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

        public async Task<int> ReOpenTask(CommandLineOptions.ReOpenTaskOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            try
            {
                switch (options.ObjectType.ToLower())
                {
                    case "task":
                        return await ChangeTaskStatus(options.ObjectId, options.Reason, TaskerConsts.OpenTaskStatus).ConfigureAwait(false);

                    default:
                        mLogger.LogError("No valid object type given (task)");
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

        public async Task<int> MarkTaskAsOnWork(CommandLineOptions.OnWorkTaskOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            try
            {
                switch (options.ObjectType.ToLower())
                {
                    case "task":
                        return await ChangeTaskStatus(options.ObjectId, options.Reason, TaskerConsts.OnWorkTaskStatus).ConfigureAwait(false);

                    default:
                        mLogger.LogError("No valid object type given (task)");
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

        private async Task<int> ChangeTaskStatus(string taskId, string reason, string newStatus)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given");
                return 1;
            }

            WorkTaskResource workTaskResource = new WorkTaskResource
            {
                TaskId = taskId,
                Status = newStatus,
                Reason = reason
            };

            Uri taskUri = TaskerUris.WorkTasksUri.CombineRelative(taskId);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, taskUri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType)
            };

            WorkTaskResource workTaskResourceResponse = await HttpMessageRequester.SendHttpRequestMessage<WorkTaskResource>(
                mHttpClient, httpRequestMessage, mLogger).ConfigureAwait(false);

            if (workTaskResourceResponse != null)
            {
                mLogger.LogInformation($"Changed status of task {workTaskResourceResponse.TaskId} " +
                    $"{workTaskResourceResponse.Description} to {newStatus}");
            }

            return 0;
        }

        public async Task<int> MoveTask(CommandLineOptions.MoveTaskOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            try
            {
                switch (options.ObjectType.ToLower())
                {
                    case "task":
                        return await MoveTask(options.ObjectId, options.TaskGroup).ConfigureAwait(false);

                    default:
                        mLogger.LogError("No valid object type given (task)");
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

        private async Task<int> MoveTask(string taskId, string taskGroup)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given to move");
                return 1;
            }

            if (string.IsNullOrEmpty(taskGroup))
            {
                mLogger.LogError("No group name or group id given");
                return 1;
            }

            WorkTaskResource workTaskResource = new WorkTaskResource
            {
                TaskId = taskId,
                GroupName = taskGroup
            };

            Uri taskUri = TaskerUris.WorkTasksUri.CombineRelative(taskId);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, taskUri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType)
            };

            WorkTaskResource workTaskResourceResponse = await HttpMessageRequester.SendHttpRequestMessage<WorkTaskResource>(
                mHttpClient, httpRequestMessage, mLogger).ConfigureAwait(false);

            if (workTaskResourceResponse != null)
            {
                mLogger.LogInformation($"Moved task {workTaskResourceResponse.TaskId} {workTaskResourceResponse.Description}" +
                    $"to group {taskGroup}");
            }

            return 0;
        }
    }
}