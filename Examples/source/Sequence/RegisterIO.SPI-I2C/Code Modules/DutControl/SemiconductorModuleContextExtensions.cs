using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl
{
    /// <summary>
    /// Factory extensions that encapsulate protocol-specific bundle creation.
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
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            protocol.SetBundle(sessionManager.Digital(protocol.PinNames));
            return protocol;
        }
    }
}
