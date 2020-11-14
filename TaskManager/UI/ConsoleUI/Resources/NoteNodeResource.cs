using System.Collections.Generic;

namespace Tasker.Resources
{
    public class NoteNodeResource
    {
        public static NoteNodeResource EmptyNodeResource =
            new NoteNodeResource()
            {
                Name = string.Empty,
                Children = new Dictionary<string, NoteNodeResource>()
            };

        public string Name { get; set; }
        public Dictionary<string, NoteNodeResource> Children { get; set; }
    }
}