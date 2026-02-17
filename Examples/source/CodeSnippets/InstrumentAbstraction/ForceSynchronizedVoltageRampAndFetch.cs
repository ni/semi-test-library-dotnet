using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    internal static class ForceSynchronizedVoltageRampAndFetch
    {
        internal static void SameValueToAllSmuPins(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);

            var result = dcPowerPins.FetchMeasurement(pointsToFetch: 10);
            tsmContext.PublishResults(result, publishedDataId: "ForceSynchronizedVoltageRampAndFetch");
        }

        internal static void DifferentValuesPerSiteAcrossAllSmuPins(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();
            var voltageSequence = HelperMethods.CreateRampSequence(siteNumbers: activeSites, outputStart: 0, outputStop: 3, numberOfPoints: 10);

            var dcPowerPins = sessionManager.DCPower(smuPinNames);
            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);

            var result = dcPowerPins.FetchMeasurement(pointsToFetch: 10);
            tsmContext.PublishResults(result, publishedDataId: "ForceSynchronizedVoltageRampAndFetch");
        }

        internal static void DifferentLevelsPerSmuPin(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();

            var voltageSequence = HelperMethods.CreateRampSequence(pinNames: smuPinNames, siteNumbers: activeSites, outputStart: 0, outputStop: 3, numberOfPoints: 10);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);
            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);

            var result = dcPowerPins.FetchMeasurement(pointsToFetch: 10);
            tsmContext.PublishResults(result, publishedDataId: "ForceSynchronizedVoltageRampAndFetch");
        }
    }
}
