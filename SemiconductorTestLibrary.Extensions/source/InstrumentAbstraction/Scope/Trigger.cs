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
                sessionInfo.Session.Trigger.EdgeTrigger.Configure(triggerSettings.TriggerSource, triggerSettings.TriggerLevel.Value, (ScopeTriggerSlope)triggerSettings.TriggerSlope.Value, (ScopeTriggerCoupling)triggerSettings.TriggerCoupling.Value, triggerSettings.HoldOff.Value, triggerSettings.Delay.Value);
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
                sessionInfo.Session.Trigger.ConfigureTriggerHysteresis(triggerSettings.TriggerSource, triggerSettings.TriggerLevel.Value, triggerSettings.TriggerHysteresis.Value, (ScopeTriggerSlope)triggerSettings.TriggerSlope.Value, (ScopeTriggerCoupling)triggerSettings.TriggerCoupling.Value, triggerSettings.HoldOff.Value, triggerSettings.Delay.Value);
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
                sessionInfo.Session.Trigger.ConfigureTriggerDigital(triggerSettings.TriggerSource, (ScopeTriggerSlope)triggerSettings.TriggerSlope.Value, triggerSettings.HoldOff.Value, triggerSettings.Delay.Value);
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
                sessionInfo.Session.Trigger.ConfigureTriggerWindow(triggerSettings.TriggerSource, triggerSettings.TriggerLevel.Value, triggerSettings.TriggerHysteresis.Value, (ScopeWindowTriggerMode)triggerSettings.TriggerSlope.Value, (ScopeTriggerCoupling)triggerSettings.TriggerCoupling.Value, triggerSettings.HoldOff.Value, triggerSettings.Delay.Value);
            });
        }
    }
}