using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for configuring the NI-Scope session.
    /// </summary>
    public static class Configure
    {
        #region Methods on ScopeSessionsBundle

        /// <summary>
        /// Configures the vertical settings of all channels in the bundle with the same settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="verticalSettings">The <see cref="VerticalSettings"/> to apply.</param>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, VerticalSettings verticalSettings)
        {
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                sessionInfo.ConfigureVertical(sitePinInfo, verticalSettings);
            });
        }

        /// <summary>
        /// Configures the vertical settings of all channels in the bundle with per-site settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="verticalSettings">The per-site <see cref="VerticalSettings"/> to apply.</param>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, SiteData<VerticalSettings> verticalSettings)
        {
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                sessionInfo.ConfigureVertical(sitePinInfo, verticalSettings.GetValue(sitePinInfo.SiteNumber));
            });
        }

        /// <summary>
        /// Configures the vertical settings of all channels in the bundle with per-pin per-site settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="verticalSettings">The per-pin per-site <see cref="VerticalSettings"/> to apply.</param>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, PinSiteData<VerticalSettings> verticalSettings)
        {
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                sessionInfo.ConfigureVertical(sitePinInfo, verticalSettings.GetValue(sitePinInfo));
            });
        }
        #endregion

        #region Methods on ScopeSessionInformation

        private static void ConfigureVertical(this ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo, VerticalSettings verticalSettings)
        {
            if (verticalSettings is null)
            {
                return;
            }
            var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
            channel.Configure(
                verticalSettings.Range,
                verticalSettings.Offset,
                verticalSettings.Coupling,
                verticalSettings.ProbeAttenuation,
                verticalSettings.Enabled);
            if (verticalSettings.EnableTimeInterleavedSampling.HasValue)
            {
                channel.EnableTimeInterleavedSampling = verticalSettings.EnableTimeInterleavedSampling.Value;
            }
        }
        #endregion
    }
}
