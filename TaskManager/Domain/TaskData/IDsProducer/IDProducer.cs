namespace TaskData.IDsProducer
{
    internal class IDProducer : IIDProducer
    {
        private int mLastID = 1000;

        public void SetNextID(int lastID)
        {
            mLastID = lastID;
        }

        public string ProduceID()
        {
            string stringID = mLastID.ToString();
            mLastID++;
            return stringID;
        }

        /// <summary>
        /// Does not increment <see cref="mLastID"/>
        /// </summary>
        public string PeekForNextId()
        {
            return mLastID.ToString();
        }
    }
}