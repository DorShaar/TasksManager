using Database.Contracts;
using Logger.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskData.Contracts;

namespace Database.JsonService
{
     public class Database<T> : IRepository<T> where T: ITaskGroup
     {
          private readonly ILogger mLogger;
          private List<T> mEntities;

          public Database(string databasePath, ILogger logger)
          {
               mLogger = logger;

               if(!File.Exists(databasePath))
               {
                    mLogger.LogError($"No database found in path {databasePath}");
                    return;
               }

               LoadFromFile(databasePath);
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

          public void Insert(T entity)
          {
               if (mEntities.Contains(entity))
               {
                    mLogger.LogError($"Group ID: {entity.ID} is already found in database");
                    return;
               }

               mEntities.Add(entity);
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
          }

          private void SaveToFile(string databasePath)
          {
               try
               {
                    string jsonText = JsonConvert.SerializeObject(mEntities, Formatting.Indented);
                    File.WriteAllText(databasePath, jsonText);
               }
               catch (Exception ex)
               {
                    mLogger.LogError($"Unable to serialize database in {databasePath}", ex);
               }
          }

          private void LoadFromFile(string databasePath)
          {
               try
               {
                    mEntities = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(databasePath));
               }
               catch (Exception ex)
               {
                    mLogger.LogError($"Unable to deserialize database in {databasePath}", ex);
               }
          }
     }
}