using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIFgen;
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
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    session.Output.SetEnabled(sitePinInfo.IndividualChannelString.Split('/').Last(), enable);
                });
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
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    session.Output.SetEnabled(sitePinInfo.IndividualChannelString.Split('/').Last(), enable.GetValue(sitePinInfo.SiteNumber));
                });
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

                // Configure at channel level. Use parallel for each to speed up the configuration.
                // Internally Driver APIs are using session level locks to serialize the calls..
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    session.Output.SetEnabled(sitePinInfo.IndividualChannelString.Split('/').Last(), enable.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName));
                });
            });
        }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="impedance">The impedance value.</param>
        public static void ConfigureOutputImpedance(this FgenSessionsBundle sessionsBundle, double impedance = 50)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    session.Output.SetImpedance(sitePinInfo.IndividualChannelString.Split('/').Last(), impedance);
                });
            });
        }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="impedance">The impedance value.</param>
        public static void ConfigureOutputImpedance(this FgenSessionsBundle sessionsBundle, SiteData<double> impedance)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    session.Output.SetImpedance(sitePinInfo.IndividualChannelString, impedance.GetValue(sitePinInfo.SiteNumber));
                });
            });
        }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="impedance">The impedance value.</param>
        public static void ConfigureOutputImpedance(this FgenSessionsBundle sessionsBundle, PinSiteData<double> impedance)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    session.Output.SetImpedance(sitePinInfo.IndividualChannelString, impedance.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName));
                });
            });
        }

        /// <summary>
        /// Configure output mode.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(this FgenSessionsBundle sessionsBundle, OutputMode outputMode)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                var session = sessionInfo.Session;
                Parallel.ForEach(sessionInfo.AssociatedSitePinList, sitePinInfo =>
                {
                    session.Output.OutputMode = outputMode;
                });
            });
        }
    }
}
