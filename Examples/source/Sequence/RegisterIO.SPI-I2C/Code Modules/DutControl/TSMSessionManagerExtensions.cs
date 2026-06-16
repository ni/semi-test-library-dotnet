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
        /// Configures the singleton protocol instance for the selected communication protocol and
        /// calls <see cref="DigitalProtocol.SetBundle"/> with a bundle scoped to the current module context.
        /// </summary>
        /// <param name="sessionManager">The session manager for the active module context.</param>
        /// <param name="communicationProtocol">The communication protocol to instantiate.</param>
        /// <returns>The configured singleton protocol instance.</returns>
        public static IDigitalProtocol DutControl(
            this TSMSessionManager sessionManager,
            CommunicationProtocol communicationProtocol)
        {
            if (sessionManager == null)
            {
                throw new ArgumentNullException(nameof(sessionManager));
            }

            IDigitalProtocol protocol;
            switch (communicationProtocol)
            {
                case CommunicationProtocol.SPI:
                    protocol = SPI.Instance;
                    break;
                case CommunicationProtocol.I2C:
                    protocol = I2C.Instance;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(communicationProtocol), communicationProtocol, "Unsupported communication protocol.");
            }
            protocol.SetBundle(sessionManager.Digital(protocol.PinNames));
            return protocol;
        }
    }
}
