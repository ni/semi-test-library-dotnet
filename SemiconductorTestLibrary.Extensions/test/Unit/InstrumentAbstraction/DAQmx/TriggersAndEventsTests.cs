using System;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    public sealed class TriggersAndEventsTests : IDisposable
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
        public void ConfigureStartTrigger_ReturnsCorrectValue()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            string triggerLine = "PXI_Trig0";
            tasksBundle.ConfigureTiming(new DAQmxTimingSampleClockSettings
            {
                SampleClockRate = 5555,
                SampleQuantityMode = SampleQuantityMode.FiniteSamples,
                SamplesPerChannel = 1000
            });

            tasksBundle.ConfigureStartTriggerDigitalEdge(triggerLine);
            tasksBundle.Do(taskInfo =>
            {
                var fullyQualifedTriggerSource = $"/{taskInfo.Task.Devices[0]}/{triggerLine}";
                Assert.Equal(StartTriggerType.DigitalEdge, taskInfo.Task.Triggers.StartTrigger.Type);
                Assert.Equal(fullyQualifedTriggerSource, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Source);
                Assert.Equal(DigitalEdgeStartTriggerEdge.Rising, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Edge);
            });

            tasksBundle.DisableStartTrigger();
            tasksBundle.Do(taskInfo =>
            {
                var fullyQualifedTriggerSource = $"/{taskInfo.Task.Devices[0]}/{triggerLine}";
                Assert.Equal(StartTriggerType.None, taskInfo.Task.Triggers.StartTrigger.Type);
                Assert.Equal(fullyQualifedTriggerSource, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Source);
                Assert.Equal(DigitalEdgeStartTriggerEdge.Rising, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Edge);
            });

            tasksBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdgeStartTriggerEdge.Falling);
            tasksBundle.Do(taskInfo =>
            {
                var fullyQualifedTriggerSource = $"/{taskInfo.Task.Devices[0]}/{triggerLine}";
                Assert.Equal(StartTriggerType.DigitalEdge, taskInfo.Task.Triggers.StartTrigger.Type);
                Assert.Equal(fullyQualifedTriggerSource, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Source);
                Assert.Equal(DigitalEdgeStartTriggerEdge.Falling, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Edge);
            });
        }

        [Fact]
        public void ExportStartTrigger_ReturnsCorrectValue()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            string destinationTriggerLine = "PXI_Trig0";
            tasksBundle.ConfigureTiming(new DAQmxTimingSampleClockSettings
            {
                SampleClockRate = 5555,
                SampleQuantityMode = SampleQuantityMode.FiniteSamples,
                SamplesPerChannel = 1000
            });

            // Test Default
            tasksBundle.Do(taskInfo =>
            {
                Assert.Empty(taskInfo.Task.ExportSignals.StartTriggerOutputTerminal);
            });

            tasksBundle.ExportSignal(ExportSignal.StartTrigger, destinationTriggerLine);
            tasksBundle.Do(taskInfo =>
            {
                var fullyQualifedTriggerSource = $"/{taskInfo.Task.Devices[0]}/{destinationTriggerLine}";
                Assert.Equal(fullyQualifedTriggerSource, taskInfo.Task.ExportSignals.StartTriggerOutputTerminal);
            });
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Not applicable, driver expects lowercase")]
        public void GetFullyQualifiedOutputTerminalsStartTrigger_ReturnsCorrectValue()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            var signal = ExportSignal.StartTrigger;
            var channelType = ChannelType.AI.ToString().ToLowerInvariant();
            tasksBundle.ConfigureTiming(new DAQmxTimingSampleClockSettings
            {
                SampleClockRate = 5555,
                SampleQuantityMode = SampleQuantityMode.FiniteSamples,
                SamplesPerChannel = 1000
            });

            var fullyQualifiedOutputTerminals = tasksBundle.GetFullyQualifiedOutputTerminals(signal);
            tasksBundle.Do((taskInfo, index) =>
            {
                var fullyQualifedTriggerSourceExpected = $"/{taskInfo.Task.Devices[0]}/{channelType}/{signal}";
                Assert.Equal(fullyQualifedTriggerSourceExpected, fullyQualifiedOutputTerminals[index]);
            });
        }
    }
}
