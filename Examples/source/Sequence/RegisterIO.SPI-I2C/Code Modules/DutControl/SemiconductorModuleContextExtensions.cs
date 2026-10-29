using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl
{
    /// <summary>
    /// Extensions of the <see cref="ISemiconductorModuleContext"/> interface that encapsulate protocol-specific bundle creation.
    /// </summary>
    public static class SemiconductorModuleContextExtensions
    {
        /// <summary>
        /// Configures the singleton protocol instance for the selected communication protocol and
        /// calls <see cref="DigitalProtocol.SetBundle"/> with a bundle scoped to the current module context.
        /// </summary>
        /// <param name="semiconductorModuleContext">The semiconductor module context for the active code module.</param>
        /// <param name="communicationProtocol">The communication protocol to instantiate.</param>
        /// <returns>The configured singleton protocol instance.</returns>
        public static IDigitalProtocol DutControl(
            this ISemiconductorModuleContext semiconductorModuleContext,
            CommunicationProtocol communicationProtocol)
        {
            if (semiconductorModuleContext == null)
            {
                throw new ArgumentNullException(nameof(semiconductorModuleContext));
            }

            IDigitalProtocol protocol = GetProtocol(communicationProtocol);
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            protocol.SetBundle(sessionManager.Digital(protocol.PinNames?.ToArray()));
            return protocol;
        }

        /// <summary>
        /// Gets the singleton protocol instance for the selected communication protocol.
        /// </summary>
        /// <param name="communicationProtocol">The communication protocol to look up.</param>
        /// <returns>The singleton protocol instance.</returns>
        internal static IDigitalProtocol GetProtocol(CommunicationProtocol communicationProtocol)
        {
            switch (communicationProtocol)
            {
                case CommunicationProtocol.SPI:
                    return SPI.Instance;
                case CommunicationProtocol.I2C:
                    return I2C.Instance;
                default:
                    throw new ArgumentOutOfRangeException(nameof(communicationProtocol), communicationProtocol, "Unsupported communication protocol.");
            }
        }

        /// <summary>
        /// Creates a new protocol instance for the selected communication protocol, holding the protocol's default parameters.
        /// </summary>
        /// <param name="communicationProtocol">The communication protocol to create.</param>
        /// <returns>A new protocol instance with default parameters.</returns>
        internal static IDigitalProtocol CreateDefaultProtocol(CommunicationProtocol communicationProtocol)
        {
            switch (communicationProtocol)
            {
                case CommunicationProtocol.SPI:
                    return new SPI();
                default:
                    throw new ArgumentOutOfRangeException(nameof(communicationProtocol), communicationProtocol, "Unsupported communication protocol.");
            }
        }
    }
}
