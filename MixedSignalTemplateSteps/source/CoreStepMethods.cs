using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.MixedSignalLibrary.Common.Utilities;
using static NationalInstruments.MixedSignalLibrary.TypeDefinitions;

namespace NationalInstruments.MixedSignalLibrary
{
    internal static class CoreStepMethods
    {
        internal const string VoltageDataId = "Voltage";
        internal const string CurrentDataId = "Current";
        internal const string TotalCurrentDataId = "Total Current";

        internal static void ForceVoltageCore(TSMSessionManager sessionManager, PinInfo pinInfo, MixedSignalParameters parameters)
        {
            InvokeInParallel(
                () =>
                {
                    var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(pinInfo.DCPinsFlat);
                    var settings = PrepareSMUSettings(pinInfo, parameters, DCPowerSourceOutputFunction.DCVoltage);
                    sessionsBundle.ForceVoltageSymmetricLimit(settings);
                },
                () =>
                {
                    var sessionsBundle = sessionManager.GetDigitalSessionsBundle(pinInfo.DigitalPinsFlat);
                    var settings = PreparePPMUSettings(pinInfo, parameters, PpmuOutputFunction.DCVoltage);
                    sessionsBundle.ForceVoltage(settings);
                });
        }

        internal static void ForceCurrentCore(TSMSessionManager sessionManager, PinInfo pinInfo, MixedSignalParameters parameters)
        {
            InvokeInParallel(
                () =>
                {
                    var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(pinInfo.DCPinsFlat);
                    var settings = PrepareSMUSettings(pinInfo, parameters, DCPowerSourceOutputFunction.DCCurrent);
                    sessionsBundle.ForceCurrentSymmetricLimit(settings);
                },
                () =>
                {
                    var sessionsBundle = sessionManager.GetDigitalSessionsBundle(pinInfo.DigitalPinsFlat);
                    var settings = PreparePPMUSettings(pinInfo, parameters, PpmuOutputFunction.DCCurrent);
                    sessionsBundle.ForceCurrent(settings);
                });
        }

        internal static void MeasureCore(TSMSessionManager sessionManager, PinInfo pinInfo, MeasurementType measurementType, ISemiconductorModuleContext tsmContext)
        {
            var perSiteTotalCurrents = new ConcurrentDictionary<int, double>();
            InvokeInParallel(
                () =>
                {
                    var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(pinInfo.DCPinsFlat);
                    var measurements = sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.Measure(), VoltageDataId, CurrentDataId);
                    CalculateAndUpdatePerSiteTotalCurrentAsNeeded(ref perSiteTotalCurrents, measurementType, sessionsBundle.InstrumentSessions, measurements);
                },
                () =>
                {
                    var sessionBundle = sessionManager.GetDigitalSessionsBundle(pinInfo.DigitalPinsFlat);
                    var measurements = sessionBundle.DoAndPublishResults(sessionInfo => sessionInfo.Measure(measurementType), VoltageDataId, CurrentDataId);
                    CalculateAndUpdatePerSiteTotalCurrentAsNeeded(ref perSiteTotalCurrents, measurementType, sessionBundle.InstrumentSessions, measurements);
                });

            PublishTotalCurrentAsNeeded(tsmContext, measurementType, perSiteTotalCurrents);
        }

        internal static void PowerDownCore(TSMSessionManager sessionManager, PinInfo pinInfo, double settlingTime)
        {
            InvokeInParallel(
                () =>
                {
                    var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(pinInfo.DCPinsFlat);
                    sessionsBundle.PowerDown(settlingTime);
                },
                () =>
                {
                    var sessionsBundle = sessionManager.GetDigitalSessionsBundle(pinInfo.DigitalPinsFlat);
                    sessionsBundle.PowerDown(settlingTime);
                });
        }

        internal static void CalculateAndUpdatePerSiteTotalCurrentAsNeeded(ref ConcurrentDictionary<int, double> perSiteTotalCurrents, MeasurementType measurementType, IEnumerable<ISessionInformation> allSessionInfo, Tuple<double[][], double[][]> measurements)
        {
            if (measurementType != MeasurementType.Voltage && measurements.Item2 != null)
            {
                var perSiteCurrents = allSessionInfo.PerInstrumentPerChannelResultsToPerSitePerPinResults(measurements.Item2);
                foreach (var element in perSiteCurrents)
                {
                    if (!perSiteTotalCurrents.ContainsKey(element.Key))
                    {
                        perSiteTotalCurrents.TryAdd(element.Key, 0);
                    }
                    perSiteTotalCurrents[element.Key] += element.Value.Values.Sum();
                }
            }
        }

        internal static void PublishTotalCurrentAsNeeded(ISemiconductorModuleContext tsmContext, MeasurementType measurementType, ConcurrentDictionary<int, double> perSiteTotalCurrents)
        {
            if (measurementType != MeasurementType.Voltage)
            {
                var perSiteTotalCurrentsArray = tsmContext.SiteNumbers.Select(siteNumber =>
                {
                    return perSiteTotalCurrents.TryGetValue(siteNumber, out var totalCurrent) ? totalCurrent : 0;
                }).ToArray();
                tsmContext.PublishPerSite(perSiteTotalCurrentsArray, TotalCurrentDataId);
            }
        }

        internal static IDictionary<string, DCPowerSettings> PrepareSMUSettings(PinInfo pinInfo, MixedSignalParameters parameters, DCPowerSourceOutputFunction outputFunction)
        {
            var settings = new Dictionary<string, DCPowerSettings>();
            for (int i = 0; i < pinInfo.DCPinIndexes.Count; i++)
            {
                var setting = new DCPowerSettings(parameters, i, outputFunction);
                foreach (var pin in pinInfo.DCPins[i])
                {
                    settings.Add(pin, setting);
                }
            }
            return settings;
        }

        internal static IDictionary<string, PPMUForcingSettings> PreparePPMUSettings(PinInfo pinInfo, MixedSignalParameters parameters, PpmuOutputFunction outputFunction)
        {
            var settings = new Dictionary<string, PPMUForcingSettings>();
            for (int i = 0; i < pinInfo.DigitalPinIndexes.Count; i++)
            {
                var setting = new PPMUForcingSettings(parameters, i, outputFunction);
                foreach (var pin in pinInfo.DigitalPins[i])
                {
                    settings.Add(pin, setting);
                }
            }
            return settings;
        }

        internal static double[] ToDoubleArray(this long[] failCounts)
        {
            return Array.ConvertAll(failCounts, value => (double)value);
        }
    }
}
