using Tasker.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UI.ConsolePrinter;
using Tasker.Resources;
using Newtonsoft.Json;
using Tasker.Extensions;
using Tasker.TaskerVariables;

namespace Tasker
{
    public class TasksProvider
    {
        private readonly HttpClient mHttpClient;
        private readonly ConsolePrinter mConsolePrinter;
        private readonly ILogger<TasksProvider> mLogger;

        public TasksProvider(HttpClient httpClient, ConsolePrinter consolePrinter,
            ILogger<TasksProvider> logger)
        {
            mHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            mConsolePrinter = consolePrinter ?? throw new ArgumentNullException(nameof(consolePrinter));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> GetObject(CommandLineOptions.GetOptions options)
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
                        return await GetAllTasks(
                            options.ObjectName,
                            options.Status,
                            options.ShouldPrintAll,
                            options.IsDetailed).ConfigureAwait(false);

                    case "group":
                    case "groups":
                        return await GatAllTaskGroup(
                            options.ShouldPrintAll,
                            options.IsDetailed).ConfigureAwait(false);

                    case "note":
                    case "notes":
                        return await GetNoteContent(
                            options.ObjectName,
                            options.ShouldPrintAll).ConfigureAwait(false);

                    case "general":
                        return await GetGeneralNoteContent(
                            options.ObjectName).ConfigureAwait(false);

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

        /// <summary>
        /// Get all un-closed tasks.
        /// In case user choose to print all option, all tasks will be printed.
        /// </summary>
        private async Task<int> GetAllTasks(string taskGroup, string status, bool shouldPrintAll, bool isDetailed)
        {
            IEnumerable<WorkTaskResource> tasksToPrint = await GetAllTasksOrTasksByGroupName(taskGroup).ConfigureAwait(false);

            if (tasksToPrint == null)
            {
                mLogger.LogError($"No task group {taskGroup} exist");
                return 1;
            }

            tasksToPrint = GetTasksToPrint(tasksToPrint, status, shouldPrintAll);

            mConsolePrinter.PrintTasks(tasksToPrint, isDetailed);
            return 0;
        }

        private IEnumerable<WorkTaskResource> GetTasksToPrint(IEnumerable<WorkTaskResource> tasksToPrint, string status, bool shouldPrintAll)
        {
            if (!string.IsNullOrEmpty(status))
                return tasksToPrint.Where(task => task.Status.Equals(status, StringComparison.InvariantCultureIgnoreCase));
            else if (!shouldPrintAll)
                return tasksToPrint.Where(task => !task.Status.Equals(TaskerConsts.ClosedTaskStatus, StringComparison.InvariantCultureIgnoreCase));

            return tasksToPrint;
        }

        private async Task<IEnumerable<WorkTaskResource>> GetAllTasksOrTasksByGroupName(string taskGroup)
        {
            if (string.IsNullOrEmpty(taskGroup))
                return await GetAllTasks().ConfigureAwait(false);
            else
                return await GetTasksByGroup(taskGroup).ConfigureAwait(false);
        }

        private async Task<IEnumerable<WorkTaskResource>> GetAllTasks()
        {
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, TaskerUris.WorkTasksUri);

            return await SendHttpRequestMessage<IEnumerable<WorkTaskResource>>(httpRequestMessage).ConfigureAwait(false);
        }

        private async Task<IEnumerable<WorkTaskResource>> GetTasksByGroup(string taskGroup)
        {
            if (string.IsNullOrEmpty(taskGroup))
                throw new ArgumentException($"{nameof(taskGroup)} is null or empty");

            Uri tasksByGroupUri = TaskerUris.WorkTasksUri.CombineRelative(taskGroup);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, tasksByGroupUri);

            return await SendHttpRequestMessage<IEnumerable<WorkTaskResource>>(httpRequestMessage).ConfigureAwait(false);
        }

        private async Task<int> GatAllTaskGroup(bool shouldPrintAll, bool isDetailed)
        {
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, TaskerUris.TasksGroupUri);

            IEnumerable<TasksGroupResource> groupsToPrint =
                await SendHttpRequestMessage<IEnumerable<TasksGroupResource>>(httpRequestMessage).ConfigureAwait(false);

            if (!shouldPrintAll)
            {
                groupsToPrint = groupsToPrint
                    .Where((TasksGroupResource group) => !group.Status.Equals(TaskerConsts.ClosedTaskStatus, StringComparison.InvariantCultureIgnoreCase));
            }

            mConsolePrinter.PrintTasksGroup(groupsToPrint, isDetailed);
            return 0;
        }

        private async Task<int> GetNoteContent(string notePath, bool shouldPrintAll)
        {
            if (shouldPrintAll)
            {
                NoteNodeResource noteNodeResource = await GetAllNotesNames().ConfigureAwait(false);
                PrintAllNotesNames(noteNodeResource);
                return 0;
            }

            await PrintNoteText(notePath, isPrivateNote: true).ConfigureAwait(false);
            return 0;
        }

        private void PrintAllNotesNames(NoteNodeResource noteNodeResource)
        {
            IEnumerable<string> notesToPrint = noteNodeResource.Children.Keys
                .Where(noteName => Path.GetExtension(noteName).Equals(TaskerConsts.NoteExtension))
                .Select(noteName => Path.GetFileNameWithoutExtension(noteName));

            mConsolePrinter.Print(notesToPrint, header: "NOTES");
        }

        private async Task<int> GetGeneralNoteContent(string noteName)
        {
            await PrintNoteText(noteName, isPrivateNote: false).ConfigureAwait(false);
            return 0;
        }

        private async Task PrintNoteText(string noteName, bool isPrivateNote)
        {
            string escapedNoteName = Uri.EscapeDataString(noteName);

            string relativeNoteUri = isPrivateNote ? $"note/{escapedNoteName}" : escapedNoteName;

            Uri noteUri = TaskerUris.NotesUri.CombineRelative(relativeNoteUri);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, noteUri);

            NoteResource noteResource = await SendHttpRequestMessage<NoteResource>(httpRequestMessage).ConfigureAwait(false);

            if (noteResource == null)
            {
                mLogger.LogDebug($"Could not find note {noteName}");
                return;
            }

            if (noteResource.IsMoreThanOneNoteFound())
            {
                mConsolePrinter.Print(noteResource.PossibleNotes, "Found the next possible notes");
                return;
            }

            mConsolePrinter.Print(noteResource.Text, $"NotePath: {Environment.NewLine}{noteResource.NotePath}{Environment.NewLine}");
        }

        private async Task<NoteNodeResource> GetAllNotesNames()
        {
            Uri privateNotesUri = TaskerUris.NotesUri.CombineRelative("private");

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, privateNotesUri);

            return await SendHttpRequestMessage<NoteNodeResource>(httpRequestMessage).ConfigureAwait(false);
        }

        private async Task<T> SendHttpRequestMessage<T>(HttpRequestMessage httpRequestMessage)
        {
            using HttpResponseMessage response =
                await mHttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

            mLogger.LogTrace($"Operation {httpRequestMessage.Method.Method} ends with response status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    throw new InvalidOperationException(
                        $"Could not perform {httpRequestMessage.Method.Method} operation, response status: {response.StatusCode}");
                }

                mLogger.LogDebug($"Resource not found, {response.StatusCode}");
                return default;
            }

            string responseStringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(responseStringContent);
        }
    }
}