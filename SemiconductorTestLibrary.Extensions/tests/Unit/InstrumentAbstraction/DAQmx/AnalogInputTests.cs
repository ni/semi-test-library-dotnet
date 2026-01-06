using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
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
    public sealed class AnalogInputTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public bool IsOffline()
        {
            return _tsmContext.IsSemiconductorModuleInOfflineMode;
        }

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
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void ReadAnalogSamplesFromOneChannelInMultipleChannelsTask_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            var results = tasksBundle.ReadAnalogMultiSample();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["VCC1"]);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["VCC1"]);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void ReadFiveAnalogSamplesFromTwoChannels_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx(new string[] { "VCC1", "VCC2" });
            var results = tasksBundle.ReadAnalogMultiSample(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["VCC1"].Length);
            Assert.Equal(5, results.ExtractSite(0)["VCC2"].Length);
            Assert.Equal(2, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["VCC1"].Length);
            Assert.Equal(5, results.ExtractSite(1)["VCC2"].Length);
        }

        [Fact]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void InitializeDAQmxTasks_ReadAnalogSamplesFromOneFilteredChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxMultiChannelTests.pinmap");
            InitializeAndClose.CreateDAQmxAOVoltageTasks(_tsmContext);
            _tsmContext.GetNIDAQmxTasks("AIPin", out _, out var channelLists);
            var aiTasksBundle = sessionManager.DAQmx("AIPin");
            var aoTasksBundle = sessionManager.DAQmx("AOPin");
            var data = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>> { { "AOPin", new Dictionary<int, double> { { 0, 0.5 }, { 1, 0.8 } } } });
            ConfigureTimingSampleClockSettings(aiTasksBundle, aoTasksBundle);

            aoTasksBundle.WriteAnalogSingleSample(data, autoStart: false);
            aiTasksBundle.Start();
            aoTasksBundle.Start();
            Thread.Sleep(100);  // Wait for the generation to get completed
            aoTasksBundle.Stop();
            var filteredSiteData = aiTasksBundle.FilterBySite(1).ReadAnalogMultiSample();
            aiTasksBundle.Stop();
            var maxValueOfFilteredSamples = filteredSiteData.GetValue(filteredSiteData.SiteNumbers[0], "AIPin").Max();

            if (IsOffline())
            {
                // Limits are based on the expected value returned by the driver when in Offline Mode.
                Assert.True(maxValueOfFilteredSamples > -1.0 && maxValueOfFilteredSamples < 1.0);
            }
            else
            {
                // When run on tester, limits are set based on the limits provided.
                AssertFilteredSample(maxValueOfFilteredSamples, aiTasksBundle, 0.75, 0.8);
            }
            InitializeAndClose.ClearDAQmxAOVoltageTasks(_tsmContext);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void ReadAnalogWaveformSamplesFromOneChannelInMultipleChannelsTask_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            var results = tasksBundle.ReadAnalogWaveform();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(1, results.ExtractSite(0)["VCC1"].SampleCount);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(1, results.ExtractSite(1)["VCC1"].SampleCount);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void ReadFiveAnalogWaveformSamplesFromTwoChannels_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx(new string[] { "VCC1", "VCC2" });
            var results = tasksBundle.ReadAnalogWaveform(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["VCC1"].SampleCount);
            Assert.Equal(5, results.ExtractSite(0)["VCC2"].SampleCount);
            Assert.Equal(2, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["VCC1"].SampleCount);
            Assert.Equal(5, results.ExtractSite(1)["VCC2"].SampleCount);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void ReadAnalogSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxSingleChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogMultiSample();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["AIPin"]);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["AIPin"]);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void ReadFiveAnalogSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxSingleChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogMultiSample(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["AIPin"].Length);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["AIPin"].Length);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void ReadAnalogWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxSingleChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogWaveform();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(1, results.ExtractSite(0)["AIPin"].SampleCount);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(1, results.ExtractSite(1)["AIPin"].SampleCount);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void ReadFiveAnalogWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxSingleChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogWaveform(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["AIPin"].SampleCount);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["AIPin"].SampleCount);
        }

        [Fact]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void InitializeDAQmxTasks_ReadAnalogWaveformSamplesFromOneFilteredChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxMultiChannelTests.pinmap");
            InitializeAndClose.CreateDAQmxAOVoltageTasks(_tsmContext);
            _tsmContext.GetNIDAQmxTasks("AIPin", out _, out var channelLists);
            var aiTasksBundle = sessionManager.DAQmx("AIPin");
            var aoTasksBundle = sessionManager.DAQmx("AOPin");
            var site0Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.5 });
            var site1Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.8 });
            var data = new PinSiteData<AnalogWaveform<double>>(new Dictionary<string, IDictionary<int, AnalogWaveform<double>>> { { "AOPin", new Dictionary<int, AnalogWaveform<double>> { { 0, site0Data }, { 1, site1Data } } } });
            ConfigureTimingSampleClockSettings(aiTasksBundle, aoTasksBundle);

            aoTasksBundle.WriteAnalogWaveform(data, autoStart: false);
            aiTasksBundle.Start();
            aoTasksBundle.Start();
            // Wait for the generation to get completed
            Thread.Sleep(100);
            aoTasksBundle.Stop();
            var filteredSite = aiTasksBundle.FilterBySite(1).ReadAnalogWaveform();
            aiTasksBundle.Stop();
            var filteredWaveformSamples = filteredSite.GetValue(filteredSite.SiteNumbers[0], "AIPin").Samples;
            var maxValueOfFilteredSamples = filteredWaveformSamples.Max(filteredSample => filteredSample.Value);

            if (IsOffline())
            {
                // Limits are based on the expected value returned by the driver when in Offline Mode.
                Assert.True(maxValueOfFilteredSamples > -1.0 && maxValueOfFilteredSamples < 1.0);
            }
            else
            {
                // When run on tester, limits are set based on the limits provided.
                AssertFilteredSample(maxValueOfFilteredSamples, aiTasksBundle, 0.75, 0.8);
            }
            InitializeAndClose.ClearDAQmxAOVoltageTasks(_tsmContext);
        }

        private void ConfigureTimingSampleClockSettings(DAQmxTasksBundle aiTasksBundle, DAQmxTasksBundle aoTasksBundle)
        {
            // Setting acquisition sampling rate to be more than the generation sampling rate to get more accurate samples
            DAQmxTimingSampleClockSettings dAQmxTimingSampleClockSettingsAI = new DAQmxTimingSampleClockSettings() { SampleClockRate = 3000, SampleQuantityMode = SampleQuantityMode.ContinuousSamples };
            DAQmxTimingSampleClockSettings dAQmxTimingSampleClockSettingsAO = new DAQmxTimingSampleClockSettings() { SampleClockRate = 1000, SampleQuantityMode = SampleQuantityMode.ContinuousSamples };

            aoTasksBundle.ConfigureTiming(dAQmxTimingSampleClockSettingsAO);
            aiTasksBundle.ConfigureTiming(dAQmxTimingSampleClockSettingsAI);
        }

        private void AssertFilteredSample(double filteredSample, DAQmxTasksBundle inputTasksBundle, string availableChannels)
        {
            double lowerLimit = 0.75;
            double upperLimit = 0.8;
            if (_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                lowerLimit = -1.0;
                upperLimit = 1.0;
            }
            Assert.True(filteredSample > lowerLimit && filteredSample < upperLimit);
            inputTasksBundle.Do(taskInfo =>
            {
                Assert.Equal(availableChannels, taskInfo.Task.Stream.ChannelsToRead);
            });
        }
    }
}
