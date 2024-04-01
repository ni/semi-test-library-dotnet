using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods to deal with digital pattern sequencer flags and registers.
    /// </summary>
    public static class SequencerFlagsAndRegisters
    {
        /// <summary>
        /// Reads the Boolean value of a pattern sequencer flag.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="flag">The name of the pattern sequencer flag to read. Possible values include "seqflag0", "seqflag1", "seqflag2", or "seqflag3".</param>
        /// <returns>A SiteData object of the site specific value of the specified pattern sequencer flag</returns>
        public static SiteData<bool> ReadSequencerFlag(this DigitalSessionsBundle sessionsBundle, string flag)
        {
            var results = new Dictionary<int, bool>();
            Parallel.ForEach(sessionsBundle.InstrumentSessions, sessionInfo =>
            {
               var flagValue = sessionInfo.Session.PatternControl.ReadSequencerFlag(flag);
               foreach (var site in sessionInfo.AssociatedSiteList)
                {
                   results.Add(site, flagValue);
               }
            });
            return new SiteData<bool>(results);
        }

        /// <summary>
        /// Writes a Boolean value of a pattern sequencer flag.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="flag">The name of the pattern sequencer flag to read. Possible values include "seqflag0", "seqflag1", "seqflag2", or "seqflag3".</param>
        /// <param name="value">The value to assign to the specified pattern sequencer flag.</param>
        public static void WriteSequencerFlag(this DigitalSessionsBundle sessionsBundle, string flag, bool value)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.PatternControl.WriteSequencerFlag(flag, value);
            });
        }

        /// <summary>
        /// Writes a Boolean value to a specified pattern sequencer flag for synchronized instruments.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="flag">The name of the pattern sequencer flag to read. Possible values include "seqflag0", "seqflag1", "seqflag2", or "seqflag3".</param>
        /// <param name="value">The value to assign to the specified pattern sequencer flag.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Not Necessary")]
        public static void WriteSequencerFlagSynchronized(this DigitalSessionsBundle sessionsBundle, string flag, bool value)
        {
            string siteListString = string.Join(",", sessionsBundle.AggregateSitePinList.Select(sitePinList => sitePinList.SiteNumber.ToString()).ToArray());
            NIDigital[] niDigitalSessions = sessionsBundle.InstrumentSessions.Select(x => x.Session).ToArray();
            DigitalPatternControl.WriteSequencerFlagSynchronized(niDigitalSessions, flag, value);
        }

        /// <summary>
        /// Reads the numeric state of a pattern sequencer register.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="registerName">Specifies pattern sequencer register to read.</param>
        /// <returns>A SiteData object of the site specific register values of the specified pattern sequencer flag.</returns>
        public static SiteData<int> ReadSequencerRegister(this DigitalSessionsBundle sessionsBundle, string registerName)
        {
            var results = new Dictionary<int, int>();
            Parallel.ForEach(sessionsBundle.InstrumentSessions, sessionInfo =>
            {
                var registerValue = sessionInfo.Session.PatternControl.ReadSequencerRegister(registerName);
                foreach (var site in sessionInfo.AssociatedSiteList)
                {
                    results.Add(site, registerValue);
                }
            });
            return new SiteData<int>(results);
        }

        /// <summary>
        /// Writes a value to a pattern sequencer register.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="register">pecifies the sequencer register to which you would like to write the specified value.</param>
        /// <param name="value">The value to write to the specified pattern sequence register.</param>
        public static void WriteSequencerRegister(this DigitalSessionsBundle sessionsBundle, string register, int value)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.PatternControl.WriteSequencerRegister(register, value);
            });
        }
    }
}
