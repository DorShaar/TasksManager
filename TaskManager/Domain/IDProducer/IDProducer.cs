namespace IDProducer
{
     public class IDProducer
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
     }
}