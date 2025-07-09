using System;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using static NationalInstruments.SemiconductorTestLibrary.Examples.StandAlone.StandAloneExampleSupport;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.StandAlone.NIDCPower.SMUMergePinGroup
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
        private const string ConnectedRelayConfiguration = "";
        //Relay configuration that disconnects all the channels
        private const string DisconnectedRelayConfiguration = "";

        // PinGroups for merging.
        private const string Vcc5A = "Vcc5A";  //Name of the pin group to be merged for sourcing 5A
        private const string Vcc10A = "Vcc10A";  //Name of the pin group to be merged for sourcing 10A

        // voltage and current settings for the merge operation
        private const double CurrentLevel1 = 5.0;
        private const double CurrentLevel2 = 10.0;
        private const double VoltageLimit = 0.4;

        // settling and aperture time used for measurements
        private const double SettlingTime = 0.001;
        private const double ApertureTimeConstant = -1; // -1 indicates no aperture time is set.

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
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("1. Initializing Semiconductor Module Context");
                ISemiconductorModuleContext semiconductorContext = CreateStandAloneSemiconductorModuleContext(PinMapFileName);

                Console.WriteLine("2. Initialize and Configuring Instrument Sessions.");
                InitializeAndClose.Initialize(semiconductorContext, resetDevice: true);

                Console.WriteLine($"3. Creating Bundle for merge pin group.");
                TSMSessionManager sessionManager = new TSMSessionManager(semiconductorContext);

                // In PXIe-4147 hardware, merging is supported for 2 or 4 channels.
                int MergingChannelCount = 2; // Default to 2 channel merging
                string VccI = Vcc5A;    // Default to Vcc5A pin group for merging
                double currentLevel = CurrentLevel1; // Default to 5A current level

                // wait for the user to acknowledge Merge operation.
                Console.WriteLine("Press 4 for four channel merging or any other key for two channel merging");
                var keyInfo = Console.ReadKey(); // ReadKey returns a ConsoleKeyInfo object
                switch (keyInfo.Key) // Use the Key property of ConsoleKeyInfo
                {
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4: // Compare against ConsoleKey enum values
                        MergingChannelCount = 4;
                        VccI = Vcc10A;
                        currentLevel = CurrentLevel2;
                        break;
                    default:
                        break;
                }
                Console.WriteLine("");

                // Create a bundle for the DCPower sessions for the specified pin group.
                DCPowerSessionsBundle smuBundle = sessionManager.DCPower(VccI);

                // Configure the instrumentation connected to the target pins
                if (!ConnectedRelayConfiguration.IsEmpty())
                {
                    // Configure the relays required for merging.
                    semiconductorContext.ApplyRelayConfiguration(ConnectedRelayConfiguration, waitSeconds: SettlingTime);
                }
                // Store the current source delay settings as a backup to restore later.
                PinSiteData<double> originalSourceDelays = smuBundle.GetSourceDelayInSeconds();


                Console.WriteLine($"5. Performing {MergingChannelCount} merging operation");
                smuBundle.MergePinGroup(VccI);
                smuBundle.ConfigureSourceDelay(SettlingTime);
                if (ApertureTimeConstant != -1)
                {
                    DCPowerMeasureSettings measureSettings = new DCPowerMeasureSettings()
                    {
                        ApertureTime = ApertureTimeConstant,
                    };
                    smuBundle.ConfigureMeasureSettings(measureSettings);
                }
                
                Console.WriteLine($"6. Forcing {currentLevel} Amps of current with {VoltageLimit} VoltageLimit for 1 sec");
                smuBundle.ForceCurrent(currentLevel, VoltageLimit, waitForSourceCompletion: true);
                PreciseWait(timeInSeconds: SettlingTime);
                smuBundle.MeasureAndPublishCurrent(publishedDataId: "Current");
                PreciseWait(1.0);

                Console.WriteLine($"7. Powering Down Output");
                // Clean up and restore the state of the instrumentation after finishing the test.
                smuBundle.ForceCurrent( 10e-3, waitForSourceCompletion: true);
                smuBundle.PowerDown();
                PreciseWait(timeInSeconds: SettlingTime);

                Console.WriteLine($"8. Unmerging Channels");
                // Use the SMU Bundle object to perform unmerge operation on the pin group and disconnect the relays.
                smuBundle.UnmergePinGroup(VccI);
                if (!DisconnectedRelayConfiguration.IsEmpty())
                {
                    // Configure the relays required for unmerging.
                    semiconductorContext.ApplyRelayConfiguration(DisconnectedRelayConfiguration, waitSeconds: SettlingTime);
                }
                // Restore the source delay to original value.
                Console.WriteLine($"9. restoring Source Delay");
                smuBundle.ConfigureSourceDelay(originalSourceDelays);
                smuBundle.Commit();
            }
            // Handle driver-specific exceptions before the general exception handler that follows.
            // An example of a driver-specific exception is Ivi.Driver.IviCDriverException.
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex}");
                Console.WriteLine("Please check the settings and configurations.");
            }
            finally
            {
                Console.WriteLine("Program complete. Press any key to close.");
                Console.ReadKey();
            }
        }
    }
}
