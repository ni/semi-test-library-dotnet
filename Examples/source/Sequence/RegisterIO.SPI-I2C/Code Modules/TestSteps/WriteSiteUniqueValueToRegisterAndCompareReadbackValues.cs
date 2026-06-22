using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c
{
    public static partial class TestStep
    {
        /// <summary>
        /// Writes site-unique values to a single register using the selected protocol and reads it back for comparison.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="protocol">The digital communication protocol to use (SPI or I2C).</param>
        public static void WriteSiteUniqueValueToRegisterAndCompareReadbackValues(ISemiconductorModuleContext tsmContext, CommunicationProtocol protocol)
        {
            uint regAddress = 0x48;
            long[] perSiteRegValues = new long[] { 1, 2, 3, 4 };
            SiteData<long> regValues = tsmContext.NewSiteData(perSiteRegValues);

            IDigitalProtocol digitalProtocol = tsmContext.DutControl(protocol);

            digitalProtocol.WriteRegister(regAddress, regValues);

            SiteData<long> regValueReadBack = digitalProtocol.ReadRegister(regAddress);

            SiteData<bool> comparisonResults = regValueReadBack.Compare(ComparisonType.EqualTo, regValues);
            tsmContext.PublishResults(regValueReadBack, "RegisterValueReadback");
            tsmContext.PublishResults(comparisonResults, "ComparisonResult");
        }
    }
}
