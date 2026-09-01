using NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class MeasureExamplesTMUTests
    {
        private const string PinMapFileName = @"NIDigitalTMUTest.pinmap";
        private const string DigitalProjectFileName = @"NIDigitalTMUTest.digiproj";
        private ISemiconductorModuleContext _tsmContext = CreateTSMContext(PinMapFileName, DigitalProjectFileName);

        #region Measure Duty Cycle TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasureDutyCycleWithSTLSucceeds()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasureDutyCycleTMU.MeasureDutyCycleWithSTL(_tsmContext);
            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Fall Time TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasureFallTimeWithSTLSucceeds()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasureFallTimeTMU.MeasureFallTimeWithSTL(_tsmContext);
            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Period TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasurePeriodWithSTLSucceeds()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasurePeriodTMU.MeasurePeriodWithSTL(_tsmContext);
            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Pulse Width TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasurePulseWidthWithSTLSucceeds()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasurePulseWidthTMU.MeasurePulseWidthWithSTL(_tsmContext);
            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Rise Time TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasureRiseTimeWithSTLSucceeds()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasureRiseTimeTMU.MeasureRiseTimeWithSTL(_tsmContext);
            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Skew TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasureSkewWithSTLSucceeds()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasureSkewTMU.MeasureSkewWithSTL(_tsmContext);
            CleanupInstrumentation(_tsmContext);
        }

        #endregion
    }
}
