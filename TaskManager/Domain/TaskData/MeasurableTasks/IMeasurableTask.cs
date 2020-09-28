using Triangle;

namespace TaskData.MeasurableTasks
{
    public interface IMeasurableTask
    {
        TaskTriangle TaskMeasurement { get; }
        void SetMeasurement(TaskTriangle measurement);
    }
}