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
        /// Writes unique values to multiple registers in a single burst using the selected protocol
        /// and reads them all back for comparison.
        /// </summary>
        /// <remarks>
        /// For each register, publishes the readback value of each site under the published data ID
        /// "Register_0x{address:X2}_Readback", and the per-site comparison of the readback value against the
        /// written value under "Register_0x{address:X2}_Comparison".
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="protocol">The digital communication protocol to use.</param>
        /// <param name="registerAddresses">The addresses of the registers to write and read back.</param>
        /// <param name="valuesToWrite">The values to write, one per register in <paramref name="registerAddresses"/> order.</param>
        public static void WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(
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
        }
    }
}
