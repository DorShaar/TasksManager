﻿using Database.Configuration;
using Database.Contracts;
using Logger.Contracts;
using Microsoft.Extensions.Options;
using ObjectSerializer.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskData.Contracts;

namespace Database
{
    public class Database<T> : ILocalRepository<T> where T : ITaskGroup   
    {
        private const string DatabaseName = "tasks.db";
        private const string NextIdHolderName = "id_producer.db";

        private readonly ILogger mLogger;
        private readonly IObjectSerializer mSerializer;
        private readonly IOptions<DatabaseLocalConfigurtaion> mConfiguration;

        private List<T> mEntities = new List<T>();

        private readonly string NextIdPath;
        public string DatabaseDirectoryPath { get; }

        /// <summary>
        /// That is the path to the directory of all the notes.
        /// </summary>
        public string NotesDirectoryPath { get => mConfiguration.Value.NotesDirectoryPath; }
        public string NotesTasksDirectoryPath { get => mConfiguration.Value.NotesTasksDirectoryPath; }

        public Database(IOptions<DatabaseLocalConfigurtaion> configuration, IObjectSerializer serializer, ILogger logger)
        {
            mLogger = logger;
            mConfiguration = configuration;
            mSerializer = serializer;

            if (!Directory.Exists(mConfiguration.Value.DatabaseDirectoryPath))
            {
                mLogger.LogError($"No database directory found in path {mConfiguration.Value.DatabaseDirectoryPath}");
                return;
            }

            DatabaseDirectoryPath = Path.Combine(mConfiguration.Value.DatabaseDirectoryPath, DatabaseName);
            NextIdPath = Path.Combine(mConfiguration.Value.DatabaseDirectoryPath, NextIdHolderName);
            LoadInformation();
        }

        private void LoadInformation()
        {
            try
            {
                LoadDatabase();
                LoadNextIdToProduce();
            }
            catch (Exception ex)
            {
                mEntities = new List<T>();
                mLogger.LogError($"Unable to deserialize whole information", ex);
            }
        }

        private void LoadDatabase()
        {
            if (!File.Exists(DatabaseDirectoryPath))
            {
                mLogger.LogError($"Database file {DatabaseDirectoryPath} does not exists");
                throw new FileNotFoundException("Database does not exists", DatabaseDirectoryPath);
            }

            mLogger.LogInformation($"Going to load database from {DatabaseDirectoryPath}");
            mEntities = mSerializer.Deserialize<List<T>>(DatabaseDirectoryPath);
        }

        private void LoadNextIdToProduce()
        {
            if (!File.Exists(NextIdPath))
            {
                mLogger.LogError($"Database file {NextIdPath} does not exists");
                throw new FileNotFoundException("Database does not exists", NextIdPath);
            }

            mLogger.LogInformation("Going to load next id");
            IDProducer.IDProducer.SetNextID(mSerializer.Deserialize<int>(NextIdPath));
        }

        /// <summary>
        /// Get entity by id or by name. In case not found, returns default of <see cref="T"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetEntity(string entityToFind)
        {
            T entityFound = mEntities.Find(entity => entity.ID == entityToFind);
            if (entityFound == null)
                entityFound = mEntities.Find(entity => entity.GroupName == entityToFind);

            return entityFound;
        }

        public IEnumerable<T> GetAll()
        {
            return mEntities.AsEnumerable();
        }

        public void Insert(T newEntity)
        {
            if (mEntities.Contains(newEntity) ||
               (mEntities.Find(entity => entity.ID == newEntity.ID) != null))
            {
                mLogger.LogError($"Group ID: {newEntity.ID} is already found in database");
                return;
            }

            if (mEntities.Find(entity => entity.GroupName == newEntity.GroupName) != null)
            {
                mLogger.LogError($"Group name: {newEntity.GroupName} is already found in database");
                return;
            }

            mEntities.Add(newEntity);
            SaveToFile();
        }

        public void Remove(T entity)
        {
            if (!mEntities.Contains(entity))
            {
                mLogger.LogError($"Group ID: {entity.ID} Group name: {entity.GroupName} - No such entity was found in database");
                return;
            }

            foreach (ITask task in entity.GetAllTasks())
            {
                mLogger.Log($"Removing inner task id {task.ID} description {task.Description}");
            }

            mEntities.Remove(entity);
            mLogger.Log($"Task group id {entity.ID} group name {entity.GroupName} removed");
            SaveToFile();
        }

        /// <summary>
        /// <param name="newEntity"/> will replace an existing identity with the same id in <see cref="mEntities"/>
        /// </summary>
        /// <param name="newEntity"></param>
        public void Update(T newEntity)
        {
            T entityToUpdate = mEntities.Find(entity => entity.ID == newEntity.ID);

            if (entityToUpdate == null)
            {
                mLogger.LogError($"Group ID: {newEntity.ID} Group name: {newEntity.GroupName} - No such entity was found in database");
                return;
            }

            entityToUpdate = newEntity;
            SaveToFile();
        }

        public void AddOrUpdate(T addOrUpdateEntity)
        {
            T entityToUpdate = mEntities.Find(entity => entity.ID == addOrUpdateEntity.ID);

            if (entityToUpdate != null)
                entityToUpdate = addOrUpdateEntity;
            else
                Insert(addOrUpdateEntity);

            SaveToFile();
        }

        private void SaveToFile()
        {
            if (string.IsNullOrEmpty(DatabaseDirectoryPath))
            {
                mLogger.LogError("No database path was given");
                return;
            }

            if (string.IsNullOrEmpty(NextIdPath))
            {
                mLogger.LogError("No next id path was given");
                return;
            }

            try
            {
                mSerializer.Serialize(mEntities, DatabaseDirectoryPath);
                mSerializer.Serialize(IDProducer.IDProducer.PeekForNextId(), NextIdPath);
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Unable to serialize database in {mConfiguration.Value.DatabaseDirectoryPath}", ex);
            }
        }
    }
}