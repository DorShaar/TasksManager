using TaskData.OperationResults;
using Triangle;

namespace TaskData.MeasurableTasks
{
    public interface IMeasurementAdder
    {
        OperationResult SetMeasurement(string taskId, TaskTriangle taskTriangle);
    }
}