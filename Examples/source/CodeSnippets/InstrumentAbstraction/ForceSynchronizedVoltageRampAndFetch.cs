using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to force voltage on pins mapped to DCPower instruments.
    /// Note that DCPower Instruments include both Source Measurement Units (SMUs) and Programmable Power Supplies (PPS) devices.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    public static class ForceSynchronizedVoltageRampAndFetch
    {
        /// <summary>
        /// This example demonstrates how to force a hardware-timed sequence of voltage sequence, created using <see cref="HelperMethods.CreateRampSequence(double, double, int)"/> on the specified pins across all sites using.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        public static void SameValueToAllSmuPins(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);

            var result = dcPowerPins.FetchMeasurement(pointsToFetch: 10);
        }

        /// <summary>
        /// This example demonstrates how to force a hardware-timed sequence of voltage sequence, created using <see cref="HelperMethods.CreateRampSequence(int[], double, double, int)"/> per site across all the SMU pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        internal static void DifferentValuesPerSiteAcrossAllSmuPins(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();
            var voltageSequence = HelperMethods.CreateRampSequence(siteNumbers: activeSites, outputStart: 0, outputStop: 3, numberOfPoints: 10);

            var dcPowerPins = sessionManager.DCPower(smuPinNames);
            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);

            var result = dcPowerPins.FetchMeasurement(pointsToFetch: 10);
        }

        /// <summary>
        /// This example demonstrates how to force a hardware-timed sequence of voltage sequence, created using <see cref="HelperMethods.CreateRampSequence(string[], int[], double, double, int)"/> across all the pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        internal static void DifferentLevelsPerSmuPin(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();

            var voltageSequence = HelperMethods.CreateRampSequence(pinNames: smuPinNames, siteNumbers: activeSites, outputStart: 0, outputStop: 3, numberOfPoints: 10);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);
            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);

            var result = dcPowerPins.FetchMeasurement(pointsToFetch: 10);
        }
    }
}
