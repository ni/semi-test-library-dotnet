using System.Linq;
using NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
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
        private readonly ISemiconductorModuleContext _tsmContext;
        private readonly IPublishedDataReader _publishedDataReader;

        public MeasureExamplesTMUTests()
        {
            _tsmContext = CreateTSMContext(PinMapFileName, out _publishedDataReader, DigitalProjectFileName);
        }

        #region Measure Duty Cycle TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasureDutyCycleWithSTL_PublishesDutyCycleRatioInRange()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasureDutyCycleTMU.MeasureDutyCycleWithSTL(_tsmContext);

            var publishedData = _publishedDataReader.GetAndClearPublishedData();
            var publishedDataForDutyCycle = publishedData.Where(d => d.PublishedDataId == "DutyCycleRatio").ToArray();
            Assert.NotEmpty(publishedDataForDutyCycle);
            Assert.Equal(_tsmContext.SiteNumbers.Count, publishedDataForDutyCycle.Length);
            Assert.All(publishedDataForDutyCycle, d => Assert.InRange(d.DoubleValue, 0.0, 1.0));

            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Fall Time TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasureFallTimeWithSTL_PublishesFallTimeForEachSite()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasureFallTimeTMU.MeasureFallTimeWithSTL(_tsmContext);

            var publishedData = _publishedDataReader.GetAndClearPublishedData();
            var publishedDataForFallTime = publishedData.Where(d => d.PublishedDataId == "FallTime").ToArray();
            Assert.NotEmpty(publishedDataForFallTime);
            Assert.Equal(_tsmContext.SiteNumbers.Count, publishedDataForFallTime.Length);

            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Period TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasurePeriodWithSTL_PublishesPeriodForEachSite()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasurePeriodTMU.MeasurePeriodWithSTL(_tsmContext);

            var publishedData = _publishedDataReader.GetAndClearPublishedData();
            var publishedDataForPeriod = publishedData.Where(d => d.PublishedDataId == "Period").ToArray();
            Assert.NotEmpty(publishedDataForPeriod);
            Assert.Equal(_tsmContext.SiteNumbers.Count, publishedDataForPeriod.Length);

            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Pulse Width TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasurePulseWidthWithSTL_PublishesPulseWidthForEachSite()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasurePulseWidthTMU.MeasurePulseWidthWithSTL(_tsmContext);

            var publishedData = _publishedDataReader.GetAndClearPublishedData();
            var publishedDataForPulseWidth = publishedData.Where(d => d.PublishedDataId == "PulseWidth").ToArray();
            Assert.NotEmpty(publishedDataForPulseWidth);
            Assert.Equal(_tsmContext.SiteNumbers.Count, publishedDataForPulseWidth.Length);

            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Rise Time TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasureRiseTimeWithSTL_PublishesRiseTimeForEachSite()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasureRiseTimeTMU.MeasureRiseTimeWithSTL(_tsmContext);

            var publishedData = _publishedDataReader.GetAndClearPublishedData();
            var publishedDataForRiseTime = publishedData.Where(d => d.PublishedDataId == "RiseTime").ToArray();
            Assert.NotEmpty(publishedDataForRiseTime);
            Assert.Equal(_tsmContext.SiteNumbers.Count, publishedDataForRiseTime.Length);

            CleanupInstrumentation(_tsmContext);
        }

        #endregion

        #region Measure Skew TMU Tests

        [Fact]
        public void InitializeNIDigital_MeasureSkewWithSTL_PublishesSkewForEachSite()
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);

            MeasureSkewTMU.MeasureSkewWithSTL(_tsmContext);

            var publishedData = _publishedDataReader.GetAndClearPublishedData();
            var publishedDataForSkew = publishedData.Where(d => d.PublishedDataId == "Skew").ToArray();
            Assert.NotEmpty(publishedDataForSkew);
            Assert.Equal(_tsmContext.SiteNumbers.Count, publishedDataForSkew.Length);

            CleanupInstrumentation(_tsmContext);
        }

        #endregion
    }
}
