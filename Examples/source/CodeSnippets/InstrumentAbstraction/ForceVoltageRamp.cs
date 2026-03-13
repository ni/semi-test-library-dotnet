using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions(specifically extensions related to hardware level sequencing) from the Semiconductor Test Library.
    /// Specifically, how to force voltage on pins mapped to DCPower instruments.
    /// Note that DCPower Instruments include both Source Measurement Units (SMUs) and Programmable Power Supplies (PPS) devices.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    public static class ForceVoltageRamp
    {
        /// <summary>
        /// This example demonstrates how to force the same voltage sequence created using <see cref="HelperMethods.CreateRampSequence(double, double, int)"/> on the specified pins across all sites using.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        public static void SameValueToAllSmuPins(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            var dcPowerPins = sessionManager.DCPower(smuPinNames);
            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            var voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequence(voltageSequence);
        }

        /// <summary>
        /// This example demonstrates how to force voltage sequence for different pins across all sites using <see cref="HelperMethods.CreateRampSequence(string[], int[], double, double, int)"/>.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        public static void DifferentValuesPerSmuPin(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();

            var dcPowerPins = sessionManager.DCPower(smuPinNames);
            // Create a voltage ramp sequence for each pin and site from 0 to 3 volts with 10 points, which will create a sequence like:
            // {Pin1: {Site1: [0V, 0.33V, 0.66V, ..., 3V]}}
            var voltageSequence = HelperMethods.CreateRampSequence(pinNames: smuPinNames, siteNumbers: activeSites, outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequence(voltageSequence);
        }

        /// <summary>
        /// This example demonstrates how to force voltage sequence for different sites using <see cref="HelperMethods.CreateRampSequence(int[], double, double, int)"/>.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        public static void DifferentValuesPerSiteAcrossAllSmuPins(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();

            var dcPowerPins = sessionManager.DCPower(smuPinNames);
            // Create a voltage ramp sequence for each site from 0 to 3 volts with 10 points, which will create a sequence like:
            // {Site1: [0V, 0.33V, 0.66V, ..., 3V]}
            var voltageSequence = HelperMethods.CreateRampSequence(siteNumbers: activeSites, outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequence(voltageSequence);
        }
    }
}
