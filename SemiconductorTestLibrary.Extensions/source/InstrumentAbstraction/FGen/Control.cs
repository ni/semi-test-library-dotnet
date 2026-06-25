using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Control class for controlling waveform generation operations.
    /// </summary>
    public static class Control
    {
        /// <summary>
        /// Commit.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void Commit(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                sessionInfo.Session.Commit();
            });
        }

        /// <summary>
        /// Initiate.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void Initiate(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// IsDone.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void IsDone(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// WaitUntilDone.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void WaitUntilDone(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// Abort.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void Abort(this FgenSessionsBundle sessionsBundle)
        { }

        internal static void ConfigureChannels(this FgenSessionInformation sessionsInfo, string channelNames)
        {
            // Implementation for configuring channels. `channelNames` is a comma-separated string of channel names to configure.
        }
    }
}
