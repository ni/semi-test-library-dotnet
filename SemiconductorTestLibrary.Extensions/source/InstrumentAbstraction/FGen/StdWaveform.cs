using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Standard waveform class
    /// </summary>
    public static class StadardWaveform
    {
        /// <summary>
        /// Confgure standard waveform parameters.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="standardWaveformSettings">Standard Waveform Settings.</param>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, StandardWaveformSettings standardWaveformSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    string channelName = sitePinInfo.IndividualChannelString.Split('/').Last();
                    session.StandardWaveform.Configure(channelName, standardWaveformSettings.FunctionType, standardWaveformSettings.Amplitude.Value, standardWaveformSettings.DCOffset, standardWaveformSettings.Frequency.Value, standardWaveformSettings.StartPhase);
                });
            });
        }

        /// <summary>
        /// Confgure standard waveform parameters.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="standardWaveformSettings">Standard Waveform Settings.</param>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, SiteData<StandardWaveformSettings> standardWaveformSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    string channelName = sitePinInfo.IndividualChannelString.Split('/').Last();
                    var standardWaveformSettingsPerChannel = standardWaveformSettings.GetValue(sitePinInfo.SiteNumber);
                    session.StandardWaveform.Configure(channelName, standardWaveformSettingsPerChannel.FunctionType, standardWaveformSettingsPerChannel.Amplitude.Value, standardWaveformSettingsPerChannel.DCOffset, standardWaveformSettingsPerChannel.Frequency.Value, standardWaveformSettingsPerChannel.StartPhase);
                });
            });
        }

        /// <summary>
        /// Confgure standard waveform parameters.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="standardWaveformSettings">Standard Waveform Settings.</param>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, PinSiteData<StandardWaveformSettings> standardWaveformSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    string channelName = sitePinInfo.IndividualChannelString.Split('/').Last();
                    var standardWaveformSettingsPerChannel = standardWaveformSettings.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName);
                    session.StandardWaveform.Configure(channelName, standardWaveformSettingsPerChannel.FunctionType, standardWaveformSettingsPerChannel.Amplitude.Value, standardWaveformSettingsPerChannel.DCOffset, standardWaveformSettingsPerChannel.Frequency.Value, standardWaveformSettingsPerChannel.StartPhase);
                });
            });
        }
    }
}
