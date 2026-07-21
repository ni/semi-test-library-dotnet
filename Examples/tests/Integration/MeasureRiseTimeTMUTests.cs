using NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class MeasureRiseTimeWithSTLTest
    {
        private const string PinMapFileName = @"NIDigitalTMUTest.pinmap";
        private const string DigitalProjectFileName = @"NIDigitalTMUTest.digiproj";
        private ISemiconductorModuleContext _tsmContext = CreateTSMContext(PinMapFileName, DigitalProjectFileName);

        [Fact]
        public void InitializeNIDigital_MeasureRiseTimeWithSTLSucceeds()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasureRiseTimeTMU.MeasureRiseTimeWithSTL(_tsmContext);
            CleanupInstrumentation(_tsmContext);
        }
    }
}