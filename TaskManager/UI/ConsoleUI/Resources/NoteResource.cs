using System.Collections.Generic;
using System.Linq;

namespace Tasker.Resources
{
    public class NoteResource
    {
        public IEnumerable<string> PossibleNotes { get; set; }
        public string Extension { get; set; }
        public string NotePath { get; set; }
        public string Text { get; set; }

        public bool IsNoteFound()
        {
            if (PossibleNotes == null)
                return false;

            return PossibleNotes.Any();
        }

        public bool IsMoreThanOneNoteFound()
        {
            if (PossibleNotes == null)
                return false;

            return PossibleNotes?.Count() > 1;
        }
    }
}