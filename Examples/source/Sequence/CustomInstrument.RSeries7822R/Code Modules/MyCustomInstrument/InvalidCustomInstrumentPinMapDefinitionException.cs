using System;
using System.Runtime.Serialization;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries7822R
{
    /// <summary>
    /// Define specific exception for Pinmap validation.
    /// </summary>
    [Serializable]
    public class InvalidCustomInstrumentPinMapDefinitionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the<see cref = "InvalidCustomInstrumentPinMapDefinitionException" /> class.
        /// </summary>
        public InvalidCustomInstrumentPinMapDefinitionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the<see cref = "InvalidCustomInstrumentPinMapDefinitionException" /> class with an error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidCustomInstrumentPinMapDefinitionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the<see cref = "InvalidCustomInstrumentPinMapDefinitionException" /> class with an error message and a reference to the inner exception resulting in this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception resulting in the current exception.</param>
        public InvalidCustomInstrumentPinMapDefinitionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private InvalidCustomInstrumentPinMapDefinitionException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
