using Newtonsoft.Json;

namespace TaskData.Notes
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Note : INote
    {
        [JsonProperty]
        public string Subject { get; }

        public string Extension { get; } = ".txt";

        [JsonProperty]
        public string Text { get; set; }

        /// <summary>
        /// For creating Note object from existing file.
        /// </summary>
        [JsonConstructor]
        internal Note(string subject)
        {
            Subject = subject;
        }

        /// <summary>
        /// For creating new Note object (<paramref name="directoryPath"/>\<paramref name="noteName"/> not exists).
        /// </summary>
        internal Note(string subject, string content)
        {
            Subject = subject;
            Text = content;

            if (string.IsNullOrEmpty(content))
            {
                Text = string.Empty;
            }
        }
    }
}