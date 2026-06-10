using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl
{
    /// <summary>
    /// Factory extensions that encapsulate protocol-specific bundle creation.
    /// </summary>
    public static class TSMSessionManagerExtensions
    {
        /// <summary>
        /// Creates a protocol implementation for the selected communication protocol using the
        /// global parameters configured via <see cref="SPI.SetProtocolParameters"/> or
        /// <see cref="I2C.SetProtocolParameters"/>.
        /// </summary>
        /// <param name="sessionManager">The session manager for the active module context.</param>
        /// <param name="communicationProtocol">The communication protocol to instantiate.</param>
        /// <returns>A protocol implementation for the selected communication protocol.</returns>
        public static IDigitalProtocol DutControl(
            this TSMSessionManager sessionManager,
            CommunicationProtocol communicationProtocol)
        {
            if (sessionManager == null)
            {
                throw new ArgumentNullException(nameof(sessionManager));
            }

            switch (communicationProtocol)
            {
                case CommunicationProtocol.SPI:
                    var spiParameters = SPI.CurrentProtocolParameters;
                    return new SPI(sessionManager.Digital(spiParameters.PinNames), spiParameters);
                case CommunicationProtocol.I2C:
                    var i2cParameters = I2C.CurrentProtocolParameters;
                    return new I2C(sessionManager.Digital(i2cParameters.PinNames), i2cParameters);
                default:
                    throw new ArgumentOutOfRangeException(nameof(communicationProtocol), communicationProtocol, "Unsupported communication protocol.");
            }
        }
    }
}
