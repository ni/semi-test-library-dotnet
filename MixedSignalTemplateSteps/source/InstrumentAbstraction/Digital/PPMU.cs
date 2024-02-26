using System;
using System.Collections.Generic;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.DataAbstraction;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.ModularInstruments.NIDigital;
using static NationalInstruments.MixedSignalLibrary.Common.Utilities;
using static NationalInstruments.MixedSignalLibrary.TypeDefinitions;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods for PPMU measurements.
    /// </summary>
    public static class PPMU
    {
        #region methods on DigitalSessionsBundle

        /// <summary>
        /// Forces voltage. Use this method to force the same voltage level on all sessions.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="voltageLevel">The voltage level.</param>
        /// <param name="currentLimitRange">The current limit range.</param>
        /// <param name="apertureTime">The aperture Time.</param>
        /// <param name="settlingTime">The settling time.</param>
        public static void ForceVoltage(
            this DigitalSessionsBundle sessionsBundle,
            double voltageLevel,
            double currentLimitRange,
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
        /// Forces voltage. Use this method to force different voltage levels on different sites.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="voltageLevels">The voltage levels for all sites.</param>
        /// <param name="currentLimitRange">The current limit range.</param>
        /// <param name="apertureTime">The aperture Time.</param>
        /// <param name="settlingTime">The settling time.</param>
        public static void ForceVoltage(
            this DigitalSessionsBundle sessionsBundle,
            IDictionary<int, double> voltageLevels,
            double currentLimitRange,
            double? apertureTime = null,
            double? settlingTime = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new PPMUForcingSettings
                {
                    OutputFunction = PpmuOutputFunction.DCVoltage,
                    VoltageLevel = voltageLevels[sitePinInfo.SiteNumber.Value],
                    CurrentLimitRange = currentLimitRange,
                    ApertureTime = apertureTime,
                    SettlingTime = settlingTime
                };
                sessionInfo.Session.Force(sitePinInfo.SitePinString, settings);
            });
        }

        /// <summary>
        /// Forces voltage. Use this method to force different voltage levels for different site-pin pairs.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="voltageLevels">The voltage levels for all site-pin pairs.</param>
        /// <param name="currentLimitRange">The current limit range.</param>
        /// <param name="apertureTime">The aperture Time.</param>
        /// <param name="settlingTime">The settling time.</param>
        public static void ForceVoltage(
            this DigitalSessionsBundle sessionsBundle,
            IDictionary<int, Dictionary<string, double>> voltageLevels,
            double currentLimitRange,
            double? apertureTime = null,
            double? settlingTime = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new PPMUForcingSettings
                {
                    OutputFunction = PpmuOutputFunction.DCVoltage,
                    VoltageLevel = voltageLevels[sitePinInfo.SiteNumber.Value][sitePinInfo.PinName],
                    CurrentLimitRange = currentLimitRange,
                    ApertureTime = apertureTime,
                    SettlingTime = settlingTime
                };
                sessionInfo.Session.Force(sitePinInfo.SitePinString, settings);
            });
        }

        /// <summary>
        /// Forces voltage.
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
        /// Forces current. Use this method to force the same current level on all sites.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="currentLevelRange">The current level range.</param>
        /// <param name="voltageLimitLow">The low voltage limit.</param>
        /// <param name="voltageLimitHigh">The high voltage limit.</param>
        /// <param name="apertureTime">The aperture time.</param>
        /// <param name="settlingTime">The settling time.</param>
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
        /// Forces current.
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
        /// <param name="measurementType">The type of the measurement, could be voltage, current or both. Default to both.</param>
        /// <returns>The measurements in per-instrument per-channel format. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<double[][], double[][]> MeasureAndReturnPerInstrumentPerChannelResults(this DigitalSessionsBundle sessionsBundle, MeasurementType measurementType = MeasurementType.Both)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Measure(measurementType));
        }

        /// <summary>
        /// Measures and returns per-site per-pin results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="measurementType">The type of the measurement, could be voltage, current or both. Default to both.</param>
        /// <returns>The measurements in per-site per-pin format. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<PinSiteData<double>, PinSiteData<double>> MeasureAndReturnPerSitePerPinResults(this DigitalSessionsBundle sessionsBundle, MeasurementType measurementType = MeasurementType.Both)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo => sessionInfo.Measure(measurementType));
        }

        /// <summary>
        /// Powers down digital devices.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="settlingTime">The settling time. Null means no need to wait for the turn off operation to settle.</param>
        public static void PowerDown(this DigitalSessionsBundle sessionsBundle, double? settlingTime = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.TurnOffChannels(sessionInfo.PinSetString);
            });
            PreciseWait(settlingTime);
        }

        #endregion methods on DigitalSessionsBundle

        #region methods on DigitalSessionInformation

        /// <summary>
        /// Does measure on digital devices.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DigitalSessionInformation"/> object.</param>
        /// <param name="measurementType">The type of the measurement, could be voltage, current or both.</param>
        /// <returns>The measurements. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<double[], double[]> Measure(this DigitalSessionInformation sessionInfo, MeasurementType measurementType)
        {
            double[] voltageMeasurements = null;
            double[] currentMeasurements = null;
            var ppmu = sessionInfo.Session.PinAndChannelMap.GetPinSet(sessionInfo.PinSetString).Ppmu;
            if (measurementType != MeasurementType.Current)
            {
                voltageMeasurements = ppmu.Measure(PpmuMeasurementType.Voltage);
            }
            if (measurementType != MeasurementType.Voltage)
            {
                currentMeasurements = ppmu.Measure(PpmuMeasurementType.Current);
            }
            return new Tuple<double[], double[]>(voltageMeasurements, currentMeasurements);
        }

        #endregion methods on DigitalSessionInformation

        #region methods on NIDigital session

        /// <summary>
        /// Does force on digital devices.
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

        /// <summary>
        /// Turns off digital device channels.
        /// </summary>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <param name="pinSetString">The pin set string.</param>
        /// <param name="settlingTime">The settling time. Null means no need to wait for the turn off operation to settle.</param>
        public static void TurnOffChannels(this NIDigital session, string pinSetString, double? settlingTime = null)
        {
            session.PinAndChannelMap.GetPinSet(pinSetString).SelectedFunction = SelectedFunction.Off;
            PreciseWait(settlingTime);
        }

        #endregion methods on NIDigital session

        #region private methods

        private static void ConfigureVoltageSettings(DigitalPpmu ppmu, PPMUForcingSettings settings)
        {
            ppmu.DCVoltage.CurrentLimitRange = Math.Abs(settings.CurrentLimitRange);
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
        public double CurrentLimitRange { get; set; } = 0.01;

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

        internal PPMUForcingSettings(MixedSignalParameters parameters, int index, PpmuOutputFunction outputFunction)
        {
            OutputFunction = outputFunction;
            ApertureTime = parameters.ApertureTimes?.ElementAtOrFirst(index);
            SettlingTime = parameters.SettlingTimes?.ElementAtOrFirst(index);
            if (outputFunction == PpmuOutputFunction.DCVoltage)
            {
                VoltageLevel = parameters.Supplies.ElementAtOrFirst(index);
                CurrentLimitRange = parameters.Limits.ElementAtOrFirst(index);
            }
            else
            {
                CurrentLevel = parameters.Supplies.ElementAtOrFirst(index);
                var absoluteValueOfVoltageLimit = Math.Abs(parameters.Limits.ElementAtOrFirst(index));
                VoltageLimitHigh = absoluteValueOfVoltageLimit;
                VoltageLimitLow = Math.Max(-2, -absoluteValueOfVoltageLimit);
            }
        }
    }
}
