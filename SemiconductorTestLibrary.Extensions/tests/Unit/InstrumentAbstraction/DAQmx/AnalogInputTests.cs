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
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    public sealed class AnalogInputTests : IDisposable
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
        public void InitializeDAQmxTasks_ReadAnalogSamplesFromOneFilteredChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxMultiChannelTests.pinmap");
            InitializeAndClose.CreateDAQmxAOVoltageTasks(_tsmContext);
            var aiTasksBundle = sessionManager.DAQmx("AIPin");
            var aoTasksBundle = sessionManager.DAQmx("AOPin");
            var data = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>> { { "AOPin", new Dictionary<int, double> { { 0, 0.5 }, { 1, 0.8 } } } });
            ConfigureTimingSampleClockSettings(aiTasksBundle, aoTasksBundle);

            aoTasksBundle.WriteAnalogSingleSample(data, autoStart: false);
            aiTasksBundle.Start();
            aoTasksBundle.Start();
            // Wait for the generation to get completed
            Thread.Sleep(100);
            aoTasksBundle.Stop();
            var filteredSiteData = aiTasksBundle.FilterBySite(1).ReadAnalogMultiSample();
            aiTasksBundle.Stop();
            var maxValueOfFilteredSamples = filteredSiteData.GetValue(filteredSiteData.SiteNumbers[0], "AIPin").Max();

            AssertFilteredSample(maxValueOfFilteredSamples, aiTasksBundle, 0.75, 0.8);
            InitializeAndClose.ClearDAQmxAOVoltageTasks(_tsmContext);
        }

        [Fact]
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
        public void InitializeDAQmxTasks_ReadAnalogWaveformSamplesFromOneFilteredChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxMultiChannelTests.pinmap");
            InitializeAndClose.CreateDAQmxAOVoltageTasks(_tsmContext);
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

            AssertFilteredSample(maxValueOfFilteredSamples, aiTasksBundle, 0.75, 0.8);
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

        private void AssertFilteredSample(double filteredSample, DAQmxTasksBundle inputTasksBundle, double lowerLimit, double upperLimit)
        {
            var availableChannels = "DAQ_4468_C2_S13/ai0, DAQ_4468_C2_S13/ai1";

            Assert.True(filteredSample > lowerLimit && filteredSample < upperLimit);
            inputTasksBundle.Do(taskInfo =>
            {
                Assert.Equal(availableChannels, taskInfo.Task.Stream.ChannelsToRead);
            });
        }
    }
}
