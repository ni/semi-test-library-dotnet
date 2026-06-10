using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c
{
    public static partial class TestStep
    {
        /// <summary>
        /// Writes unique values to multiple registers in a single burst using the selected protocol
        /// and reads them all back for comparison.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="protocol">The digital communication protocol to use (SPI or I2C).</param>
        public static void WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(ISemiconductorModuleContext tsmContext, CommunicationProtocol protocol)
        {
            uint[] regAddresses = new uint[] { 0x48, 0x49, 0x51, 0x52 };
            long[] regValues = new long[] { 0x8, 0x9, 0x1, 0x2 };

            var sessionManager = new TSMSessionManager(tsmContext);
            IDigitalProtocol digitalProtocol = sessionManager.DutControl(protocol);

            digitalProtocol.WriteRegisters(regAddresses, regValues);

            SiteData<long[]> regValuesReadBack = digitalProtocol.ReadRegisters(regAddresses);

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
