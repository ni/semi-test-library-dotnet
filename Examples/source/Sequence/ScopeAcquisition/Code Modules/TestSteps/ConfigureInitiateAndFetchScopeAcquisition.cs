using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ScopeAcquisition
{
    /// <summary>
    /// Provides test step helpers for scope configuration and waveform acquisition within a semiconductor module
    /// context.
    /// </summary>
    /// <remarks>Contains static operations that configure scope vertical and electrical characteristics,
    /// apply trigger settings, initiate acquisition, and retrieve waveform timing data for a specified site and pin or
    /// pin group.</remarks>
    public static class TestSteps
    {
        /// <summary>
        /// Configures the scope settings, such as input range, offset, probe attenuation,etc then initiates acquisition
        /// for the target scope pins. Then the method fetches the waveform data and returns the sample interval for the acquired waveform.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">Name of the scope pin or pin group to be used for waveform acquisition.</param>
        /// <param name="range">The range to be applied to the scope.</param>
        /// <param name="offset">The offset to be applied to the scope.</param>
        /// <param name="probeAttenuation">The probe attenuation to be applied to the scope.</param>
        /// <param name="enableTimeInterleavedSampling">A boolean value indicating whether to enable time-interleaved sampling.</param>
        /// <param name="inputImpedance">The input impedance of the channel, in ohms.</param>
        /// <param name="inputFrequencyMax">The maximum input frequency of the channel, in hertz.</param>
        /// <param name="immediateTrigger">A boolean value indicating whether to configure an immediate trigger or a digital trigger.</param>
        public static void ConfigureInitiateAndFetchScopeAcquisition(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            double range,
            double offset,
            double probeAttenuation,
            bool enableTimeInterleavedSampling,
            double inputImpedance,
            double inputFrequencyMax,
            bool immediateTrigger)
        {
            var verticalSettings = new VerticalSettings
            {
                Range = range,
                Offset = offset,
                ProbeAttenuation = probeAttenuation,
                EnableTimeInterleavedSampling = enableTimeInterleavedSampling,
            };
            ElectricalCharacteristics electricalCharacteristics = new ElectricalCharacteristics
            {
                InputImpedance = inputImpedance,
                InputFrequencyMax = inputFrequencyMax,
            };
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
            scopeBundle.ConfigureVertical(verticalSettings);
            scopeBundle.ConfigureElectricalCharacteristics(electricalCharacteristics);
            scopeBundle.Commit();
            if (immediateTrigger)
            {
                scopeBundle.ConfigureTriggerImmediate();
            }
            else
            {
                DigitalTriggerSettings digitalTriggerSettings = new DigitalTriggerSettings
                {
                    TriggerSlope = ScopeTriggerSlope.Positive,
                    HoldOff = PrecisionTimeSpan.FromSeconds(0.0),
                    Delay = PrecisionTimeSpan.FromSeconds(0.0),
                };
                scopeBundle.ConfigureTriggerDigital(digitalTriggerSettings);
            }
            scopeBundle.Initiate();
            var waveforms = scopeBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(50.0), 1000);

            PinSiteData<double> frequencies = waveforms.Select(wfm =>
            {
                MeasureTone(wfm.GetRawData(), 1.0 / wfm.Timing.SampleInterval.TotalSeconds, out double frequencyHz, out _);
                return frequencyHz;
            });
            PinSiteData<double> amplitudes = waveforms.Select(wfm =>
            {
                MeasureTone(wfm.GetRawData(), 1.0 / wfm.Timing.SampleInterval.TotalSeconds, out _, out double amplitude);
                return amplitude;
            });
            tsmContext.PublishResults(frequencies, "Frequency");
            tsmContext.PublishResults(amplitudes, "Amplitude");
        }

        private static void MeasureTone(
            double[] samples,
            double sampleRateHz,
            out double frequencyHz,
            out double amplitude)
        {
            double mean = samples.Average();
            amplitude = (samples.Max() - samples.Min()) / 2.0;

            var crossingTimes = new List<double>();
            for (int i = 1; i < samples.Length; i++)
            {
                double y0 = samples[i - 1] - mean;
                double y1 = samples[i] - mean;

                if (y0 <= 0 && y1 > 0)
                {
                    // Interpolate the crossing for better timing resolution.
                    double fraction = -y0 / (y1 - y0);
                    crossingTimes.Add((i - 1 + fraction) / sampleRateHz);
                }
            }

            if (crossingTimes.Count < 2)
            {
                throw new InvalidOperationException("Not enough cycles to estimate frequency.");
            }

            double elapsed = crossingTimes[crossingTimes.Count - 1] - crossingTimes[0];
            frequencyHz = (crossingTimes.Count - 1) / elapsed;
        }
    }
}
