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
        public void Inititalize_ConfigureSkewMeasurementWithOverlappingReferencAndTargetPinsThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUSkewMeasurement(
                    new[] { "C0" }, new[] { "C0" }, TmuPolarity.RisingEdge, 1));
            sessionsBundle.DisableTMU();
            sessionsBundle.ClearTMUAssignment();
        }

        [Fact]
        public void Inititalize_ConfigureSkewMeasurementWithMultiplePinsAndEitherEdge_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = InititalzeAndCreateBundle();

            Assert.Throws<NISemiconductorTestException>(() =>
                sessionsBundle.ConfigureTMUSkewMeasurement(
                    new[] { "C0" }, new[] { "C1" }, TmuPolarity.EitherEdge, 1));
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

            var result = sessionsBundle.GetTMUStartSource(pinNames);

            Assert.False(string.IsNullOrEmpty(result.ExtractSite(0)["C0"]));
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

            var result = sessionsBundle.GetTMUStopSource(pinNames);

            Assert.False(string.IsNullOrEmpty(result.ExtractSite(0)["C0"]));
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

            var result = sessionsBundle.GetTMUEdgeArmSource(pinNames);

            Assert.False(string.IsNullOrEmpty(result.ExtractSite(0)["C0"]));
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
