using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.MixedSignalLibrary.CoreStepMethods;
using static NationalInstruments.MixedSignalLibrary.GeneralUtilities;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower.Measurement;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower.Source;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital.PPMU;
using static NationalInstruments.MixedSignalLibrary.TypeDefinitions;

namespace NationalInstruments.MixedSignalLibrary
{
    /// <summary>
    /// Defines entry points for mixed signal steps.
    /// </summary>
    public static class Steps
    {
        #region Setup and Cleanup Steps

        /// <summary>
        /// Initializes NI-DCPower sessions.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void DCPowerInitialize(ISemiconductorModuleContext tsmContext)
        {
            try
            {
                NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower.InitializeAndClose.Initialize(tsmContext);
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Closes all NI-DCPower sessions.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void DCPowerClose(ISemiconductorModuleContext tsmContext)
        {
            try
            {
                NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower.InitializeAndClose.Close(tsmContext);
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Initializes NI-Digital sessions.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="levelsSheetToApply">The name of the levels sheet to apply.</param>
        /// <param name="timingSheetToApply">The name of the timing sheet to apply.</param>
        public static void DigitalInitialize(ISemiconductorModuleContext tsmContext, string levelsSheetToApply = "", string timingSheetToApply = "")
        {
            try
            {
                NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital.InitializeAndClose.Initialize(tsmContext, levelsSheetToApply, timingSheetToApply);
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Closes NI-Digital sessions.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void DigitalClose(ISemiconductorModuleContext tsmContext)
        {
            try
            {
                NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital.InitializeAndClose.Close(tsmContext);
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        #endregion Setup and Cleanup Steps

        /// <summary>
        /// Forces voltage on a set of pins and pin groups.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The set of pins and pin groups to use for the step.</param>
        /// <param name="supplyVoltages">Voltage set points used for all pins specified in the step.</param>
        /// <param name="currentLimits">Current limits, in amps, used for all pins specified in the step. The current range is automatically set to the closest range based on this limit.</param>
        /// <param name="apertureTimes">Aperture times used for the step.</param>
        /// <param name="settlingTimes">Delay time used after a force operation is complete.</param>
        /// <param name="dcPowerMeasurementSenses">DCPower measurement senses used for the step.</param>
        /// <param name="dcPowerSourceTransientResponses">DCPower source transient responses used for the step.</param>
        /// <param name="returnUntilCompletion">Whether to wait after the forcing completes before returning.</param>
        /// <returns>A task one can wait on for the completion of the voltage forcing.</returns>
        public static Task ForceVoltage(
            ISemiconductorModuleContext tsmContext,
            string[] pinsAndPinGroups,
            double[] supplyVoltages,
            double[] currentLimits,
            double[] apertureTimes,
            double[] settlingTimes,
            DCPowerMeasurementSense[] dcPowerMeasurementSenses,
            DCPowerSourceTransientResponse[] dcPowerSourceTransientResponses,
            bool returnUntilCompletion = true)
        {
            try
            {
                var pinInfo = FilterPinsAndPinGroups(tsmContext, pinsAndPinGroups);
                var parameters = new MixedSignalParameters(
                    supplyVoltages,
                    currentLimits,
                    apertureTimes,
                    settlingTimes,
                    dcPowerMeasurementSenses,
                    dcPowerSourceTransientResponses,
                    pinsAndPinGroups.Length);

                var forceVoltageTask = Task.Run(() =>
                {
                    ForceVoltageCore(new TSMSessionManager(tsmContext), pinInfo, parameters);
                });

                if (!returnUntilCompletion)
                {
                    return forceVoltageTask;
                }

                forceVoltageTask.Wait();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Forces current on a set of pins and pin groups.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The set of pins and pin groups to use for the step.</param>
        /// <param name="supplyCurrents">Current set points used for all pins specified in the step.</param>
        /// <param name="voltageLimits">Voltage limits, in volts, used for all pins specified in the step.</param>
        /// <param name="apertureTimes">Aperture times used for the step.</param>
        /// <param name="settlingTimes">Delay time used after a force operation is complete.</param>
        /// <param name="dcPowerMeasurementSenses">DCPower measurement senses used for the step.</param>
        /// <param name="dcPowerSourceTransientResponses">DCPower source transient responses used for the step.</param>
        /// <param name="returnUntilCompletion">Whether to wait after the forcing completes before returning.</param>
        /// <returns>A task one can wait on for the completion of the voltage forcing.</returns>
        public static Task ForceCurrent(
            ISemiconductorModuleContext tsmContext,
            string[] pinsAndPinGroups,
            double[] supplyCurrents,
            double[] voltageLimits,
            double[] apertureTimes,
            double[] settlingTimes,
            DCPowerMeasurementSense[] dcPowerMeasurementSenses,
            DCPowerSourceTransientResponse[] dcPowerSourceTransientResponses,
            bool returnUntilCompletion = true)
        {
            try
            {
                var pinInfo = FilterPinsAndPinGroups(tsmContext, pinsAndPinGroups);
                var parameters = new MixedSignalParameters(
                    supplyCurrents,
                    voltageLimits,
                    apertureTimes,
                    settlingTimes,
                    dcPowerMeasurementSenses,
                    dcPowerSourceTransientResponses,
                    pinsAndPinGroups.Length);

                var forceCurrentTask = Task.Run(() =>
                {
                    ForceCurrentCore(new TSMSessionManager(tsmContext), pinInfo, parameters);
                });

                if (!returnUntilCompletion)
                {
                    return forceCurrentTask;
                }

                forceCurrentTask.Wait();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Measures voltage, current or both on a set of pins and pin groups.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The set of pins and pin groups to use for the step.</param>
        /// <param name="measurementType">Whether to measure voltage, current or both.</param>
        public static void Measure(ISemiconductorModuleContext tsmContext, string[] pinsAndPinGroups, MeasurementType measurementType)
        {
            try
            {
                var pinInfo = FilterPinsAndPinGroups(tsmContext, pinsAndPinGroups);
                var sessionManager = new TSMSessionManager(tsmContext);
                MeasureCore(sessionManager, pinInfo, measurementType, tsmContext);
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Forces voltage and then measures voltage, current or both on a set of pins and pin groups.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The set of pins and pin groups to use for the step.</param>
        /// <param name="supplyVoltages">Voltage set points used for all pins specified in the step.</param>
        /// <param name="currentLimits">Current limits, in amps, used for all pins specified in the step. The current range is automatically set to the closest range based on this limit.</param>
        /// <param name="apertureTimes">Aperture times used for the step.</param>
        /// <param name="settlingTimes">Delay time used after a force operation is complete.</param>
        /// <param name="dcPowerMeasurementSenses">DCPower measurement senses used for the step.</param>
        /// <param name="dcPowerSourceTransientResponses">DCPower source transient responses used for the step.</param>
        /// <param name="measurementType">Whether to measure voltage, current or both.</param>
        public static void ForceVoltageAndMeasure(
            ISemiconductorModuleContext tsmContext,
            string[] pinsAndPinGroups,
            double[] supplyVoltages,
            double[] currentLimits,
            double[] apertureTimes,
            double[] settlingTimes,
            DCPowerMeasurementSense[] dcPowerMeasurementSenses,
            DCPowerSourceTransientResponse[] dcPowerSourceTransientResponses,
            MeasurementType measurementType)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var pinInfo = FilterPinsAndPinGroups(tsmContext, pinsAndPinGroups);
                var parameters = new MixedSignalParameters(
                    supplyVoltages,
                    currentLimits,
                    apertureTimes,
                    settlingTimes,
                    dcPowerMeasurementSenses,
                    dcPowerSourceTransientResponses,
                    pinsAndPinGroups.Length);

                ForceVoltageCore(sessionManager, pinInfo, parameters);
                MeasureCore(sessionManager, pinInfo, measurementType, tsmContext);
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Forces current and then measures voltage, current or both on a set of pins and pin groups.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The set of pins and pin groups to use for the step.</param>
        /// <param name="supplyCurrents">Current set points used for all pins specified in the step.</param>
        /// <param name="voltageLimits">Voltage limits, in volts, used for all pins specified in the step</param>
        /// <param name="apertureTimes">Aperture times used for the step.</param>
        /// <param name="settlingTimes">Delay time used after a force operation is complete.</param>
        /// <param name="dcPowerMeasurementSenses">DCPower measurement senses used for the step.</param>
        /// <param name="dcPowerSourceTransientResponses">DCPower source transient responses used for the step.</param>
        /// <param name="measurementType">Whether to measure voltage, current or both.</param>
        public static void ForceCurrentAndMeasure(
            ISemiconductorModuleContext tsmContext,
            string[] pinsAndPinGroups,
            double[] supplyCurrents,
            double[] voltageLimits,
            double[] apertureTimes,
            double[] settlingTimes,
            DCPowerMeasurementSense[] dcPowerMeasurementSenses,
            DCPowerSourceTransientResponse[] dcPowerSourceTransientResponses,
            MeasurementType measurementType)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var pinInfo = FilterPinsAndPinGroups(tsmContext, pinsAndPinGroups);
                var parameters = new MixedSignalParameters(
                    supplyCurrents,
                    voltageLimits,
                    apertureTimes,
                    settlingTimes,
                    dcPowerMeasurementSenses,
                    dcPowerSourceTransientResponses,
                    pinsAndPinGroups.Length);

                ForceCurrentCore(sessionManager, pinInfo, parameters);
                MeasureCore(sessionManager, pinInfo, measurementType, tsmContext);
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Powers down all DC and Digital channels.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The set of pins and pin groups to use for the step.</param>
        /// <param name="settlingTime">Delay time used after the channels are turned off.</param>
        public static void PowerDown(ISemiconductorModuleContext tsmContext, string[] pinsAndPinGroups, double settlingTime = 0.001)
        {
            try
            {
                var pinInfo = FilterPinsAndPinGroups(tsmContext, pinsAndPinGroups);
                PowerDownCore(new TSMSessionManager(tsmContext), pinInfo, settlingTime);
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Bursts a digital pattern to a DUT.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The set of pins and pin groups to use for the step.</param>
        /// <param name="patternName">The name of the pattern to burst.</param>
        /// <param name="captureWaveformName">Fetch a capture waveform with the given name if this parameter is specified.</param>
        /// <param name="numberOfWaveformSamplesToRead">The number of samples to read when fetching a capture waveform after bursting the pattern.</param>
        public static void BurstPattern(ISemiconductorModuleContext tsmContext, string[] pinsAndPinGroups, string patternName, string captureWaveformName = "", int numberOfWaveformSamplesToRead = 0)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var pinInfo = FilterPinsAndPinGroups(tsmContext, pinsAndPinGroups);
                var sessionsBundle = sessionManager.GetDigitalSessionsBundle(pinInfo.DigitalPinsFlat);

                sessionsBundle.BurstPatternAndPublishResults(patternName, publishedDataId: "Pattern Pass/Fail Result");
                sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.PinSet.GetFailCount().ToDoubleArray(), "Pattern Fail Count");

                if (!string.IsNullOrEmpty(captureWaveformName))
                {
                    var capturedPerInstrumentWaveforms = sessionsBundle.FetchCaptureWaveform(captureWaveformName, numberOfWaveformSamplesToRead);
                    var capturedPerSiteWaveforms = (sessionsBundle.PinQueryContext as NIDigitalPatternPinQueryContext).PerInstrumentToPerSiteWaveforms(capturedPerInstrumentWaveforms);
                    var capturedPerSiteWaveformsInString = capturedPerSiteWaveforms.Select(samples => string.Join(separator: null, samples.Select(sample => sample.ToString(CultureInfo.InvariantCulture))));
                    tsmContext.PublishPerSite(capturedPerSiteWaveformsInString.ToArray(), "Captured Waveform");
                }
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        private static readonly double[] _voltage0V = new double[] { 0.0 };

        /// <summary>
        /// Conducts continuity test.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The set of pins and pin groups to use for the step.</param>
        /// <param name="supplyCurrents">Current set points used for all pins specified in the step.</param>
        /// <param name="voltageLimits">Voltage limits, in volts, used for all pins specified in the step</param>
        /// <param name="apertureTimes">Aperture times used for the step.</param>
        /// <param name="settlingTimes">Delay time used after a force operation is complete.</param>
        /// <param name="dcPowerMeasurementSenses">DCPower measurement senses used for the step.</param>
        /// <param name="dcPowerSourceTransientResponses">DCPower source transient responses used for the step.</param>
        public static void ContinuityTest(
            ISemiconductorModuleContext tsmContext,
            string[] pinsAndPinGroups,
            double[] supplyCurrents,
            double[] voltageLimits,
            double[] apertureTimes,
            double[] settlingTimes,
            DCPowerMeasurementSense[] dcPowerMeasurementSenses,
            DCPowerSourceTransientResponse[] dcPowerSourceTransientResponses)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var pinInfo = FilterPinsAndPinGroups(tsmContext, pinsAndPinGroups);
                var parameters = new MixedSignalParameters(
                    supplyCurrents,
                    voltageLimits,
                    apertureTimes,
                    settlingTimes,
                    dcPowerMeasurementSenses,
                    dcPowerSourceTransientResponses,
                    pinsAndPinGroups.Length);
                var parametersFor0V = new MixedSignalParameters(
                    _voltage0V,
                    supplyCurrents,
                    apertureTimes, // Could consider making it configurable in the signature to improve test time later
                    settlingTimes, // Could consider making it configurable in the signature to improve test time later
                    dcPowerMeasurementSenses,
                    dcPowerSourceTransientResponses);

                var smuSettings = PrepareSMUSettings(pinInfo, parameters, DCPowerSourceOutputFunction.DCCurrent);
                var smuSettingsFor0V = PrepareSMUSettings(pinInfo, parametersFor0V, DCPowerSourceOutputFunction.DCVoltage);
                var ppmuSettings = PreparePPMUSettings(pinInfo, parameters, PpmuOutputFunction.DCCurrent);
                var ppmuSettingsFor0V = PreparePPMUSettings(pinInfo, parametersFor0V, PpmuOutputFunction.DCVoltage);

                ForceVoltageCore(sessionManager, pinInfo, parametersFor0V);

                string publishedDataId = "Continuity";
                // The nature of continuity test requires to loop all pins sequentially.
                for (int i = 0; i < pinInfo.DCPins.Count; i++)
                {
                    var pins = pinInfo.DCPins[i];
                    var pinIndex = pinInfo.DCPinIndexes[i];
                    foreach (string pin in pins)
                    {
                        var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(pin);
                        sessionsBundle.ForceCurrentSymmetricLimit(smuSettings);
                        sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.Measure(), publishedDataId1: publishedDataId);

                        // The nature of continuity test requires to force current pin to 0V before starting to test the next pin.
                        sessionsBundle.ForceVoltageSymmetricLimit(smuSettingsFor0V);
                    }
                }
                // The nature of continuity test requires to loop all pins sequentially.
                for (int i = 0; i < pinInfo.DigitalPins.Count; i++)
                {
                    var pins = pinInfo.DigitalPins[i];
                    var pinIndex = pinInfo.DigitalPinIndexes[i];
                    foreach (string pin in pins)
                    {
                        var sessionsBundle = sessionManager.GetDigitalSessionsBundle(pin);
                        sessionsBundle.ForceCurrent(ppmuSettings);
                        sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.Measure(MeasurementType.Voltage), publishedDataId1: publishedDataId);

                        // The nature of continuity test requires to force current pin to 0V before starting to test the next pin.
                        sessionsBundle.ForceVoltage(ppmuSettingsFor0V);
                    }
                }

                PowerDownCore(sessionManager, pinInfo, settlingTimes.Max());
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Conducts leakage test.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The set of pins and pin groups to use for the step.</param>
        /// <param name="supplyVoltages">Voltage set points used for all pins specified in the step.</param>
        /// <param name="currentLimits">Current limits, in amps, used for all pins specified in the step. The current range is automatically set to the closest range based on this limit.</param>
        /// <param name="apertureTimes">Aperture times used for the step.</param>
        /// <param name="settlingTimes">Delay time used after a force operation is complete.</param>
        /// <param name="dcPowerMeasurementSenses">DCPower measurement senses used for the step.</param>
        /// <param name="dcPowerSourceTransientResponses">DCPower source transient responses used for the step.</param>
        public static void LeakageTest(
            ISemiconductorModuleContext tsmContext,
            string[] pinsAndPinGroups,
            double[] supplyVoltages,
            double[] currentLimits,
            double[] apertureTimes,
            double[] settlingTimes,
            DCPowerMeasurementSense[] dcPowerMeasurementSenses,
            DCPowerSourceTransientResponse[] dcPowerSourceTransientResponses)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var pinInfo = FilterPinsAndPinGroups(tsmContext, pinsAndPinGroups);
                var parameters = new MixedSignalParameters(
                    supplyVoltages,
                    currentLimits,
                    apertureTimes,
                    settlingTimes,
                    dcPowerMeasurementSenses,
                    dcPowerSourceTransientResponses,
                    pinsAndPinGroups.Length);
                var parametersFor0V = new MixedSignalParameters(
                    _voltage0V,
                    currentLimits,
                    apertureTimes, // Could consider making it configurable in the signature to improve test time later
                    settlingTimes, // Could consider making it configurable in the signature to improve test time later
                    dcPowerMeasurementSenses,
                    dcPowerSourceTransientResponses);

                var smuSettings = PrepareSMUSettings(pinInfo, parameters, DCPowerSourceOutputFunction.DCVoltage);
                var smuSettingsFor0V = PrepareSMUSettings(pinInfo, parametersFor0V, DCPowerSourceOutputFunction.DCVoltage);
                var ppmuSettings = PreparePPMUSettings(pinInfo, parameters, PpmuOutputFunction.DCVoltage);
                var ppmuSettingsFor0V = PreparePPMUSettings(pinInfo, parametersFor0V, PpmuOutputFunction.DCVoltage);

                ForceVoltageCore(sessionManager, pinInfo, parametersFor0V);

                string publishedDataId = "Leakage";
                // The nature of leakage test requires to loop all pins sequentially.
                for (int i = 0; i < pinInfo.DCPins.Count; i++)
                {
                    var pins = pinInfo.DCPins[i];
                    var pinIndex = pinInfo.DCPinIndexes[i];
                    foreach (string pin in pins)
                    {
                        var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(pin);
                        sessionsBundle.ForceVoltageSymmetricLimit(smuSettings);
                        sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.Measure(), publishedDataId2: publishedDataId);

                        // The nature of continuity test requires to force current pin to 0V before starting to test the next pin.
                        sessionsBundle.ForceVoltageSymmetricLimit(smuSettingsFor0V);
                    }
                }
                // The nature of leakage test requires to loop all pins sequentially.
                for (int i = 0; i < pinInfo.DigitalPins.Count; i++)
                {
                    var pins = pinInfo.DigitalPins[i];
                    var pinIndex = pinInfo.DigitalPinIndexes[i];
                    foreach (string pin in pins)
                    {
                        var sessionsBundle = sessionManager.GetDigitalSessionsBundle(pin);
                        sessionsBundle.ForceVoltage(ppmuSettings);
                        sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.Measure(MeasurementType.Current), publishedDataId2: publishedDataId);

                        // The nature of continuity test requires to force current pin to 0V before starting to test the next pin.
                        sessionsBundle.ForceVoltage(ppmuSettingsFor0V);
                    }
                }

                PowerDownCore(sessionManager, pinInfo, settlingTimes.Max());
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }
    }
}
