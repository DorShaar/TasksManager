using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tasker.Communication;
using Tasker.Extensions;
using Tasker.Resources;
using Tasker.TaskerVariables;

namespace Tasker.TaskerWorkers
{
    public static class NotesProvider
    {
        public static async Task<NoteResource> GetNoteResource(HttpClient httpClient, string noteName, bool isPrivateNote, ILogger logger)
        {
            string escapedNoteName = Uri.EscapeDataString(noteName);

            string relativeNoteUri = isPrivateNote ? $"note/{escapedNoteName}" : escapedNoteName;

            Uri noteUri = TaskerUris.NotesUri.CombineRelative(relativeNoteUri);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, noteUri);

            return await HttpMessageRequester.SendHttpRequestMessage<NoteResource>(
                httpClient, httpRequestMessage, logger).ConfigureAwait(false);
        }
    }
}