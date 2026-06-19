using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Standard waveform class
    /// </summary>
    public static class StdWaveform
    {
        /// <summary>
        /// Confgure standard waveform parameters.
        /// </summary>
        /// <param name="stdWaveformSettings">Standard Waveform Settings.</param>
        public static void ConfigureStdWaveform(StdWaveformSettings stdWaveformSettings)
        { }

        /// <summary>
        /// Confgure standard waveform parameters.
        /// </summary>
        /// <param name="stdWaveformSettings">Standard Waveform Settings.</param>
        public static void ConfigureStdWaveform(SiteData<StdWaveformSettings> stdWaveformSettings)
        { }

        /// <summary>
        /// Confgure standard waveform parameters.
        /// </summary>
        /// <param name="stdWaveformSettings">Standard Waveform Settings.</param>
        public static void ConfigureStdWaveform(PinSiteData<StdWaveformSettings> stdWaveformSettings)
        { }
    }
}
