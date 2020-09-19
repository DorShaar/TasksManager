using Database.Configuration;
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
    public class Database : ILocalRepository<ITasksGroup>
    {
        private const string DatabaseName = "tasks.db";
        private const string NextIdHolderName = "id_producer.db";

        private readonly ILogger mLogger;
        private readonly IObjectSerializer mSerializer;
        private readonly DatabaseLocalConfigurtaion mConfiguration;

        private List<ITasksGroup> mEntities = new List<ITasksGroup>();

        private readonly string NextIdPath;
        public string DatabaseDirectoryPath { get; }
        public string DefaultTasksGroup { get; }

        /// <summary>
        /// That is the path to the directory of all the notes.
        /// </summary>
        public string NotesDirectoryPath { get => mConfiguration.NotesDirectoryPath; }
        public string NotesTasksDirectoryPath { get => mConfiguration.NotesTasksDirectoryPath; }

        public Database(IOptions<DatabaseLocalConfigurtaion> configuration, IObjectSerializer serializer, ILogger logger)
        {
            mLogger = logger;
            mConfiguration = configuration.Value;
            mSerializer = serializer;

            if (!Directory.Exists(mConfiguration.DatabaseDirectoryPath))
            {
                mLogger.LogError($"No database directory found in path {mConfiguration.DatabaseDirectoryPath}");
                return;
            }

            DefaultTasksGroup = mConfiguration.DefaultTasksGroup;
            DatabaseDirectoryPath = Path.Combine(mConfiguration.DatabaseDirectoryPath, DatabaseName);
            NextIdPath = Path.Combine(mConfiguration.DatabaseDirectoryPath, NextIdHolderName);
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
                mEntities = new List<ITasksGroup>();
                mLogger.LogError("Unable to deserialize whole information", ex);
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
            mEntities = mSerializer.Deserialize<List<ITasksGroup>>(DatabaseDirectoryPath);
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
        public ITasksGroup GetEntity(string entityToFind)
        {
            ITasksGroup entityFound =
                mEntities.Find(entity => entity.ID == entityToFind) ??
                mEntities.Find(entity => entity.Name == entityToFind);

            return entityFound;
        }

        public IEnumerable<ITasksGroup> GetAll()
        {
            return mEntities.AsEnumerable();
        }

        public void Insert(ITasksGroup newEntity)
        {
            if (mEntities.Contains(newEntity) ||
               (mEntities.Find(entity => entity.ID == newEntity.ID) != null))
            {
                mLogger.LogError($"Group ID: {newEntity.ID} is already found in database");
                return;
            }

            if (mEntities.Find(entity => entity.Name == newEntity.Name) != null)
            {
                mLogger.LogError($"Group name: {newEntity.Name} is already found in database");
                return;
            }

            mEntities.Add(newEntity);
            SaveToFile();
        }

        public void Remove(ITasksGroup entity)
        {
            if (!mEntities.Contains(entity))
            {
                mLogger.LogError($"Group ID: {entity.ID} Group name: {entity.Name} - No such entity was found in database");
                return;
            }

            foreach (IWorkTask task in entity.GetAllTasks())
            {
                mLogger.Log($"Removing inner task id {task.ID} description {task.Description}");
            }

            mEntities.Remove(entity);
            mLogger.Log($"Task group id {entity.ID} group name {entity.Name} removed");
            SaveToFile();
        }

        /// <summary>
        /// <param name="newEntity"/> will replace an existing identity with the same id in <see cref="mEntities"/>
        /// </summary>
        /// <param name="newEntity"></param>
        public void Update(ITasksGroup newEntity)
        {
            ITasksGroup entityToUpdate = mEntities.Find(entity => entity.ID == newEntity.ID);

            if (entityToUpdate == null)
            {
                mLogger.LogError($"Group ID: {newEntity.ID} Group name: {newEntity.Name} - No such entity was found in database");
                return;
            }

            entityToUpdate = newEntity;
            SaveToFile();
        }

        public void AddOrUpdate(ITasksGroup addOrUpdateEntity)
        {
            ITasksGroup entityToUpdate = mEntities.Find(entity => entity.ID == addOrUpdateEntity.ID);

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
                mLogger.LogError($"Unable to serialize database in {mConfiguration.DatabaseDirectoryPath}", ex);
            }
        }
    }
}