using System.Globalization;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using static NationalInstruments.SemiconductorTestLibrary.Common.HelperMethods;

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
        /// <returns>An array of the states for the specified pattern sequencer flag, one state value per session.</returns>
        public static bool[] ReadSequencerFlag(this DigitalSessionsBundle sessionsBundle, string flag)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.PatternControl.ReadSequencerFlag(flag);
            });
        }

        /// <summary>
        /// Reads the Boolean state of a pattern sequencer flag.
        /// </summary>
        /// <remarks>
        /// This method is the same as <see cref="ReadSequencerFlag(DigitalSessionsBundle, string)"/>,
        /// except it also checks to confirm if the flag state is the same across all sessions in the bundle.
        /// If the states are indeed the same, it will return the single boolean state value.
        /// Otherwise, it will throw an exception.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="flag">The name of the pattern sequencer flag to read. Possible values include "seqflag0", "seqflag1", "seqflag2", or "seqflag3".</param>
        /// <returns>An array of the states for the specified pattern sequencer flag, one state value per session.</returns>
        /// <exception cref="NIMixedSignalException">The state of the sequence flag is not the same between instrument sessions.</exception>
        public static bool ReadSequencerFlagDistinct(this DigitalSessionsBundle sessionsBundle, string flag)
        {
            return GetDistinctValue(sessionsBundle.ReadSequencerFlag(flag), string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_SequencerFlagNotDistinct, flag));
        }

        /// <summary>
        /// Writes a Boolean state of a pattern sequencer flag.
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
        /// Writes a Boolean state to a specified pattern sequencer flag for synchronized instruments.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="flag">The name of the pattern sequencer flag to read. Possible values include "seqflag0", "seqflag1", "seqflag2", or "seqflag3".</param>
        /// <param name="value">The value to assign to the specified pattern sequencer flag.</param>
        public static void WriteSequencerFlagSynchronized(this DigitalSessionsBundle sessionsBundle, string flag, bool value)
        {
            string siteListString = string.Join(",", sessionsBundle.AggregateSitePinList.Select(sitePinInfo => sitePinInfo.SiteNumber));
            NIDigital[] niDigitalSessions = sessionsBundle.InstrumentSessions.Select(x => x.Session).ToArray();
            DigitalPatternControl.WriteSequencerFlagSynchronized(niDigitalSessions, flag, value);
        }

        /// <summary>
        /// Reads the numeric state of a pattern sequencer register.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="registerName">Specifies pattern sequencer register to read. Possible values include "reg0" through "reg15".</param>
        /// <returns>An array of the values for the specified pattern sequencer register, one integer value per session.</returns>
        public static int[] ReadSequencerRegister(this DigitalSessionsBundle sessionsBundle, string registerName)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.PatternControl.ReadSequencerRegister(registerName);
            });
        }

        /// <summary>
        /// Reads the numeric state of a pattern sequencer flag.
        /// </summary>
        /// <remarks>
        /// This method is the same as <see cref="ReadSequencerRegister(DigitalSessionsBundle, string)"/>,
        /// except it also checks to confirm if the register values are the same across all sessions in the bundle.
        /// If the states are indeed the same, it will return the single integer value.
        /// Otherwise, it will throw an exception.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="registerName">Specifies pattern sequencer register to read. Possible values include "reg0" through "reg15".</param>
        /// <returns>An single int value for the specified pattern sequencer register.</returns>
        /// <exception cref="NIMixedSignalException">The state of the sequence register is not the same between instrument sessions.</exception>
        public static int ReadSequencerRegisterDistinct(this DigitalSessionsBundle sessionsBundle, string registerName)
        {
            return GetDistinctValue(sessionsBundle.ReadSequencerRegister(registerName), string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_SequencerRegisterNotDistinct, registerName));
        }

        /// <summary>
        /// Writes a value to a pattern sequencer register.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="register">Specifies the sequencer register to which you would like to write the specified value. Possible values include "reg0" through "reg15".</param>
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
