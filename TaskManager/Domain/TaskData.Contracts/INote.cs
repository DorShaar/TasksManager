namespace TaskData.Contracts
{
    public interface INote
    {
        string NotePath { get; }
        string Text { get; }

        void Open();
    }
}