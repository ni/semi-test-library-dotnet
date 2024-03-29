using System;
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
    }
}
