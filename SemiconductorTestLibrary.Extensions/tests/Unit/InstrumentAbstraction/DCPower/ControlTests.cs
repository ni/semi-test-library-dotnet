using System;
using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    [Collection("NonParallelizable")]
    public sealed class ControlTests : IDisposable
    {
        /* Connections defined in pinmap:
         * VCC - Chassis:1 - PXI4110 - Channel:0
         * VCC - Chassis:2 - PXI4130 - Channel:0
         * VCC - Chassis:3 - PXIe4154 - Channel:0
         * VCC - Chassis:4 - PXIe4112 - Channel:0
         * VDD - Chassis:1 - PXIe4147 - Channel:0
         * VDD - Chassis:2 - PXIe4147 - Channel:0
         * VDD - Chassis:3 - PXIe4147 - Channel:0
         * VDD - Chassis:4 - PXIe4147 - Channel:0
         * VDET - Chassis:1 - PXI4110 - Channel:1
         * VDET - Chassis:2 - PXI4130 - Channel:1
         * VDET - Chassis:3 - PXIe4154 - Channel:1
         * VDET - Chassis:4 - PXIe4112 - Channel:1
         */

        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(bool pinMapWithChannelGroup)
        {
            string pinMapFileName = pinMapWithChannelGroup ? "DifferentSMUDevicesWithChannelGroup.pinmap" : "DifferentSMUDevices.pinmap";
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.Close(_tsmContext);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequenceAsInactive_InitiateAdvancedSequences_AdvanceSequenceActivated(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "TestSequence";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: false);

            sessionsBundle.InitiateAdvancedSequence(sequenceName);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(sequenceName, sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureMultipleAdvanceSequenceAsInactive_InitiateSingleAdvancedSequenceOutofMultipleConfiguredAdvanceSequence_AdvanceSequenceActivated(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName1 = "Sequence1";
            string sequenceName2 = "Sequence2";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName1, stepProperties, setAsActiveSequence: false);
            sessionsBundle.ConfigureAdvancedSequence(sequenceName2, stepProperties, setAsActiveSequence: false);

            sessionsBundle.InitiateAdvancedSequence(sequenceName1);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(sequenceName1, sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequence_InitiateClearedAdvancedSequence_AdvanceSequenceActivated(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "Sequence";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true);

            sessionsBundle.ClearAdvancedSequences();
            sessionsBundle.InitiateAdvancedSequence(sequenceName);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(sequenceName, sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence);
            });
        }
    }
}
