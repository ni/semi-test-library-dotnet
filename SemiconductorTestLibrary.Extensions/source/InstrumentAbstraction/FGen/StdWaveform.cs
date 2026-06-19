using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;

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
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="stdWaveformSettings">Standard Waveform Settings.</param>
        public static void ConfigureStdWaveform(this FgenSessionsBundle sessionsBundle, StdWaveformSettings stdWaveformSettings)
        { }

        /// <summary>
        /// Confgure standard waveform parameters.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="stdWaveformSettings">Standard Waveform Settings.</param>
        public static void ConfigureStdWaveform(this FgenSessionsBundle sessionsBundle, SiteData<StdWaveformSettings> stdWaveformSettings)
        { }

        /// <summary>
        /// Confgure standard waveform parameters.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="stdWaveformSettings">Standard Waveform Settings.</param>
        public static void ConfigureStdWaveform(this FgenSessionsBundle sessionsBundle, PinSiteData<StdWaveformSettings> stdWaveformSettings)
        { }
    }
}
