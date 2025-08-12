using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer.Common.Constants;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Checks to confirm the PartNumber register of the DUT can be read under multiple Vcc conditions.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="spiPortPinNames">The SPI port pin names, as defined in the specified pattern.</param>
        /// <param name="vccPinName">The DUT's Vcc supply pin name, as defined in the pin map file.</param>
        /// <param name="vccVoltageSymbolsToTest">The symbols defining the Vcc voltage level specifications to test, as defined in the specifications file.</param>
        public static void CheckPartNumberRegisterWithMultipleVccValues(
            ISemiconductorModuleContext semiconductorModuleContext,
            string[] spiPortPinNames,
            string vccPinName,
            string[] vccVoltageSymbolsToTest)
        {
            double[] voltagesToTest = semiconductorModuleContext.GetSpecificationsValues(vccVoltageSymbolsToTest);
            double settlingTimeInSeconds = 250e-6;

            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DCPowerSessionsBundle vcc = sessionManager.DCPower(vccPinName);
            DigitalSessionsBundle spi = sessionManager.Digital(spiPortPinNames);

            for (int i = 0; i < voltagesToTest.Length; i++)
            {
                double voltageToTest = voltagesToTest[i];
                string publishedDataId = "Part Number Match:" + vccVoltageSymbolsToTest[i];
                vcc.ConfigureSourceDelay(settlingTimeInSeconds);
                vcc.ForceVoltage(voltageToTest, waitForSourceCompletion: true);
                spi.BurstPatternAndPublishResults(ReadPartNumberPatternName, publishedDataId: publishedDataId);
            }
        }
    }
}