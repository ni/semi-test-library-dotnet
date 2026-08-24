using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.TMU;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Unit tests for STLDigitalTmuExtensions.
    /// These tests require TMU to be configured in NIDigital hardware.
    /// </summary>
    [Collection("NonParallelizable")]
    [Trait(nameof(Feature), nameof(Feature.TMU))]
    [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
    public sealed class TmuExtensionsTests : IDisposable
    {
        private const string PinMapFileName = @"NIDigitalTMUTest.pinmap";
        private const string DigitalProjectFileName = @"NIDigitalTMUTest.digiproj";
        private ISemiconductorModuleContext _tsmContext = CreateTSMContext(PinMapFileName, DigitalProjectFileName);

        public TSMSessionManager InitializeSessionsAndCreateSessionManager()
        {
            _tsmContext = CreateTSMContext(PinMapFileName, DigitalProjectFileName);
            Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
            GC.SuppressFinalize(this);
        }

        #region Configure TMU Start Source Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Initialize_ConfigureTMUStartSource_Succeeds(bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUStartSource(pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Stop Source Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Initialize_ConfigureTMUStopSource_Succeeds(bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUStopSource(pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Start Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol, false)]
        [InlineData(TmuSourceEvent.Vol, true)]
        [InlineData(TmuSourceEvent.Voh, false)]
        [InlineData(TmuSourceEvent.Voh, true)]
        public void Initialize_ConfigureTMUStartSourceEvent_Succeeds(TmuSourceEvent sourceEvent, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUStartSourceEvent(sourceEvent, pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Stop Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol, false)]
        [InlineData(TmuSourceEvent.Vol, true)]
        [InlineData(TmuSourceEvent.Voh, false)]
        [InlineData(TmuSourceEvent.Voh, true)]
        public void Initialize_ConfigureTMUStopSourceEvent_Succeeds(TmuSourceEvent sourceEvent, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUStopSourceEvent(sourceEvent, pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Start Source Event Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge, false)]
        [InlineData(TmuPolarity.RisingEdge, true)]
        [InlineData(TmuPolarity.FallingEdge, false)]
        [InlineData(TmuPolarity.FallingEdge, true)]
        [InlineData(TmuPolarity.EitherEdge, false)]
        [InlineData(TmuPolarity.EitherEdge, true)]
        public void Initialize_ConfigureTMUStartSourceEventPolarity_Succeeds(TmuPolarity polarity, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUStartSourceEventPolarity(polarity, pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Stop Source Event Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge, false)]
        [InlineData(TmuPolarity.RisingEdge, true)]
        [InlineData(TmuPolarity.FallingEdge, false)]
        [InlineData(TmuPolarity.FallingEdge, true)]
        [InlineData(TmuPolarity.EitherEdge, false)]
        [InlineData(TmuPolarity.EitherEdge, true)]
        public void Initialize_ConfigureTMUStopSourceEventPolarity_Succeeds(TmuPolarity polarity, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUStopSourceEventPolarity(polarity, pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Arm Type Tests

        [Theory]
        [InlineData(TmuArmType.Immediate, false)]
        [InlineData(TmuArmType.Immediate, true)]
        [InlineData(TmuArmType.Edge, false)]
        [InlineData(TmuArmType.Edge, true)]
        public void Initialize_ConfigureTMUArmType_Succeeds(TmuArmType armType, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUStartSource(pinNames);
            sessionsBundle.ConfigureTMUEdgeArmSource(pinNames);
            sessionsBundle.ConfigureTMUEdgeArmPolarity(TmuPolarity.RisingEdge, pinNames);

            sessionsBundle.ConfigureTMUArmType(armType, pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Edge Arm Source Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Initialize_ConfigureTMUEdgeArmSource_Succeeds(bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUEdgeArmSource(pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Edge Arm Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol, false)]
        [InlineData(TmuSourceEvent.Vol, true)]
        [InlineData(TmuSourceEvent.Voh, false)]
        [InlineData(TmuSourceEvent.Voh, true)]
        public void Initialize_ConfigureTMUEdgeArmSourceEvent_Succeeds(TmuSourceEvent sourceEvent, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUEdgeArmSourceEvent(sourceEvent, pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Edge Arm Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge, false)]
        [InlineData(TmuPolarity.RisingEdge, true)]
        [InlineData(TmuPolarity.FallingEdge, false)]
        [InlineData(TmuPolarity.FallingEdge, true)]
        public void Initialize_ConfigureTMUEdgeArmPolarity_Succeeds(TmuPolarity polarity, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUEdgeArmPolarity(polarity, pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Samples To Acquire Tests

        [Theory]
        [InlineData(100, false)]
        [InlineData(1, false)]
        [InlineData(50, true)]
        public void Initialize_ConfigureTMUSamplesToAcquire_Succeeds(long sampleNumber, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUSamplesToAcquire(sampleNumber, pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Sample Timeout Tests

        [Theory]
        [InlineData(10.0, false)]
        [InlineData(0.001, false)]
        [InlineData(10.0, true)]
        public void Initialize_ConfigureTMUSampleTimeout_Succeeds(double timeout, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUSampleTimeout(timeout, pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure Period Measurement Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge, 1, false)]
        [InlineData(TmuPolarity.RisingEdge, 100, true)]
        public void Initialize_ConfigurePeriodMeasurement_Succeeds(TmuPolarity edgeType, long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigurePeriodMeasurement(edgeType, samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigurePeriodMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigurePeriodMeasurementWithPinNotInBundle()
            {
                sessionsBundle.ConfigurePeriodMeasurement(TmuPolarity.RisingEdge, 1, pinNames: new string[] { "NonExistentPin" });
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigurePeriodMeasurementWithPinNotInBundle());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigurePeriodMeasurementWithEitherEdge_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigurePeriodMeasurementWithEitherEdge()
            {
                sessionsBundle.ConfigurePeriodMeasurement(TmuPolarity.EitherEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigurePeriodMeasurementWithEitherEdge());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Rise Time Measurement Tests

        [Theory]
        [InlineData(1, false)]
        [InlineData(100, false)]
        [InlineData(1, true)]
        public void Initialize_ConfigureTMURiseTimeMeasurement_Succeeds(long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMURiseTimeMeasurement(samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMURiseTimeMeasurementWithArmType_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMURiseTimeMeasurement(samplesToAcquire: 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMURiseTimeMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureTMURiseTimeMeasurementWithPinNotInBundle()
            {
                sessionsBundle.ConfigureTMURiseTimeMeasurement(1, pinNames: new string[] { "NonExistentPin" });
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureTMURiseTimeMeasurementWithPinNotInBundle());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Fall Time Measurement Tests

        [Theory]
        [InlineData(1, false)]
        [InlineData(100, false)]
        [InlineData(1, true)]
        public void Initialize_ConfigureTMUFallTimeMeasurement_Succeeds(long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUFallTimeMeasurement(samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUFallTimeMeasurementWithArmType_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUFallTimeMeasurement(samplesToAcquire: 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUFallTimeMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureTMUFallTimeMeasurementWithPinNotInBundle()
            {
                sessionsBundle.ConfigureTMUFallTimeMeasurement(1, pinNames: new string[] { "NonExistentPin" });
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureTMUFallTimeMeasurementWithPinNotInBundle());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Duty Cycle Measurement Tests

        [Theory]
        [InlineData(TmuDutyCycle.High, 1, false)]
        [InlineData(TmuDutyCycle.High, 100, true)]
        [InlineData(TmuDutyCycle.Low, 1, false)]
        [InlineData(TmuDutyCycle.Low, 100, true)]
        public void Initialize_ConfigureTMUDutyCycleMeasurement_Succeeds(TmuDutyCycle dutyCycleType, long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUDutyCycleMeasurement(dutyCycleType, samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUDutyCycleMeasurementWithInvalidDutyCycleType_ThrowsArgumentOutOfRangeException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUDutyCycleMeasurement((TmuDutyCycle)999, 1));
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUDutyCycleMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureTMUDutyCycleMeasurementWithPinNotInBundle()
            {
                sessionsBundle.ConfigureTMUDutyCycleMeasurement(TmuDutyCycle.High, 1, pinNames: new string[] { "NonExistentPin" });
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureTMUDutyCycleMeasurementWithPinNotInBundle());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Pulse Width Measurement Tests

        [Theory]
        [InlineData(TmuPulseWidth.High, 1, false)]
        [InlineData(TmuPulseWidth.High, 100, true)]
        [InlineData(TmuPulseWidth.Low, 1, false)]
        [InlineData(TmuPulseWidth.Low, 100, true)]
        public void Initialize_ConfigureTMUPulseWidthMeasurement_Succeeds(TmuPulseWidth pulseWidthType, long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUPulseWidthMeasurement(pulseWidthType, samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUPulseWidthMeasurementWithInvalidPulseWidthType_ThrowsArgumentOutOfRangeException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUPulseWidthMeasurement((TmuPulseWidth)999, 1));
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUPulseWidthMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureTMUPulseWidthMeasurementWithPinNotInBundle()
            {
                sessionsBundle.ConfigureTMUPulseWidthMeasurement(TmuPulseWidth.High, 1, pinNames: new string[] { "NonExistentPin" });
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureTMUPulseWidthMeasurementWithPinNotInBundle());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure Skew Measurement (Single Pin Pair) Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge)]
        [InlineData(TmuPolarity.FallingEdge)]
        public void Initialize_ConfigureSkewMeasurement_Succeeds(TmuPolarity edgeType)
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0" }, new string[] { "C1" }, edgeType, 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithEitherEdge_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0" }, new string[] { "C1" }, TmuPolarity.EitherEdge, 1));
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithSamePinAsReferenceAndTarget_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0" }, new string[] { "C0" }, TmuPolarity.RisingEdge, 1));
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithMismatchedReferenceAndTargetPinCounts_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureSkewMeasurementWithMismatchedPinCounts()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0", "C1" }, new string[] { "C1" }, TmuPolarity.RisingEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureSkewMeasurementWithMismatchedPinCounts());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithNullReferencePins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureSkewMeasurementWithNullReferencePins()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(null, new string[] { "C1" }, TmuPolarity.RisingEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureSkewMeasurementWithNullReferencePins());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithNullTargetPins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureSkewMeasurementWithNullTargetPins()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0" }, null, TmuPolarity.RisingEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureSkewMeasurementWithNullTargetPins());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithEmptyReferencePins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureSkewMeasurementWithEmptyReferencePins()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { }, new string[] { "C1" }, TmuPolarity.RisingEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureSkewMeasurementWithEmptyReferencePins());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithEmptyTargetPins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureSkewMeasurementWithEmptyTargetPins()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0" }, new string[] { }, TmuPolarity.RisingEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureSkewMeasurementWithEmptyTargetPins());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithReferencePinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureSkewMeasurementWithReferencePinNotInBundle()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "NonExistentPin" }, new string[] { "C1" }, TmuPolarity.RisingEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureSkewMeasurementWithReferencePinNotInBundle());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithTargetPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureSkewMeasurementWithTargetPinNotInBundle()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0" }, new string[] { "NonExistentPin" }, TmuPolarity.RisingEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureSkewMeasurementWithTargetPinNotInBundle());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure Skew Measurement (Multiple Pin Pairs) Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge)]
        [InlineData(TmuPolarity.FallingEdge)]
        public void Initialize_ConfigureSkewMeasurementWithUniquePins_Succeeds(TmuPolarity edgeType)
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSkewMeasurement(new[] { "C0" }, new[] { "C1" }, edgeType, 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithOverlappingReferenceAndTargetPins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureSkewMeasurementWithOverlappingPins()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(
                    new[] { "C0" }, new[] { "C0" }, TmuPolarity.RisingEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureSkewMeasurementWithOverlappingPins());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureSkewMeasurementWithUniquePinsAndEitherEdge_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureSkewMeasurementWithEitherEdge()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(
                    new[] { "C0" }, new[] { "C1" }, TmuPolarity.EitherEdge, 1);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureSkewMeasurementWithEitherEdge());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Single Pin Overload Tests

        [Fact]
        public void Initialize_ConfigureTMUStartSourceWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStartSource("C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStopSourceWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStopSource("C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStartSourceEventWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStartSourceEvent(TmuSourceEvent.Vol, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStopSourceEventWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStopSourceEvent(TmuSourceEvent.Voh, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStartSourceEventPolarityWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStartSourceEventPolarity(TmuPolarity.RisingEdge, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStopSourceEventPolarityWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStopSourceEventPolarity(TmuPolarity.FallingEdge, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUArmTypeWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource("C0");
            sessionsBundle.ConfigureTMUEdgeArmSource("C0");
            sessionsBundle.ConfigureTMUEdgeArmPolarity(TmuPolarity.RisingEdge, "C0");

            sessionsBundle.ConfigureTMUArmType(TmuArmType.Edge, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUEdgeArmSourceWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUEdgeArmSource("C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUEdgeArmSourceEventWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUEdgeArmSourceEvent(TmuSourceEvent.Vol, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUEdgeArmPolarityWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUEdgeArmPolarity(TmuPolarity.RisingEdge, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUSamplesToAcquireWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSamplesToAcquire(10, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUSampleTimeoutWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSampleTimeout(10.0, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigurePeriodMeasurementWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigurePeriodMeasurement(TmuPolarity.RisingEdge, 1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMURiseTimeMeasurementWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMURiseTimeMeasurement(1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUFallTimeMeasurementWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUFallTimeMeasurement(1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUDutyCycleMeasurementWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUDutyCycleMeasurement(TmuDutyCycle.High, 1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUPulseWidthMeasurementWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUPulseWidthMeasurement(TmuPulseWidth.High, 1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUSkewMeasurementWithSinglePinPair_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSkewMeasurement("C0", "C1", TmuPolarity.RisingEdge, 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_AssignTMUResourcesAndClearTMUAssignmentWithSinglePin_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            sessionsBundle.AssignTMUResources("C0");
            sessionsBundle.ClearTMUAssignment("C0");
        }

        [Fact]
        public void Initialize_EnableAndDisableTMUWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigurePeriodMeasurement(TmuPolarity.RisingEdge, 1, "C0");

            sessionsBundle.EnableTMU("C0");
            sessionsBundle.DisableTMU("C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_TMUInitiateAndAbortWithSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigurePeriodMeasurement(TmuPolarity.RisingEdge, 1, "C0");

            sessionsBundle.TMUInitiate("C0");
            sessionsBundle.TMUAbort("C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region TMU Arm Setting Tests

        [Theory]
        [InlineData(TmuArmSetting.Immediate)]
        [InlineData(TmuArmSetting.StartEdge)]
        [InlineData(TmuArmSetting.StopEdge)]
        public void Initialize_ConfigurePeriodMeasurementWithArmSetting_Succeeds(TmuArmSetting armSetting)
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigurePeriodMeasurement(TmuPolarity.RisingEdge, 1, armSetting);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Theory]
        [InlineData(TmuArmSetting.Immediate)]
        [InlineData(TmuArmSetting.StartEdge)]
        [InlineData(TmuArmSetting.StopEdge)]
        public void Initialize_ConfigureTMURiseTimeMeasurementWithArmSetting_Succeeds(TmuArmSetting armSetting)
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMURiseTimeMeasurement(1, armSetting);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Theory]
        [InlineData(TmuArmSetting.Immediate)]
        [InlineData(TmuArmSetting.StartEdge)]
        [InlineData(TmuArmSetting.StopEdge)]
        public void Initialize_ConfigureTMUFallTimeMeasurementWithArmSetting_Succeeds(TmuArmSetting armSetting)
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUFallTimeMeasurement(1, armSetting);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Theory]
        [InlineData(TmuArmSetting.Immediate)]
        [InlineData(TmuArmSetting.StartEdge)]
        [InlineData(TmuArmSetting.StopEdge)]
        public void Initialize_ConfigureTMUDutyCycleMeasurementWithArmSetting_Succeeds(TmuArmSetting armSetting)
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUDutyCycleMeasurement(TmuDutyCycle.High, 1, armSetting);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Theory]
        [InlineData(TmuArmSetting.Immediate)]
        [InlineData(TmuArmSetting.StartEdge)]
        [InlineData(TmuArmSetting.StopEdge)]
        public void Initialize_ConfigureTMUPulseWidthMeasurementWithArmSetting_Succeeds(TmuArmSetting armSetting)
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUPulseWidthMeasurement(TmuPulseWidth.High, 1, armSetting);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigurePeriodMeasurementWithInvalidArmSetting_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigurePeriodMeasurementWithInvalidArmSetting()
            {
                sessionsBundle.ConfigurePeriodMeasurement(TmuPolarity.RisingEdge, 1, (TmuArmSetting)999);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigurePeriodMeasurementWithInvalidArmSetting());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUDutyCycleMeasurementWithInvalidArmSetting_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureTMUDutyCycleMeasurementWithInvalidArmSetting()
            {
                sessionsBundle.ConfigureTMUDutyCycleMeasurement(TmuDutyCycle.High, 1, (TmuArmSetting)999);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureTMUDutyCycleMeasurementWithInvalidArmSetting());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUPulseWidthMeasurementWithArmSettingAndSinglePin_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUPulseWidthMeasurement(TmuPulseWidth.Low, 1, "C0", TmuArmSetting.StartEdge);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Theory]
        [InlineData(TmuArmSetting.Immediate)]
        [InlineData(TmuArmSetting.StartEdge)]
        [InlineData(TmuArmSetting.StopEdge)]
        public void Initialize_ConfigureTMUSkewMeasurementWithArmSetting_Succeeds(TmuArmSetting armSetting)
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSkewMeasurement(new[] { "C0" }, new[] { "C1" }, TmuPolarity.RisingEdge, 1, armSetting);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Theory]
        [InlineData(TmuArmSetting.Immediate)]
        [InlineData(TmuArmSetting.StartEdge)]
        [InlineData(TmuArmSetting.StopEdge)]
        public void Initialize_ConfigureTMUSkewMeasurementWithArmSettingAndSinglePinPair_Succeeds(TmuArmSetting armSetting)
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSkewMeasurement("C0", "C1", TmuPolarity.FallingEdge, 1, armSetting);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUSkewMeasurementWithInvalidArmSetting_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            void ConfigureTMUSkewMeasurementWithInvalidArmSetting()
            {
                sessionsBundle.ConfigureTMUSkewMeasurement(new[] { "C0" }, new[] { "C1" }, TmuPolarity.RisingEdge, 1, (TmuArmSetting)999);
            }

            Assert.Throws<NISemiconductorTestException>(() =>
                ConfigureTMUSkewMeasurementWithInvalidArmSetting());
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Start Source Tests

        [Fact]
        public void Initialize_GetTMUStartSourceWithTMUAssigned_ReturnsConfiguredChannelString()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource();
            var expected = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) => sitePinInfo.IndividualChannelString);

            var result = sessionsBundle.GetTMUStartSource();

            Assert.Equal(expected.ExtractSite(0)["C0"], result.ExtractSite(0)["C0"]);
            Assert.Equal(expected.ExtractSite(0)["C1"], result.ExtractSite(0)["C1"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStartSourceWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            var result = sessionsBundle.GetTMUStartSource();

            Assert.NotNull(result);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStartSourceWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUStartSource());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Stop Source Tests

        [Fact]
        public void Initialize_GetTMUStopSourceWithTMUAssigned_ReturnsConfiguredChannelString()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStopSource();
            var expected = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) => sitePinInfo.IndividualChannelString);

            var result = sessionsBundle.GetTMUStopSource();

            Assert.Equal(expected.ExtractSite(0)["C0"], result.ExtractSite(0)["C0"]);
            Assert.Equal(expected.ExtractSite(0)["C1"], result.ExtractSite(0)["C1"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStopSourceWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            var result = sessionsBundle.GetTMUStopSource();

            Assert.NotNull(result);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStopSourceWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUStopSource());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Start Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol)]
        [InlineData(TmuSourceEvent.Voh)]
        public void Initialize_GetTMUStartSourceEventWithTMUAssigned_ReturnsConfiguredValue(TmuSourceEvent sourceEvent)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSourceEvent(sourceEvent);

            var result = sessionsBundle.GetTMUStartSourceEvent();

            Assert.Equal(sourceEvent, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStartSourceEventWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUStartSourceEvent();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStartSourceEventWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUStartSourceEvent());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Stop Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol)]
        [InlineData(TmuSourceEvent.Voh)]
        public void Initialize_GetTMUStopSourceEventWithTMUAssigned_ReturnsConfiguredValue(TmuSourceEvent sourceEvent)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStopSourceEvent(sourceEvent);

            var result = sessionsBundle.GetTMUStopSourceEvent();

            Assert.Equal(sourceEvent, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStopSourceEventWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUStopSourceEvent();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStopSourceEventWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUStopSourceEvent());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Start Source Event Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge)]
        [InlineData(TmuPolarity.FallingEdge)]
        public void Initialize_GetTMUStartSourceEventPolarityWithTMUAssigned_ReturnsConfiguredValue(TmuPolarity polarity)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSourceEventPolarity(polarity);

            var result = sessionsBundle.GetTMUStartSourceEventPolarity();

            Assert.Equal(polarity, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStartSourceEventPolarityWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUStartSourceEventPolarity();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStartSourceEventPolarityWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUStartSourceEventPolarity());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Stop Source Event Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge)]
        [InlineData(TmuPolarity.FallingEdge)]
        public void Initialize_GetTMUStopSourceEventPolarityWithTMUAssigned_ReturnsConfiguredValue(TmuPolarity polarity)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStopSourceEventPolarity(polarity);

            var result = sessionsBundle.GetTMUStopSourceEventPolarity();

            Assert.Equal(polarity, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStopSourceEventPolarityWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUStopSourceEventPolarity();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStopSourceEventPolarityWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUStopSourceEventPolarity());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Enabled Tests

        [Fact]
        public void InitializeAndEnableTMU_GetTMUEnabledWithTMUAssigned_ReturnsTrue()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource();
            sessionsBundle.ConfigureTMUStopSource();
            sessionsBundle.EnableTMU();

            var result = sessionsBundle.GetTMUEnabled();

            Assert.True(result.ExtractSite(0)["C0"]);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void InitializeAndDisableTMU_GetTMUEnabledWithTMUAssigned_ReturnsFalse()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource();
            sessionsBundle.ConfigureTMUStopSource();
            sessionsBundle.EnableTMU();
            sessionsBundle.DisableTMU();

            var result = sessionsBundle.GetTMUEnabled();

            Assert.False(result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUEnabledWithTMUAssignedBeforeConfiguration_ReturnsFalse()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            var result = sessionsBundle.GetTMUEnabled();

            Assert.False(result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUEnabledWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUEnabled());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Arm Type Tests

        [Theory]
        [InlineData(TmuArmType.Immediate)]
        [InlineData(TmuArmType.Edge)]
        public void Initialize_GetTMUArmTypeWithTMUAssigned_ReturnsConfiguredValue(TmuArmType armType)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource();
            sessionsBundle.ConfigureTMUEdgeArmSource();
            sessionsBundle.ConfigureTMUEdgeArmPolarity(TmuPolarity.RisingEdge);
            sessionsBundle.ConfigureTMUArmType(armType);

            var result = sessionsBundle.GetTMUArmType();

            Assert.Equal(armType, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUArmTypeWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUArmType();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUArmTypeWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUArmType());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Edge Arm Source Tests

        [Fact]
        public void Initialize_GetTMUEdgeArmSourceWithTMUAssigned_ReturnsConfiguredChannelString()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUEdgeArmSource();
            var expected = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) => sitePinInfo.IndividualChannelString);

            var result = sessionsBundle.GetTMUEdgeArmSource();

            Assert.Equal(expected.ExtractSite(0)["C0"], result.ExtractSite(0)["C0"]);
            Assert.Equal(expected.ExtractSite(0)["C1"], result.ExtractSite(0)["C1"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUEdgeArmSourceWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            var result = sessionsBundle.GetTMUEdgeArmSource();

            Assert.NotNull(result);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUEdgeArmSourceWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUEdgeArmSource());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Edge Arm Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol)]
        [InlineData(TmuSourceEvent.Voh)]
        public void Initialize_GetTMUEdgeArmSourceEventWithTMUAssigned_ReturnsConfiguredValue(TmuSourceEvent sourceEvent)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUEdgeArmSourceEvent(sourceEvent);

            var result = sessionsBundle.GetTMUEdgeArmSourceEvent();

            Assert.Equal(sourceEvent, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUEdgeArmSourceEventWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUEdgeArmSourceEvent();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUEdgeArmSourceEventWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUEdgeArmSourceEvent());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Edge Arm Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge)]
        [InlineData(TmuPolarity.FallingEdge)]
        public void Initialize_GetTMUEdgeArmPolarityWithTMUAssigned_ReturnsConfiguredValue(TmuPolarity polarity)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUEdgeArmPolarity(polarity);

            var result = sessionsBundle.GetTMUEdgeArmPolarity();

            Assert.Equal(polarity, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUEdgeArmPolarityWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUEdgeArmPolarity();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUEdgeArmPolarityWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUEdgeArmPolarity());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Samples To Acquire Tests

        [Theory]
        [InlineData(100)]
        [InlineData(1)]
        [InlineData(50)]
        public void Initialize_GetTMUSamplesToAcquireWithTMUAssigned_ReturnsConfiguredValue(long samplesToAcquire)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUSamplesToAcquire(samplesToAcquire);

            var result = sessionsBundle.GetTMUSamplesToAcquire();

            Assert.Equal(samplesToAcquire, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUSamplesToAcquireWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUSamplesToAcquire();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUSamplesToAcquireWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUSamplesToAcquire());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Sample Timeout Tests

        [Theory]
        [InlineData(10.0)]
        [InlineData(0.001)]
        public void Initialize_GetTMUSampleTimeoutWithTMUAssigned_ReturnsConfiguredValue(double timeout)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUSampleTimeout(timeout);

            var result = sessionsBundle.GetTMUSampleTimeout();

            Assert.Equal(timeout, result.ExtractSite(0)["C0"]);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUSampleTimeoutWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUSampleTimeout();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUSampleTimeoutWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUSampleTimeout());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Start Input Debounce Time Tests

        [Fact]
        public void Initialize_GetTMUStartInputDebounceTimeWithTMUAssigned_ReturnsNonNegativeValue()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            var result = sessionsBundle.GetTMUStartInputDebounceTime();

            Assert.True(result.ExtractSite(0)["C0"] >= 0);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStartInputDebounceTimeWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUStartInputDebounceTime();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStartInputDebounceTimeWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUStartInputDebounceTime());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Stop Input Debounce Time Tests

        [Fact]
        public void Initialize_GetTMUStopInputDebounceTimeWithTMUAssigned_ReturnsNonNegativeValue()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            var result = sessionsBundle.GetTMUStopInputDebounceTime();

            Assert.True(result.ExtractSite(0)["C0"] >= 0);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStopInputDebounceTimeWithTMUAssignedBeforeConfiguration_Succeeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.GetTMUStopInputDebounceTime();

            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUStopInputDebounceTimeWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var exception = Record.Exception(() => sessionsBundle.GetTMUStopInputDebounceTime());

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("No TMU resource has been assigned to one or more pins.", exception.Message);
        }

        #endregion

        #region Get TMU Count Tests

        [Fact]
        public void Initialize_GetTMUCountWithTMUAssigned_ReturnsAtLeastTwoTMUs()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            var result = sessionsBundle.GetTMUCount();

            Assert.True(Array.TrueForAll(result, count => count >= 2));
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_GetTMUCountWithoutTMUAssigned_ReturnsAtLeastTwoTMUs()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            var result = sessionsBundle.GetTMUCount();

            Assert.True(Array.TrueForAll(result, count => count >= 2));
        }

        #endregion

        #region Assign TMU Resources Across Multiple Bundles Tests

        [Fact]
        public void Inititalize_AssignTMUResourcesOnSeparateBundlesForDifferentPins_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var bundle1 = sessionManager.Digital(new string[] { "C0" });
            var bundle2 = sessionManager.Digital(new string[] { "C1" });

            bundle1.AssignTMUResources();
            bundle2.AssignTMUResources();

            bundle1.ClearTMUAssignment();
            bundle2.ClearTMUAssignment();
        }

        [Fact]
        public void InititalizeAndAssignTMUResourcesOnFirstBundle_AssignTMUResourcesForSamePinOnSecondBundle_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var bundle1 = sessionManager.Digital(new string[] { "C0" });
            var bundle2 = sessionManager.Digital(new string[] { "C0" });
            bundle1.AssignTMUResources();

            bundle2.AssignTMUResources();

            // Releasing the first bundle should not affect the second bundle's assignment.
            bundle1.ClearTMUAssignment();
            bundle2.ClearTMUAssignment();
        }

        [Fact]
        public void InititalizeAndAssignTMUResourcesOnFirstBundle_AssignTMUResourcesExhaustsResourcesOnSecondBundle_ThrowsAndCleansUp()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var bundle1 = sessionManager.Digital(new string[] { "C0", "C1" });
            var bundle2 = sessionManager.Digital(new string[] { "C0", "C1" });

            // First bundle claims the available TMU resources.
            bundle1.AssignTMUResources();

            // Second bundle attempts to claim the same already-assigned resources and should fail.
            Assert.Throws<NISemiconductorTestException>(() => bundle2.AssignTMUResources());
            // The failed assignment should have rolled back its partial claims,
            // so releasing the first bundle must still succeed.
            bundle1.ClearTMUAssignment();
            bundle2.ClearTMUAssignment();
        }

        #endregion

        #region Helper Methods

        private DigitalSessionsBundle InitializeAndCreateBundle()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.AssignTMUResources();
            return sessionsBundle;
        }

        #endregion
    }
}