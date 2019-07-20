using Logger.Contracts;
using System.Collections.Generic;
using System.IO;
using TaskData.Contracts;

namespace TaskManager
{
    public class GeneralNotesDatabase
    {
        private readonly ILogger mLogger;

        private bool mIsInitialized = false;
        private readonly string mNotesDirectory;
        private readonly INoteBuilder mNoteBuilder;

        private readonly Dictionary<string, string> mNotesShortNameAdapter = new Dictionary<string, string>();
        private readonly Dictionary<string, INote> mGeneralNotes = new Dictionary<string, INote>();

        public GeneralNotesDatabase(string notesDirectory, INoteBuilder noteBuilder, ILogger logger)
        {
            mLogger = logger;
            mNotesDirectory = notesDirectory;
            mNoteBuilder = noteBuilder;
        }

        private void InitializeNotes()
        {
            foreach (string notePath in Directory.GetFiles(mNotesDirectory))
            {
                mNotesShortNameAdapter.Add(Path.GetFileNameWithoutExtension(notePath), notePath);
                mGeneralNotes.Add(notePath, mNoteBuilder.Load(notePath));
            }
        }

        /// <summary>
        /// Get the note file name without extension and return <see cref="INote"/>.
        /// </summary>
        /// <param name="noteName"></param>
        /// <returns></returns>
        public INote GetNote(string noteName)
        {
            if(!mIsInitialized)
            {
                InitializeNotes();
                mIsInitialized = true;
            }

            if (mNotesShortNameAdapter.TryGetValue(noteName, out string value))
                return mGeneralNotes[value];
            else
            {
                mLogger.LogError($"Note {noteName} does not exist");
                return default;
            }
        }
    }
}