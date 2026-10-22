using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C
{
    /// <summary>
    /// Set of protocol-agnostic examples to demonstrate using the <see cref="SPI"/> class
    /// through the shared <see cref="IDigitalProtocol"/> interface.
    /// The <see cref="CommunicationProtocol"/> parameter selects which protocol is used
    /// at runtime.
    /// </summary>
    public static partial class TestStep
    {
        /// <summary>
        /// Writes a broadcast value to a single register using the selected protocol and reads it back for comparison.
        /// </summary>
        /// <remarks>
        /// Publishes the readback value of each site under the published data ID "RegisterValueReadback",
        /// and the per-site comparison of the readback value against <paramref name="valueToWrite"/> under "ComparisonResult".
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="protocol">The digital communication protocol to use.</param>
        /// <param name="registerAddress">The address of the register to write and read back.</param>
        /// <param name="valueToWrite">The value to write to the register and expect on readback.</param>
        public static void WriteValueToRegisterAndCompareReadbackValue(
            ISemiconductorModuleContext tsmContext,
            CommunicationProtocol protocol,
            uint registerAddress,
            long valueToWrite)
        {
            IDigitalProtocol digitalProtocol = tsmContext.DutControl(protocol);

            digitalProtocol.WriteRegister(registerAddress, valueToWrite);

            SiteData<long> regValueReadBack = digitalProtocol.ReadRegister(registerAddress);

            SiteData<bool> comparisonResults = regValueReadBack.Compare(ComparisonType.EqualTo, valueToWrite);
            tsmContext.PublishResults(regValueReadBack, "RegisterValueReadback");
            tsmContext.PublishResults(comparisonResults, "ComparisonResult");
        }
    }
}
