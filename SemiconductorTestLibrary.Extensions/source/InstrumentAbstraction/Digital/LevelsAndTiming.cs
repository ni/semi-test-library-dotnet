using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using IviDriverPrecisionTimeSpan = Ivi.Driver.PrecisionTimeSpan;
using static NationalInstruments.SemiconductorTestLibrary.Common.HelperMethods;

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
        /// Configures a single level. Use this method to configure the same level for all sessions.
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
        /// <inheritdoc cref="DigitalTiming.GetTimeSet(string)"/>
        /// </summary>
        /// <remarks>
        /// Gets the configured time set period.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">The name of the time set.</param>
        /// <returns>The pin-site aware timespan period for the specified timeset.</returns>
        public static PinSiteData<IviDriverPrecisionTimeSpan> GetTimeSetPeriod(this DigitalSessionsBundle sessionsBundle, string timeSet)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((DigitalSessionInformation sessionInfo) =>
            {
                var period = sessionInfo.Session.Timing.GetTimeSet(timeSet).Period;
                return Enumerable.Repeat(period, sessionInfo.AssociatedSitePinList.Count).ToArray();
            });
        }

        /// <summary>
        /// <inheritdoc cref="DigitalTiming.GetTimeSet(string)"/>
        /// Gets the configured time set period.
        /// </summary>
        /// <remarks>
        /// It also checks to confirm if the timeset period values are the same across all sessions in the bundle.
        /// If the values are indeed the same, it will return the single timeset period value.
        /// Otherwise, it will throw an exception.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">The name of the time set.</param>
        /// <returns>A single timeset period value for the specified timeset.</returns>
        public static IviDriverPrecisionTimeSpan GetTimeSetPeriodDistinct(this DigitalSessionsBundle sessionsBundle, string timeSet)
        {
            return GetDistinctValue(
                sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Timing.GetTimeSet(timeSet).Period),
                string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TimeSetPeriodNotDistinct, timeSet));
        }

        /// <summary>
        /// <inheritdoc cref="DigitalTiming.GetTimeSet(string)"/>
        /// </summary>
        /// <remarks>
        /// Gets the configured timeset edge.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">The name of the time set.</param>
        /// <param name="driveEdge">The drive edge to be read.</param>
        /// <returns>The pin-site aware timespan edge values for the specified timeset.</returns>
        public static PinSiteData<IviDriverPrecisionTimeSpan> GetTimeSetEdge(this DigitalSessionsBundle sessionsBundle, string timeSet, TimeSetEdge driveEdge)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, pinSiteInfo) =>
            {
                return sessionInfo.Session.Timing.GetTimeSet(timeSet).GetEdge(pinSiteInfo.SitePinString, driveEdge);
            });
        }

        /// <summary>
        /// <inheritdoc cref="DigitalTiming.GetTimeSet(string)"/>
        /// </summary>
        /// <remarks>
        /// Gets the configured timeset edge multiplier value.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">the name of the time set.</param>
        /// <returns>The pin-site aware timespan edge multiplier values for the specified timeset.</returns>
        public static PinSiteData<int> GetTimeSetEdgeMultiplier(this DigitalSessionsBundle sessionsBundle, string timeSet)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, pinSiteInfo) =>
            {
                return sessionInfo.Session.Timing.GetTimeSet(timeSet).GetEdgeMultiplier(pinSiteInfo.SitePinString);
            });
        }

        /// <summary>
        /// <inheritdoc cref="DigitalTiming.GetTimeSet(string)"/>
        /// </summary>
        /// <remarks>
        /// Gets the configured timeset drive format.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeSet">the name of the time set.</param>
        /// <returns>The pin-site aware drive format for the specified timeset.</returns>
        public static PinSiteData<DriveFormat> GetTimeSetDriveFormat(this DigitalSessionsBundle sessionsBundle, string timeSet)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, pinSiteInfo) =>
            {
                return sessionInfo.Session.Timing.GetTimeSet(timeSet).GetDriveFormat(pinSiteInfo.SitePinString);
            });
        }

        /// <inheritdoc cref="DigitalTiming.TdrEndpointTermination"/>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="tdrEndpointTermination">TDR Endpoint Termination type. The default value is <see cref="TdrEndpointTermination.TdrToOpenCircuit"/>.</param>
        public static void ConfigureTdrEndpointTermination(this DigitalSessionsBundle sessionsBundle, TdrEndpointTermination tdrEndpointTermination)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.TdrEndpointTermination = tdrEndpointTermination;
            });
        }

        /// <summary>
        /// Measures propagation delays through cables, connectors, and load boards using Time-Domain Reflectometry (TDR).
        /// You can optionally apply the offsets to the pins.
        /// Use this method to measure per-site per-pin offsets.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="apply">Whether to apply the offsets to the pins.</param>
        public static PinSiteData<Ivi.Driver.PrecisionTimeSpan> MeasureTDROffsets(this DigitalSessionsBundle sessionsBundle, bool apply = false)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                return sessionInfo.Session.PinAndChannelMap.GetPinSet(sitePinInfo.SitePinString).Tdr(apply)[0];
            });
        }

        /// <summary>
        /// Measures propagation delays through cables, connectors, and load boards using Time-Domain Reflectometry (TDR).
        /// You can optionally apply the offsets to the pins.
        /// Use this method to measure per-instrument session per-pin offsets.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="offsets">The measured TDR offsets. Where the first dimension represents instrument sessions, and the second dimension represents pins.</param>
        /// <param name="apply">Whether to apply the offsets to the pins.</param>
        public static void MeasureTDROffsets(this DigitalSessionsBundle sessionsBundle, out Ivi.Driver.PrecisionTimeSpan[][] offsets, bool apply = false)
        {
            offsets = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.PinSet.Tdr(apply);
            });
        }

        /// <summary>
        /// Applies the correction for propagation delay offsets to a digital pattern instrument.
        /// Use this method to apply per-site per-pin offsets.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="offsets">The per-site per-pin offsets to apply.</param>
        public static void ApplyTDROffsets(this DigitalSessionsBundle sessionsBundle, PinSiteData<Ivi.Driver.PrecisionTimeSpan> offsets)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.PinAndChannelMap.GetPinSet(sitePinInfo.SitePinString).ApplyTdrOffsets(new Ivi.Driver.PrecisionTimeSpan[] { offsets.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName) });
            });
        }

        /// <summary>
        /// Applies the correction for propagation delay offsets to a digital pattern instrument.
        /// Use this method to apply per-instrument session per-pin offsets.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="offsets">Offsets to apply. Where the first dimension represents instrument sessions and the second dimension represents pins.</param>
        public static void ApplyTDROffsets(this DigitalSessionsBundle sessionsBundle, Ivi.Driver.PrecisionTimeSpan[][] offsets)
        {
            sessionsBundle.Do((DigitalSessionInformation sessionInfo, int instrumentIndex) =>
            {
                for (int pinSetIndex = 0; pinSetIndex < sessionInfo.AssociatedSitePinList.Count; pinSetIndex++)
                {
                    sessionInfo.Session.PinAndChannelMap
                        .GetPinSet(sessionInfo.AssociatedSitePinList.ElementAt(pinSetIndex).SitePinString)
                        .ApplyTdrOffsets(new Ivi.Driver.PrecisionTimeSpan[] { offsets[instrumentIndex][pinSetIndex] });
                }
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
        /// <param name="offsets">The per-site per-pin offsets to save.</param>
        /// <param name="filePath">The path of the file to save the offsets to.</param>
        public static void SaveTDROffsetsToFile(this DigitalSessionsBundle sessionsBundle, PinSiteData<Ivi.Driver.PrecisionTimeSpan> offsets, string filePath)
        {
            using (var file = new StreamWriter(filePath))
            {
                foreach (var sitePinInfo in sessionsBundle.AggregateSitePinList)
                {
                    file.WriteLine($"{sitePinInfo.SitePinString}:{offsets.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName).ToDecimal()}");
                }
            }
        }

        /// <summary>
        /// Saves TDR offsets to a file.
        /// </summary>
        /// <remarks>
        /// The resulting file is pinmap specific. It is recommended that the filename provided contains the same name as the pinmap, as well as timestamp.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="offsets">The per-instrument session per-pin offsets to save. Where the first dimension represents instrument sessions and the second dimension represents pins.</param>
        /// <param name="filePath">The path of the file to save the offsets to.</param>
        public static void SaveTDROffsetsToFile(this DigitalSessionsBundle sessionsBundle, Ivi.Driver.PrecisionTimeSpan[][] offsets, string filePath)
        {
            using (var file = new StreamWriter(filePath))
            {
                for (int instrumentIndex = 0; instrumentIndex < offsets.Length; instrumentIndex++)
                {
                    var sitePinList = sessionsBundle.InstrumentSessions.ElementAt(instrumentIndex).AssociatedSitePinList;
                    for (int channelIndex = 0; channelIndex < sitePinList.Count; channelIndex++)
                    {
                        file.WriteLine($"{sitePinList[channelIndex].SitePinString}:{offsets[instrumentIndex][channelIndex].ToDecimal()}");
                    }
                }
            }
        }

        /// <summary>
        /// Loads TDR offsets from a file.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="filePath">The path of the file to load the offsets.</param>
        /// <param name="throwOnMissingChannels">Whether to throw an exception if the offset for any channel is missing.</param>
        /// <returns>TDR offset values retrieved from the file.</returns>
        /// <exception cref="ArgumentException">
        /// This exception will be thrown if throwOnMissingChannels is true and an offset value was not found in the file for one or more of channels in the sessions bundle.
        /// </exception>
        public static PinSiteData<Ivi.Driver.PrecisionTimeSpan> LoadTDROffsetsFromFile(this DigitalSessionsBundle sessionsBundle, string filePath, bool throwOnMissingChannels = true)
        {
            var offsetsFromFile = ReadTdrOffsetsFromFile(filePath);

            var offsetsDict = new Dictionary<string, IDictionary<int, Ivi.Driver.PrecisionTimeSpan>>();
            var missingChannels = new List<string>();
            // Check if channels match what is in the current bundle.
            foreach (var sitePinInfo in sessionsBundle.AggregateSitePinList)
            {
                if (!offsetsFromFile.ContainsKey(sitePinInfo.SitePinString))
                {
                    missingChannels.Add($"{sitePinInfo.SitePinString}");
                    break;
                }
                if (offsetsDict.TryGetValue(sitePinInfo.PinName, out var perSitePinValues))
                {
                    perSitePinValues.Add(sitePinInfo.SiteNumber, offsetsFromFile[sitePinInfo.SitePinString]);
                    break;
                }
                offsetsDict.Add(sitePinInfo.PinName, new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>());
                offsetsDict[sitePinInfo.PinName].Add(sitePinInfo.SiteNumber, offsetsFromFile[sitePinInfo.SitePinString]);
            }

            if (throwOnMissingChannels && missingChannels.Count != 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TDROffsetsMissing, filePath, string.Join(",", missingChannels)));
            }

            return new PinSiteData<Ivi.Driver.PrecisionTimeSpan>(offsetsDict);
        }

        /// <summary>
        /// Loads TDR offsets from a file.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="filePath">The path of the file to load the offsets.</param>
        /// <param name="offsets">TDR offset values retrieved from the file. Where the first dimension represents instrument sessions and the second dimension represents pins.</param>
        /// <param name="throwOnMissingChannels">Whether to throw a message if the offset for any channel is missing.</param>
        /// <exception cref="ArgumentException">
        /// This exception will be thrown if throwOnMissingChannels is true and an offset value was not found in the file for one or more of channels in the sessions bundle.
        /// </exception>
        public static void LoadTDROffsetsFromFile(this DigitalSessionsBundle sessionsBundle, string filePath, out Ivi.Driver.PrecisionTimeSpan[][] offsets, bool throwOnMissingChannels = true)
        {
            var offsetsFromFile = ReadTdrOffsetsFromFile(filePath);

            int instrumentCount = sessionsBundle.InstrumentSessions.Count();
            offsets = new Ivi.Driver.PrecisionTimeSpan[instrumentCount][];
            var missingChannels = new List<string>();
            for (int instrumentIndex = 0; instrumentIndex < instrumentCount; instrumentIndex++)
            {
                var sitePinList = sessionsBundle.InstrumentSessions.ElementAt(instrumentIndex).AssociatedSitePinList;
                offsets[instrumentIndex] = new Ivi.Driver.PrecisionTimeSpan[sitePinList.Count];
                for (int channelIndex = 0; channelIndex < sitePinList.Count; channelIndex++)
                {
                    string sitePinString = sitePinList[channelIndex].SitePinString;
                    if (offsetsFromFile.TryGetValue(sitePinString, out var _))
                    {
                        offsets[instrumentIndex][channelIndex] = offsetsFromFile[sitePinString];
                    }
                    else
                    {
                        missingChannels.Add(sitePinString);
                    }
                }
            }

            if (throwOnMissingChannels && missingChannels.Count != 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TDROffsetsMissing, filePath, string.Join(",", missingChannels)));
            }
        }

        #endregion utility methods

        #region private methods

        private static Dictionary<string, Ivi.Driver.PrecisionTimeSpan> ReadTdrOffsetsFromFile(string filePath)
        {
            var offsetsFromFile = new Dictionary<string, Ivi.Driver.PrecisionTimeSpan>();

            using (var file = new StreamReader(filePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var contents = line.Split(':');
                    var tdrValue = Ivi.Driver.PrecisionTimeSpan.FromSeconds(Convert.ToDouble(contents[1].Trim(), CultureInfo.InvariantCulture));
                    offsetsFromFile.Add(contents[0], tdrValue);
                }
            }

            return offsetsFromFile;
        }

        #endregion private methods
    }
}
