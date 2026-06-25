using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Output class for controlling singnal output.
    /// </summary>
    public static class Output
    {
        /// <summary>
        /// Output enable.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="enable">The enable state.</param>
        public static void ConfigureOutputEnable(this FgenSessionsBundle sessionsBundle, bool enable)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;

                // Configure at channel level. This needs to be sequencial, these are not atomic operations,
                // internally it sets given channel as ative channel and then configures it.
                foreach (var sitePininfo in sessionInfo.AssociatedSitePinList)
                {
                    session.Output.SetEnabled(sitePininfo.IndividualChannelString, enable);
                }
            });
        }

        /// <summary>
        /// Output enable.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="enable">The enable state.</param>
        public static void ConfigureOutputEnable(this FgenSessionsBundle sessionsBundle, SiteData<bool> enable)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;

                // Configure at channel level. This needs to be sequencial, these are not atomic operations,
                // internally it sets given channel as ative channel and then configures it.
                foreach (var sitePininfo in sessionInfo.AssociatedSitePinList)
                {
                    session.Output.SetEnabled(sitePininfo.IndividualChannelString, enable.GetValue(sitePininfo.SiteNumber));
                }
            });
        }

        /// <summary>
        /// Output enable.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="enable">The enable state.</param>
        public static void ConfigureOutputEnable(this FgenSessionsBundle sessionsBundle, PinSiteData<bool> enable)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;

                // Configure at channel level. This needs to be sequencial, these are not atomic operations,
                // internally it sets given channel as ative channel and then configures it.
                foreach (var sitePininfo in sessionInfo.AssociatedSitePinList)
                {
                    session.Output.SetEnabled(sitePininfo.IndividualChannelString, enable.GetValue(sitePininfo.SiteNumber, sitePininfo.PinName));
                }
            });
        }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="impedance">The impedance value.</param>
        public static void ConfigureOutputImpedance(this FgenSessionsBundle sessionsBundle, double impedance = 50)
        { }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="impedance">The impedance value.</param>
        public static void ConfigureOutputImpedance(this FgenSessionsBundle sessionsBundle, SiteData<double> impedance)
        { }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="impedance">The impedance value.</param>
        public static void ConfigureOutputImpedance(this FgenSessionsBundle sessionsBundle, PinSiteData<double> impedance)
        { }

        /// <summary>
        /// Configure Active Channel.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <remarks>
        /// Active channel should be configured if session is opened for whole device instead of specific channel. All the control operations called after that are applied to the active channel.
        /// </remarks>
        public static void ConfigureChannel(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// Configure output mode.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(this FgenSessionsBundle sessionsBundle, OutputMode outputMode)
        { }

        /// <summary>
        /// Configure output mode.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(this FgenSessionsBundle sessionsBundle, SiteData<OutputMode> outputMode)
        { }

        /// <summary>
        /// Configure output mode.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(this FgenSessionsBundle sessionsBundle, PinSiteData<OutputMode> outputMode)
        { }
    }
}
