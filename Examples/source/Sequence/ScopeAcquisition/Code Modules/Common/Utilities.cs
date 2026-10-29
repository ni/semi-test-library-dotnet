using System;
using System.Collections.Generic;
using System.Linq;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ScopeAcquisition
{
    /// <summary>
    /// Provides helper methods for analyzing sampled scope signals.
    /// </summary>
    /// <remarks>Includes zero-crossing-based tone analysis to estimate frequency and peak amplitude from
    /// uniformly sampled data.</remarks>
    public static class Utilities
    {
        /// <summary>
        /// Measures the frequency and peak amplitude of a tone from uniformly sampled data.
        /// </summary>
        /// <param name="samples">The sampled data points.</param>
        /// <param name="sampleRateHz">The sample rate in Hertz.</param>
        /// <param name="frequencyHz">The measured frequency in Hertz.</param>
        /// <param name="amplitude">The measured peak amplitude.</param>
        /// <exception cref="InvalidOperationException">Thrown if there are not enough cycles to estimate frequency.</exception>
        public static void MeasureTone(
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
