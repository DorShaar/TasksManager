using Newtonsoft.Json;
using System;
using System.IO;

namespace TaskData.Notes
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Note : INote
    {
        [JsonProperty]
        private readonly string mNoteDirecoryPath;

        [JsonProperty]
        private readonly string mNoteName;

        public string Extension { get; } = ".txt";

        public string NotePath => Path.Combine(mNoteDirecoryPath, mNoteName + Extension);

        public string Text => ReadText();

        /// <summary>
        /// For creating Note object from existing file.
        /// </summary>
        [JsonConstructor]
        internal Note(string directoryPath, string noteName)
        {
            mNoteDirecoryPath = directoryPath;
            mNoteName = noteName;
        }

        /// <summary>
        /// For creating new Note object (<paramref name="directoryPath"/>\<paramref name="noteName"/> not exists).
        /// </summary>
        internal Note(string directoryPath, string noteName, string content)
        {
            mNoteDirecoryPath = directoryPath;
            mNoteName = noteName;

            if (!string.IsNullOrEmpty(directoryPath) && !string.IsNullOrEmpty(noteName))
            {
                Directory.CreateDirectory(directoryPath);
                File.WriteAllText(NotePath, content);
            }
        }

        public event OpenNoteHandler OpenRequested;

        private string ReadText()
        {
            try
            {
                return File.ReadAllText(NotePath);
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                return string.Empty;
            }
        }
    }
}