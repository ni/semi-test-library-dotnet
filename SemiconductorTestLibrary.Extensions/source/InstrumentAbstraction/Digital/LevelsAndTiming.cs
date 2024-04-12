using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods for levels and timing.
    /// </summary>
    public static class LevelsAndTiming
    {
        /// <summary>
        /// Defines voltage level types.
        /// </summary>
        public enum LevelType
        {
            /// <summary>
            /// The input voltage that the digital pattern instrument applies to the input of the DUT when the test instrument drives a logic low (0).
            /// </summary>
            Vil,

            /// <summary>
            /// The input voltage that the digital pattern instrument applies to the input of the DUT when the test instrument drives a logic high (1).
            /// </summary>
            Vih,

            /// <summary>
            /// The output voltage from the DUT below which the comparator on the test instrument interprets a logic low (L).
            /// </summary>
            Vol,

            /// <summary>
            /// The output voltage from the DUT above which the comparator on the test instrument interprets a logic high (H).
            /// </summary>
            Voh,

            /// <summary>
            /// The termination voltage the instrument applies during non-drive cycles.
            /// </summary>
            Vterm,

            /// <summary>
            /// The current that the DUT sinks from the active load while outputting a voltage below <see cref="Vcom"/>.
            /// </summary>
            Iol,

            /// <summary>
            /// The current that the DUT sources to the active load while outputting a voltage above <see cref="Vcom"/>.
            /// </summary>
            Ioh,

            /// <summary>
            /// The commutating voltage at which the active load circuit switches between sourcing current and sinking current.
            /// </summary>
            Vcom
        }

        #region methods on DigitalSessionsBundle

        /// <summary>
        /// Configures a single level. Use this method to configure the same level to all sessions.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="levelType">The type of level to configure.</param>
        /// <param name="levelValue">The value of level to configure.</param>
        public static void ConfigureSingleLevel(this DigitalSessionsBundle sessionsBundle, LevelType levelType, double levelValue)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.ConfigureSingleLevel(levelType, levelValue);
            });
        }

        /// <summary>
        /// Configures a single level. Use this method to configure different levels for different sites.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="levelType">The type of level to configure.</param>
        /// <param name="perSiteLevelValues">The per-site value of level to configure.</param>
        public static void ConfigureSingleLevel(this DigitalSessionsBundle sessionsBundle, LevelType levelType, SiteData<double> perSiteLevelValues)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.PinAndChannelMap.GetPinSet(sitePinInfo.SitePinString).ConfigureSingleLevel(levelType, perSiteLevelValues.GetValue(sitePinInfo.SiteNumber));
            });
        }

        /// <summary>
        /// Configures multiple types of levels.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="vil">The vil value to configure.</param>
        /// <param name="vih">The vih value to configure.</param>
        /// <param name="vol">The vol value to configure.</param>
        /// <param name="voh">The voh value to configure.</param>
        /// <param name="vterm">The vterm value to configure.</param>
        public static void ConfigureVoltageLevels(this DigitalSessionsBundle sessionsBundle, double vil, double vih, double vol, double voh, double vterm)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.DigitalLevels.ConfigureVoltageLevels(vil, vih, vol, voh, vterm);
            });
        }

        /// <summary>
        /// Configures termination mode.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="terminationMode">The termination mode to configure.</param>
        public static void ConfigureTerminationMode(this DigitalSessionsBundle sessionsBundle, TerminationMode terminationMode)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.DigitalLevels.TerminationMode = terminationMode;
            });
        }

        /// <summary>
        /// Configures the strobe edge time. Use this method to configure the same strobe edge time to all sessions.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">The name of the time set.</param>
        /// <param name="compareEdge">The strobe edge time to configure.</param>
        public static void ConfigureTimeSetCompareEdgesStrobe(this DigitalSessionsBundle sessionsBundle, string timeSet, double compareEdge)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.GetTimeSet(timeSet).ConfigureCompareEdgesStrobe(sessionInfo.PinSet, Ivi.Driver.PrecisionTimeSpan.FromSeconds(compareEdge));
            });
        }

        /// <summary>
        /// Configures the strobe edge time. Use this method to configure different strobe edge times for different sites.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">The name of the time set.</param>
        /// <param name="compareEdges">The per-site strobe edge time to configure.</param>
        public static void ConfigureTimeSetCompareEdgesStrobe(this DigitalSessionsBundle sessionsBundle, string timeSet, SiteData<double> compareEdges)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Timing.GetTimeSet(timeSet).ConfigureCompareEdgesStrobe(sitePinInfo.SitePinString, Ivi.Driver.PrecisionTimeSpan.FromSeconds(compareEdges.GetValue(sitePinInfo.SiteNumber)));
            });
        }

        /// <summary>
        /// Configures the strobe edge time. Use this method to configure different strobe edge times for different site-pin pairs.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">The name of the time set.</param>
        /// <param name="compareEdges">The strobe edge time for all site-pin pairs to configure.</param>
        public static void ConfigureTimeSetCompareEdgesStrobe(this DigitalSessionsBundle sessionsBundle, string timeSet, PinSiteData<double> compareEdges)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Timing.GetTimeSet(timeSet).ConfigureCompareEdgesStrobe(sitePinInfo.SitePinString, Ivi.Driver.PrecisionTimeSpan.FromSeconds(compareEdges.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName)));
            });
        }

        /// <summary>
        /// Configures the period of a time set.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">The name of the time set.</param>
        /// <param name="period">The period to configure.</param>
        public static void ConfigureTimeSetPeriod(this DigitalSessionsBundle sessionsBundle, string timeSet, double period)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.GetTimeSet(timeSet).ConfigurePeriod(Ivi.Driver.PrecisionTimeSpan.FromSeconds(period));
            });
        }

        /// <summary>
        /// Configures the drive format and drive edge placement.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">The name of the time set.</param>
        /// <param name="format">The drive format of the time set.</param>
        /// <param name="driveOn">The delay from the beginning of the vector period for turning on the pin driver.</param>
        /// <param name="driveData">The delay from the beginning of the vector period until the pattern data is driven to the pattern value.</param>
        /// <param name="driveReturn">The delay from the beginning of the vector period until the pin changes from the pattern data to the return value.</param>
        /// <param name="driveOff">The delay from the beginning of the vector period to turn off the pin driver.</param>
        public static void ConfigureTimeSetDriveEdges(
            this DigitalSessionsBundle sessionsBundle,
            string timeSet,
            DriveFormat format,
            double driveOn,
            double driveData,
            double driveReturn,
            double driveOff)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.GetTimeSet(timeSet).ConfigureDriveEdges(
                    sessionInfo.PinSet,
                    format,
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveOn),
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveData),
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveReturn),
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveOff));
            });
        }

        /// <summary>
        /// Configures the drive format and drive edge placement.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">The name of the time set.</param>
        /// <param name="format">The drive format of the time set.</param>
        /// <param name="driveOn">The delay from the beginning of the vector period for turning on the pin driver.</param>
        /// <param name="driveData">The delay from the beginning of the vector period until the pattern data is driven to the pattern value.</param>
        /// <param name="driveReturn">The delay from the beginning of the vector period until the pin changes from the pattern data to the return value.</param>
        /// <param name="driveOff">The delay from the beginning of the vector period to turn off the pin driver.</param>
        /// <param name="driveData2">The delay from the beginning of the vector period until the pattern data is driven to the second pattern value.</param>
        /// <param name="driveReturn2">The delay from the beginning of the vector period until the pin changes from the second pattern data to the return value.</param>
        public static void ConfigureTimeSetDriveEdges(
            this DigitalSessionsBundle sessionsBundle,
            string timeSet,
            DriveFormat format,
            double driveOn,
            double driveData,
            double driveReturn,
            double driveOff,
            double driveData2,
            double driveReturn2)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.GetTimeSet(timeSet).ConfigureDriveEdges(
                    sessionInfo.PinSet,
                    format,
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveOn),
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveData),
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveReturn),
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveOff),
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveData2),
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(driveReturn2));
            });
        }

        /// <summary>
        /// Measures propagation delays through cables, connectors, and load boards using Time-Domain Reflectometry (TDR). You can optionally apply the offsets to the pins.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="apply">Whether to apply the offsets to the pins.</param>
        /// <returns>The measured TDR offsets.</returns>
        public static Ivi.Driver.PrecisionTimeSpan[][] MeasureTDROffsets(this DigitalSessionsBundle sessionsBundle, bool apply = false)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.PinSet.Tdr(apply);
            });
        }

        /// <summary>
        /// Applies the correction for propagation delay offsets to a digital pattern instrument. Use this method to apply per-site per-pin offsets.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="offsets">The per-site per-pin offsets to apply.</param>
        public static void ApplyTDROffsets(this DigitalSessionsBundle sessionsBundle, PinSiteData<double> offsets)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.PinAndChannelMap.GetPinSet(sitePinInfo.SitePinString).ApplyTdrOffsets(new Ivi.Driver.PrecisionTimeSpan[] { Ivi.Driver.PrecisionTimeSpan.FromSeconds(offsets.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName)) });
            });
        }

        /// <summary>
        /// Applies digital levels and timing defined in the loaded levels and timing sheets.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="levelsSheet">The name of the levels sheet to apply.</param>
        /// <param name="timingSheet">The name of the timing sheet to apply.</param>
        public static void ApplyLevelsAndTiming(this DigitalSessionsBundle sessionsBundle, string levelsSheet, string timingSheet)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.ApplyLevelsAndTiming(sessionInfo.SiteListString, levelsSheet, timingSheet);
            });
        }

        #endregion methods on DigitalSessionsBundle

        #region methods on DigitalPinSet

        /// <summary>
        /// Configures a single level.
        /// </summary>
        /// <param name="pinSet">The <see cref="DigitalPinSet"/> object.</param>
        /// <param name="levelType">The type of the level to configure.</param>
        /// <param name="levelValue">The value of the level to configure.</param>
        public static void ConfigureSingleLevel(this DigitalPinSet pinSet, LevelType levelType, double levelValue)
        {
            switch (levelType)
            {
                case LevelType.Vil:
                    pinSet.DigitalLevels.Vil = levelValue;
                    break;

                case LevelType.Vih:
                    pinSet.DigitalLevels.Vih = levelValue;
                    break;

                case LevelType.Vol:
                    pinSet.DigitalLevels.Vol = levelValue;
                    break;

                case LevelType.Voh:
                    pinSet.DigitalLevels.Voh = levelValue;
                    break;

                case LevelType.Vterm:
                    pinSet.DigitalLevels.Vterm = levelValue;
                    break;

                case LevelType.Iol:
                    pinSet.DigitalLevels.Iol = levelValue;
                    break;

                case LevelType.Ioh:
                    pinSet.DigitalLevels.Ioh = levelValue;
                    break;

                case LevelType.Vcom:
                    pinSet.DigitalLevels.Vcom = levelValue;
                    break;
            }
        }

        #endregion methods on DigitalPinSet

        #region utility methods

        /// <summary>
        /// Saves TDR offsets to a file.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="perInstrumentPerChannelOffsets">The offsets to save.</param>
        /// <param name="filePath">The path of the file to save the offsets to.</param>
        public static void SaveTDROffsetsToFile(this DigitalSessionsBundle sessionsBundle, Ivi.Driver.PrecisionTimeSpan[][] perInstrumentPerChannelOffsets, string filePath)
        {
            using (var file = new StreamWriter(filePath))
            {
                for (int instrumentIndex = 0; instrumentIndex < perInstrumentPerChannelOffsets.Length; instrumentIndex++)
                {
                    var sitePinList = sessionsBundle.InstrumentSessions.ElementAt(instrumentIndex).AssociatedSitePinList;
                    for (int channelIndex = 0; channelIndex < sitePinList.Count; channelIndex++)
                    {
                        file.WriteLine($"{sitePinList[channelIndex].SitePinString}: {perInstrumentPerChannelOffsets[instrumentIndex][channelIndex].ToDecimal()}");
                    }
                }
            }
        }

        /// <summary>
        /// Loads TDR offsets from a file.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="filePath">The path of the file to load the offsets.</param>
        /// <param name="throwOnMissingChannels">Whether to throw a message if the offset for any channel is missing.</param>
        /// <returns>The loaded offsets.</returns>
        public static PrecisionTimeSpan[][] LoadTDROffsetsFromFile(this DigitalSessionsBundle sessionsBundle, string filePath, bool throwOnMissingChannels = true)
        {
            var offsets = new Dictionary<string, PrecisionTimeSpan>();
            using (var file = new StreamReader(filePath))
            {
                var contents = file.ReadLine().Split(':');
                offsets.Add(contents[0], PrecisionTimeSpan.FromSeconds(Convert.ToDouble(contents[1].Trim(), CultureInfo.InvariantCulture)));
            }

            int instrumentCount = sessionsBundle.InstrumentSessions.Count();
            var loadedOffsets = new PrecisionTimeSpan[instrumentCount][];
            var missingChannels = new List<string>();
            for (int instrumentIndex = 0; instrumentIndex < instrumentCount; instrumentIndex++)
            {
                var sitePinList = sessionsBundle.InstrumentSessions.ElementAt(instrumentIndex).AssociatedSitePinList;
                loadedOffsets[instrumentIndex] = new PrecisionTimeSpan[sitePinList.Count];
                for (int channelIndex = 0; channelIndex < sitePinList.Count; channelIndex++)
                {
                    string sitePinString = sitePinList[channelIndex].SitePinString;
                    if (offsets.TryGetValue(sitePinString, out var _))
                    {
                        loadedOffsets[instrumentIndex][channelIndex] = offsets[sitePinString];
                    }
                    else
                    {
                        missingChannels.Add(sitePinString);
                    }
                }
            }

            return throwOnMissingChannels && missingChannels.Count == 0
                ? throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TDROffsetsMissing, filePath, string.Join(",", missingChannels)))
                : loadedOffsets;
        }

        #endregion utility methods
    }
}
