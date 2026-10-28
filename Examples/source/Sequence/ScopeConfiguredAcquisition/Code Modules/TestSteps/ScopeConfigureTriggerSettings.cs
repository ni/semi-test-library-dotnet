using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ConfiguredAcquisition
{
    public static partial class TestSteps
    {
        /// <summary>
        /// This method used for configuring trigger settings.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">Name of the scope pin or pin group to be used for waveform acquisition.</param>
        /// <param name="immediateTrigger">A boolean value indicating whether to configure an immediate trigger or a digital trigger.</param>
        /// <param name="triggerSlope">The slope of the digital trigger.</param>
        /// <param name="holdOffInSeconds">The digital trigger holdoff, in seconds.</param>
        /// <param name="delayInSeconds">The digital trigger delay, in seconds.</param>
        public static void ScopeConfigureTriggerSettings(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            bool immediateTrigger,
            ScopeTriggerSlope triggerSlope,
            double holdOffInSeconds,
            double delayInSeconds)
        {
            DigitalTriggerSettings digitalTriggerSettings = new DigitalTriggerSettings
            {
                TriggerSlope = triggerSlope,
                HoldOff = PrecisionTimeSpan.FromSeconds(holdOffInSeconds),
                Delay = PrecisionTimeSpan.FromSeconds(delayInSeconds),
            };
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
            if (immediateTrigger)
            {
                scopeBundle.ConfigureTriggerImmediate();
            }
            else
            {
                scopeBundle.ConfigureTriggerDigital(digitalTriggerSettings);
            }
        }
    }
}
