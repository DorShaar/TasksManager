namespace TaskData.Contracts
{
    public interface INote
    {
        string Extension { get; }
        string NotePath { get; }
        string Text { get; }

        void Open();
    }
}