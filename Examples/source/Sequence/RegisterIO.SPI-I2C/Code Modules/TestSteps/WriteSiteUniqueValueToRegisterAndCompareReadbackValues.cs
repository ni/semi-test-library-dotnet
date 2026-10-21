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
        /// Writes site-unique values to a single register using the selected protocol and reads it back for comparison.
        /// </summary>
        /// <remarks>
        /// Publishes the readback value of each site under the published data ID "RegisterValueReadback",
        /// and the per-site comparison of the readback value against the written value under "ComparisonResult".
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="protocol">The digital communication protocol to use (SPI).</param>
        /// <param name="registerAddress">The address of the register to write and read back.</param>
        /// <param name="perSiteValuesToWrite">The values to write, one per site in site order.</param>
        public static void WriteSiteUniqueValueToRegisterAndCompareReadbackValues(
            ISemiconductorModuleContext tsmContext,
            CommunicationProtocol protocol,
            uint registerAddress,
            long[] perSiteValuesToWrite)
        {
            SiteData<long> regValues = tsmContext.NewSiteData(perSiteValuesToWrite);

            IDigitalProtocol digitalProtocol = tsmContext.DutControl(protocol);

            digitalProtocol.WriteRegister(registerAddress, regValues);

            SiteData<long> regValueReadBack = digitalProtocol.ReadRegister(registerAddress);

            SiteData<bool> comparisonResults = regValueReadBack.Compare(ComparisonType.EqualTo, regValues);
            tsmContext.PublishResults(regValueReadBack, "RegisterValueReadback");
            tsmContext.PublishResults(comparisonResults, "ComparisonResult");
        }
    }
}
