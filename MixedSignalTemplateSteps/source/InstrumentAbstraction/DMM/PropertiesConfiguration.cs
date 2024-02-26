using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.ModularInstruments.NIDmm;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DMM
{
    /// <summary>
    /// Defines methods to configure <see cref="NIDmm"/> sessions.
    /// </summary>
    public static class PropertiesConfiguration
    {
        /// <summary>
        /// Configures AC bandwidth for AC measurements.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="minimumFrequency">The minimum frequency value in Hz to be used.</param>
        /// <param name="maximumFrequency">The maximum frequency value in Hz to be used.</param>
        public static void ConfigureACBandwidth(this DMMSessionsBundle sessionsBundle, double minimumFrequency = 20, double maximumFrequency = 25000)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.AC.ConfigureBandwidth(minimumFrequency, maximumFrequency);
            });
        }

        /// <summary>
        /// Configures aperture time.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="apertureTimeUnits">The unit of the aperture time.</param>
        /// <param name="apertureTime">The aperture time to be used.</param>
        public static void ConfigureApertureTime(this DMMSessionsBundle sessionsBundle, DmmApertureTimeUnits apertureTimeUnits = DmmApertureTimeUnits.Seconds, double apertureTime = 0)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Advanced.ApertureTimeUnits = apertureTimeUnits;
                sessionInfo.Session.Advanced.ApertureTime = apertureTime;
            });
        }

        /// <summary>
        /// Configures measurements using absolute resolution.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="measurementFunction">The measurement function to be used.</param>
        /// <param name="range">The range to be used.</param>
        /// <param name="resolutionAbsolute">The absolute resolution to be used.</param>
        public static void ConfigureMeasurementAbsolute(this DMMSessionsBundle sessionsBundle, DmmMeasurementFunction measurementFunction = DmmMeasurementFunction.DCVolts, double range = 0.02, double resolutionAbsolute = 0.001)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Configure(measurementFunction, range, resolutionAbsolute);
            });
        }

        /// <summary>
        /// Configures measurements using number of digits resolution.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="measurementFunction">The measurement function to be used.</param>
        /// <param name="range">The range to be used.</param>
        /// <param name="resolutionDigits">The resolution number of digits to be used.</param>
        public static void ConfigureMeasurementDigits(this DMMSessionsBundle sessionsBundle, DmmMeasurementFunction measurementFunction = DmmMeasurementFunction.DCVolts, double range = 0.02, double resolutionDigits = 5.5)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.ConfigureMeasurementDigits(measurementFunction, range, resolutionDigits);
            });
        }

        /// <summary>
        /// Configures multi point acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="triggerCount">The number of the triggers the device receives before returning to the idle state.</param>
        /// <param name="sampleCount">The number of measurements the device makes in each measurement sequence initiated by a trigger.</param>
        /// <param name="sampleTrigger">The name of the trigger source that initiates the acquisition.</param>
        /// <param name="sampleIntervalInSeconds">The seconds that the device waits between measurements.</param>
        public static void ConfigureMultiPoint(this DMMSessionsBundle sessionsBundle, int triggerCount, int sampleCount, string sampleTrigger, double sampleIntervalInSeconds)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.MultiPoint.Configure(triggerCount, sampleCount, sampleTrigger, PrecisionTimeSpan.FromSeconds(sampleIntervalInSeconds));
            });
        }

        /// <summary>
        /// Configures power line frequency.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="powerlineFrequency">The frequency in Hz to be used.</param>
        public static void ConfigurePowerlineFrequency(this DMMSessionsBundle sessionsBundle, double powerlineFrequency = 60)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Advanced.PowerlineFrequency = powerlineFrequency;
            });
        }

        /// <summary>
        /// Configures trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="triggerSource">The name of the trigger source that initiates the acquisition.</param>
        /// <param name="triggerDelayInSeconds">The seconds that the device waits after it has received a trigger before taking a measurement.</param>
        public static void ConfigureTrigger(this DMMSessionsBundle sessionsBundle, string triggerSource, double triggerDelayInSeconds)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.Configure(triggerSource, PrecisionTimeSpan.FromSeconds(triggerDelayInSeconds));
            });
        }

        /// <summary>
        /// Sends out software trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        public static void SendSoftwareTrigger(this DMMSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Measurement.SendSoftwareTrigger();
            });
        }
    }
}
