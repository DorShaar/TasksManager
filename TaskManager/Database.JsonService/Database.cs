﻿using Database.Contracts;
using Logger.Contracts;
using ObjectSerializer.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskData.Contracts;

namespace Database
{
     public class Database<T> : IRepository<T> where T: ITaskGroup
     {
          private const string DatabaseName = "tasks.db";
          private const string NextIdHolderName = "id_producer.db";

          private readonly ILogger mLogger;
          private readonly IConfiguration mConfiguration;
          private readonly IObjectSerializer mSerializer;

          private List<T> mEntities = new List<T>();

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

               LoadFromFile();
          }

          /// <summary>
          /// Get entity by id. In case not found, returns default of <see cref="T"/>
          /// </summary>
          /// <param name="id"></param>
          /// <returns></returns>
          public T GetById(string id)
          {
               return mEntities.Find(entity => entity.ID == id);
          }

          /// <summary>
          /// Get entity by name. In case not found, returns default of <see cref="T"/>
          /// </summary>
          /// <param name="id"></param>
          /// <returns></returns>
          public T GetByName(string name)
          {
               return mEntities.Find(entity => entity.GroupName == name);
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
                    mLogger.LogError($"Group ID: {newEntity.GroupName} is already found in database");
                    return;
               }

               mEntities.Add(newEntity);
          }

          public void Remove(T entity)
          {
               if (!mEntities.Contains(entity))
               {
                    mLogger.LogError($"Group ID: {entity.ID} Group name: {entity.GroupName} - No such entity was found in database");
                    return;
               }

               mEntities.Remove(entity);
          }

          public void RemoveById(string id)
          {
               T entityToRemove = mEntities.Find(entity => entity.ID == id);
               if (entityToRemove == null)
               {
                    mLogger.LogError($"Group ID: {id} was not found in database");
                    return;
               }

               Remove(entityToRemove);
          }

          public void RemoveByName(string name)
          {
               T entityToRemove = mEntities.Find(entity => entity.GroupName == name);
               if (entityToRemove == null)
               {
                    mLogger.LogError($"Group Name: {name} was not found in database");
                    return;
               }

               Remove(entityToRemove);
          }

          /// <summary>
          /// <param name="newEntity"/> will replace an existing identity with the same id in <see cref="mEntities"/>
          /// </summary>
          /// <param name="newEntity"></param>
          public void Update(T newEntity)
          {
               T entityToUpdate = mEntities.Find(entity => entity.ID == newEntity.ID);

               if(entityToUpdate == null)
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
          }

          private void SaveToFile()
          {
               if(string.IsNullOrEmpty(mConfiguration.DatabaseDirectoryPath))
               {
                    mLogger.LogError("No database path was given");
                    return;
               }

               try
               {
                    mSerializer.Serialize(mEntities, mConfiguration.DatabaseDirectoryPath);
               }
               catch (Exception ex)
               {
                    mLogger.LogError($"Unable to serialize database in {mConfiguration.DatabaseDirectoryPath}", ex);
               }
          }

          private void LoadFromFile()
          {
               if (string.IsNullOrEmpty(mConfiguration.DatabaseDirectoryPath))
               {
                    mLogger.LogError("No database path was given");
                    return;
               }


               mLogger.Log($"Loading information from {mConfiguration.DatabaseDirectoryPath}");
               try
               {
                    mLogger.Log("Going to load database");
                    mEntities = mSerializer.Deserialize<List<T>>(Path.Combine(mConfiguration.DatabaseDirectoryPath, DatabaseName));

                    mLogger.Log("Going to load next id");
                    IDProducer.IDProducer.SetNextID(mSerializer.Deserialize<int>(Path.Combine(mConfiguration.DatabaseDirectoryPath, NextIdHolderName)));
               }
               catch (Exception ex)
               {
                    mLogger.LogError($"Unable to deserialize database in {mConfiguration.DatabaseDirectoryPath}", ex);
               }
          }

          public void SetDatabasePath(string newDatabasePathDirectory)
          {
               mConfiguration.SetDatabaseDirectoryPath(newDatabasePathDirectory);
          }
     }
}