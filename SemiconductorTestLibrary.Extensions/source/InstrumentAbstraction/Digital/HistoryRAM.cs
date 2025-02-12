using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods for History RAM.
    /// </summary>
    public static class HistoryRAM
    {
        #region methods on DigitalSessionsBundle

        /// <summary>
        /// Configures History RAM.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="settings">The configuration to apply.</param>
        public static void ConfigureHistoryRAM(this DigitalSessionsBundle sessionsBundle, HistoryRAMSettings settings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Configure(settings);
            });
        }

        /// <summary>
        /// Gets History RAM configuration.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>Current History RAM configuration.</returns>
        public static HistoryRAMSettings GetHistoryRAMConfiguration(this DigitalSessionsBundle sessionsBundle)
        {
            // Assumes all sessions have the same configuration.
            return sessionsBundle.InstrumentSessions.First().Session.GetConfiguration();
        }

        /// <summary>
        /// Fetches results from the History RAM and returns as a site-aware object of type DigitalHistoryRamCycleInformation.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>The per-site History RAM cycle information and scan cycle numbers.</returns>
        public static SiteData<HistoryRAMResults> FetchHistoryRAMResults(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSiteResults(sessionInfo =>
            {
                return sessionInfo.Session.FetchHistoryRAMResults(sessionInfo.GetPerSitePinSetStrings());
            });
        }

        #endregion methods on DigitalSessionsBundle

        #region methods on NIDigital session

        /// <summary>
        /// Configures History RAM.
        /// </summary>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <param name="settings">The configuration to apply.</param>
        public static void Configure(this NIDigital session, HistoryRAMSettings settings)
        {
            session.HistoryRam.NumberOfSamplesIsFinite = settings.NumberOfSamplesIsFinite;
            session.HistoryRam.CyclesToAcquire = settings.CyclesToAcquire;
            session.HistoryRam.MaximumSamplesToAcquirePerSite = settings.MaximumSamplesToAcquirePerSite;
            session.HistoryRam.BufferSizePerSite = settings.BufferSizePerSite;
            session.Trigger.HistoryRamTrigger.TriggerType = settings.TriggerSettings.TriggerType;
            session.Trigger.HistoryRamTrigger.PretriggerSamples = settings.TriggerSettings.PretriggerSamples;
            switch (settings.TriggerSettings.TriggerType)
            {
                case HistoryRamTriggerType.CycleNumber:
                    session.Trigger.HistoryRamTrigger.CycleNumber.Number = settings.TriggerSettings.CycleNumber;
                    break;

                case HistoryRamTriggerType.PatternLabel:
                    session.Trigger.HistoryRamTrigger.PatternLabel.Label = settings.TriggerSettings.PatternLabel;
                    session.Trigger.HistoryRamTrigger.PatternLabel.CycleOffset = settings.TriggerSettings.CycleOffset;
                    session.Trigger.HistoryRamTrigger.PatternLabel.VectorOffset = settings.TriggerSettings.VectorOffset;
                    break;
            }
        }

        /// <summary>
        /// Gets History RAM configuration.
        /// </summary>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <returns>Current History RAM configuration.</returns>
        public static HistoryRAMSettings GetConfiguration(this NIDigital session)
        {
            var settings = new HistoryRAMSettings
            {
                NumberOfSamplesIsFinite = session.HistoryRam.NumberOfSamplesIsFinite,
                CyclesToAcquire = session.HistoryRam.CyclesToAcquire,
                MaximumSamplesToAcquirePerSite = session.HistoryRam.MaximumSamplesToAcquirePerSite,
                BufferSizePerSite = session.HistoryRam.BufferSizePerSite
            };
            settings.TriggerSettings.TriggerType = session.Trigger.HistoryRamTrigger.TriggerType;
            settings.TriggerSettings.PretriggerSamples = session.Trigger.HistoryRamTrigger.PretriggerSamples;
            switch (settings.TriggerSettings.TriggerType)
            {
                case HistoryRamTriggerType.CycleNumber:
                    settings.TriggerSettings.CycleNumber = session.Trigger.HistoryRamTrigger.CycleNumber.Number;
                    break;

                case HistoryRamTriggerType.PatternLabel:
                    settings.TriggerSettings.PatternLabel = session.Trigger.HistoryRamTrigger.PatternLabel.Label;
                    settings.TriggerSettings.CycleOffset = session.Trigger.HistoryRamTrigger.PatternLabel.CycleOffset;
                    settings.TriggerSettings.VectorOffset = session.Trigger.HistoryRamTrigger.PatternLabel.VectorOffset;
                    break;
            }
            return settings;
        }

        /// <summary>
        /// Fetches History RAM results.
        /// </summary>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <param name="perSitePinSetStrings">The per-site pin set strings associated with the session.</param>
        /// <returns>The single session per-site History RAM cycle information and scan cycle numbers.</returns>
        public static SiteData<HistoryRAMResults> FetchHistoryRAMResults(this NIDigital session, IDictionary<string, string> perSitePinSetStrings)
        {
            var results = new Dictionary<int, HistoryRAMResults>();
            var lockObject = new object();
            Parallel.ForEach(perSitePinSetStrings, pinSetString =>
            {
                long readPosition = 0;
                string siteString = pinSetString.Key;
                var perSiteCycleInformation = new List<DigitalHistoryRamCycleInformation>();
                var perSiteScanCycleNumbers = new List<long>();
                while (true)
                {
                    long totalSampleCount = session.HistoryRam.GetSampleCount(siteString);
                    long newSamplesCount = totalSampleCount - readPosition;
                    if (session.PatternControl.IsDone && newSamplesCount == 0)
                    {
                        break;
                    }
                    perSiteCycleInformation.AddRange(session.HistoryRam.FetchCycleInformation(siteString, pinSetString.Value, readPosition, newSamplesCount));
                    perSiteScanCycleNumbers.AddRange(session.HistoryRam.FetchScanCycleNumbers(siteString, readPosition, newSamplesCount));
                    readPosition = totalSampleCount;
                }
                lock (lockObject)
                {
                    int site = int.Parse(siteString.Remove(0, 4), CultureInfo.InvariantCulture);
                    var historyRamResults = new HistoryRAMResults(perSiteCycleInformation, perSiteScanCycleNumbers);
                    results.Add(site, historyRamResults);
                }
            });
            return new SiteData<HistoryRAMResults>(results);
        }

        #endregion methods on NIDigital session

        #region utility methods

        private const string HistoryRAMResultsFileHeader = "Vector, Time Set Name, Cycle, Scan Cycle, Pass/Fail, Pin List, Per Pin Pass/Fail, Expected Pin States, Actual Pin States";

        /// <summary>
        /// Logs History RAM results to CSV files. This method should be used for debug only.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="cycleInformation">The per-site cycle information.</param>
        /// <param name="scanCycleNumbers">The per-site scan cycle numbers.</param>
        /// <param name="patternName">The name of the pattern.</param>
        /// <param name="outputDirectory">The directory to save the CSV files.</param>
        public static void LogResultsToFiles(this DigitalSessionsBundle sessionsBundle, SiteData<List<DigitalHistoryRamCycleInformation>> cycleInformation, SiteData<List<long>> scanCycleNumbers, string patternName, string outputDirectory)
        {
            foreach (var siteNumber in scanCycleNumbers.SiteNumbers)
            {
                string fileName = $"HistoryRAMResults_{patternName}_site{siteNumber}_{GetCurrentTime()}.csv";
                Directory.CreateDirectory(outputDirectory);
                using (var file = new StreamWriter(Path.Combine(outputDirectory, fileName)))
                {
                    if (scanCycleNumbers.GetValue(siteNumber).Count == 0)
                    {
                        file.WriteLine(HistoryRAMResultsFileHeader);
                        for (int i = 0; i < scanCycleNumbers.GetValue(siteNumber).Count; i++)
                        {
                            var cycleInfo = cycleInformation.GetValue(siteNumber)[i].Serialize();
                            var info = $"{cycleInfo.Item1}, {scanCycleNumbers.GetValue(siteNumber)[i]}, {cycleInfo.Item2}, {sessionsBundle.Pins.Serialize()}, {cycleInfo.Item3}";
                            file.WriteLine(info);
                        }
                    }
                    else
                    {
                        file.WriteLine("PATTERN PASSED - NO FAILURES");
                    }
                }
            }
        }

        private static Tuple<string, string, string> Serialize(this DigitalHistoryRamCycleInformation info)
        {
            var basicInfo = $"{info.VectorNumber}, {info.TimeSetName}, {info.CycleNumber}";
            var passFailInfo = info.PerPinPassFail.First().All(element => element) ? "P" : "F";
            var perPinInfo = $"{{{info.PerPinPassFail.Serialize()}}}, {{{info.ExpectedPinStates.Serialize()}}}, {{{info.ActualPinStates.Serialize()}}}";
            return new Tuple<string, string, string>(basicInfo, passFailInfo, perPinInfo);
        }

        private static string Serialize(this IEnumerable<string> info)
        {
            return string.Join(",", info);
        }

        private static string Serialize(this bool[][] info)
        {
            return string.Join(",", info.First().Select(element => element ? "P" : "F"));
        }

        private static string Serialize(this PinState[][] info)
        {
            return string.Join(",", info.First());
        }

        private static string GetCurrentTime()
        {
            var now = DateTime.Now;
            return $"{now.Year}-{now.Month}-{now.Day}-{now.Hour}-{now.Minute}-{now.Second}";
        }

        #endregion utility methods
    }

    /// <summary>
    /// Defines settings of History RAM.
    /// </summary>
    public class HistoryRAMSettings
    {
        /// <summary>
        /// Whether the number of samples is finite.
        /// </summary>
        public bool NumberOfSamplesIsFinite { get; set; } = true;

        /// <summary>
        /// The cycles to acquire after the trigger conditions are met.
        /// </summary>
        public HistoryRamCycle CyclesToAcquire { get; set; } = HistoryRamCycle.Failed;

        /// <summary>
        /// The maximum samples to acquire for each site.
        /// </summary>
        public int MaximumSamplesToAcquirePerSite { get; set; } = 8191;

        /// <summary>
        /// The in-memory History RAM buffer size in samples.
        /// </summary>
        public long BufferSizePerSite { get; set; } = 32000;

        /// <summary>
        /// The History RAM trigger settings.
        /// </summary>
        public HistoryRAMTriggerSettings TriggerSettings { get; set; } = new HistoryRAMTriggerSettings();

        /// <summary>
        /// Creates an object to define and set History RAM settings.
        /// This object is used as a parameter for configuring History RAM settings. <see cref="HistoryRAM.ConfigureHistoryRAM(DigitalSessionsBundle, HistoryRAMSettings)"/>
        /// </summary>
        public HistoryRAMSettings()
        {
        }
    }

    /// <summary>
    /// Defines History RAM trigger settings.
    /// </summary>
    public class HistoryRAMTriggerSettings
    {
        /// <summary>
        /// The trigger type.
        /// </summary>
        public HistoryRamTriggerType TriggerType { get; set; } = HistoryRamTriggerType.FirstFailure;

        /// <summary>
        /// The number of samples to acquire before the History RAM trigger.
        /// </summary>
        public int PretriggerSamples { get; set; }

        /// <summary>
        /// The number of cycles to execute. Use this property only when <see cref="TriggerType"/> is set to <see cref="HistoryRamTriggerType.CycleNumber"/>.
        /// </summary>
        public long CycleNumber { get; set; }

        /// <summary>
        /// The pattern label. Use this property only when <see cref="TriggerType"/> is set to <see cref="HistoryRamTriggerType.PatternLabel"/>.
        /// </summary>
        public string PatternLabel { get; set; } = string.Empty;

        /// <summary>
        /// The cycle offset. Use this property only when <see cref="TriggerType"/> is set to <see cref="HistoryRamTriggerType.PatternLabel"/>.
        /// </summary>
        public long CycleOffset { get; set; }

        /// <summary>
        /// The vector offset. Use this property only when <see cref="TriggerType"/> is set to <see cref="HistoryRamTriggerType.PatternLabel"/>.
        /// </summary>
        public long VectorOffset { get; set; }

        /// <summary>
        /// Creates an object to define and set History RAM trigger settings.
        /// This object is used as a parameter for configuring trigger settings of History RAM settings. <see cref="HistoryRAMSettings.TriggerSettings"/>
        /// </summary>
        public HistoryRAMTriggerSettings()
        {
        }
    }

    /// <summary>
    /// Defines History RAM results type.
    /// </summary>
    public class HistoryRAMResults
    {
        /// <summary>
        /// The per-site cycle information.
        /// </summary>
        public List<DigitalHistoryRamCycleInformation> CycleInformation { get; }

        /// <summary>
        /// The per-site scan cycle numbers.
        /// </summary>
        public List<long> ScanCycleNumbers { get; }

        /// <summary>
        /// Constructs a History RAM results object.
        /// </summary>
        /// <param name="cycleInformation">The cycle information.</param>
        /// <param name="scanCycleNumbers">The scan cycle numbers.</param>
        public HistoryRAMResults(List<DigitalHistoryRamCycleInformation> cycleInformation, List<long> scanCycleNumbers)
        {
            CycleInformation = cycleInformation;
            ScanCycleNumbers = scanCycleNumbers;
        }
    }
}
