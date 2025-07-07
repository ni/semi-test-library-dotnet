using System;
using System.IO;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using static NationalInstruments.SemiconductorTestLibrary.Examples.StandAlone.StandAloneExampleSupport; 

namespace NationalInstruments.SemiconductorTestLibrary.Examples.StandAlone.SMUMergePinGroup
{
    /// <summary>
    /// Demonstrates the use of MergePinGroup and UnmergePinGroup functions of the
    /// Semiconductor Test Library. Specifically, how to merge Pin Groups
    /// for forcing high current and measure voltage for pins mapped to DCPower Instruments from same module.
    /// Later unmerge the pin group to disconnect the pins and instrumentation after the test is complete.
    /// </summary>
    /// <remarks>
    /// Using this example:
    /// 1. Expand the Settings and Configuration region and familiarize yourself with the settings.
    /// 2. Ensure the resource names within the PinMap.pinmap file match the resource name of the NI-DCPower Instrument in your system.
    ///    The default value for the resource name is "SMU_4147_C1_S05".
    /// 3. Note that only few DCPower Instruments support merging feature which includes PXIe-4147, PXIe-4162 and PXIe-4163.
    /// This also requires the pinmap to define the pingroup to contain all the pins to be merged and each
    /// pin is mapped to SMU channels of same module.
    /// This example assumes:
    ///  - Relay configurations to be applied before the merging operation ensures the SMUs channels are connected
    ///  in parallel configuration and disconnect after unmerging.
    ///  - The relay operations are skipped and they are provided as code template purpose only.
    /// 4. Build and run the example.
    /// </remarks>
    public sealed class Program
    {
        #region Settings and Configuration

        //Relay configuration that connects all the channels in parallel
        private const string ConnectedRelayConfiguration = "ConnectedRelayConfig";
        //Relay configuration that disconnects all the channels
        private const string DisconnectedRelayConfiguration = "DisconnectedRelayConfig";

        // PinGroups for merging.
        private const string Vcc5A = "Vcc5A";  //Name of the pin group to be merged for sourcing 5A
        private const string Vcc10A = "Vcc10A";  //Name of the pin group to be merged for sourcing 10A

        // voltage and current settings for the merge operation
        private const double currentLevel1 = 5.0;
        private const double currentLevel2 = 10.0;
        private const double voltageLimit = 0.4;

        // settling and aperture time used for measurements
        private static readonly double SettlingTime = 0.001;
        private static readonly double ApertureTimeConstant = -1; // -1 indicates no aperture time is set.

        // Files to load.
        private const string PinMapFileName = "PinMap.pinmap";
        #endregion Settings and Configuration

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "NI1704:Identifiers should be spelled correctly", Justification = "ExampleCode")]

        /// <summary>
        /// Merges the specified pin group to force high current and measure voltage for pins mapped to DCPower Instruments from same module.
        /// Specifically, this method merges the pin group, forces a voltage level, measures the current, and then unmerges the pin group.
        /// </summary>
        /// <param name="tsmContext">Teststand Semiconductor module context</param>
        /// <returns>PinSiteData measurement  in double precision </returns>
        public static void SMUMergeToForceHighCurrent(
            ISemiconductorModuleContext tsmContext)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle smuBundle = sessionManager.DCPower(Vcc5A);

            // Configure the instrumentation connected to the target pins
            if (!ConnectedRelayConfiguration.IsEmpty())
            {
                // Configure the relays required for merging.
                tsmContext.ApplyRelayConfiguration(ConnectedRelayConfiguration, waitSeconds: SettlingTime);
            }
            // Store the current source delay settings as a backup to restore later.
            PinSiteData<double> originalSourceDelays = smuBundle.GetSourceDelayInSeconds();

            // Perform merge operation on the pin group.
            smuBundle.MergePinGroup(Vcc5A);
            smuBundle.ConfigureSourceDelay(SettlingTime);
            if (ApertureTimeConstant != -1)
            {
                DCPowerMeasureSettings measureSettings = new DCPowerMeasureSettings()
                {
                    ApertureTime = ApertureTimeConstant,
                };
                smuBundle.ConfigureMeasureSettings(measureSettings);
            }
            
            // Source and/or measure the signals.
            smuBundle.ForceCurrent(currentLimit, voltageLevel, waitForSourceCompletion: true);
            PreciseWait(timeInSeconds: SettlingTime);
            smuBundle.MeasureAndPublishCurrent(publishedDataId: "Current");
            
            // Clean up and restore the state of the instrumentation after finishing the test.
            smuBundle.ForceVoltage(voltageLevel: 0, currentLimit: 0.001);
            smuBundle.PowerDown();
            PreciseWait(timeInSeconds: SettlingTime);

            // Use the SMU Bundle object to perform unmerge operation on the pin group and disconnect the relays.
            smuBundle.UnmergePinGroup(Vcc5A);
            if (!DisconnectedRelayConfiguration.IsEmpty())
            {
                // Configure the relays required for unmerging.
                tsmContext.ApplyRelayConfiguration(DisconnectedRelayConfiguration, waitSeconds: SettlingTime);
            }
            // Restore the source delay to original value.
            smuBundle.ConfigureSourceDelay(originalSourceDelays);
        }

    }
}
