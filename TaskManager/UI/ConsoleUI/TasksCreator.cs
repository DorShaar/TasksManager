using Tasker.Options;
using Tasker.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tasker
{
    public class TasksCreator
    {
        private const string PostMediaType = "application/json";

        private readonly HttpClient mHttpClient;
        private readonly ILogger<TasksCreator> mLogger;

        public TasksCreator(HttpClient httpClient, ILogger<TasksCreator> logger)
        {
            mHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> CreateObject(CommandLineOptions.CreateOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task, group, note, general)");
                return 1;
            }

            switch (options.ObjectType.ToLower())
            {
                case "task":
                case "tasks":
                    return await CreateNewTask(options.ObjectName, options.Description).ConfigureAwait(false);

                case "group":
                case "groups":
                    return await CreateNewTaskGroup(options.ObjectName).ConfigureAwait(false);

                // TODO
                //case "note":
                //case "notes":
                //    return await CreateNote(options.ObjectName, options.Description).ConfigureAwait(false);

                //case "general note":
                //case "general":
                //    return await CreateGeneralNote(options.ObjectName, options.Description).ConfigureAwait(false);

                default:
                    mLogger.LogError("No valid object type given (task, group, note, general)");
                    return 1;
            }
        }

        private async Task<int> CreateNewTask(string taskGroupName, string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                mLogger.LogError("Cannot create empty task. Use -d for adding a description");
                return -1;
            }

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, TaskerUris.WorkTasksUri);

            WorkTaskResource workTaskResource = new WorkTaskResource
            {
                GroupName = taskGroupName,
                Description = description
            };

            // TODO send json content.
            using StringContent jsonContent =
                new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);

            await SendHttpRequestMessage(httpRequestMessage).ConfigureAwait(false);

            return 0;
        }

        private async Task<int> CreateNewTaskGroup(string taskGroupName)
        {
            if (string.IsNullOrEmpty(taskGroupName))
            {
                mLogger.LogError($"{nameof(taskGroupName)} is null or empty");
                return 1;
            }

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, TaskerUris.TasksGroupUri);

            TasksGroupResource tasksGroupResource = new TasksGroupResource
            {
                GroupName = taskGroupName
            };

            // TODO send json content.
            using StringContent jsonContent =
                new StringContent(JsonConvert.SerializeObject(tasksGroupResource), Encoding.UTF8, PostMediaType);

            await SendHttpRequestMessage(httpRequestMessage).ConfigureAwait(false);
            return 0;
        }

        // TODO
        //private async Task<int> CreateNote(string taskId, string textToWrite)
        //{
        //    if (string.IsNullOrEmpty(taskId))
        //    {
        //        mLogger.LogError("No task id given to create note");
        //        return 1;
        //    }

        //    if (textToWrite == null)
        //        textToWrite = string.Empty;

        //    mTaskManager.CreateTaskNote(taskId, textToWrite);
        //    return 0;
        //}

        //private static async Task<int> CreateGeneralNote(string noteSubject, string textToWrite)
        //{
        //    if (string.IsNullOrEmpty(noteSubject))
        //    {
        //        mLogger.LogError("No task subject given to create note");
        //        return 1;
        //    }

        //    if (textToWrite == null)
        //        textToWrite = string.Empty;

        //    mTaskManager.CreateGeneralNote(noteSubject, textToWrite);
        //    return 0;
        //}

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