using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPI
{
    /// <summary>
    /// Set of examples to demonstrate using the <see cref="RegisterIO_SPI"/> class
    /// through the <see cref="IDigitalProtocol"/> interface.
    /// </summary>
    public static class TestStepUsingSPI
    {
        /// <summary>
        /// Writes a broadcast value to a single register via SPI and reads it back for comparison.
        /// </summary>
        public static void WriteValueToRegisterAndCompareReadbackValue(ISemiconductorModuleContext tsmContext)
        {
            uint regAddress = 0x48;
            long regValue = 4;

            IDigitalProtocol protocol = new RegisterIO_SPI(tsmContext);

            protocol.WriteRegister(regAddress, regValue);

            SiteData<long> regValueReadBack = protocol.ReadRegister(regAddress);

            SiteData<bool> comparisonResults = regValueReadBack.Compare(ComparisonType.EqualTo, regValue);
            tsmContext.PublishResults(regValueReadBack, "RegisterValueReadback");
            tsmContext.PublishResults(comparisonResults, "ComparisonResult");
        }

        /// <summary>
        /// Writes site-unique values to a single register via SPI and reads it back for comparison.
        /// </summary>
        public static void WriteSiteUniqueValueToRegisterAndCompareReadbackValues(ISemiconductorModuleContext tsmContext)
        {
            uint regAddress = 0x48;
            long[] perSiteRegValues = new long[] { 1, 2, 3, 4 };
            SiteData<long> regValues = tsmContext.NewSiteData(perSiteRegValues);

            IDigitalProtocol protocol = new RegisterIO_SPI(tsmContext);

            protocol.WriteRegister(regAddress, regValues);

            SiteData<long> regValueReadBack = protocol.ReadRegister(regAddress);

            SiteData<bool> comparisonResults = regValueReadBack.Compare(ComparisonType.EqualTo, regValues);
            tsmContext.PublishResults(regValueReadBack, "RegisterValueReadback");
            tsmContext.PublishResults(comparisonResults, "ComparisonResult");
        }

        /// <summary>
        /// Writes unique values to multiple registers via SPI and reads them all back for comparison.
        /// </summary>
        public static void WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(ISemiconductorModuleContext tsmContext)
        {
            uint[] regAddresses = new uint[] { 0x48, 0x49, 0x51, 0x52 };
            long[] regValues = new long[] { 0x8, 0x9, 0x1, 0x2 };

            IDigitalProtocol protocol = new RegisterIO_SPI(tsmContext);

            protocol.WriteRegisters(regAddresses, regValues);

            SiteData<long[]> regValuesReadBack = protocol.ReadRegisters(regAddresses);

            for (int i = 0; i < regAddresses.Length; i++)
            {
                int index = i;
                SiteData<long> singleRegReadBack = regValuesReadBack.Select(x => x[index]);
                SiteData<bool> comparisonResult = singleRegReadBack.Compare(ComparisonType.EqualTo, regValues[i]);
                tsmContext.PublishResults(singleRegReadBack, $"Register_0x{regAddresses[i]:X2}_Readback");
                tsmContext.PublishResults(comparisonResult, $"Register_0x{regAddresses[i]:X2}_Comparison");
            }
        }
    }
}
