using System;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.Examples.SemiconductorTestLibrary.ExampleSupport;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SMUMergePinGroup
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
    ///    The default value for the resource name is "SMU_4147_C1_S04".
    /// 3.  The pin map must define a pin group to contain all the pins that are to be merged together.
    /// 4. Each pin in the pin group must be mapped to an SMU channels of same module for a given site.
    /// 5.The SMU module must support the niDCPower Merged Channels feature.
    /// For example: PXIe-4147, PXIe-4162, and PXIe-4163.
    /// 6. The pins are physically connected externally on the application load board, either in a fixed configuration or via relays.
    /// The example methods of this class demonstrate how relay configurations can be applied
    /// to ensures the SMUs channels are physically connected in parallel before the MergePinGroup operation,
    /// and subsequently disconnected after the UnmergePinGroup operation.
    /// 7. Build and run the example.
    /// </remarks>
    public sealed class Program
    {
        #region Settings and Configuration

        // Relay configuration that connects all the channels in parallel
        private const string ConnectedRelayConfiguration = "";
        // Relay configuration that disconnects all the channels
        private const string DisconnectedRelayConfiguration = "";

        // PinGroups for merging.
        private const string Vcc2ch = "Vcc2ch0";  //Name of the pin group to be merged for sourcing 5A
        private const string Vcc4ch = "Vcc4ch0";  //Name of the pin group to be merged for sourcing 10A

        // voltage and current settings for the merge operation
        private const double CurrentLevel1 = 5.0;
        private const double CurrentLevel2 = 10.0;
        private const double VoltageLimit = 0.4;

        // settling and aperture time used for measurements
        private const double SettlingTime = 0.001;
        private const double ApertureTimeConstant = 0.01; // -1 indicates no aperture time is set.

        // Files to load.
        private const string PinMapFileName = "STLExample.SMUMergePinGroup.pinmap";

        #endregion Settings and Configuration

        /// <summary>
        /// Merges the pins in specified pin group, allowing them to operate in unison to achieve a higher current output.
        /// Then, high current is forced on the merged pin group and a voltage measurement is published.
        /// Finally, the merged pin group is powered down and unmerged.
        /// Use the connectedRelayConfiguration and disconnectedRelayConfiguration parameters to specify the appropriate relay configurations
        /// that will physically connect/disconnect the pins in the pin group via external relays on the application load board.
        /// If the application load board is designed with the target pins permanently connected together,
        /// do not specify a value for the connectedRelayConfiguration/disconnectedRelayConfiguration parameters.
        /// </summary>
        /// <param name="args">Command line arguments (not used in this example).</param>
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("1. Initializing Semiconductor Module Context.");
                ISemiconductorModuleContext semiconductorContext = CreateStandaloneTSMContext(PinMapFileName);

                Console.WriteLine("2. Initializing Instrument Sessions.");
                InitializeAndClose.Initialize(semiconductorContext);

                Console.WriteLine($"3. Creating Session Manager.");
                TSMSessionManager sessionManager = new TSMSessionManager(semiconductorContext);

                // Configure the appropriate relays required to physically connect the pins externally.
                if (!string.IsNullOrEmpty(ConnectedRelayConfiguration))
                {
                    semiconductorContext.ApplyRelayConfiguration(ConnectedRelayConfiguration, waitSeconds: SettlingTime);
                }

                ApplicationLogic(sessionManager);

                // Configure the appropriate relays required to physically disconnect the pins externally.
                if (!string.IsNullOrEmpty(DisconnectedRelayConfiguration))
                {
                    semiconductorContext.ApplyRelayConfiguration(DisconnectedRelayConfiguration, waitSeconds: SettlingTime);
                }

                Console.WriteLine("9. Closing Instrument Sessions.");
                InitializeAndClose.Close(semiconductorContext);
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

        /// <summary>
        /// Core application logic for merging pin group, Forcing Current and unmerging pin group happens here.
        /// </summary>
        /// <param name="sessionManager">SessionManager for creating the DCPower bundle.</param>
        private static void ApplicationLogic(TSMSessionManager sessionManager)
        {
            // In PXIe-4147 hardware, merging is supported for 2 or 4 channels.
            // wait for the user to acknowledge Merge operation.
            Console.WriteLine("4. Please press number `4` for four channel merging or any other key for two channel merging.");
            var keyInfo = Console.ReadKey(); // ReadKey returns a ConsoleKeyInfo object
            int mergingChannelCount = keyInfo.Key == ConsoleKey.NumPad4 || keyInfo.Key == ConsoleKey.D4 ? 4 : 2;
            string vccI = mergingChannelCount == 4 ? Vcc4ch : Vcc2ch;
            double currentLevel = mergingChannelCount == 4 ? CurrentLevel2 : CurrentLevel1;

            Console.WriteLine("");

            // Create a bundle for the DCPower sessions for the specified pin group.
            DCPowerSessionsBundle smuBundle = sessionManager.DCPower(vccI);
            PinSiteData<double> originalSourceDelay = smuBundle.GetSourceDelayInSeconds();

            Console.WriteLine($"5. Performing {mergingChannelCount} channel merging operation.");
            // Abort any previous single channel operations before merging the pin group.
            smuBundle.Abort();
            smuBundle.MergePinGroup(vccI);
            smuBundle.ConfigureSourceDelay(SettlingTime);
            if (ApertureTimeConstant != -1)
            {
                DCPowerMeasureSettings measureSettings = new DCPowerMeasureSettings()
                {
                    ApertureTime = ApertureTimeConstant,
                };
                smuBundle.ConfigureMeasureSettings(measureSettings);
            }

            Console.WriteLine($"6. Forcing {currentLevel} Amps of current with {VoltageLimit} voltage limit for 1 second.");
            smuBundle.ForceCurrent(currentLevel, VoltageLimit, waitForSourceCompletion: true);
            PreciseWait(timeInSeconds: SettlingTime);

            smuBundle.MeasureAndPublishCurrent(publishedDataId: "Current", out var currentOut);
            PreciseWait(1.0);
            Console.WriteLine($"Measured Current: {currentOut[0][0]} A.");

            Console.WriteLine($"7. Powering down output.");

            // Clean up and restore the state of the instrumentation after finishing the test.
            smuBundle.ForceCurrent(10e-3, waitForSourceCompletion: true);
            smuBundle.PowerDown();
            PreciseWait(timeInSeconds: SettlingTime);

            Console.WriteLine($"8. Unmerging channels.");

            // Use the SMU Bundle object to perform unmerge operation on the pin group.
            smuBundle.UnmergePinGroup(vccI);
            smuBundle.Abort();
            smuBundle.ConfigureSourceDelay(originalSourceDelay);
            smuBundle.Commit();
        }
    }
}
