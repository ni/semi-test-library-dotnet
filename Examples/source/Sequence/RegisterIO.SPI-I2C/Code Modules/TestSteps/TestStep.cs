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
        /// Sets the global SPI and I2C protocol parameters for the test program.
        /// This only needs to be called once (e.g., from a setup step) before any
        /// protocol instances are created. Override only the values you need.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void ConfigureProtocolParameters(ISemiconductorModuleContext tsmContext)
        {
            SPI.SetProtocolParameters(new SPIProtocolParameters { AddressBitWidth = 16 });
            I2C.SetProtocolParameters(new I2CProtocolParameters { AddressBitWidth = 8 });
        }
    }
}
