using System;
using System.Runtime.Serialization;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// Define specific exception for Pinmap validation.
    /// </summary>
    [Serializable]
    public class InvalidPinMapDefinitionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the<see cref = "InvalidPinMapDefinitionException" /> class.
        /// </summary>
        public InvalidPinMapDefinitionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the<see cref = "InvalidPinMapDefinitionException" /> class with an error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidPinMapDefinitionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the<see cref = "InvalidPinMapDefinitionException" /> class with an error message and a reference to the inner exception resulting in this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception resulting in the current exception.</param>
        public InvalidPinMapDefinitionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private InvalidPinMapDefinitionException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
