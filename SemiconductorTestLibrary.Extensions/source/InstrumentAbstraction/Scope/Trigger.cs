using NationalInstruments.ModularInstruments.NIScope;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines extension methods for configuring the triggers of the NI-Scope session.
    /// </summary>
    public static class Trigger
    {
        #region Methods on ScopeSessionsBundle

        /// <summary>
        /// Configures an edge trigger for all sessions in the bundle.
        /// </summary>
        /// <remarks>
        /// Only the first channel associated with each session is used as the trigger source.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The <see cref="TriggerSettings"/> to apply.</param>
        public static void ConfigureEdgeTrigger(this ScopeSessionsBundle sessionsBundle, TriggerSettings triggerSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.EdgeTrigger.Configure(
                    sessionInfo.GetTriggerSource(),
                    triggerSettings.TriggerLevel,
                    triggerSettings.TriggerSlope,
                    triggerSettings.TriggerCoupling,
                    triggerSettings.HoldOff,
                    triggerSettings.Delay);
            });
        }

        /// <summary>
        /// Configures a hysteresis trigger for all sessions in the bundle.
        /// </summary>
        /// <remarks>
        /// Only the first channel associated with each session is used as the trigger source.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The <see cref="TriggerSettings"/> to apply.</param>
        public static void ConfigureTriggerHysteresis(this ScopeSessionsBundle sessionsBundle, TriggerSettings triggerSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConfigureTriggerHysteresis(
                    sessionInfo.GetTriggerSource(),
                    triggerSettings.TriggerLevel,
                    triggerSettings.TriggerHysteresis,
                    triggerSettings.TriggerSlope,
                    triggerSettings.TriggerCoupling,
                    triggerSettings.HoldOff,
                    triggerSettings.Delay);
            });
        }

        /// <summary>
        /// Configures a digital trigger for all sessions in the bundle.
        /// </summary>
        /// <remarks>
        /// Only the first channel associated with each session is used as the trigger source.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The <see cref="TriggerSettings"/> to apply.</param>
        public static void ConfigureTriggerDigital(this ScopeSessionsBundle sessionsBundle, TriggerSettings triggerSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConfigureTriggerDigital(
                    sessionInfo.GetTriggerSource(),
                    triggerSettings.TriggerSlope,
                    triggerSettings.HoldOff,
                    triggerSettings.Delay);
            });
        }

        /// <summary>
        /// Configures a window trigger for all sessions in the bundle.
        /// </summary>
        /// <remarks>
        /// Only the first channel associated with each session is used as the trigger source.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The <see cref="TriggerSettings"/> to apply.</param>
        public static void ConfigureTriggerWindow(this ScopeSessionsBundle sessionsBundle, TriggerSettings triggerSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConfigureTriggerWindow(
                    sessionInfo.GetTriggerSource(),
                    triggerSettings.WindowLowLevel,
                    triggerSettings.WindowHighLevel,
                    triggerSettings.TriggerWindowMode,
                    triggerSettings.TriggerCoupling,
                    triggerSettings.HoldOff,
                    triggerSettings.Delay);
            });
        }
        #endregion

        #region Methods on ScopeSessionInformation

        private static ScopeTriggerSource GetTriggerSource(this ScopeSessionInformation sessionInfo)
        {
            return sessionInfo.AssociatedSitePinList[0].IndividualChannelString;
        }
        #endregion
    }
}
