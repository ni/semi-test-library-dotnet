using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ConfiguredAcquisition
{
     /// <summary>
     /// This class provides example methods to demonstrate how to use the Scope APIs.
     /// Scope Instrument Extension methods from the Semiconductor Test Library.
     /// These methods can be used to perform waveform acquisition.
     /// </summary>
    public static partial class ScopeTestSteps
    {
        /// <summary>
        /// Gets raw sample values from a waveform acquired from a site pin based scope data.
        /// </summary>
        /// <param name="waveforms">Collection of analog waveforms indexed by site and pin.</param>
        /// <param name="siteNumber">Site number used to select the waveform.</param>
        /// <param name="pin">Pin name used to select the waveform.</param>
        /// <param name="sampleInterval">When this method returns, contains the waveform sample interval in seconds.</param>
        /// <returns>An array containing the raw sample values of the selected waveform.</returns>
        public static double[] ScopeGetWaveformSamples(
            PinSiteData<AnalogWaveform<double>> waveforms, int siteNumber, string pin, out double sampleInterval)
        {
            if (waveforms is null)
            {
                sampleInterval = 0.0;
                return null;
            }
            else
            {
                AnalogWaveform<double> waveform = waveforms.GetValue(siteNumber, pin);
                sampleInterval = waveform.Timing.SampleInterval.TotalSeconds;
                return waveform.GetRawData();
            }
        }
    }
}
