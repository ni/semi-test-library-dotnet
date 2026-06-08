using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c
{
    /// <summary>
    /// Set of protocol-agnostic examples to demonstrate using the <see cref="SPI"/> and
    /// <see cref="I2C"/> classes through the shared <see cref="IDigitalProtocol"/> interface.
    /// The <see cref="CommunicationProtocol"/> parameter selects which implementation is used
    /// at runtime.
    /// </summary>
    public static partial class TestStep
    {
        /// <summary>
        /// Creates the <see cref="IDigitalProtocol"/> implementation that matches the
        /// specified <see cref="CommunicationProtocol"/>.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="protocol">The digital communication protocol to use.</param>
        /// <returns>An <see cref="IDigitalProtocol"/> implementation for the selected protocol.</returns>
        private static IDigitalProtocol CreateProtocol(ISemiconductorModuleContext tsmContext, CommunicationProtocol protocol)
        {
            switch (protocol)
            {
                case CommunicationProtocol.SPI:
                    return new SPI(tsmContext);
                case CommunicationProtocol.I2C:
                    return new I2C(tsmContext);
                default:
                    throw new ArgumentOutOfRangeException(nameof(protocol), protocol, "Unsupported communication protocol.");
            }
        }
    }
}
