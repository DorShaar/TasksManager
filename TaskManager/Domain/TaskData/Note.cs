using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using TaskData.Contracts;

namespace TaskData
{
    public class Note : INote
    {
        private readonly string Extension;

        [JsonProperty]
        private readonly string mNoteDirecoryPath;

        [JsonProperty]
        private readonly string mNoteName;

        [JsonIgnore]
        public string NotePath => Path.Combine(mNoteDirecoryPath, mNoteName + Extension);

        [JsonIgnore]
        public string Text => File.ReadAllText(NotePath);

        [JsonConstructor]
        public Note(string directoryPath, string noteName)
        {
            Extension = ".txt";
            mNoteDirecoryPath = directoryPath;
            mNoteName = noteName;
        }

        internal Note(string directoryPath, string noteName, string content)
        {
            Extension = ".txt";
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

            Process process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
        }
    }
}
