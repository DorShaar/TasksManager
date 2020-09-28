using Newtonsoft.Json;
using System.Diagnostics;
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

        public string Text => File.ReadAllText(NotePath);

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

        public void Open()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(NotePath)
            {
                UseShellExecute = true
            };

            using Process process = new Process { StartInfo = startInfo };
            process.Start();
        }
    }
}