using System;
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
        /// Only the first channel associated with each session is used as the trigger source.<br/>
        /// Throws <see cref="ArgumentNullException"/> if the <paramref name="triggerSettings"/> parameter is null.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The <see cref="EdgeTriggerSettings"/> to apply.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="triggerSettings"/> parameter is null.</exception>
        public static void ConfigureEdgeTrigger(this ScopeSessionsBundle sessionsBundle, EdgeTriggerSettings triggerSettings)
        {
            ThrowIfNull(triggerSettings);
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
        /// Only the first channel associated with each session is used as the trigger source.<br/>
        /// Throws <see cref="ArgumentNullException"/> if the <paramref name="triggerSettings"/> parameter is null.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The <see cref="HysteresisTriggerSettings"/> to apply.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="triggerSettings"/> parameter is null.</exception>
        public static void ConfigureTriggerHysteresis(this ScopeSessionsBundle sessionsBundle, HysteresisTriggerSettings triggerSettings)
        {
            ThrowIfNull(triggerSettings);
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
        /// Only the first channel associated with each session is used as the trigger source.<br/>
        /// Throws <see cref="ArgumentNullException"/> if the <paramref name="triggerSettings"/> parameter is null.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The <see cref="DigitalTriggerSettings"/> to apply.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="triggerSettings"/> parameter is null.</exception>
        public static void ConfigureTriggerDigital(this ScopeSessionsBundle sessionsBundle, DigitalTriggerSettings triggerSettings)
        {
            ThrowIfNull(triggerSettings);
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
        /// Only the first channel associated with each session is used as the trigger source.<br/>
        /// Throws <see cref="ArgumentNullException"/> if the <paramref name="triggerSettings"/> parameter is null.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="triggerSettings">The <see cref="WindowTriggerSettings"/> to apply.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="triggerSettings"/> parameter is null.</exception>
        public static void ConfigureTriggerWindow(this ScopeSessionsBundle sessionsBundle, WindowTriggerSettings triggerSettings)
        {
            ThrowIfNull(triggerSettings);
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

        /// <summary>
        /// Configures an immediate trigger for all sessions in the bundle.
        /// </summary>
        /// <remarks>
        /// The acquisition starts as soon as the session is initiated, without waiting for a trigger condition.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        public static void ConfigureTriggerImmediate(this ScopeSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConfigureTriggerImmediate();
            });
        }
        #endregion

        #region Private Methods

        private static void ThrowIfNull(object triggerSettings)
        {
            if (triggerSettings is null)
            {
                throw new ArgumentNullException(nameof(triggerSettings));
            }
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
