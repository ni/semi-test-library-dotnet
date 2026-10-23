using System;
using System.IO;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;

namespace ResetDeviceConsoleApp
{
    /// <summary>
    /// Resets the device(s) behind sessions bundle A (site 0) and verifies that A stays in the reset state while
    /// sessions bundle B (all other sites) gets its configuration back, printing per-step timings along the way.
    /// </summary>
    /// <remarks>
    /// Usage: ResetDeviceConsoleApp.exe [pinMap] [restoreConfiguration] [pins]
    ///   pinMap               Pin map file name (looked up in .\PinMaps) or a full path. Default: ResetDevice_SessionPerChannel_TwoModules.pinmap.
    ///   restoreConfiguration true or false. Default: true.
    ///   pins                 Comma-separated DCPower pins. Default: VCC,VDD.
    /// </remarks>
    internal static class Program
    {
        private const string DefaultPinMap = "ResetDevice_SessionPerChannel_TwoModules.pinmap";
        private const double ConfiguredApertureTime = 0.08;
        private const double Tolerance = 1e-9;

        private static int Main(string[] args)
        {
            var pinMapPath = ResolvePinMapPath(args.Length > 0 ? args[0] : DefaultPinMap);
            var restoreConfiguration = args.Length <= 1 || bool.Parse(args[1]);
            var pins = (args.Length > 2 ? args[2] : "VCC,VDD").Split(',').Select(pin => pin.Trim()).ToArray();

            ResetDeviceTimings.Logger = Console.WriteLine;
            Console.WriteLine($"Pin map: {pinMapPath}");
            Console.WriteLine($"Pins: {string.Join(", ", pins)}; restoreConfiguration: {restoreConfiguration}");
            Console.WriteLine();

            var tsmContext = CreateStandaloneTSMContext(pinMapPath);
            Console.WriteLine("--- Initialize ---");
            InitializeAndClose.Initialize(tsmContext);
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var otherSites = tsmContext.SiteNumbers.Where(site => site != 0).ToArray();
                var bundleA = sessionManager.DCPower(pins).FilterBySite(0);
                var bundleB = sessionManager.DCPower(pins).FilterBySite(otherSites);

                var defaultApertureTimes = bundleA.GetApertureTimeInSeconds(out _);

                bundleA.ConfigureApertureTimeInSeconds(ConfiguredApertureTime, UpdateMode.Commit);
                bundleB.ConfigureApertureTimeInSeconds(ConfiguredApertureTime, UpdateMode.Commit);
                Console.WriteLine();
                Console.WriteLine($"Configured aperture time {ConfiguredApertureTime} s on bundle A (site 0) and bundle B (sites {string.Join(", ", otherSites)}).");

                Console.WriteLine();
                Console.WriteLine("--- A.ResetDevice() ---");
                ResetDeviceTimings.Clear();
                bundleA.ResetDevice(restoreConfiguration);

                var apertureTimesB = bundleB.GetApertureTimeInSeconds(out _);
                var apertureTimesA = bundleA.GetApertureTimeInSeconds(out _);

                Console.WriteLine();
                Console.WriteLine("--- Results ---");
                var bundleAPassed = true;
                foreach (var site in apertureTimesA.SiteNumbers)
                {
                    foreach (var pin in apertureTimesA.PinNames)
                    {
                        var actual = apertureTimesA.GetValue(site, pin);
                        var expected = defaultApertureTimes.GetValue(site, pin);
                        var isDefault = Math.Abs(actual - expected) < Tolerance;
                        bundleAPassed &= isDefault;
                        Console.WriteLine($"A site {site} {pin}: aperture time {actual} s, expected default {expected} s -> {isDefault}");
                    }
                }

                // Without restore, B is reinitialized at the reset defaults, so its configured aperture time is expected to be gone.
                var bundleBPassed = true;
                foreach (var site in apertureTimesB.SiteNumbers)
                {
                    foreach (var pin in apertureTimesB.PinNames)
                    {
                        var actual = apertureTimesB.GetValue(site, pin);
                        var isRestored = Math.Abs(actual - ConfiguredApertureTime) < Tolerance;
                        var passed = isRestored == restoreConfiguration;
                        bundleBPassed &= passed;
                        Console.WriteLine($"B site {site} {pin}: aperture time {actual} s, restored to {ConfiguredApertureTime} s: {isRestored} -> {passed}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"Bundle A stayed at the default aperture time: {bundleAPassed}");
                Console.WriteLine($"Bundle B {(restoreConfiguration ? "was restored to" : "was not restored to")} {ConfiguredApertureTime} s: {bundleBPassed}");

                Console.WriteLine();
                Console.WriteLine("--- Phase totals ---");
                foreach (var phase in ResetDeviceTimings.Snapshot())
                {
                    Console.WriteLine($"  {phase.Key,-38} total {phase.Value.Total.TotalSeconds,8:F3} s   calls {phase.Value.Count,3}");
                }

                return bundleAPassed && bundleBPassed ? 0 : 1;
            }
            finally
            {
                InitializeAndClose.Close(tsmContext);
            }
        }

        private static ISemiconductorModuleContext CreateStandaloneTSMContext(string pinMapPath)
        {
            var publishedDataReaderFactory = new PublishedDataReaderFactory();
            var tsmContext = publishedDataReaderFactory.NewSemiconductorModuleContext(pinMapPath, null, specificationsFilePaths: null, out _);
            return SemiconductorModuleContextFactory.ConstructSemiconductorModuleContextForDotNet(tsmContext);
        }

        private static string ResolvePinMapPath(string pinMap)
        {
            if (File.Exists(pinMap))
            {
                return Path.GetFullPath(pinMap);
            }
            var bundledPinMap = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PinMaps", pinMap);
            if (File.Exists(bundledPinMap))
            {
                return bundledPinMap;
            }
            throw new FileNotFoundException($"Pin map '{pinMap}' was not found as a path or in '{Path.GetDirectoryName(bundledPinMap)}'.");
        }
    }
}
