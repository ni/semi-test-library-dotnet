﻿using System;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
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
    public sealed class TriggersAndEventsTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public DAQmxTasksBundle Initialize(string pinMapFileName, string pinName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.CreateDAQmxAIVoltageTasks(_tsmContext);
            var sessionManager = new TSMSessionManager(_tsmContext);
            var tasksBundle = sessionManager.DAQmx(pinName);
            tasksBundle.ConfigureTiming(new DAQmxTimingSampleClockSettings
            {
                SampleClockRate = 5555,
                SampleQuantityMode = SampleQuantityMode.FiniteSamples,
                SamplesPerChannel = 1000
            });
            return tasksBundle;
        }

        public void Dispose()
        {
            InitializeAndClose.ClearDAQmxAIVoltageTasks(_tsmContext);
        }

        [Theory]
        [InlineData(DigitalEdgeStartTriggerEdge.Rising)]
        [InlineData(DigitalEdgeStartTriggerEdge.Falling)]
        public void SessionsIntialized_ConfigureStartTriggerDigitalEdge_ReturnsCorrectValue(DigitalEdgeStartTriggerEdge digitalEdge)
        {
            var tasksBundle = Initialize("DAQmxTests.pinmap", "VCC1");
            string triggerLine = "PXI_Trig0";

            tasksBundle.ConfigureStartTriggerDigitalEdge(triggerLine, digitalEdge);

            tasksBundle.Do(taskInfo =>
            {
                var fullyQualifedTriggerSource = $"/{taskInfo.Task.Devices[0]}/{triggerLine}";
                Assert.Equal(StartTriggerType.DigitalEdge, taskInfo.Task.Triggers.StartTrigger.Type);
                Assert.Equal(fullyQualifedTriggerSource, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Source);
                Assert.Equal(digitalEdge, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Edge);
            });
        }

        [Fact]
        public void SessionsIntialized_DisableStartTrigger_ReturnsCorrectValue()
        {
            var tasksBundle = Initialize("DAQmxTests.pinmap", "VCC1");
            string triggerLine = "PXI_Trig0";

            tasksBundle.ConfigureStartTriggerDigitalEdge(triggerLine);
            tasksBundle.DisableStartTrigger();

            tasksBundle.Do(taskInfo =>
            {
                var fullyQualifedTriggerSource = $"/{taskInfo.Task.Devices[0]}/{triggerLine}";
                Assert.Equal(StartTriggerType.None, taskInfo.Task.Triggers.StartTrigger.Type);
                Assert.Equal(fullyQualifedTriggerSource, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Source);
                Assert.Equal(DigitalEdgeStartTriggerEdge.Rising, taskInfo.Task.Triggers.StartTrigger.DigitalEdge.Edge);
            });
        }

        [Fact]
        public void SessionsIntialized_ExportStartTrigger_ReturnsCorrectValue()
        {
            var tasksBundle = Initialize("DAQmxTests.pinmap", "VCC1");
            string destinationTriggerLine = "PXI_Trig0";
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
            var tasksBundle = Initialize("DAQmxTests.pinmap", "VCC1");
            var signal = ExportSignal.StartTrigger;
            var channelType = ChannelType.AI.ToString().ToLowerInvariant();

            var fullyQualifiedOutputTerminals = tasksBundle.GetFullyQualifiedOutputTerminals(signal);

            tasksBundle.Do((taskInfo, index) =>
            {
                var fullyQualifedTriggerSourceExpected = $"/{taskInfo.Task.Devices[0]}/{channelType}/{signal}";
                Assert.Equal(fullyQualifedTriggerSourceExpected, fullyQualifiedOutputTerminals[index]);
            });
        }
    }
}
