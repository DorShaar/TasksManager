using Tasker.Options;
using Tasker.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Tasker.TaskerVariables;

namespace Tasker
{
    public class TasksChanger
    {
        private const string PostMediaType = "application/json";

        private readonly HttpClient mHttpClient;
        private readonly ILogger<TasksChanger> mLogger;

        public TasksChanger(HttpClient httpClient, ILogger<TasksChanger> logger)
        {
            mHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> CloseTask(CommandLineOptions.CloseOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                case "tasks":
                    return await CloseTask(options.ObjectId, options.Reason).ConfigureAwait(false);

                default:
                    mLogger.LogError("No valid object type given (task)");
                    return 1;
            }
        }

        private async Task<int> CloseTask(string taskId, string reason)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given");
                return 1;
            }

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, TaskerUris.WorkTasksUri);

            WorkTaskResource workTaskResource = new WorkTaskResource
            {
                TaskId = taskId,
                Status = "closed", // TODO const.
                Reason = reason
            };

            // TODO send json content.
            using StringContent jsonContent =
                new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);

            await SendHttpRequestMessage(httpRequestMessage).ConfigureAwait(false);

            return 0;
        }

        public async Task<int> MoveTask(CommandLineOptions.MoveTaskOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                    return await MoveTask(options.TaskId, options.TaskGroup).ConfigureAwait(false);

                default:
                    mLogger.LogError("No valid object type given (task)");
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

            // TODO send json content.
            using StringContent jsonContent =
                new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, TaskerUris.WorkTasksUri);

            await SendHttpRequestMessage(httpRequestMessage).ConfigureAwait(false);

            return 0;
        }

        public async Task<int> ReOpenTask(CommandLineOptions.ReOpenTaskOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                    return await ReOpenTask(options.TaskId, options.Reason).ConfigureAwait(false);

                default:
                    mLogger.LogError("No valid object type given (task)");
                    return 1;
            }
        }

        private async Task<int> ReOpenTask(string taskId, string reason)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given");
                return 1;
            }

            WorkTaskResource workTaskResource = new WorkTaskResource
            {
                TaskId = taskId,
                Status = "open", // TODO const
                Reason = reason
            };

            // TODO send json content.
            using StringContent jsonContent =
                new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, TaskerUris.WorkTasksUri);

            await SendHttpRequestMessage(httpRequestMessage).ConfigureAwait(false);

            return 0;
        }

        public async Task<int> MarkTaskAsOnWork(CommandLineOptions.OnWorkTaskOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                    return await MarkTaskAsOnWork(options.TaskId, options.Reason).ConfigureAwait(false);

                default:
                    mLogger.LogError("No valid object type given (task)");
                    return 1;
            }
        }

        private async Task<int> MarkTaskAsOnWork(string taskId, string reason)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                mLogger.LogError("No task id given");
                return 1;
            }

            WorkTaskResource workTaskResource = new WorkTaskResource
            {
                TaskId = taskId,
                Status = "on-work", // TODO const
                Reason = reason
            };

            // TODO send json content.
            using StringContent jsonContent =
                new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, TaskerUris.WorkTasksUri);

            await SendHttpRequestMessage(httpRequestMessage).ConfigureAwait(false);

            return 0;
        }

        // TODO think if we can share that method.
        private async Task SendHttpRequestMessage(HttpRequestMessage httpRequestMessage)
        {
            using HttpResponseMessage response =
                await mHttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                // TODO make more safe with getting string.
                throw new InvalidOperationException(
                    $"Could not perform {httpRequestMessage.Method.Method} operation, response status: {response.StatusCode}," +
                    $"response body {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            }
        }
    }
}