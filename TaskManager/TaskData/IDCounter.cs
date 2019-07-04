namespace TaskData
{
     public class IDCounter
     {
          private static int mLastID = 1000;

          public IDCounter(int lastID)
          {
               mLastID = lastID;
          }

          public static string GetNextID()
          {
               mLastID++;
               return mLastID.ToString();
          }
     }
}