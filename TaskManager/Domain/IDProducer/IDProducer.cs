namespace IDProducer
{
     public static class IDProducer
     {
          private static int mLastID = 1000;

          public static void SetNextID(int lastID)
          {
               mLastID = lastID;
          }

          public static string ProduceID()
          {
               string stringID = mLastID.ToString();
               mLastID++;
               return stringID;
          }

          /// <summary>
          /// Does not increment <see cref="mLastID"/>
          /// </summary>
          /// <returns></returns>
          public static string PeekForNextId()
          {
               return mLastID.ToString();
          }
     }
}