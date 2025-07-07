using System;
using System.IO;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Examples.StandAlone.StandAloneExampleSupport;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.StandAlone.NIDigital.BurstWithStartTrigger
{
    /// <summary>
    /// Demonstrates the use of the NIDigital InstrumentAbstraction of the Semiconductor Test Library
    /// to create and configure NI-Digital Pattern Instrument sessions and then burst a pattern using a start trigger.
    /// </summary>
    /// <remarks>
    /// Using this example:
    /// 1. Expand the Settings and Configuration region and familiarize yourself with the settings.
    /// 2. Ensure the resource names within the PinMap.pinmap file match the resource name of the NI-Digital Pattern Instrument in your system.
    ///    The default value for the resource name is "HSD_6571_C1_S02".
    /// 3. In the trigger settings section, set the TriggerTypeSetting field to the trigger type you
    ///    would like to use. If you choose the 'DigitalEdge' trigger type, set the TriggerSource
    ///    field to an appropriate trigger source. The default value, "PXI_Trig0," indicates the
    ///    trigger will be received from PXI trigger line 0.
    /// 4. Build and run the example.
    /// </remarks>
    public sealed class Program2
    {
        #region Settings and Configuration

        // Trigger settings.
        private static readonly TriggerType TriggerTypeSetting = TriggerType.Software;

        private const string TriggerSource = "PXI_Trig0";

        // Timeout to use when waiting for the pattern burst to complete.
        private static readonly double TimeoutSeconds = 10;

        // Files to load.
        private const string PinMapFileName = "PinMap.pinmap";
        private const string DigitalProjectFileName = "BurstWithStartTrigger.digiproj";
        private const string PinLevelsFileName = "PinLevels.digilevels";
        private const string TimingSheetFileName = "Timing.digitiming";

        // Pattern settings.
        private const string PatternStartLabel = "new_pattern";

        #endregion Settings and Configuration

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "NI1704:Identifiers should be spelled correctly", Justification = "ExampleCode")]
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("1. Initializing Semiconductor Module Context");
                ISemiconductorModuleContext semiconductorContext = CreateStandAloneSemiconductorModuleContext(PinMapFileName, DigitalProjectFileName);

                Console.WriteLine("2. Initialize and Configuring Instrument Sessions.");
                InitializeAndClose.Initialize(
                    semiconductorContext,
                    Path.GetFileNameWithoutExtension(PinLevelsFileName),
                    Path.GetFileNameWithoutExtension(TimingSheetFileName),
                    resetDevice: true);

                Console.WriteLine($"3. Bursting Pattern in {TriggerTypeSetting} Trigger Mode.");
                TSMSessionManager sessionManager = new TSMSessionManager(semiconductorContext);
                DigitalSessionsBundle sessionsBundle = sessionManager.Digital();
                switch (TriggerTypeSetting)
                {
                    // Select 'None' as the start trigger mode to burst the pattern from your
                    // instrument based on the configured start label. Pass 'true' for the
                    // waitUntilDone parameter of BurstPattern to ensure the pattern finishes
                    // bursting before execution of this program continues to cleanup.
                    case TriggerType.None:
                        sessionsBundle.BurstPattern(PatternStartLabel, timeoutInSeconds: TimeoutSeconds);
                        break;

                    // Select 'DigitalEdge' as the start trigger mode to configure the start
                    // trigger for digital edge and select the source and edge on which to
                    // trigger. A valid digital edge must be received in order for BurstPattern
                    // to execute without a timeout error.
                    case TriggerType.DigitalEdge:
                        sessionsBundle.ConfigureStartTriggerDigitalEdge(TriggerSource, DigitalEdge.Rising);
                        sessionsBundle.BurstPattern(PatternStartLabel, timeoutInSeconds: TimeoutSeconds);
                        break;

                    // Select 'Software' as the start trigger mode to configure the start trigger
                    // for software edge and burst the pattern. Pass 'false' for the
                    // waitUntilDone parameter of BurstPattern to make it a non-blocking call.
                    // After BurstPattern is called, the pattern waits for a software trigger
                    // before bursting. Once the software trigger is sent to the hardware, the
                    // pattern begins bursting.
                    case TriggerType.Software:
                        sessionsBundle.ConfigureStartTriggerSoftwareEdge();

                        // This call to BurstPattern returns immediately because waitUntilDone is 'false.'
                        sessionsBundle.BurstPattern(PatternStartLabel, timeoutInSeconds: TimeoutSeconds, waitUntilDone: false);

                        // Wait for user input to send the software trigger
                        Console.WriteLine("Waiting for software trigger. Press any key to send software trigger.");
                        Console.ReadKey();

                        // When you want to burst the pattern, call Send to send the software trigger.
                        sessionsBundle.SendSoftwareEdgeStartTrigger();
                        sessionsBundle.WaitUntilDone(TimeoutSeconds);
                        break;
                    default:
                        break;
                }

                // Disconnect all channels using programmable, on-board switching.
                Console.WriteLine("4. Cleaning up for session closure.");
                sessionsBundle.DisconnectOutput();
            }
            // Handle driver-specific exceptions before the general exception handler that follows.
            // An example of a driver-specific exception is Ivi.Driver.IviCDriverException.
            catch (Exception e)
            {
                Console.Write(e);
            }
            finally
            {
                Console.WriteLine("Program complete. Press any key to close.");
                Console.ReadKey();
            }
        }
    }
}