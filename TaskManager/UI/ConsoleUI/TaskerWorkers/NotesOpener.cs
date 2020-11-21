using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tasker.Options;
using Tasker.Resources;
using Tasker.TaskerVariables;

namespace Tasker.TaskerWorkers
{
    public class NotesOpener
    {
        private readonly HttpClient mHttpClient;
        private readonly string mNoteClientViewer;
        private readonly ILogger<NotesOpener> mLogger;

        public NotesOpener(IHttpClientFactory httpClientFactory, IOptionsMonitor<TaskerConfiguration> options, ILogger<NotesOpener> logger)
        {
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));

            mHttpClient = httpClientFactory.CreateClient(TaskerConsts.HttpClientName);

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            mNoteClientViewer = options.CurrentValue.NoteClientViewer;
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<int> OpenNote(CommandLineOptions.OpenNoteOptions options)
        {
            if (options.ObjectType == null)
            {
                mLogger.LogError("No valid object type given (note, general)");
                return Task.FromResult(1);
            }

            switch (options.ObjectType.ToLower())
            {
                case "note":
                case "notes":
                    return OpenNote(options.NoteName, isPrivateNote: true);

                case "general":
                    return OpenNote(options.NoteName, isPrivateNote: false);

                default:
                    mLogger.LogError("No valid object type given (note, general)");
                    return Task.FromResult(1);
            }
        }

        private async Task<int> OpenNote(string noteName, bool isPrivateNote)
        {
            if (string.IsNullOrWhiteSpace(noteName))
            {
                mLogger.LogError($"{nameof(noteName)} is null or whitespace. Invalid note name.");
                return 1;
            }

            NoteResource noteResource = await NotesProvider.GetNoteResource(
                mHttpClient, noteName, isPrivateNote, mLogger).ConfigureAwait(false);

            if (noteResource == null)
            {
                mLogger.LogError($"Could not find note {noteName}");
                return 1;
            }

            if (noteResource.IsMoreThanOneNoteFound())
            {
                StringBuilder stringBuilder = new StringBuilder();
                noteResource.PossibleNotes.ToList().ForEach(note => stringBuilder.AppendLine(note));

                mLogger.LogInformation($"Found the next possible notes: {stringBuilder}");

                return 0;
            }

            bool isOpenSuccess = Open(noteResource);

            if (!isOpenSuccess)
                mLogger.LogError($"Could not open note {noteResource.NotePath}");

            mLogger.LogInformation($"Note {noteResource.NotePath} opened");
            return 0;
        }

        private bool Open(NoteResource note)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(mNoteClientViewer)
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"\"{note.NotePath}\"",
            };

            using Process process = new Process
            {
                StartInfo = startInfo,
            };

            process.Start();

            return true;
        }
    }
}