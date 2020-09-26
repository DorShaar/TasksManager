﻿namespace Database.Configuration
{
    public interface IDatabaseLocalConfiguration
    {
        string DatabaseDirectoryPath { get; }
        string NotesDirectoryPath { get; }
        string NotesTasksDirectoryPath { get; }
        string DefaultTasksGroup { get; }
    }
}