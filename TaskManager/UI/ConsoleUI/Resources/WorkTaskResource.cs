namespace ConsoleUI.Resources
{
    public class WorkTaskResource
    {
        public string TaskGroupName { get; }
        public string Description { get; }
        public WorkTaskResource(string groupName, string description)
        {
            TaskGroupName = groupName;
            Description = description;
        }
    }
}