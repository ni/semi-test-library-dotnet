using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Bursts pattern and publishes pass/fail results.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="spiPortPins">The SPI port pin names, as defined in the specified pattern.</param>
        /// <param name="patternName">Pattern label to burst.</param>
        /// <param name="publishedDataId">Published Data Id.</param>
        public static void BurstPattern(ISemiconductorModuleContext semiconductorModuleContext, string[] spiPortPins, string patternName, string publishedDataId)
        {
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            _ = sessionManager.Digital(spiPortPins).BurstPatternAndPublishResults(patternName, publishedDataId: publishedDataId);
        }
    }
}