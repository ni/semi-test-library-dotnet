using System.Globalization;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines extension methods for configuring the <see cref="ScopeSessionsBundle"/>.
    /// </summary>
    public static class Configure
    {
        #region Methods on ScopeSessionsBundle

        /// <summary>
        /// Configures the vertical settings of all channels in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="verticalSettings">The <see cref="VerticalSettings"/> to apply.</param>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, VerticalSettings verticalSettings)
        {
            if (verticalSettings is null)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Scope_VerticalSettingsNull, nameof(verticalSettings)));
            }
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                ConfigureVertical(sessionInfo, sitePinInfo, verticalSettings);
            });
        }

        /// <inheritdoc cref="ConfigureVertical(ScopeSessionsBundle, VerticalSettings)"/>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, SiteData<VerticalSettings> verticalSettings)
        {
            if (verticalSettings is null)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Scope_VerticalSettingsNull, nameof(verticalSettings)));
            }
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                ConfigureVertical(sessionInfo, sitePinInfo, verticalSettings.GetValue(sitePinInfo.SiteNumber));
            });
        }

        /// <inheritdoc cref="ConfigureVertical(ScopeSessionsBundle, VerticalSettings)"/>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, PinSiteData<VerticalSettings> verticalSettings)
        {
            if (verticalSettings is null)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Scope_VerticalSettingsNull, nameof(verticalSettings)));
            }
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                ConfigureVertical(sessionInfo, sitePinInfo, verticalSettings.GetValue(sitePinInfo));
            });
        }
        #endregion

        #region Private Methods

        private static void ConfigureVertical(ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo, VerticalSettings verticalSettings)
        {
            if (verticalSettings is null)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Scope_VerticalSettingsNull, nameof(verticalSettings)));
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
