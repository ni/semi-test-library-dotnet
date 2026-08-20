using NationalInstruments.ModularInstruments.NIScope;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for configuring oscilloscope trigger behavior.
    /// </summary>
    public static class Trigger
    {
        /// <summary>
        /// Configures an edge trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The trigger settings.</param>
        public static void ConfigureEdgeTrigger(this ScopeSessionsBundle sessionsBundle, TriggerSettings triggerSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                var channelName = sessionInfo.AssociatedSitePinList[0].IndividualChannelString;
                sessionInfo.Session.Trigger.EdgeTrigger.Configure(channelName, triggerSettings.TriggerLevel, triggerSettings.TriggerSlope, triggerSettings.TriggerCoupling, triggerSettings.HoldOff, triggerSettings.Delay);
            });
        }

        /// <summary>
        /// Configures a trigger with hysteresis.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The trigger settings.</param>
        public static void ConfigureTriggerHysteresis(this ScopeSessionsBundle sessionsBundle, TriggerSettings triggerSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                var channelName = sessionInfo.AssociatedSitePinList[0].IndividualChannelString;
                sessionInfo.Session.Trigger.ConfigureTriggerHysteresis(channelName, triggerSettings.TriggerLevel, triggerSettings.TriggerHysteresis, triggerSettings.TriggerSlope, triggerSettings.TriggerCoupling, triggerSettings.HoldOff, triggerSettings.Delay);
            });
        }

        /// <summary>
        /// Configures an immediate trigger that triggers immediately without any condition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        public static void ConfigureTriggerImmediate(this ScopeSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConfigureTriggerImmediate();
            });
        }

        /// <summary>
        /// Configures a digital trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The trigger settings.</param>
        public static void ConfigureTriggerDigital(this ScopeSessionsBundle sessionsBundle, TriggerSettings triggerSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                var channelName = sessionInfo.AssociatedSitePinList[0].IndividualChannelString;
                sessionInfo.Session.Trigger.ConfigureTriggerDigital(channelName, triggerSettings.TriggerSlope, triggerSettings.HoldOff, triggerSettings.Delay);
            });
        }

        /// <summary>
        /// Configures a window trigger that triggers when the signal enters or exits a voltage window.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The trigger settings.</param>
        public static void ConfigureTriggerWindow(this ScopeSessionsBundle sessionsBundle, TriggerSettings triggerSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                var channelName = sessionInfo.AssociatedSitePinList[0].IndividualChannelString;
                sessionInfo.Session.Trigger.ConfigureTriggerWindow(channelName, triggerSettings.TriggerLevel, triggerSettings.TriggerHysteresis, triggerSettings.TriggerWindowMode, triggerSettings.TriggerCoupling, triggerSettings.HoldOff, triggerSettings.Delay);
            });
        }
    }
}