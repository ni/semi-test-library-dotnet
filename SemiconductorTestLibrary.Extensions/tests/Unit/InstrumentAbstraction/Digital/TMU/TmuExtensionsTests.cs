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
        public void Inititalize_ConfigureTMUStartSourceSucceeds(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUStartSource(pinNames);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Configure TMU Stop Source Tests

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Inititalize_ConfigureTMUStopSourceSucceeds(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUStartSourceEventSucceeds(TmuSourceEvent sourceEvent, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUStopSourceEventSucceeds(TmuSourceEvent sourceEvent, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUStartSourceEventPolaritySucceeds(TmuPolarity polarity, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUStopSourceEventPolaritySucceeds(TmuPolarity polarity, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUArmTypeSucceeds(TmuArmType armType, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUEdgeArmSourceSucceeds(bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUEdgeArmSourceEventSucceeds(TmuSourceEvent sourceEvent, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUEdgeArmPolaritySucceeds(TmuPolarity polarity, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUSamplesToAcquireSucceeds(long sampleNumber, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigureTMUSampleTimeoutSucceeds(double timeout, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
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
        public void Inititalize_ConfigurePeriodMeasurementSucceeds(TmuPolarity edgeType, long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigurePeriodMeasurement(edgeType, samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigurePeriodMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigurePeriodMeasurementWithEitherEdge_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureTMURiseTimeMeasurementSucceeds(long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMURiseTimeMeasurement(samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureTMURiseTimeMeasurementWithArmTypeSucceeds()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            sessionsBundle.ConfigureTMURiseTimeMeasurement(samplesToAcquire: 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureTMURiseTimeMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureTMUFallTimeMeasurementSucceeds(long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUFallTimeMeasurement(samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureTMUFallTimeMeasurementWithArmTypeSucceeds()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            sessionsBundle.ConfigureTMUFallTimeMeasurement(samplesToAcquire: 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureTMUFallTimeMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureTMUDutyCycleMeasurementSucceeds(TmuDutyCycle dutyCycleType, long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUDutyCycleMeasurement(dutyCycleType, samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureTMUDutyCycleMeasurementWithInvalidDutyCycleTypeThrowsArgumentOutOfRangeException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUDutyCycleMeasurement((TmuDutyCycle)999, 1));
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureTMUDutyCycleMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureTMUPulseWidthMeasurementSucceeds(TmuPulseWidth pulseWidthType, long samplesToAcquire, bool useSpecificPins)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            var pinNames = useSpecificPins ? new string[] { "C0" } : null;

            sessionsBundle.ConfigureTMUPulseWidthMeasurement(pulseWidthType, samplesToAcquire, pinNames: pinNames);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureTMUPulseWidthMeasurementWithInvalidPulseWidthTypeThrowsArgumentOutOfRangeException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUPulseWidthMeasurement((TmuPulseWidth)999, 1));
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureTMUPulseWidthMeasurementWithPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureSkewMeasurementSucceeds(TmuPolarity edgeType)
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0" }, new string[] { "C1" }, edgeType, 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureSkewMeasurementWithEitherEdge_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0" }, new string[] { "C1" }, TmuPolarity.EitherEdge, 1));
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureSkewMeasurementWithSamePinAsReferenceAndTarget_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { "C0" }, new string[] { "C0" }, TmuPolarity.RisingEdge, 1));
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureSkewMeasurementWithMismatchedReferenceAndTargetPinCounts_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureSkewMeasurementWithNullReferencePins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureSkewMeasurementWithNullTargetPins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureSkewMeasurementWithEmptyReferencePins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureSkewMeasurementWithEmptyTargetPins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureSkewMeasurementWithReferencePinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureSkewMeasurementWithTargetPinNotInBundle_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureSkewMeasurementWithMultiplePinsSucceeds(TmuPolarity edgeType)
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            sessionsBundle.ConfigureTMUSkewMeasurement(new[] { "C0" }, new[] { "C1" }, edgeType, 1);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureSkewMeasurementWithOverlappingReferencAndTargetPins_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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
        public void Inititalize_ConfigureSkewMeasurementWithMultiplePinsAndEitherEdge_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

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

        #region Get TMU Start Source Tests

        [Fact]
        public void Inititalize_GetTMUStartSource_ReturnsConfiguredChannelString()
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource();
            var expected = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) => sitePinInfo.IndividualChannelString);

            var result = sessionsBundle.GetTMUStartSource();

            Assert.Equal(expected.ExtractSite(0)["C0"], result.ExtractSite(0)["C0"]);
            Assert.Equal(expected.ExtractSite(0)["C1"], result.ExtractSite(0)["C1"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUStartSourceWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUStartSource());
        }

        #endregion

        #region Get TMU Stop Source Tests

        [Fact]
        public void Inititalize_GetTMUStopSource_ReturnsConfiguredChannelString()
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUStopSource();
            var expected = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) => sitePinInfo.IndividualChannelString);

            var result = sessionsBundle.GetTMUStopSource();

            Assert.Equal(expected.ExtractSite(0)["C0"], result.ExtractSite(0)["C0"]);
            Assert.Equal(expected.ExtractSite(0)["C1"], result.ExtractSite(0)["C1"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUStopSourceWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUStopSource());
        }

        #endregion

        #region Get TMU Start Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol)]
        [InlineData(TmuSourceEvent.Voh)]
        public void Inititalize_GetTMUStartSourceEvent_ReturnsConfiguredValue(TmuSourceEvent sourceEvent)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSourceEvent(sourceEvent);

            var result = sessionsBundle.GetTMUStartSourceEvent();

            Assert.Equal(sourceEvent, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUStartSourceEventWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUStartSourceEvent());
        }

        #endregion

        #region Get TMU Stop Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol)]
        [InlineData(TmuSourceEvent.Voh)]
        public void Inititalize_GetTMUStopSourceEvent_ReturnsConfiguredValue(TmuSourceEvent sourceEvent)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUStopSourceEvent(sourceEvent);

            var result = sessionsBundle.GetTMUStopSourceEvent();

            Assert.Equal(sourceEvent, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUStopSourceEventWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUStopSourceEvent());
        }

        #endregion

        #region Get TMU Start Source Event Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge)]
        [InlineData(TmuPolarity.FallingEdge)]
        public void Inititalize_GetTMUStartSourceEventPolarity_ReturnsConfiguredValue(TmuPolarity polarity)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSourceEventPolarity(polarity);

            var result = sessionsBundle.GetTMUStartSourceEventPolarity();

            Assert.Equal(polarity, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUStartSourceEventPolarityWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUStartSourceEventPolarity());
        }

        #endregion

        #region Get TMU Stop Source Event Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge)]
        [InlineData(TmuPolarity.FallingEdge)]
        public void Inititalize_GetTMUStopSourceEventPolarity_ReturnsConfiguredValue(TmuPolarity polarity)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUStopSourceEventPolarity(polarity);

            var result = sessionsBundle.GetTMUStopSourceEventPolarity();

            Assert.Equal(polarity, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUStopSourceEventPolarityWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUStopSourceEventPolarity());
        }

        #endregion

        #region Get TMU Enabled Tests

        [Fact]
        public void InititalzeAndEnableTMU_GetTMUEnabled_ReturnsTrue()
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource();
            sessionsBundle.ConfigureTMUStopSource();
            sessionsBundle.EnableTMU();

            var result = sessionsBundle.GetTMUEnabled();

            Assert.True(result.ExtractSite(0)["C0"]);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void InititalzeAndDisableTMU_GetTMUEnabled_ReturnsFalse()
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource();
            sessionsBundle.ConfigureTMUStopSource();
            sessionsBundle.EnableTMU();
            sessionsBundle.DisableTMU();

            var result = sessionsBundle.GetTMUEnabled();

            Assert.False(result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUEnabledWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUEnabled());
        }

        #endregion

        #region Get TMU Arm Type Tests

        [Theory]
        [InlineData(TmuArmType.Immediate)]
        [InlineData(TmuArmType.Edge)]
        public void Inititalize_GetTMUArmType_ReturnsConfiguredValue(TmuArmType armType)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUStartSource();
            sessionsBundle.ConfigureTMUEdgeArmSource();
            sessionsBundle.ConfigureTMUEdgeArmPolarity(TmuPolarity.RisingEdge);
            sessionsBundle.ConfigureTMUArmType(armType);

            var result = sessionsBundle.GetTMUArmType();

            Assert.Equal(armType, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUArmTypeWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUArmType());
        }

        #endregion

        #region Get TMU Edge Arm Source Tests

        [Fact]
        public void Inititalize_GetTMUEdgeArmSource_ReturnsConfiguredChannelString()
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUEdgeArmSource();
            var expected = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) => sitePinInfo.IndividualChannelString);

            var result = sessionsBundle.GetTMUEdgeArmSource();

            Assert.Equal(expected.ExtractSite(0)["C0"], result.ExtractSite(0)["C0"]);
            Assert.Equal(expected.ExtractSite(0)["C1"], result.ExtractSite(0)["C1"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUEdgeArmSourceWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUEdgeArmSource());
        }

        #endregion

        #region Get TMU Edge Arm Source Event Tests

        [Theory]
        [InlineData(TmuSourceEvent.Vol)]
        [InlineData(TmuSourceEvent.Voh)]
        public void Inititalize_GetTMUEdgeArmSourceEvent_ReturnsConfiguredValue(TmuSourceEvent sourceEvent)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUEdgeArmSourceEvent(sourceEvent);

            var result = sessionsBundle.GetTMUEdgeArmSourceEvent();

            Assert.Equal(sourceEvent, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUEdgeArmSourceEventWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUEdgeArmSourceEvent());
        }

        #endregion

        #region Get TMU Edge Arm Polarity Tests

        [Theory]
        [InlineData(TmuPolarity.RisingEdge)]
        [InlineData(TmuPolarity.FallingEdge)]
        public void Inititalize_GetTMUEdgeArmPolarity_ReturnsConfiguredValue(TmuPolarity polarity)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUEdgeArmPolarity(polarity);

            var result = sessionsBundle.GetTMUEdgeArmPolarity();

            Assert.Equal(polarity, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUEdgeArmPolarityWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUEdgeArmPolarity());
        }

        #endregion

        #region Get TMU Samples To Acquire Tests

        [Theory]
        [InlineData(100)]
        [InlineData(1)]
        [InlineData(50)]
        public void Inititalize_GetTMUSamplesToAcquire_ReturnsConfiguredValue(long samplesToAcquire)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUSamplesToAcquire(samplesToAcquire);

            var result = sessionsBundle.GetTMUSamplesToAcquire();

            Assert.Equal(samplesToAcquire, result.ExtractSite(0)["C0"]);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUSamplesToAcquireWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUSamplesToAcquire());
        }

        #endregion

        #region Get TMU Sample Timeout Tests

        [Theory]
        [InlineData(10.0)]
        [InlineData(0.001)]
        public void Inititalize_GetTMUSampleTimeout_ReturnsConfiguredValue(double timeout)
        {
            var sessionsBundle = InititalzeAndCreateBundle();
            sessionsBundle.ConfigureTMUSampleTimeout(timeout);

            var result = sessionsBundle.GetTMUSampleTimeout();

            Assert.Equal(timeout, result.ExtractSite(0)["C0"]);
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUSampleTimeoutWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUSampleTimeout());
        }

        #endregion

        #region Get TMU Start Input Debounce Time Tests

        [Fact]
        public void Inititalize_GetTMUStartInputDebounceTime_ReturnsNonNegativeValue()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            var result = sessionsBundle.GetTMUStartInputDebounceTime();

            Assert.True(result.ExtractSite(0)["C0"] >= 0);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUStartInputDebounceTimeWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUStartInputDebounceTime());
        }

        #endregion

        #region Get TMU Stop Input Debounce Time Tests

        [Fact]
        public void Inititalize_GetTMUStopInputDebounceTime_ReturnsNonNegativeValue()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            var result = sessionsBundle.GetTMUStopInputDebounceTime();

            Assert.True(result.ExtractSite(0)["C0"] >= 0);
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_GetTMUStopInputDebounceTimeWithoutTMUAssigned_ThrowsNISemiconductorTestException()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTMUStopInputDebounceTime());
        }

        #endregion

        #region Get TMU Count Tests

        [Fact]
        public void Inititalize_GetTMUCount_ReturnsPositiveValue()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            var result = sessionsBundle.GetTMUCount();

            Assert.True(result.ExtractSite(0)["C0"] > 0);
            sessionsBundle.ClearTMUAssignment();
        }

        #endregion

        #region Helper Methods

        private DigitalSessionsBundle InititalzeAndCreateBundle()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager();
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.AssignTMUResources();
            return sessionsBundle;
        }

        #endregion
    }
}
