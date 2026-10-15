using System;
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
        /// <remarks>
        /// Throws <see cref="ArgumentNullException"/> if the <paramref name="verticalSettings"/> parameter is null.<br/>
        /// Throws <see cref="NISemiconductorTestException"/> if the driver fails to apply the settings for any pin or site.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="verticalSettings">The <see cref="VerticalSettings"/> to apply.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="verticalSettings"/> parameter is null.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when the driver fails to apply the settings for any pin or site.</exception>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, VerticalSettings verticalSettings)
        {
            ThrowIfNull(verticalSettings, nameof(verticalSettings));
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                ConfigureVertical(sessionInfo, sitePinInfo, verticalSettings);
            });
        }

        /// <inheritdoc cref="ConfigureVertical(ScopeSessionsBundle, VerticalSettings)"/>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, SiteData<VerticalSettings> verticalSettings)
        {
            ThrowIfNull(verticalSettings, nameof(verticalSettings));
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                ConfigureVertical(sessionInfo, sitePinInfo, verticalSettings.GetValue(sitePinInfo.SiteNumber));
            });
        }

        /// <inheritdoc cref="ConfigureVertical(ScopeSessionsBundle, VerticalSettings)"/>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, PinSiteData<VerticalSettings> verticalSettings)
        {
            ThrowIfNull(verticalSettings, nameof(verticalSettings));
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                ConfigureVertical(sessionInfo, sitePinInfo, verticalSettings.GetValue(sitePinInfo));
            });
        }

        /// <summary>
        /// Configures the electrical characteristics of all channels in the bundle.
        /// </summary>
        /// <remarks>
        /// Throws <see cref="ArgumentNullException"/> if the <paramref name="electricalCharacteristics"/> parameter is null.<br/>
        /// Throws <see cref="NISemiconductorTestException"/> if the driver fails to apply the settings for any pin or site.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="electricalCharacteristics">The <see cref="ElectricalCharacteristics"/> to apply.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="electricalCharacteristics"/> parameter is null.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when the driver fails to apply the settings for any pin or site.</exception>
        public static void ConfigureElectricalCharacteristics(this ScopeSessionsBundle sessionsBundle, ElectricalCharacteristics electricalCharacteristics)
        {
            ThrowIfNull(electricalCharacteristics, nameof(electricalCharacteristics));
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                ConfigureElectricalCharacteristics(sessionInfo, sitePinInfo, electricalCharacteristics);
            });
        }

        /// <inheritdoc cref="ConfigureElectricalCharacteristics(ScopeSessionsBundle, ElectricalCharacteristics)"/>
        public static void ConfigureElectricalCharacteristics(this ScopeSessionsBundle sessionsBundle, SiteData<ElectricalCharacteristics> electricalCharacteristics)
        {
            ThrowIfNull(electricalCharacteristics, nameof(electricalCharacteristics));
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                ConfigureElectricalCharacteristics(sessionInfo, sitePinInfo, electricalCharacteristics.GetValue(sitePinInfo.SiteNumber));
            });
        }

        /// <inheritdoc cref="ConfigureElectricalCharacteristics(ScopeSessionsBundle, ElectricalCharacteristics)"/>
        public static void ConfigureElectricalCharacteristics(this ScopeSessionsBundle sessionsBundle, PinSiteData<ElectricalCharacteristics> electricalCharacteristics)
        {
            ThrowIfNull(electricalCharacteristics, nameof(electricalCharacteristics));
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                ConfigureElectricalCharacteristics(sessionInfo, sitePinInfo, electricalCharacteristics.GetValue(sitePinInfo));
            });
        }

        #endregion

        #region Private Methods

        private static void ConfigureVertical(ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo, VerticalSettings verticalSettings)
        {
            ThrowIfNull(verticalSettings, nameof(verticalSettings));
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

        private static void ConfigureElectricalCharacteristics(ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo, ElectricalCharacteristics characteristics)
        {
            ThrowIfNull(characteristics, nameof(characteristics));
            sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString].ConfigureCharacteristics(
                characteristics.InputImpedance,
                characteristics.InputFrequencyMax);
        }

        private static void ThrowIfNull(object value, string parameterName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(parameterName, string.Format(CultureInfo.InvariantCulture, ResourceStrings.Scope_VerticalSettingsNull, parameterName));
            }
        }
        #endregion
    }
}
