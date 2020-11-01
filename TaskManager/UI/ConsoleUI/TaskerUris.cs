using System;

namespace ConsoleUI
{
    public static class TaskerUris
    {
        public static readonly Uri TasksGroupUri = new Uri("api/TasksGroupsController", UriKind.Relative);
        public static readonly Uri WorkTasksUri = new Uri("api/WorkTasksController", UriKind.Relative);
        public static readonly Uri NotesUri = new Uri("api/NotesController", UriKind.Relative);
    }
}