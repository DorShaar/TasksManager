﻿using System;

namespace TaskData.Contracts
{
    public interface ITask
    {
        string ID { get; }
        string Description { get; set; }
        bool IsFinished { get; }
        Status Status { get; }

        DateTime TimeCreated { get; }
        DateTime TimeLastOpened { get; }
        DateTime TimeClosed { get; }

        void CloseTask();
        void ReOpenTask();
        void MarkTaskOnWork();

        // Private Notes.
        void CreateNote(string noteDirectoryPath, string content);
        void OpenNote();
        string GetNote();
    }
}