using ConsoleUI.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using TaskData.Notes;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using UI.ConsolePrinter;

namespace ConsoleUI
{
    public class TasksProvider : IDisposable // TODO Idisposable pattern.
    {
        private bool mDisposed;

        private readonly HttpClient mHttpClient;
        private readonly ConsolePrinter mConsolePrinter;
        private readonly ILogger<TasksProvider> mLogger;

        public TasksProvider(HttpClient httpClient, ConsolePrinter consolePrinter, ILogger<TasksProvider> logger)
        {
            mHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            mConsolePrinter = consolePrinter ?? throw new ArgumentNullException(nameof(consolePrinter));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> GetObject(CommandLineOptions.GetOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (task, group, note, general, db, config)");
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
                            options.ShouldPrintNotOnlyDefault,
                            options.Days,
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

                    case "config":
                    case "configuration":
                        return await GetConfigruationPath().ConfigureAwait(false);

                    default:
                        mLogger.LogError("No valid object type given (task, group, note, general, db)");
                        return 1;
                }
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
        private async Task<int> GetAllTasks(
            string taskGroup, string status, bool shouldPrintAll, bool shouldPrintNotOnlyDefaultGroup, int days, bool isDetailed)
        {
            IEnumerable<IWorkTask> tasksToPrint = await GetAllTasksOrTasksByGroupName(taskGroup).ConfigureAwait(false);

            if (tasksToPrint == null)
            {
                mLogger.LogError($"No task group {taskGroup} exist");
                return 1;
            }

            if (!shouldPrintAll)
                tasksToPrint = tasksToPrint.Where(task => !task.IsFinished);

            // TODO
            //if (!shouldPrintNotOnlyDefaultGroup && mTaskManager.DefaultTaskGroupName != null)
            //{
            //    tasksToPrint = tasksToPrint.Where(task =>
            //        AreNamesEquals(task.GroupName, mTaskManager.DefaultTaskGroupName.Name));
            //}

            if (!string.IsNullOrEmpty(status))
                tasksToPrint = tasksToPrint.Where(task => AreStringsValidAndEqual(task.Status.ToString(), status));

            if (days != 0)
                tasksToPrint = tasksToPrint.Where(task => IsTaskUpdateSince(task, days));

            mConsolePrinter.PrintTasks(tasksToPrint, isDetailed);
            return 0;
        }

        private static bool AreStringsValidAndEqual(string groupName1, string groupName2)
        {
            if (string.IsNullOrEmpty(groupName1) || string.IsNullOrEmpty(groupName2))
                return false;
            else
                return groupName1.Equals(groupName2, StringComparison.CurrentCultureIgnoreCase);
        }

        private async Task<IEnumerable<IWorkTask>> GetAllTasksOrTasksByGroupName(string taskGroup)
        {
            if (string.IsNullOrEmpty(taskGroup))
                return await GetAllTasks().ConfigureAwait(false);
            else
                return await GetTasksByGroup(taskGroup).ConfigureAwait(false);
        }

        private async Task<IEnumerable<IWorkTask>> GetAllTasks()
        {
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, TaskerUris.WorkTasksUri);

            return await SendHttpRequestMessage<IEnumerable<IWorkTask>>(httpRequestMessage).ConfigureAwait(false);
        }

        private async Task<IEnumerable<IWorkTask>> GetTasksByGroup(string taskGroup)
        {
            if (string.IsNullOrEmpty(taskGroup))
                throw new ArgumentException($"{nameof(taskGroup)} is null or empty");

            if (!Uri.TryCreate(TaskerUris.WorkTasksUri, taskGroup, out Uri tasksByGroupUri))
                throw new ArgumentException($"Could not create uri from {TaskerUris.WorkTasksUri} and {taskGroup}");

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, tasksByGroupUri);

            return await SendHttpRequestMessage<IEnumerable<IWorkTask>>(httpRequestMessage).ConfigureAwait(false);
        }

        private static bool IsTaskUpdateSince(IWorkTask task, int days)
        {
            return task.TaskStatusHistory.TimeCreated.AddDays(days) >= DateTime.Now ||
                    task.TaskStatusHistory.TimeClosed.AddDays(days) >= DateTime.Now ||
                    task.TaskStatusHistory.TimeLastOnWork.AddDays(days) >= DateTime.Now ||
                    task.TaskStatusHistory.TimeLastOpened.AddDays(days) >= DateTime.Now;
        }

        private async Task<int> GatAllTaskGroup(bool shouldPrintAll, bool isDetailed)
        {
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, TaskerUris.TasksGroupUri);

            IEnumerable<ITasksGroup> groupsToPrint =
                await SendHttpRequestMessage<IEnumerable<ITasksGroup>>(httpRequestMessage).ConfigureAwait(false);

            if (!shouldPrintAll)
                groupsToPrint = groupsToPrint.Where((ITasksGroup group) => (!group.IsFinished));

            mConsolePrinter.PrintTasksGroup(groupsToPrint, isDetailed);
            return 0;
        }

        private async Task<int> GetNoteContent(string notePath, bool shouldPrintAll)
        {
            if (shouldPrintAll)
                return await GetAllNotesNames().ConfigureAwait(false);

            string noteText = await GetNoteText(notePath, isPrivateNote: true).ConfigureAwait(false);
            mConsolePrinter.Print(noteText, notePath);
            return 0;
        }

        private async Task<int> GetGeneralNoteContent(string notePath)
        {
            string noteText = await GetNoteText(notePath, isPrivateNote: false).ConfigureAwait(false);
            mConsolePrinter.Print(noteText, notePath);
            return 0;
        }

        private async Task<string> GetNoteText(string notePath, bool isPrivateNote)
        {
            string relativeNoteUri = isPrivateNote ? $"note/{notePath}" : notePath;

            if (!Uri.TryCreate(TaskerUris.NotesUri, relativeNoteUri, out Uri tasksByGroupUri))
                throw new ArgumentException($"Could not create uri from {TaskerUris.NotesUri} and {relativeNoteUri}");

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, tasksByGroupUri);

            return await SendHttpRequestMessage<string>(httpRequestMessage).ConfigureAwait(false);
        }

        private async Task<int> GetAllNotesNames()
        {
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, TaskerUris.NotesUri);

            IEnumerable<INote> allNotes =
                await SendHttpRequestMessage<IEnumerable<INote>>(httpRequestMessage).ConfigureAwait(false);

            IEnumerable<string> notesToPrint = allNotes
                .Where(note => Path.GetExtension(note.NotePath).Equals(note.Extension))
                .Select(note => Path.GetFileNameWithoutExtension(note.NotePath));

            mConsolePrinter.Print(notesToPrint, header: "NOTES");
            return 0;
        }

        private Task<int> GetConfigruationPath()
        {
            mConsolePrinter.Print(Path.Combine(GetAssemblyDirectory(), "config", "Config.yaml"), header: "Configuration path");
            return Task.FromResult(0);
        }

        private static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        private async Task<T> SendHttpRequestMessage<T>(HttpRequestMessage httpRequestMessage)
        {
            using HttpResponseMessage response =
                await mHttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Could not perform {httpRequestMessage.Method.Method} operation, response status: {response.StatusCode}");
            }

            string responseStringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(responseStringContent);
        }

        public void Dispose()
        {
            if (!mDisposed)
            {
                mHttpClient.Dispose();
                mDisposed = true;
            }
        }
    }
}