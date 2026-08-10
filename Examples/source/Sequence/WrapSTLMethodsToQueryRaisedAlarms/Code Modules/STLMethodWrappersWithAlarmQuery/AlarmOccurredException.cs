using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery
{
    /// <summary>
    /// Defines a specific exception that represents an alarm occurrence during a semiconductor test operation.
    /// </summary>
    [Serializable]
    public sealed class AlarmOccurredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmOccurredException"/> class.
        /// </summary>
        public AlarmOccurredException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NISemiconductorTestException"/> class with an error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public AlarmOccurredException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmOccurredException"/> class with an error message and a reference to the inner exception resulting in this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception resulting in the current exception.</param>
        public AlarmOccurredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="NISemiconductorTestException"/> with an error code and an inner exception.
        /// </summary>
        /// <param name="errorCode">The error code to be included.</param>
        /// <param name="innerException">The exception to be embedded.</param>
        public AlarmOccurredException(int errorCode, Exception innerException) : base(innerException.ToString(), innerException)
        {
            HResult = errorCode;
        }

        private AlarmOccurredException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        /// <summary>
        /// Wraps an exception into a <see cref="NISemiconductorTestException"/> and throws.
        /// </summary>
        /// <param name="e">The exception to be wrapped.</param>
        public static void Throw(Exception e)
        {
            if (e is AlarmOccurredException)
            {
                throw e;
            }

            Exception innerException = e;
            if (innerException is AggregateException)
            {
                innerException = ((AggregateException)e).Flatten().InnerExceptions.First();
            }

            int errorCode = innerException.HResult;
            _ = TryParseErrorCode(innerException, ref errorCode);

            throw new AlarmOccurredException(errorCode, innerException);
        }

        private static bool TryParseErrorCode(Exception e, ref int errorCode)
        {
            var rx = new Regex(@"(Error code:\s*)(-\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(e.Message);
            if (matches.Count == 1)
            {
                string errorCodeString = matches[0].Groups[2].Value;
                if (int.TryParse(errorCodeString, out int result))
                {
                    errorCode = result;
                    return true;
                }
            }
            return false;
        }
    }
}
