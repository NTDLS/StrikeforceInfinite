using System;
using System.Threading.Tasks;

namespace Ae.MpCommsMessages.ReliableMessages
{
    /// <summary>
    /// Provides extension methods for validating and handling asynchronous query operations.
    /// </summary>
    /// <remarks>This static class contains extension methods that assist in ensuring the success of
    /// asynchronous query tasks and handling their results. The methods are intended to simplify error checking and
    /// result retrieval for tasks representing query operations. Use these methods to streamline validation and error
    /// handling when working with query replies that implement the IMultiPlayQueryReply interface.</remarks>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Ensures that the asynchronous query operation represented by the specified task completed successfully and
        /// returns its result.
        /// </summary>
        /// <remarks>If the query reply contains an error message, the error is written to the console
        /// before returning the result. This method should be used to ensure that the query operation did not fail
        /// before accessing its result.</remarks>
        /// <typeparam name="T">The type of the query reply, which must implement the <see cref="IMultiPlayQueryReply"/> interface.</typeparam>
        /// <param name="task">The task representing the asynchronous query operation to validate. Must not be null.</param>
        /// <returns>The result of the completed query operation if the task succeeded.</returns>
        /// <exception cref="Exception">Thrown if the task has faulted or if the query reply contains an error message.</exception>
        public static T EnsureQuerySuccess<T>(this Task<T> task) where T : IMultiPlayQueryReply
        {
            var result = task.Result;
            if (task.IsCompletedSuccessfully == false)
            {
                throw new Exception($"Task did not complete successfully {task.Exception?.GetBaseException().Message}");
            }
            if (task.IsFaulted)
            {
                throw new Exception($"Task faulted {task.Exception?.GetBaseException().Message}");
            }
            if (!string.IsNullOrEmpty(task.Result.ErrorMessage))
            {
                Console.WriteLine($"Remote task failed: {task.Result.ErrorMessage}");
            }
            return result;
        }
    }
}
