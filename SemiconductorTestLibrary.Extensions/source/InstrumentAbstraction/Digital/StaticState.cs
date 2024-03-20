using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods for static state read/write.
    /// </summary>
    public static class StaticState
    {
        /// <summary>
        /// Writes static state. Use this method to write the same state on all sessions.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="state">The state to write.</param>
        public static void WriteStatic(this DigitalSessionsBundle sessionsBundle, PinState state)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.WriteStatic(state);
            });
        }

        /// <summary>
        /// Writes static states. Use this method to write different state on different sites.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="states">The states to write for all sites.</param>
        public static void WriteStatic(this DigitalSessionsBundle sessionsBundle, IDictionary<int, PinState> states)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.PinAndChannelMap.GetPinSet(sitePinInfo.SitePinString).WriteStatic(states[sitePinInfo.SiteNumber]);
            });
        }

        /// <summary>
        /// Writes static states. Use this method to write different state for different site-pin pairs.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="states">The states to write for all site-pin pairs.</param>
        public static void WriteStatic(this DigitalSessionsBundle sessionsBundle, IDictionary<int, Dictionary<string, PinState>> states)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.PinAndChannelMap.GetPinSet(sitePinInfo.SitePinString).WriteStatic(states[sitePinInfo.SiteNumber][sitePinInfo.PinName]);
            });
        }

        /// <summary>
        /// Reads and returns per-site per-pin static states.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>The states in per-site per-pin format.</returns>
        public static PinSiteData<PinState> ReadStatic(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return sessionInfo.PinSet.ReadStatic();
            });
        }
    }
}
