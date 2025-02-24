using System;
using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    public sealed class ConfigureTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(string pinMapFileName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.CreateDAQmxAIVoltageTasks(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.ClearDAQmxAIVoltageTasks(_tsmContext);
        }

        [Fact]
        [Obsolete("The GetSampleClockRateDistinct method is obsolete, but behavior can still be tested at this time.")]
        public void ConfigureTiming_GetSampleClockRateDistinct_ReturnsCorrectValue()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            DAQmxTimingSampleClockSettings timingSettings = new DAQmxTimingSampleClockSettings
            {
                SampleClockRate = 5555,
                SampleQuantityMode = SampleQuantityMode.FiniteSamples,
                SamplesPerChannel = 1000
            };
            tasksBundle.ConfigureTiming(timingSettings);

            // This method has been deprecated.
            double sampleClockRateActual = tasksBundle.GetSampleClockRateDistinct();

            Assert.Equal(5555, sampleClockRateActual, 0);
        }

        [Fact]
        [Obsolete("The GetSampleClockRateDistinct method is obsolete, this test validates the suggested alternative.")]
        public void ConfigureTiming_GetSampleClockRatesDistinctSingle_ReturnsSameValueAsGetSampleClockRateDistinct()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            DAQmxTimingSampleClockSettings timingSettings = new DAQmxTimingSampleClockSettings
            {
                SampleClockRate = 5555,
                SampleQuantityMode = SampleQuantityMode.FiniteSamples,
                SamplesPerChannel = 1000
            };
            tasksBundle.ConfigureTiming(timingSettings);

            double sampleClockRateActual1 = tasksBundle.GetSampleClockRateDistinct();
            double sampleClockRateActual2 = tasksBundle.GetSampleClockRates().Distinct().Single();

            Assert.Equal(sampleClockRateActual1, sampleClockRateActual2, 0);
        }

        [Fact]
        public void ConfigureTiming_GetSampleClockRates_ReturnsCorrectValue()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            DAQmxTimingSampleClockSettings timingSettings = new DAQmxTimingSampleClockSettings
            {
                SampleClockRate = 5555,
                SampleQuantityMode = SampleQuantityMode.FiniteSamples,
                SamplesPerChannel = 1000
            };
            tasksBundle.ConfigureTiming(timingSettings);

#pragma warning disable CS0618 // Type or member is obsolete, but can still be used.
            double[] sampleClockRateActualPerInstrument = tasksBundle.GetSampleClockRates();
#pragma warning restore CS0618 // Type or member is obsolete, but can still be used.

            foreach (var sampleClockRate in sampleClockRateActualPerInstrument)
            {
                Assert.Equal(5555, sampleClockRate, 0);
            }
        }

        [Fact]
        public void ConfigureTiming_GetSampleClockRate_ReturnsCorrectValue()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx(new[] { "VCC1", "VCC2" });
            DAQmxTimingSampleClockSettings timingSettings = new DAQmxTimingSampleClockSettings
            {
                SampleClockRate = 5555,
                SampleQuantityMode = SampleQuantityMode.FiniteSamples,
                SamplesPerChannel = 1000
            };
            tasksBundle.ConfigureTiming(timingSettings);

            PinSiteData<double> sampleClockRate = tasksBundle.GetSampleClockRate();

            foreach (var siteNumber in sampleClockRate.SiteNumbers)
            {
                Assert.Equal(5555, sampleClockRate.GetValue(siteNumber, "VCC1"), 0);
                Assert.Equal(5555, sampleClockRate.GetValue(siteNumber, "VCC2"), 0);
            }
        }
    }
}
