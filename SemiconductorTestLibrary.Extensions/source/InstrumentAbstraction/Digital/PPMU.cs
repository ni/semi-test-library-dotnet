using System;
using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods for PPMU measurements.
    /// </summary>
    public static class PPMU
    {
        #region methods on DigitalSessionsBundle

        /// <summary>
        /// Forces voltage on the target pin(s) at the specified level. You must provide the voltage level value, and the method will assume all other properties that have been previously set.  Optionally, you can also provide a specific current limit, current limit range, and voltage level range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="voltageLevel">The voltage level.</param>
        /// <param name="currentLimitRange">The current limit range.</param>
        /// <param name="apertureTime">The aperture Time.</param>
        /// <param name="settlingTime">The settling time.</param>
        /// <remarks>Use this method to force the same voltage level on all sessions.</remarks>
        public static void ForceVoltage(
            this DigitalSessionsBundle sessionsBundle,
            double voltageLevel,
            double? currentLimitRange = null,
            double? apertureTime = null,
            double? settlingTime = null)
        {
            var settings = new PPMUForcingSettings
            {
                OutputFunction = PpmuOutputFunction.DCVoltage,
                VoltageLevel = voltageLevel,
                CurrentLimitRange = currentLimitRange,
                ApertureTime = apertureTime,
                SettlingTime = settlingTime
            };

            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Force(sessionInfo.PinSetString, settings);
            });
        }

        /// <summary>
        /// Forces voltage on the target pin(s) at the specified level. You must provide the voltage level values, and the method will assume all other properties that have been previously set.  Optionally, you can also provide a specific current limit, current limit range, and voltage level range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="voltageLevels">The voltage levels for all sites.</param>
        /// <param name="currentLimitRange">The current limit range.</param>
        /// <param name="apertureTime">The aperture Time.</param>
        /// <param name="settlingTime">The settling time.</param>
        /// <remarks>Use this method to force different voltage levels on different sites.</remarks>
        public static void ForceVoltage(
            this DigitalSessionsBundle sessionsBundle,
            SiteData<double> voltageLevels,
            double? currentLimitRange = null,
            double? apertureTime = null,
            double? settlingTime = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new PPMUForcingSettings
                {
                    OutputFunction = PpmuOutputFunction.DCVoltage,
                    VoltageLevel = voltageLevels.GetValue(sitePinInfo.SiteNumber),
                    CurrentLimitRange = currentLimitRange,
                    ApertureTime = apertureTime,
                    SettlingTime = settlingTime
                };
                sessionInfo.Session.Force(sitePinInfo.SitePinString, settings);
            });
        }

        /// <summary>
        /// Forces voltage on the target pin(s) at the specified level. You must provide the voltage level values, and the method will assume all other properties that have been previously set.  Optionally, you can also provide a specific current limit, current limit range, and voltage level range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="voltageLevels">The voltage levels for all site-pin pairs.</param>
        /// <param name="currentLimitRange">The current limit range.</param>
        /// <param name="apertureTime">The aperture Time.</param>
        /// <param name="settlingTime">The settling time.</param>
        /// <remarks>Use this method to force different voltage levels for different site-pin pairs.</remarks>
        public static void ForceVoltage(
            this DigitalSessionsBundle sessionsBundle,
            PinSiteData<double> voltageLevels,
            double? currentLimitRange = null,
            double? apertureTime = null,
            double? settlingTime = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new PPMUForcingSettings
                {
                    OutputFunction = PpmuOutputFunction.DCVoltage,
                    VoltageLevel = voltageLevels.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName),
                    CurrentLimitRange = currentLimitRange,
                    ApertureTime = apertureTime,
                    SettlingTime = settlingTime
                };
                sessionInfo.Session.Force(sitePinInfo.SitePinString, settings);
            });
        }

        /// <summary>
        /// Forces voltage on the target pin(s).
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="settings">The per-pin settings to use.</param>
        public static void ForceVoltage(this DigitalSessionsBundle sessionsBundle, IDictionary<string, PPMUForcingSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                settings[sitePinInfo.PinName].OutputFunction = PpmuOutputFunction.DCVoltage;
                sessionInfo.Session.Force(sitePinInfo.SitePinString, settings[sitePinInfo.PinName]);
            });
        }

        /// <summary>
        /// Forces current on the target pin(s) at the specified level. You must provide a current level value, and the method will assume all other properties that have been previously set.  Optionally, you can also provide a specific voltage limit and current level range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="currentLevelRange">The current level range.</param>
        /// <param name="voltageLimitLow">The low voltage limit.</param>
        /// <param name="voltageLimitHigh">The high voltage limit.</param>
        /// <param name="apertureTime">The aperture time.</param>
        /// <param name="settlingTime">The settling time.</param>
        /// <remarks>Use this method to force the same current level on all sites.</remarks>
        public static void ForceCurrent(
            this DigitalSessionsBundle sessionsBundle,
            double currentLevel,
            double? currentLevelRange = null,
            double? voltageLimitLow = null,
            double? voltageLimitHigh = null,
            double? apertureTime = null,
            double? settlingTime = null)
        {
            var settings = new PPMUForcingSettings
            {
                OutputFunction = PpmuOutputFunction.DCCurrent,
                CurrentLevel = currentLevel,
                CurrentLevelRange = currentLevelRange,
                VoltageLimitLow = voltageLimitLow,
                VoltageLimitHigh = voltageLimitHigh,
                ApertureTime = apertureTime,
                SettlingTime = settlingTime
            };

            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Force(sessionInfo.PinSetString, settings);
            });
        }

        /// <summary>
        /// Forces current on the target pin(s).
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="settings">The per-pin settings to use.</param>
        public static void ForceCurrent(this DigitalSessionsBundle sessionsBundle, IDictionary<string, PPMUForcingSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                settings[sitePinInfo.PinName].OutputFunction = PpmuOutputFunction.DCCurrent;
                sessionInfo.Session.Force(sitePinInfo.SitePinString, settings[sitePinInfo.PinName]);
            });
        }

        /// <summary>
        /// Measures and returns per-instrument per-channel results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="measurementType">The type of the measurement, could be either voltage or current.</param>
        /// <returns>The measurements in per-instrument per-channel format.</returns>
        public static double[][] MeasureAndReturnPerInstrumentPerChannelResults(this DigitalSessionsBundle sessionsBundle, MeasurementType measurementType)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Measure(measurementType));
        }

        /// <summary>
        /// Measures the voltage on the target pin(s) and returns a pin- and site-aware data object.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>The voltage measurements.</returns>
        public static PinSiteData<double> MeasureVoltage(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo => sessionInfo.Measure(MeasurementType.Voltage));
        }

        /// <summary>
        /// Measures the current on the target pin(s) and returns a pin- and site-aware data object.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>The current measurements.</returns>
        public static PinSiteData<double> MeasureCurrent(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo => sessionInfo.Measure(MeasurementType.Current));
        }

        /// <summary>
        /// Measures the voltage on the target pin(s) and immediately publishes the results using the Published Data Id passed in.
        /// </summary>
        /// <remarks>
        /// Use this method to save test time if the measurement results do not needed for any other operations.
        /// Otherwise, use the override for this method that returns PinSiteData.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="publishedDataId">The Published Data Id string.</param>
        /// <param name="voltageMeasurements">The returned voltage measurements.</param>
        public static void MeasureAndPublishVoltage(this DigitalSessionsBundle sessionsBundle, string publishedDataId, out double[][] voltageMeasurements)
        {
            voltageMeasurements = sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.Measure(MeasurementType.Voltage), publishedDataId);
        }

        /// <summary>
        /// Measures the voltage on the target pin(s) and immediately publishes the results using the Published Data Id passed in.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="publishedDataId">The Published Data Id string.</param>
        /// <returns>The pin-site aware voltage measurements.</returns>
        public static PinSiteData<double> MeasureAndPublishVoltage(this DigitalSessionsBundle sessionsBundle, string publishedDataId)
        {
            MeasureAndPublishVoltage(sessionsBundle, publishedDataId, out var voltageMeasurements);
            return sessionsBundle.InstrumentSessions.PerInstrumentPerChannelResultsToPerPinPerSiteResults(voltageMeasurements);
        }

        /// <summary>
        /// Measures the current on the target pin(s) and immediately publishes the results using the Published Data Id passed in.
        /// </summary>
        /// <remarks>
        /// Use this method to save test time if the measurement results do not needed for any other operations.
        /// Otherwise, use the override for this method that returns PinSiteData.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="publishedDataId">The Published Data Id string.</param>
        /// <param name="currentMeasurements">The returned current measurements.</param>
        public static void MeasureAndPublishCurrent(this DigitalSessionsBundle sessionsBundle, string publishedDataId, out double[][] currentMeasurements)
        {
            currentMeasurements = sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.Measure(MeasurementType.Current), publishedDataId);
        }

        /// <summary>
        /// Measures the current on the target pin(s) and immediately publishes the results using the Published Data Id passed in.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="publishedDataId">The Published Data Id string.</param>
        /// <returns>The pin-site aware current measurements.</returns>
        public static PinSiteData<double> MeasureAndPublishCurrent(this DigitalSessionsBundle sessionsBundle, string publishedDataId)
        {
            MeasureAndPublishCurrent(sessionsBundle, publishedDataId, out var currentMeasurements);
            return sessionsBundle.InstrumentSessions.PerInstrumentPerChannelResultsToPerPinPerSiteResults(currentMeasurements);
        }

        /// <summary>
        /// Sets the Selected Function mode to Off to put the digital driver into a non-drive state, disables the active load, disconnects the PPMU, and closes the I/O switch connecting the instrument channel.
        /// </summary>
        /// <remarks>
        /// Note that the output channel is still physically connected after calling this method. To physically disconnect the output channel, call DisconnectOutput() method.
        /// <para></para>
        /// <example>
        /// Example:
        /// <code>
        /// DigitalSessionsBundle myDutPin = new TSMSessionManager().Digital("MyDutPin");
        /// myDutPin.TurnOffOutput(settlingTimeInSeconds: 0.1);
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="settlingTimeInSeconds">The settling time in seconds. Null means immediately turning off operation to settle.</param>
        public static void TurnOffOutput(this DigitalSessionsBundle sessionsBundle, double? settlingTimeInSeconds = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.SelectedFunction = SelectedFunction.Off;
            });
            PreciseWait(settlingTimeInSeconds);
        }

        /// <summary>
        /// The pin is electrically disconnected from instrument functions.
        /// </summary>
        /// <remarks>
        /// Remarks:
        /// <list type="bullet">
        /// <item>Selecting this function causes the PPMU to stop sourcing prior to disconnecting the pin.</item>
        /// <item>CAUTION: In the Disconnect state, some I/O protection and sensing circuitry remains exposed. Do not subject the instrument to voltage beyond its operating range.</item>
        /// </list>
        /// <example>
        /// Example:
        /// <code>
        /// DigitalSessionsBundle myDutPin = new TSMSessionManager().Digital("MyDutPin");
        /// myDutPin.DisconnectOutput(settlingTimeInSeconds: 0.1);
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="settlingTimeInSeconds">The settling time in seconds. Passing a Null value (default) bypasses the wait operation (No-op).</param>
        public static void DisconnectOutput(this DigitalSessionsBundle sessionsBundle, double? settlingTimeInSeconds = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.SelectedFunction = SelectedFunction.Disconnect;
            });
            PreciseWait(settlingTimeInSeconds);
        }

        /// <summary>
        /// Sets the Selected Function mode to PPMU, such that the PPMU controls the specified pin(s) and connects the PPMU.
        /// The pin driver is in a non-drive state, and the active load is disabled.
        /// </summary>
        /// <remarks>
        /// Remarks:
        /// <list type="bullet">
        /// <item>The PPMU does not start sourcing or measuring until ForceVoltage(), ForceCurrent(), MeasureVoltage(), or MeasureCurrent() is called.</item>
        /// <item>The driver, comparator, and active load are off while this function is selected.</item>
        /// <item>If you change the Selected Function mode to PPMU using this method, the PPMU is initially not sourcing.</item>
        /// <item>Note: you can make PPMU voltage measurements calling MeasureVoltage or MeasureCurrent from within any selected function.</item>
        /// </list>
        /// <example>
        /// Example:
        /// <code>
        /// DigitalSessionsBundle myDutPin = new TSMSessionManager().Digital("MyDutPin");
        /// myDutPin.SelectPPMU();
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="settlingTimeInSeconds">The settling time in seconds. Passing a Null value (default) bypasses the wait operation (No-op).</param>
        public static void SelectPPMU(this DigitalSessionsBundle sessionsBundle, double? settlingTimeInSeconds = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.SelectedFunction = SelectedFunction.Ppmu;
            });
            PreciseWait(settlingTimeInSeconds);
        }

        /// <summary>
        /// Sets the Selected Function mode to Digital, such that the pattern sequencer controls the specified pin(s).
        /// </summary>
        /// <remarks>
        /// Remarks:
        /// <list type="bullet">
        /// <item>If a pattern is being bursted, the pin immediately switches to bursting the pattern.</item>
        /// <item>The PPMU stops sourcing and turns off when the Digital function is selected. Despite this, you can still make voltage measurements.</item>
        /// <item>Internally within the instrument the pin electrics are now connected to the driver, comparator, and active load functions.</item>
        /// <item>The state of the digital pin driver when you change the selected function to Digital is determined by the most recent call to WriteStatic or the last vector of the most recently bursted pattern, whichever is latter.</item>
        /// </list>
        /// <example>
        /// Example:
        /// <code>
        /// DigitalSessionsBundle myDutPin = new TSMSessionManager().Digital("MyDutPin");
        /// myDutPin.SelectDigital();
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="settlingTimeInSeconds">The settling time in seconds. Passing a Null value (default) bypasses the wait operation (No-op).</param>
        public static void SelectDigital(this DigitalSessionsBundle sessionsBundle, double? settlingTimeInSeconds = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.SelectedFunction = SelectedFunction.Digital;
            });
            PreciseWait(settlingTimeInSeconds);
        }

        /// <summary>
        /// Configures the aperture time for the PPMU measurement.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="apertureTimeInSeconds">The aperture time in seconds.</param>
        public static void ConfigureApertureTime(this DigitalSessionsBundle sessionsBundle, double apertureTimeInSeconds)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.Ppmu.ConfigureApertureTime(apertureTimeInSeconds, PpmuApertureTimeUnits.Seconds);
            });
        }

        #endregion methods on DigitalSessionsBundle

        #region methods on DigitalSessionInformation

        /// <summary>
        /// Measures on digital devices.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DigitalSessionInformation"/> object.</param>
        /// <param name="measurementType">The type of the measurement, could be either voltage or current.</param>
        /// <returns>The measurements.</returns>
        public static double[] Measure(this DigitalSessionInformation sessionInfo, MeasurementType measurementType)
        {
            var ppmu = sessionInfo.Session.PinAndChannelMap.GetPinSet(sessionInfo.PinSetString).Ppmu;
            return ppmu.Measure(measurementType is MeasurementType.Voltage ? PpmuMeasurementType.Voltage : PpmuMeasurementType.Current);
        }

        #endregion methods on DigitalSessionInformation

        #region methods on NIDigital session

        /// <summary>
        /// Forces on digital devices.
        /// </summary>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <param name="pinSetString">The pin set string.</param>
        /// <param name="settings">The forcing settings.</param>
        public static void Force(this NIDigital session, string pinSetString, PPMUForcingSettings settings)
        {
            var ppmu = session.PinAndChannelMap.GetPinSet(pinSetString).Ppmu;
            ppmu.OutputFunction = settings.OutputFunction;
            if (settings.OutputFunction.Equals(PpmuOutputFunction.DCVoltage))
            {
                ConfigureVoltageSettings(ppmu, settings);
            }
            else
            {
                ConfigureCurrentSettings(ppmu, settings);
            }
            if (settings.ApertureTime.HasValue)
            {
                ppmu.ConfigureApertureTime(settings.ApertureTime.Value, PpmuApertureTimeUnits.Seconds);
            }
            ppmu.Source();
            PreciseWait(settings.SettlingTime);
        }

        #endregion methods on NIDigital session

        #region private methods

        private static void ConfigureVoltageSettings(DigitalPpmu ppmu, PPMUForcingSettings settings)
        {
            if (settings.CurrentLimitRange.HasValue)
            {
                ppmu.DCVoltage.CurrentLimitRange = Math.Abs(settings.CurrentLimitRange.Value);
            }
            ppmu.DCVoltage.VoltageLevel = settings.VoltageLevel;
        }

        private static void ConfigureCurrentSettings(DigitalPpmu ppmu, PPMUForcingSettings settings)
        {
            ppmu.DCCurrent.CurrentLevelRange = settings.CurrentLevelRange ?? Math.Min(0.032, Math.Max(2e-6, Math.Abs(settings.CurrentLevel)));
            ppmu.DCCurrent.CurrentLevel = settings.CurrentLevel;
            if (settings.VoltageLimitLow.HasValue && settings.VoltageLimitHigh.HasValue)
            {
                ppmu.DCCurrent.ConfigureVoltageLimits(settings.VoltageLimitLow.Value, settings.VoltageLimitHigh.Value);
            }
        }

        #endregion private methods
    }

    /// <summary>
    /// Defines settings for PPMU forcing.
    /// </summary>
    public class PPMUForcingSettings
    {
        /// <summary>
        /// The output function.
        /// </summary>
        public PpmuOutputFunction OutputFunction { get; set; } = PpmuOutputFunction.DCVoltage;

        /// <summary>
        /// The voltage level when forcing voltage.
        /// </summary>
        public double VoltageLevel { get; set; }

        /// <summary>
        /// The current limit range when forcing voltage.
        /// </summary>
        public double? CurrentLimitRange { get; set; } = 0.01;

        /// <summary>
        /// The current level when forcing current.
        /// </summary>
        public double CurrentLevel { get; set; }

        /// <summary>
        /// The current level range when forcing current. Null value will be ignored.
        /// </summary>
        public double? CurrentLevelRange { get; set; }

        /// <summary>
        /// The low voltage limit when forcing current. Null value will be ignored.
        /// </summary>
        public double? VoltageLimitLow { get; set; }

        /// <summary>
        /// The high voltage limit when forcing current. Null value will be ignored.
        /// </summary>
        public double? VoltageLimitHigh { get; set; }

        /// <summary>
        /// The aperture time. Null value will be ignored.
        /// </summary>
        public double? ApertureTime { get; set; }

        /// <summary>
        /// The settling time. Null value will be ignored.
        /// </summary>
        public double? SettlingTime { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PPMUForcingSettings()
        {
        }
    }
}
