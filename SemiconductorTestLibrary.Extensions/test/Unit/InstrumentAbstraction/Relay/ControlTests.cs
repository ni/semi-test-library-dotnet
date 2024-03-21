using System;
using System.Collections.Generic;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Relay.Control;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Relay.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Relay
{
    [Collection("NonParallelizable")]
    public sealed class ControlTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;

        public ControlTests()
        {
            _tsmContext = CreateTSMContext("RelayTests.pinmap");
            Initialize(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Fact]
        public void ControlSingleRelayWithSingleActionSucceeds()
        {
            ControlRelay(_tsmContext, "SiteRelay1", RelayDriverAction.OpenRelay);
        }

        [Fact]
        public void ControlMultipleRelaysWithTheSameActionSucceeds()
        {
            ControlRelay(_tsmContext, new string[] { "SiteRelay2", "SystemRelay" }, RelayDriverAction.CloseRelay);
        }

        [Fact]
        public void ControlMultipleRelaysWithDifferentActionsSucceeds()
        {
            ControlRelay(
                _tsmContext,
                new string[] { "SiteRelay1", "SiteRelay2", "SystemRelay" },
                new RelayDriverAction[] { RelayDriverAction.OpenRelay, RelayDriverAction.CloseRelay, RelayDriverAction.OpenRelay });
        }

        [Fact]
        public void ControlMultipleRelaysWithPerSiteActionsSucceeds()
        {
            ControlRelay(
                _tsmContext,
                new string[] { "SiteRelay1", "SiteRelay2", "SystemRelay" },
                new Dictionary<int, RelayDriverAction>()
                {
                    [0] = RelayDriverAction.OpenRelay,
                    [1] = RelayDriverAction.CloseRelay
                });
        }

        [Fact]
        public void ApplyRelayConfigurationSucceeds()
        {
            ApplyRelayConfiguration(_tsmContext, "RelayConfiguration");
        }
    }
}
