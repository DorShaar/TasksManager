using Database.Contracts;
using Logger.Contracts;
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
        private readonly IConfiguration mConfiguration;
        private readonly IObjectSerializer mSerializer;

        private List<T> mEntities = new List<T>();

        private readonly string NextIdPath;
        public string DatabasePath { get; }

        /// <summary>
        /// That is the path to the directory of all the notes.
        /// </summary>
        public string NotesDatabaseDirectoryPath { get => mConfiguration.NotesDirectoryPath; }
        public string NotesTasksDatabaseDirectoryPath { get => mConfiguration.NotesTasksDirectoryPath; }

        public Database(IConfiguration configuration, IObjectSerializer serializer, ILogger logger)
        {
            mLogger = logger;
            mConfiguration = configuration;
            mSerializer = serializer;

            if (!Directory.Exists(mConfiguration.DatabaseDirectoryPath))
            {
                mLogger.LogError($"No database directory found in path {mConfiguration.DatabaseDirectoryPath}");
                return;
            }

            DatabasePath = Path.Combine(mConfiguration.DatabaseDirectoryPath, DatabaseName);
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
                mEntities = new List<T>();
                mLogger.LogError($"Unable to deserialize whole information", ex);
            }
        }

        private void LoadDatabase()
        {
            if (!File.Exists(DatabasePath))
            {
                mLogger.LogError($"Database file {DatabasePath} does not exists");
                throw new FileNotFoundException("Database does not exists", DatabasePath);
            }

            mLogger.LogInformation($"Going to load database from {DatabasePath}");
            mEntities = mSerializer.Deserialize<List<T>>(DatabasePath);
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
            if (string.IsNullOrEmpty(DatabasePath))
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
                mSerializer.Serialize(mEntities, DatabasePath);
                mSerializer.Serialize(IDProducer.IDProducer.PeekForNextId(), NextIdPath);
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Unable to serialize database in {mConfiguration.DatabaseDirectoryPath}", ex);
            }
        }

        public void SetDatabasePath(string newDatabasePathDirectory)
        {
            mConfiguration.SetDatabaseDirectoryPath(newDatabasePathDirectory);
        }
    }
}