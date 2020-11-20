using Tasker.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tasker.TaskerVariables;

namespace Tasker
{
    public class TasksRemover
    {
        private readonly HttpClient mHttpClient;
        private readonly ILogger<TasksRemover> mLogger;

        public TasksRemover(HttpClient httpClient, ILogger<TasksRemover> logger)
        {
            mHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
            // TODO this in every TasksX + handle specific error for case where server is not available.
            catch (HttpRequestException ex)
            {
                mLogger.LogError(ex, $"Could not connect to server at {mHttpClient.BaseAddress}, please check server is up");
                return 1;
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Operation failed");
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

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, TaskerUris.WorkTasksUri);

            await SendHttpRequestMessage(httpRequestMessage).ConfigureAwait(false);

            return 0;
        }

        private async Task<int> RemoveTaskGroup(string taskGroup)
        {
            if (string.IsNullOrEmpty(taskGroup))
            {
                mLogger.LogError("No group name or group id given");
                return 1;
            }

            // TODO check if group is not empty - what happens in server?

            //mLogger.LogDebug("Are you sure you want to delete that group with all of its inner tasks? If so, press y");
            //string userInput = Console.ReadLine();
            //if (!string.Equals(userInput, "y", StringComparison.OrdinalIgnoreCase))
            //{
            //    mLogger.LogDebug($"Group {taskGroup} was not deleted");
            //    return 0;
            //}

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, TaskerUris.TasksGroupUri);

            await SendHttpRequestMessage(httpRequestMessage).ConfigureAwait(false);

            return 0;
        }

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