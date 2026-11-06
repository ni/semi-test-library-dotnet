using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ScopeAcquisition
{
    /// <summary>
    /// Provides helper methods for analyzing sampled scope signals.
    /// </summary>
    /// <remarks>
    /// Includes zero-crossing-based tone analysis to estimate frequency and peak-to-peak amplitude from
    /// uniformly sampled data.
    /// </remarks>
    public static class Utilities
    {
        /// <summary>
        /// Calculates the frequency and peak-to-peak amplitude of each waveform in the provided <see cref="PinSiteData{T}"/>.
        /// </summary>
        /// <param name="waveforms">The <see cref="PinSiteData{T}"/> containing the waveforms to analyze.</param>
        /// <param name="frequencies">The calculated frequencies for each pin and site.</param>
        /// <param name="amplitudes">The calculated peak-to-peak amplitudes for each pin and site.</param>
        /// <remarks>
        /// Uses zero-crossing-based tone analysis to estimate frequency and peak-to-peak amplitude from uniformly sampled data.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if there are not enough cycles to estimate frequency.</exception>
        public static void CalculateWaveformFrequencyAndPeakToPeakAmplitude(
            PinSiteData<AnalogWaveform<double>> waveforms,
            out PinSiteData<double> frequencies,
            out PinSiteData<double> amplitudes)
        {
            frequencies = new PinSiteData<double>();
            amplitudes = new PinSiteData<double>();
            foreach (var pin in waveforms.PinNames)
            {
                frequencies.AddPin(pin);
                frequencies.AddSite(waveforms.SiteNumbers);
                amplitudes.AddPin(pin);
                amplitudes.AddSite(waveforms.SiteNumbers);
                foreach (var site in waveforms.SiteNumbers)
                {
                    var waveform = waveforms.GetValue(site, pin);
                    MeasureTone(waveform.GetRawData(), 1.0 / waveform.Timing.SampleInterval.TotalSeconds, out double frequencyHz, out double amplitude);
                    frequencies.SetValue(frequencyHz, pin, site);
                    amplitudes.SetValue(amplitude, pin, site);
                }
            }
        }

        /// <summary>
        /// Measures the frequency and peak-to-peak amplitude of a tone from uniformly sampled data.
        /// </summary>
        /// <param name="samples">The uniformly sampled data points.</param>
        /// <param name="sampleRateHz">The sample rate, in Hertz.</param>
        /// <param name="frequencyHz">The measured frequency, in Hertz.</param>
        /// <param name="amplitude">The measured peak-to-peak amplitude.</param>
        /// <exception cref="InvalidOperationException">Thrown if there are not enough cycles to estimate frequency.</exception>
        private static void MeasureTone(
            double[] samples,
            double sampleRateHz,
            out double frequencyHz,
            out double amplitude)
        {
            double mean = samples.Average();
            amplitude = samples.Max() - samples.Min();

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
