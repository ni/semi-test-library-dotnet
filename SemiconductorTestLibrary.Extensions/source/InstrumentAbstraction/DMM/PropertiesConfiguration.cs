using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM
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
        /// <param name="minimumFrequency">The minimum frequency value in Hz to use.</param>
        /// <param name="maximumFrequency">The maximum frequency value in Hz to use.</param>
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
        /// <param name="apertureTime">The aperture time to use.</param>
        public static void ConfigureApertureTime(this DMMSessionsBundle sessionsBundle, DmmApertureTimeUnits apertureTimeUnits = DmmApertureTimeUnits.Seconds, double apertureTime = 0)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Advanced.ApertureTimeUnits = apertureTimeUnits;
                sessionInfo.Session.Advanced.ApertureTime = apertureTime;
            });
        }

        /// <summary>
        /// Configures settle time.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="settleTime">The settle time to use.</param>
        public static void ConfigureSettleTime(this DMMSessionsBundle sessionsBundle, double settleTime = 0.01)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Advanced.SettleTime = settleTime;
            });
        }

        /// <summary>
        /// Configures measurements using absolute resolution.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="measurementFunction">The measurement function to use.</param>
        /// <param name="range">The range to use.</param>
        /// <param name="resolutionAbsolute">The absolute resolution to use.</param>
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
        /// <param name="measurementFunction">The measurement function to use.</param>
        /// <param name="range">The range to use.</param>
        /// <param name="resolutionDigits">The resolution number of digits to use.</param>
        public static void ConfigureMeasurementDigits(this DMMSessionsBundle sessionsBundle, DmmMeasurementFunction measurementFunction = DmmMeasurementFunction.DCVolts, double range = 0.02, double resolutionDigits = 5.5)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.ConfigureMeasurementDigits(measurementFunction, range, resolutionDigits);
            });
        }

        /// <summary>
        /// Configures multi-point acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="triggerCount">The number of the triggers the device receives before returning to the idle state.</param>
        /// <param name="sampleCount">The number of measurements the device makes in each measurement sequence initiated by a trigger.</param>
        /// <param name="sampleTrigger">The name of the trigger source that initiates the acquisition.</param>
        /// <param name="sampleIntervalInSeconds">The interval in seconds that the device waits between measurements.</param>
        public static void ConfigureMultiPoint(this DMMSessionsBundle sessionsBundle, int triggerCount, int sampleCount, string sampleTrigger, double sampleIntervalInSeconds)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.MultiPoint.Configure(triggerCount, sampleCount, sampleTrigger, PrecisionTimeSpan.FromSeconds(sampleIntervalInSeconds));
            });
        }

        /// <summary>
        /// Configures multi-point acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="triggerCount">The number of the triggers the device receives before returning to the idle state.</param>
        /// <param name="sampleCount">The number of measurements the device makes in each measurement sequence initiated by a trigger.</param>
        /// <param name="sampleTrigger">The trigger source that initiates the acquisition.</param>
        /// <param name="sampleIntervalInSeconds">The interval in seconds that the device waits between measurements.</param>
        public static void ConfigureMultiPoint(this DMMSessionsBundle sessionsBundle, int triggerCount, int sampleCount, DmmSampleTrigger sampleTrigger, double sampleIntervalInSeconds)
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
        /// <param name="powerlineFrequency">The frequency in Hz to use.</param>
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
        /// <param name="triggerDelayInSeconds">The interval in seconds that the device waits after it has received a trigger before taking a measurement.</param>
        public static void ConfigureTrigger(this DMMSessionsBundle sessionsBundle, DmmTriggerSource triggerSource, double triggerDelayInSeconds)
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

        /// <summary>
        /// Allows the DMM to compensate for gain drift since the last external or self-calibration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When ADC Calibration is AUTO, the DMM enables or disables ADC calibration.
        /// </para>
        /// <para>
        /// When ADC Calibration is ON, the DMM measures an internal reference to calculate the correct gain for the measurement.
        /// </para>
        /// <para>
        /// When ADC Calibration is OFF, the DMM does not compensate for changes to the gain.
        /// </para>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="dmmAdcCalibrationMode">The ADC calibration mode to be used.</param>
        public static void ConfigureADCCalibration(this DMMSessionsBundle sessionsBundle, DmmAdcCalibration dmmAdcCalibrationMode)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Advanced.AdcCalibration = dmmAdcCalibrationMode;
            });
        }

        /// <summary>
        /// Configures the DMM for auto zero.
        /// </summary>
        /// <remarks>
        /// <para>When Auto Zero is AUTO, the DMM chooses the AutoZero setting based on the configured function and resolution.</para>
        /// <para>When Auto Zero is OFF, the DMM does not compensate for zero reading offset. Not Supported on 4065 DMM models.</para>
        /// <para>When Auto Zero is ONCE, the DMM takes a zero reading once and then turns off Auto Zero. Not Supported on 4065 DMM models.</para>
        /// <para>
        /// When Auto Zero is ON, the DMM internally disconnects the input, takes a zero reading, and then subtracts the zero reading from the measurement.
        /// This prevents offset voltages present on the input circuitry of the DMM from affecting measurement accuracy.
        /// </para>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="autoZeroMode">The auto zero mode to be used: AUTO, OFF, ON, or ONCE.</param>
        /// <exception cref="NISemiconductorTestException">A device in an underlying session does not support configuring Auto Zero.</exception>
        public static void ConfigureAutoZero(this DMMSessionsBundle sessionsBundle, DmmAuto autoZeroMode)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Advanced.AutoZero = autoZeroMode;
            });
        }
    }
}
