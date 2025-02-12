using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
    public class SequencerFlagsAndRegistersTests
    {
        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WriteAndReadSequencerFlag_ValuesCorrectlySetAndReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            // Not fully supported in OfflineMode.
            var expectedValues = tsmContext.IsSemiconductorModuleInOfflineMode ? new bool[] { false, false, false, false } : new bool[] { true, false, true, true };

            sessionsBundle.WriteSequencerFlag($"seqflag{0}", true);
            sessionsBundle.WriteSequencerFlag($"seqflag{1}", false);
            sessionsBundle.WriteSequencerFlag($"seqflag{2}", true);
            sessionsBundle.WriteSequencerFlag($"seqflag{3}", true);

            Assert.Equal(expectedValues[0], sessionsBundle.ReadSequencerFlagDistinct($"seqflag{0}"));
            Assert.Equal(expectedValues[1], sessionsBundle.ReadSequencerFlagDistinct($"seqflag{1}"));
            Assert.Equal(expectedValues[2], sessionsBundle.ReadSequencerFlagDistinct($"seqflag{2}"));
            Assert.Equal(expectedValues[3], sessionsBundle.ReadSequencerFlagDistinct($"seqflag{3}"));
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WriteAndReadSequencerRegister_ValuesCorrectlySetAndReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            // Not fully supported in OfflineMode.
            var expectedValues = tsmContext.IsSemiconductorModuleInOfflineMode ? new int[] { 0, 0, 0, 0 } : new int[] { 0x5, 0x6, 0x7, 0x8 };

            sessionsBundle.WriteSequencerRegister($"reg{0}", expectedValues[0]);
            sessionsBundle.WriteSequencerRegister($"reg{1}", expectedValues[1]);
            sessionsBundle.WriteSequencerRegister($"reg{2}", expectedValues[2]);
            sessionsBundle.WriteSequencerRegister($"reg{3}", expectedValues[3]);

            Assert.Equal(expectedValues[0], sessionsBundle.ReadSequencerRegisterDistinct($"reg{0}"));
            Assert.Equal(expectedValues[1], sessionsBundle.ReadSequencerRegisterDistinct($"reg{1}"));
            Assert.Equal(expectedValues[2], sessionsBundle.ReadSequencerRegisterDistinct($"reg{2}"));
            Assert.Equal(expectedValues[3], sessionsBundle.ReadSequencerRegisterDistinct($"reg{3}"));
            Close(tsmContext);
        }

        [Fact]
        public void SessionsInitialized_WriteAndReadSequencerFlagWithoutSpecifyingPins_ValuesCorrectlySetAndReadBack()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            // Not fully supported in OfflineMode.
            var expectedValues = tsmContext.IsSemiconductorModuleInOfflineMode ? new bool[] { false, false, false, false } : new bool[] { true, false, true, true };

            var sessionsBundle = sessionManager.Digital();
            sessionsBundle.WriteSequencerFlag($"seqflag{0}", true);
            sessionsBundle.WriteSequencerFlag($"seqflag{1}", false);
            sessionsBundle.WriteSequencerFlag($"seqflag{2}", true);
            sessionsBundle.WriteSequencerFlag($"seqflag{3}", true);

            Assert.Equal(5, sessionsBundle.Pins.Count());
            Assert.Equal(expectedValues[0], sessionsBundle.ReadSequencerFlagDistinct($"seqflag{0}"));
            Assert.Equal(expectedValues[1], sessionsBundle.ReadSequencerFlagDistinct($"seqflag{1}"));
            Assert.Equal(expectedValues[2], sessionsBundle.ReadSequencerFlagDistinct($"seqflag{2}"));
            Assert.Equal(expectedValues[3], sessionsBundle.ReadSequencerFlagDistinct($"seqflag{3}"));
            Close(tsmContext);
        }
    }
}
