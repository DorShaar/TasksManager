using System.Collections.Generic;
using System.Linq;

namespace Tasker.Resources
{
    public class NoteResource
    {
        public bool IsNoteFound => PossibleNotes.Any();
        public bool IsMoreThanOneNoteFound => PossibleNotes.Count() > 1;
        public IEnumerable<string> PossibleNotes { get; set; }
        public string Extension { get; set; }
        public string NotePath { get; set; }
        public string Text { get; set; }
    }
}