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
        public void Initialize_ConfigureTMUStartSourceSucceeds(bool useSpecificPins)
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
        public void Initialize_ConfigureTMUStopSourceSucceeds(bool useSpecificPins)
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
        public void Initialize_ConfigureTMUStartSourceEventSucceeds(TmuSourceEvent sourceEvent, bool useSpecificPins)
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
        public void Initialize_ConfigureTMUStopSourceEventSucceeds(TmuSourceEvent sourceEvent, bool useSpecificPins)
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
        public void Initialize_ConfigureTMUStartSourceEventPolaritySucceeds(TmuPolarity polarity, bool useSpecificPins)
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
        public void Initialize_ConfigureTMUStopSourceEventPolaritySucceeds(TmuPolarity polarity, bool useSpecificPins)
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
        public void Initialize_ConfigureTMUArmTypeSucceeds(TmuArmType armType, bool useSpecificPins)
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
        public void Initialize_ConfigureTMUEdgeArmSourceSucceeds(bool useSpecificPins)
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
        public void Initialize_ConfigureTMUEdgeArmSourceEventSucceeds(TmuSourceEvent sourceEvent, bool useSpecificPins)
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
        public void Initialize_ConfigureTMUEdgeArmPolaritySucceeds(TmuPolarity polarity, bool useSpecificPins)
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
        public void Initialize_ConfigureTMUSamplesToAcquireSucceeds(long sampleNumber, bool useSpecificPins)
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
        public void Initialize_ConfigureTMUSampleTimeoutSucceeds(double timeout, bool useSpecificPins)
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
        public void Initialize_ConfigurePeriodMeasurementSucceeds(TmuPolarity edgeType, long samplesToAcquire, bool useSpecificPins)
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
        public void Initialize_ConfigureTMURiseTimeMeasurementSucceeds(long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMURiseTimeMeasurement(samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMURiseTimeMeasurementWithArmTypeSucceeds()
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
        public void Initialize_ConfigureTMUFallTimeMeasurementSucceeds(long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUFallTimeMeasurement(samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUFallTimeMeasurementWithArmTypeSucceeds()
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
        public void Initialize_ConfigureTMUDutyCycleMeasurementSucceeds(TmuDutyCycle dutyCycleType, long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUDutyCycleMeasurement(dutyCycleType, samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUDutyCycleMeasurementWithInvalidDutyCycleTypeThrowsArgumentOutOfRangeException()
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
        public void Initialize_ConfigureTMUPulseWidthMeasurementSucceeds(TmuPulseWidth pulseWidthType, long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InitializeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUPulseWidthMeasurement(pulseWidthType, samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUPulseWidthMeasurementWithInvalidPulseWidthTypeThrowsArgumentOutOfRangeException()
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
        public void Initialize_ConfigureSkewMeasurementSucceeds(TmuPolarity edgeType)
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
        public void Initialize_ConfigureSkewMeasurementWithUniquePinsSucceeds(TmuPolarity edgeType)
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
        public void Initialize_ConfigureTMUStartSourceWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStartSource("C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStopSourceWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStopSource("C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStartSourceEventWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStartSourceEvent(TmuSourceEvent.Vol, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStopSourceEventWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStopSourceEvent(TmuSourceEvent.Voh, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStartSourceEventPolarityWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStartSourceEventPolarity(TmuPolarity.RisingEdge, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUStopSourceEventPolarityWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUStopSourceEventPolarity(TmuPolarity.FallingEdge, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUArmTypeWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource("C0");
            sessionsBundle.ConfigureTMUEdgeArmSource("C0");
            sessionsBundle.ConfigureTMUEdgeArmPolarity(TmuPolarity.RisingEdge, "C0");

            sessionsBundle.ConfigureTMUArmType(TmuArmType.Edge, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUEdgeArmSourceWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUEdgeArmSource("C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUEdgeArmSourceEventWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUEdgeArmSourceEvent(TmuSourceEvent.Vol, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUEdgeArmPolarityWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUEdgeArmPolarity(TmuPolarity.RisingEdge, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUSamplesToAcquireWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSamplesToAcquire(10, "C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUSampleTimeoutWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSampleTimeout(10.0, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigurePeriodMeasurementWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigurePeriodMeasurement(TmuPolarity.RisingEdge, 1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMURiseTimeMeasurementWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMURiseTimeMeasurement(1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUFallTimeMeasurementWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUFallTimeMeasurement(1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUDutyCycleMeasurementWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUDutyCycleMeasurement(TmuDutyCycle.High, 1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUPulseWidthMeasurementWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUPulseWidthMeasurement(TmuPulseWidth.High, 1, "C0");
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_ConfigureTMUSkewMeasurementWithSinglePinPairSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();

            sessionsBundle.ConfigureTMUSkewMeasurement("C0", "C1", TmuPolarity.RisingEdge, 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_AssignTMUResourcesAndClearTMUAssignmentWithSinglePinSucceeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            sessionsBundle.AssignTMUResources("C0");
            sessionsBundle.ClearTMUAssignment("C0");
        }

        [Fact]
        public void Initialize_EnableAndDisableTMUWithSinglePinSucceeds()
        {
            var sessionsBundle = InitializeAndCreateBundle();
            sessionsBundle.ConfigurePeriodMeasurement(TmuPolarity.RisingEdge, 1, "C0");

            sessionsBundle.EnableTMU("C0");
            sessionsBundle.DisableTMU("C0");
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Initialize_TMUInitiateAndAbortWithSinglePinSucceeds()
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
        public void Initialize_ConfigurePeriodMeasurementWithArmSettingSucceeds(TmuArmSetting armSetting)
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
        public void Initialize_ConfigureTMURiseTimeMeasurementWithArmSettingSucceeds(TmuArmSetting armSetting)
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
        public void Initialize_ConfigureTMUFallTimeMeasurementWithArmSettingSucceeds(TmuArmSetting armSetting)
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
        public void Initialize_ConfigureTMUDutyCycleMeasurementWithArmSettingSucceeds(TmuArmSetting armSetting)
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
        public void Initialize_ConfigureTMUPulseWidthMeasurementWithArmSettingSucceeds(TmuArmSetting armSetting)
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
        public void Initialize_ConfigureTMUPulseWidthMeasurementWithArmSettingAndSinglePinSucceeds()
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
        public void Initialize_ConfigureTMUSkewMeasurementWithArmSettingSucceeds(TmuArmSetting armSetting)
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
        public void Initialize_ConfigureTMUSkewMeasurementWithArmSettingAndSinglePinPairSucceeds(TmuArmSetting armSetting)
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Inititalize_GetTMUStartSourceReturnsNonEmptyChannelString(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUStartSource(pinNames);
            var expected = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) => sitePinInfo.IndividualChannelString);

            var result = sessionsBundle.GetTMUStartSource(pinNames);

            Assert.Equal(expected.ExtractSite(0)["C0"], result.ExtractSite(0)["C0"]);
            if (useSpecificPins)
            {
                Assert.Null(result.ExtractSite(0)["C1"]);
            }
            else
            {
                Assert.Equal(expected.ExtractSite(0)["C1"], result.ExtractSite(0)["C1"]);
            }
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Stop Source Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Inititalize_GetTMUStopSourceReturnsNonEmptyChannelString(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUStopSource(pinNames);
            var expected = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) => sitePinInfo.IndividualChannelString);

            var result = sessionsBundle.GetTMUStopSource(pinNames);

            Assert.Equal(expected.ExtractSite(0)["C0"], result.ExtractSite(0)["C0"]);
            if (useSpecificPins)
            {
                Assert.Null(result.ExtractSite(0)["C1"]);
            }
            else
            {
                Assert.Equal(expected.ExtractSite(0)["C1"], result.ExtractSite(0)["C1"]);
            }
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Start Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol, false)]
        [InlineData(TmuSourceEvent.Vol, true)]
        [InlineData(TmuSourceEvent.Voh, false)]
        [InlineData(TmuSourceEvent.Voh, true)]
        public void Inititalize_GetTMUStartSourceEventReturnsConfiguredValue(TmuSourceEvent sourceEvent, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUStartSourceEvent(sourceEvent, pinNames);

            var result = sessionsBundle.GetTMUStartSourceEvent(pinNames);

            Assert.Equal(sourceEvent, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Stop Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol, false)]
        [InlineData(TmuSourceEvent.Vol, true)]
        [InlineData(TmuSourceEvent.Voh, false)]
        [InlineData(TmuSourceEvent.Voh, true)]
        public void Inititalize_GetTMUStopSourceEventReturnsConfiguredValue(TmuSourceEvent sourceEvent, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUStopSourceEvent(sourceEvent, pinNames);

            var result = sessionsBundle.GetTMUStopSourceEvent(pinNames);

            Assert.Equal(sourceEvent, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Start Source Event Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge, false)]
        [InlineData(TmuPolarity.RisingEdge, true)]
        [InlineData(TmuPolarity.FallingEdge, false)]
        [InlineData(TmuPolarity.FallingEdge, true)]
        public void Inititalize_GetTMUStartSourceEventPolarityReturnsConfiguredValue(TmuPolarity polarity, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUStartSourceEventPolarity(polarity, pinNames);

            var result = sessionsBundle.GetTMUStartSourceEventPolarity(pinNames);

            Assert.Equal(polarity, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Stop Source Event Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge, false)]
        [InlineData(TmuPolarity.RisingEdge, true)]
        [InlineData(TmuPolarity.FallingEdge, false)]
        [InlineData(TmuPolarity.FallingEdge, true)]
        public void Inititalize_GetTMUStopSourceEventPolarityReturnsConfiguredValue(TmuPolarity polarity, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUStopSourceEventPolarity(polarity, pinNames);

            var result = sessionsBundle.GetTMUStopSourceEventPolarity(pinNames);

            Assert.Equal(polarity, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Enabled Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Inititalize_GetTMUEnabledReturnsTrueAfterEnableTMU(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.EnableTMU(pinNames);

            var result = sessionsBundle.GetTMUEnabled(pinNames);

            Assert.True(result.ExtractSite(0)["C0"]);
            sessionsBundle.DisableTMU(pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Inititalize_GetTMUEnabledReturnsFalseAfterDisableTMU(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.EnableTMU(pinNames);
            sessionsBundle.DisableTMU(pinNames);

            var result = sessionsBundle.GetTMUEnabled(pinNames);

            Assert.False(result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Arm Type Tests

        [Theory]
        [InlineData(TmuArmType.Immediate, false)]
        [InlineData(TmuArmType.Immediate, true)]
        [InlineData(TmuArmType.Edge, false)]
        [InlineData(TmuArmType.Edge, true)]
        public void Inititalize_GetTMUArmTypeReturnsConfiguredValue(TmuArmType armType, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUStartSource(pinNames);
            sessionsBundle.ConfigureTMUEdgeArmSource(pinNames);
            sessionsBundle.ConfigureTMUEdgeArmPolarity(TmuPolarity.RisingEdge, pinNames);
            sessionsBundle.ConfigureTMUArmType(armType, pinNames);

            var result = sessionsBundle.GetTMUArmType(pinNames);

            Assert.Equal(armType, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Edge Arm Source Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Inititalize_GetTMUEdgeArmSourceReturnsNonEmptyChannelString(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUEdgeArmSource(pinNames);
            var expected = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) => sitePinInfo.IndividualChannelString);

            var result = sessionsBundle.GetTMUEdgeArmSource(pinNames);

            Assert.Equal(expected.ExtractSite(0)["C0"], result.ExtractSite(0)["C0"]);
            if (useSpecificPins)
            {
                Assert.Null(result.ExtractSite(0)["C1"]);
            }
            else
            {
                Assert.Equal(expected.ExtractSite(0)["C1"], result.ExtractSite(0)["C1"]);
            }
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Edge Arm Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol, false)]
        [InlineData(TmuSourceEvent.Vol, true)]
        [InlineData(TmuSourceEvent.Voh, false)]
        [InlineData(TmuSourceEvent.Voh, true)]
        public void Inititalize_GetTMUEdgeArmSourceEventReturnsConfiguredValue(TmuSourceEvent sourceEvent, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUEdgeArmSourceEvent(sourceEvent, pinNames);

            var result = sessionsBundle.GetTMUEdgeArmSourceEvent(pinNames);

            Assert.Equal(sourceEvent, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Edge Arm Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge, false)]
        [InlineData(TmuPolarity.RisingEdge, true)]
        [InlineData(TmuPolarity.FallingEdge, false)]
        [InlineData(TmuPolarity.FallingEdge, true)]
        public void Inititalize_GetTMUEdgeArmPolarityReturnsConfiguredValue(TmuPolarity polarity, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUEdgeArmPolarity(polarity, pinNames);

            var result = sessionsBundle.GetTMUEdgeArmPolarity(pinNames);

            Assert.Equal(polarity, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Samples To Acquire Tests

        [Theory]
        [InlineData(100, false)]
        [InlineData(1, false)]
        [InlineData(50, true)]
        public void Inititalize_GetTMUSamplesToAcquireReturnsConfiguredValue(long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUSamplesToAcquire(samplesToAcquire, pinNames);

            var result = sessionsBundle.GetTMUSamplesToAcquire(pinNames);

            Assert.Equal(samplesToAcquire, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Sample Timeout Tests

        [Theory]
        [InlineData(10.0, false)]
        [InlineData(0.001, false)]
        [InlineData(10.0, true)]
        public void Inititalize_GetTMUSampleTimeoutReturnsConfiguredValue(double timeout, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;
            sessionsBundle.ConfigureTMUSampleTimeout(timeout, pinNames);

            var result = sessionsBundle.GetTMUSampleTimeout(pinNames);

            Assert.Equal(timeout, result.ExtractSite(0)["C0"]);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Start Input Debounce Time Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Inititalize_GetTMUStartInputDebounceTimeReturnsNonNegativeValue(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            var result = sessionsBundle.GetTMUStartInputDebounceTime(pinNames);

            Assert.True(result.ExtractSite(0)["C0"] >= 0);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Stop Input Debounce Time Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Inititalize_GetTMUStopInputDebounceTimeReturnsNonNegativeValue(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            var result = sessionsBundle.GetTMUStopInputDebounceTime(pinNames);

            Assert.True(result.ExtractSite(0)["C0"] >= 0);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Get TMU Count Tests

        [Fact]
        public void Inititalize_GetTMUCountReturnsPositiveValue()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            var result = sessionsBundle.GetTMUCount();

            Assert.True(result.ExtractSite(0)["C0"] > 0);
            sessionsBundle.ClearTMUAssignment();
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
