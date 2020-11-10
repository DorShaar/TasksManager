using System;

namespace Tasker
{
    public static class TaskerUris
    {
        public static readonly Uri TasksGroupUri = new Uri("api/TasksGroups", UriKind.Relative);
        public static readonly Uri WorkTasksUri = new Uri("api/WorkTasks", UriKind.Relative);
        public static readonly Uri NotesUri = new Uri("api/Notes", UriKind.Relative);
    }
}