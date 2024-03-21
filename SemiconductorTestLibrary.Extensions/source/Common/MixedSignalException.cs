using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace NationalInstruments.SemiconductorTestLibrary.Common
{
    /// <summary>
    /// The mixed signal test exception type.
    /// </summary>
    [Serializable]
    public class MixedSignalException : Exception
    {
        /// <summary>
        /// Initializes an instance of <see cref="MixedSignalException"/>.
        /// </summary>
        public MixedSignalException()
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="MixedSignalException"/> with error message.
        /// </summary>
        /// <param name="message">The message to be included.</param>
        public MixedSignalException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes and instance of <see cref="MixedSignalException"/> with error message and inner exception.
        /// </summary>
        /// <param name="message">The message to be included.</param>
        /// <param name="innerException">The exception to be embedded.</param>
        public MixedSignalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="MixedSignalException"/> with error code and inner exception.
        /// </summary>
        /// <param name="errorCode">The error code to be included.</param>
        /// <param name="innerException">The exception to be embedded.</param>
        public MixedSignalException(int errorCode, Exception innerException) : base(string.Empty, innerException)
        {
            HResult = errorCode;
        }

        /// <summary>
        ///  Initializes an instance of <see cref="MixedSignalException"/> with serialization info and streaming context.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="streamingContext">The streaming context.</param>
        protected MixedSignalException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        /// <summary>
        /// Wraps an exception into a <see cref="MixedSignalException"/> and throws.
        /// </summary>
        /// <param name="e">The exception to be wrapped.</param>
        public static void Throw(Exception e)
        {
            Exception innerException = e;
            if (innerException is AggregateException)
            {
                innerException = ((AggregateException)e).Flatten().InnerExceptions.First();
            }

            int errorCode = innerException.HResult;
            _ = TryParseErrorCode(innerException, ref errorCode);

            throw new MixedSignalException(errorCode, innerException);
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
