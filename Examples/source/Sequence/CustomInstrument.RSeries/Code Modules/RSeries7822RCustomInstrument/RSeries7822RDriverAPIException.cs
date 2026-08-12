using System;
using System.Runtime.Serialization;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// Define specific exception for FGPA-level errors returned by the RSeries7822RDriverAPI methods.
    /// </summary>
    [Serializable]
    public class RSeries7822RDriverAPIException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the<see cref = "RSeries7822RDriverAPIException" /> class.
        /// </summary>
        public RSeries7822RDriverAPIException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the<see cref = "RSeries7822RDriverAPIException" /> class with an error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public RSeries7822RDriverAPIException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the<see cref = "RSeries7822RDriverAPIException" /> class with an error message and a reference to the inner exception resulting in this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception resulting in the current exception.</param>
        public RSeries7822RDriverAPIException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private RSeries7822RDriverAPIException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
