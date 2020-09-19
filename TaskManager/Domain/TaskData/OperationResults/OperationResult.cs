using Microsoft.Extensions.Logging;
using System;

namespace TaskData.OperationResults
{
    public class OperationResult
    {
        public bool Success { get; }
        public string Reason { get; }

        public OperationResult(bool success, string reason)
        {
            Success = success;

            if (!success && string.IsNullOrEmpty(reason))
                throw new ArgumentException("Must provide a reason in case of failed operation");

            Reason = reason ?? string.Empty;
        }

        public OperationResult(bool success) : this(success, string.Empty)
        {
        }

        public void Log(ILogger logger)
        {
            if (!Success)
            {
                logger.LogWarning(Reason);
            }

            if (!string.IsNullOrEmpty(Reason))
                logger.LogDebug(Reason);
        }
    }

    public class OperationResult<T>
    {
        public bool Success { get; }
        public string Reason { get; }
        public T Value { get; }

        public OperationResult(bool success, string reason, T value = default)
        {
            Success = success;

            if (!success && string.IsNullOrEmpty(reason))
                throw new ArgumentException("Must provide a reason in case of failed operation");

            Reason = reason ?? string.Empty;

            Value = success ? value : default;
        }

        public OperationResult(bool success, T value = default) : this(success, string.Empty, value)
        {
        }

        public void Log(ILogger logger)
        {
            if (!Success)
            {
                logger.LogWarning(Reason);
            }

            if (!string.IsNullOrEmpty(Reason))
                logger.LogDebug(Reason);
        }
    }
}