using Tasker.Options;
using Tasker.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tasker.TaskerVariables;

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

            try
            {
                switch (options.ObjectType.ToLower())
                {
                    case "task":
                    case "tasks":
                        return await CreateNewTask(options.ObjectName, options.Description).ConfigureAwait(false);

                    case "group":
                    case "groups":
                        return await CreateNewTaskGroup(options.ObjectName).ConfigureAwait(false);

                    case "note":
                    case "notes":
                        return await CreateNote(options.ObjectName, options.Description).ConfigureAwait(false);

                    default:
                        mLogger.LogError("No valid object type given (task, group, note, general)");
                        return 1;
                }
            }
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

        private async Task<int> CreateNewTask(string taskGroupName, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                mLogger.LogError($"{nameof(description)} is null or empty");
                return 1;
            }

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, TaskerUris.WorkTasksUri);

            WorkTaskResource workTaskResource = new WorkTaskResource
            {
                GroupName = taskGroupName,
                Description = description
            };

            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);

            WorkTaskResource workTaskResponse = await SendHttpRequestMessage<WorkTaskResource>(httpRequestMessage).ConfigureAwait(false);

            if (workTaskResponse != null)
                mLogger.LogDebug($"Created new task id {workTaskResponse.TaskId} '{workTaskResponse.Description}' at group {workTaskResponse.GroupName}");

            return 0;
        }

        private async Task<int> CreateNewTaskGroup(string taskGroupName)
        {
            if (string.IsNullOrWhiteSpace(taskGroupName))
            {
                mLogger.LogError($"{nameof(taskGroupName)} is null or empty");
                return 1;
            }

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, TaskerUris.TasksGroupUri);

            TasksGroupResource tasksGroupResource = new TasksGroupResource
            {
                GroupName = taskGroupName
            };

            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(tasksGroupResource), Encoding.UTF8, PostMediaType);

            TasksGroupResource tasksGroupResponse = await SendHttpRequestMessage<TasksGroupResource>(httpRequestMessage).ConfigureAwait(false);

            if(tasksGroupResponse != null)
                mLogger.LogDebug($"Created new group id {tasksGroupResponse.GroupId} '{tasksGroupResponse.GroupName}'");

            return 0;
        }

        private async Task<int> CreateNote(string noteId, string textToWrite)
        {
            if (string.IsNullOrWhiteSpace(noteId))
            {
                mLogger.LogError("No task id given to create note");
                return 1;
            }

            if (textToWrite == null)
                textToWrite = string.Empty;

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, TaskerUris.NotesUri);

            NoteResource noteResource = new NoteResource
            {
                NotePath = noteId,
                Text = textToWrite,
                PossibleNotes = null
            };

            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(noteResource), Encoding.UTF8, PostMediaType);

            NoteResource noteResourceResponse = await SendHttpRequestMessage<NoteResource>(httpRequestMessage).ConfigureAwait(false);

            if (noteResourceResponse != null)
                mLogger.LogDebug($"Created new note {noteResourceResponse.NotePath}");

            return 0;
        }

        private async Task<T> SendHttpRequestMessage<T>(HttpRequestMessage httpRequestMessage)
        {
            using HttpResponseMessage response =
                await mHttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

            mLogger.LogTrace($"Operation {httpRequestMessage.Method.Method} ends with response status: {response.StatusCode}");

            string responseStringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode != System.Net.HttpStatusCode.MethodNotAllowed)
                {
                    throw new InvalidOperationException(
                        $"Could not perform {httpRequestMessage.Method.Method} operation, response status: {response.StatusCode}");
                }

                mLogger.LogDebug($"Could not perform operation, result message: {responseStringContent}");
                return default;
            }

            return JsonConvert.DeserializeObject<T>(responseStringContent);
        }
    }
}