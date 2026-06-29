using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
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
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                sessionInfo.Session.InitiateGeneration();
            });
        }

        /// <summary>
        /// IsDone.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static bool[] IsDone(this FgenSessionsBundle sessionsBundle)
        {
            // Returning array of bool for each session in the bundle, indicating whether each session is done.
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo) =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);
                return sessionInfo.Session.IsDone;
            });
        }

        /// <summary>
        /// WaitUntilDone.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="timeout">Max wait time in milliseconds.</param>
        public static void WaitUntilDone(this FgenSessionsBundle sessionsBundle, int timeout = 10000)
        {
            TimeSpan timeoutSpan = TimeSpan.FromMilliseconds(timeout);
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                sessionInfo.Session.WaitUntilDone(timeoutSpan);
            });
         }

        /// <summary>
        /// Abort.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void Abort(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                // Enable only those channels associated with the FGen Session.
                var associatedChannels = sessionInfo.AllChannelsString;
                sessionInfo.ConfigureChannels(associatedChannels);

                sessionInfo.Session.AbortGeneration();
            });
        }

        internal static void ConfigureChannels(this FgenSessionInformation sessionsInfo, string channelNames)
        {
            // Implementation for configuring channels. `channelNames` is a comma-separated string of channel names to configure.
            sessionsInfo.ConfigureChannels(channelNames);
        }
    }
}
