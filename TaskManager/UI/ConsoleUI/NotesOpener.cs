using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Tasker.Options;
using Tasker.Resources;

namespace Tasker
{
    public class NotesOpener
    {
        private readonly HttpClient mHttpClient; // TODO needed here?
        private readonly ILogger<NotesOpener> mLogger;

        public NotesOpener(HttpClient httpClient, ILogger<NotesOpener> logger)
        {
            mHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
                // TODO
                //case "note":
                //case "notes":
                //    return OpenNote(options.NoteName);

                //case "general":
                //    return OpenGeneralNote(options.NoteName);

                default:
                    mLogger.LogError("No valid object type given (note, general)");
                    return Task.FromResult(1);
            }
        }

        // TODO 1. add null check and printing. 2. add using delegates in git.
        //private async Task<int> OpenNote(string noteName)
        //{
        //    INote note = GetNote(mTaskManager.NotesTasksDatabase, noteName);

        //    note.OpenRequested += Note_OpenRequested;

        //    return 0;
        //}

        //private async Task<int> OpenGeneralNote(string noteName)
        //{
        //    INote note = GetNote(mTaskManager.NotesRootDatabase, noteName);

        //    note.OpenRequested += Note_OpenRequested;

        //    return 0;
        //}

        private static void Note_OpenRequested(NoteResource note)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(note.NotePath)
            {
                UseShellExecute = true
            };

            using Process process = new Process { StartInfo = startInfo };
            process.Start();
        }
    }
}