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
        /// <param name="triggerSource">The source terminal for the trigger.</param>
        /// <param name="level">The voltage threshold for the trigger.</param>
        /// <param name="slope">The slope on which to trigger (Rising or Falling).</param>
        /// <param name="triggerCoupling">The coupling for the trigger.</param>
        /// <param name="holdOff">The hold-off time.</param>
        /// <param name="delay">The trigger delay.</param>
        public static void ConfigureEdgeTrigger(this ScopeSessionsBundle sessionsBundle, string triggerSource, double level, ScopeTriggerSlope slope, ScopeTriggerCoupling triggerCoupling, PrecisionTimeSpan holdOff, PrecisionTimeSpan delay)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.EdgeTrigger.Configure(triggerSource, level, slope, triggerCoupling, holdOff, delay);
            });
        }

        /// <summary>
        /// Configures a trigger with hysteresis.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSource">The source terminal for the trigger.</param>
        /// <param name="level">The voltage threshold for the trigger.</param>
        /// <param name="hysteresis">The hysteresis voltage.</param>
        /// <param name="slope">The slope on which to trigger (Rising or Falling).</param>
        /// <param name="triggerCoupling">The coupling for the trigger.</param>
        /// <param name="holdOff">The hold-off time.</param>
        /// <param name="delay">The trigger delay.</param>
        public static void ConfigureTriggerHysteresis(this ScopeSessionsBundle sessionsBundle, string triggerSource, double level, double hysteresis, ScopeTriggerSlope slope, ScopeTriggerCoupling triggerCoupling, PrecisionTimeSpan holdOff, PrecisionTimeSpan delay)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConfigureTriggerHysteresis(triggerSource, level, hysteresis, slope, triggerCoupling, holdOff, delay);
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
        /// <param name="triggerSource">The source terminal for the digital trigger.</param>
        /// <param name="slope">The slope on which to trigger (Rising or Falling).</param>
        /// <param name="holdOff">The hold-off time.</param>
        /// <param name="delay">The trigger delay.</param>
        public static void ConfigureTriggerDigital(this ScopeSessionsBundle sessionsBundle, string triggerSource, ScopeTriggerSlope slope, PrecisionTimeSpan holdOff, PrecisionTimeSpan delay)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConfigureTriggerDigital(triggerSource, slope, holdOff, delay);
            });
        }

        /// <summary>
        /// Configures a window trigger that triggers when the signal enters or exits a voltage window.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSource">The source terminal for the trigger.</param>
        /// <param name="low">The low voltage level of the window.</param>
        /// <param name="high">The high voltage level of the window.</param>
        /// <param name="mode">The window trigger mode.</param>
        /// <param name="triggerCoupling">The coupling for the trigger.</param>
        /// <param name="holdOff">The hold-off time.</param>
        /// <param name="delay">The trigger delay.</param>
        public static void ConfigureTriggerWindow(this ScopeSessionsBundle sessionsBundle, string triggerSource, double low, double high, ScopeWindowTriggerMode mode, ScopeTriggerCoupling triggerCoupling, PrecisionTimeSpan holdOff, PrecisionTimeSpan delay)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConfigureTriggerWindow(triggerSource, low, high, mode, triggerCoupling, holdOff, delay);
            });
        }
    }
}