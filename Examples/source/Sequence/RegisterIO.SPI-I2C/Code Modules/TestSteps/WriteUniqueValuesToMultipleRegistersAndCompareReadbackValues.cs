using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C
{
    public static partial class TestStep
    {
        /// <summary>
        /// Writes unique values to multiple registers in a single burst using the selected protocol
        /// and reads them all back for comparison.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="protocol">The digital communication protocol to use (SPI).</param>
        /// <param name="registerAddresses">The addresses of the registers to write and read back.</param>
        /// <param name="valuesToWrite">The values to write, one per register in <paramref name="registerAddresses"/> order.</param>
        /// <returns>The per-site, per-register comparison of each readback value against its written value.</returns>
        public static SiteData<bool[]> WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(
            ISemiconductorModuleContext tsmContext,
            CommunicationProtocol protocol,
            uint[] registerAddresses,
            long[] valuesToWrite)
        {
            IDigitalProtocol digitalProtocol = tsmContext.DutControl(protocol);

            digitalProtocol.WriteRegisters(registerAddresses, valuesToWrite);

            SiteData<long[]> regValuesReadBack = digitalProtocol.ReadRegisters(registerAddresses);

            for (int i = 0; i < registerAddresses.Length; i++)
            {
                int index = i;
                SiteData<long> singleRegReadBack = regValuesReadBack.Select(x => x[index]);
                SiteData<bool> comparisonResult = singleRegReadBack.Compare(ComparisonType.EqualTo, valuesToWrite[i]);
                tsmContext.PublishResults(singleRegReadBack, $"Register_0x{registerAddresses[i]:X2}_Readback");
                tsmContext.PublishResults(comparisonResult, $"Register_0x{registerAddresses[i]:X2}_Comparison");
            }

            return regValuesReadBack.Select(readValues =>
            {
                bool[] perRegisterResults = new bool[registerAddresses.Length];
                for (int i = 0; i < registerAddresses.Length; i++)
                {
                    perRegisterResults[i] = readValues[i] == valuesToWrite[i];
                }
                return perRegisterResults;
            });
        }
    }
}
