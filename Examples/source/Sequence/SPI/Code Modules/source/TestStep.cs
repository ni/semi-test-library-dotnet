using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SPI
{
    /// <summary>
    /// Set of examples to demonstrate using the <see cref="DUTRegisterIO"/> class.
    /// </summary>
    public static class TestStep
    {
        /// <summary>
        /// Writes a broadcast value to a single register and reads it back for comparison.
        /// </summary>
        public static void WriteValueToRegisterAndCompareReadbackValue(ISemiconductorModuleContext tsmContext)
        {
            // Setup local variables.
            uint regAddress = 0x48;
            long regValue = 4;

            // Create new DUTRegisterIO object (must be created for every code module similar to TSMSessionManager).
            var dutDigiIO = new DUTRegisterIO(tsmContext);

            // Write value to target address.
            dutDigiIO.WriteRegister(regAddress, regValue);

            // Read back value from register.
            SiteData<long> regValueReadBack = dutDigiIO.ReadRegister(regAddress);

            // Compare and publish the results.
            SiteData<bool> comparisonResults = regValueReadBack.Compare(ComparisonType.EqualTo, regValue);
            tsmContext.PublishResults(regValueReadBack, "RegisterValueReadback");
            tsmContext.PublishResults(comparisonResults, "ComparisonResult");
        }

        /// <summary>
        /// Writes site-unique values to a single register and reads it back for comparison.
        /// </summary>
        public static void WriteSiteUniqueValueToRegisterAndCompareReadbackValues(ISemiconductorModuleContext tsmContext)
        {
            // Setup local variables.
            uint regAddress = 0x48;
            long[] perSiteRegValues = new long[] { 1, 2, 3, 4 };
            SiteData<long> regValues = tsmContext.NewSiteData(perSiteRegValues);

            // Create new DUTRegisterIO object.
            var dutDigiIO = new DUTRegisterIO(tsmContext);

            // Write site-unique value to target address.
            dutDigiIO.WriteRegister(regAddress, regValues);

            // Read back value from register.
            SiteData<long> regValueReadBack = dutDigiIO.ReadRegister(regAddress);

            // Compare and publish the results.
            SiteData<bool> comparisonResults = regValueReadBack.Compare(ComparisonType.EqualTo, regValues);
            tsmContext.PublishResults(regValueReadBack, "RegisterValueReadback");
            tsmContext.PublishResults(comparisonResults, "ComparisonResult");
        }

        /// <summary>
        /// Writes unique values to multiple registers and reads them all back for comparison.
        /// </summary>
        public static void WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(ISemiconductorModuleContext tsmContext)
        {
            // Setup local variables.
            uint[] regAddresses = new uint[] { 0x48, 0x49, 0x51, 0x52 };
            long[] regValues = new long[] { 0x8, 0x9, 0x1, 0x2 };

            // Create new DUTRegisterIO object.
            var dutDigiIO = new DUTRegisterIO(tsmContext);

            // Write values to target addresses.
            dutDigiIO.WriteRegisters(regAddresses, regValues);

            // Read back values from registers.
            SiteData<long[]> regValuesReadBack = dutDigiIO.ReadRegisters(regAddresses);

            // Publish the results.
            // Note: Comparison of SiteData<long[]> requires per-element logic;
            // iterate over each register to compare and publish individual results.
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
