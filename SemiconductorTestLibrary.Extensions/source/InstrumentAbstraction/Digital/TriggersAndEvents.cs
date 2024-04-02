using System;
using Ivi.Driver;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods to deal with digital pattern triggers and events.
    /// </summary>
    public static class TriggersAndEvents
    {
        /// <summary>
        /// Configures a digital edge trigger for the specified Conditional Jump Trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="source">
        /// The source terminal for the start trigger.
        /// Possible values include but are not limited to "PXI_Trig0", "PXI_Trig1", "PXI_Trig2", "PXI_Trig3", "PXI_Trig4", "PXI_Trig5", "PXI_Trig6", or "PXI_Trig7".
        /// </param>
        /// <param name="conditionalJumpTriggerId">
        /// The ID of the of ConditionalJumpTrigger to configure.
        /// Valid values include: 0, 1, 2, and 3,
        /// which all equate to conditionalJumpTrigger0, conditionalJumpTrigger1, conditionalJumpTrigger2, and conditionalJumpTrigger3, respectively.
        /// </param>
        /// <param name="digitalEdge">The edge of the digital signal that should be triggered on: Rising edge (deafult) or Falling edge.</param>
        /// <exception cref="OutOfRangeException">The value for type is invalid.</exception>
        public static void ConfigureConditionalJumpTriggerDigitalEdge(this DigitalSessionsBundle sessionsBundle, string source, int conditionalJumpTriggerId, DigitalEdge digitalEdge = DigitalEdge.Rising)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConditionalJumpTriggers[conditionalJumpTriggerId].DigitalEdge.Configure(source, digitalEdge);
            });
        }

        /// <summary>
        /// Configures a software edge trigger for the specified Conditional Jump Trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="conditionalJumpTriggerId">
        /// The ID of the of ConditionalJumpTrigger to configure.
        /// Valid values include: 0, 1, 2, and 3,
        /// which all equate to conditionalJumpTrigger0, conditionalJumpTrigger1, conditionalJumpTrigger2, and conditionalJumpTrigger3, respectively.
        /// </param>
        public static void ConfigureConditionalJumpTriggerSoftwareEdge(this DigitalSessionsBundle sessionsBundle, int conditionalJumpTriggerId)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConditionalJumpTriggers[conditionalJumpTriggerId].Software.Configure();
            });
        }

        /// <summary>
        /// Configures a digital edge trigger for the Start Trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="source">
        /// The source terminal for the start trigger.
        /// Possible values include but are not limited to "PXI_Trig0", "PXI_Trig1", "PXI_Trig2", "PXI_Trig3", "PXI_Trig4", "PXI_Trig5", "PXI_Trig6", or "PXI_Trig7".
        /// </param>
        /// <param name="digitalEdge">The edge of the digital signal that should be triggered on: Rising edge (deafult) or Falling edge.</param>
        /// <exception cref="OutOfRangeException">The value for type is invalid.</exception>
        public static void ConfigureStartTriggerDigitalEdge(this DigitalSessionsBundle sessionsBundle, string source, DigitalEdge digitalEdge = DigitalEdge.Rising)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.StartTrigger.DigitalEdge.Configure(source, digitalEdge);
            });
        }

        /// <summary>
        /// Configures a software edge trigger for the Start Trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        public static void ConfigureStartTriggerSoftwareEdge(this DigitalSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.StartTrigger.Software.Configure();
            });
        }

        /// <summary>
        /// Configures the History RAM to trigger when it reaches a specified number of cycles.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="cycleNumber">The number of cycles to execute before the History RAM Trigger.</param>
        /// <param name="pretriggerSamples">The number of samples to acquire before the History RAM trigger.</param>
        /// <exception cref="OutOfRangeException">The value for pretriggerSamples is invalid.</exception>
        /// <exception cref="IviCDriverException">The underlying NI-Digital driver returned an error.</exception>
        /// <exception cref="ObjectDisposedException">The low-level Configure(Int64, Int32) NIDigital driver method was accessed after the associated NIDigital or DigitalDriverUtility object was disposed.</exception>
        public static void ConfigureHistoryRamTriggerOnCycleNumber(this DigitalSessionsBundle sessionsBundle, long cycleNumber, int pretriggerSamples = 0)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.HistoryRamTrigger.CycleNumber.Configure(cycleNumber, pretriggerSamples);
            });
        }

        /// <summary>
        /// Configures the History RAM to trigger if/when it reaches the first failure.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pretriggerSamples">The number of samples to acquire before the History RAM trigger.</param>
        /// <exception cref="OutOfRangeException">The value for pretriggerSamples is invalid.</exception>
        /// <exception cref="IviCDriverException">The underlying NI-Digital driver returned an error.</exception>
        /// <exception cref="ObjectDisposedException">The low-level Configure(Int32) NIDigital driver method was accessed after the associated NIDigital or DigitalDriverUtility object was disposed.</exception>
        public static void ConfigureHistoryRamTriggerOnFirstFailure(this DigitalSessionsBundle sessionsBundle, int pretriggerSamples = 0)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.HistoryRamTrigger.FirstFailure.Configure(pretriggerSamples);
            });
        }

        /// <summary>
        /// Configures the History RAM to trigger when it reaches the specified Pattern Label.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="patternLabel">The pattern label to augment by the vector and cycle offsets where History RAM will start acquiring pattern information.</param>
        /// <param name="vectorOffset">The number of vectors following the pattern label specified by Label where History RAM will start acquiring pattern information.</param>
        /// <param name="cycleOffset">The number of cycles following the pattern label specified by pattern label and vector offset specified by VectorOffset where History RAM will start acquiring pattern information.</param>
        /// <param name="pretriggerSamples">The number of samples to acquire before the History RAM trigger.</param>
        /// <exception cref="OutOfRangeException">The value for pretriggerSamples is invalid.</exception>
        /// <exception cref="IviCDriverException">The underlying NI-Digital driver returned an error.</exception>
        /// <exception cref="ObjectDisposedException">The low-level Configure(String, Int64, Int64, Int32) NIDigital driver method was accessed after the associated NIDigital or DigitalDriverUtility object was disposed.</exception>
        public static void ConfigureHistoryRamTriggerOnPattern(this DigitalSessionsBundle sessionsBundle, string patternLabel, long vectorOffset, long cycleOffset, int pretriggerSamples = 0)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.HistoryRamTrigger.PatternLabel.Configure(patternLabel, vectorOffset, cycleOffset, pretriggerSamples);
            });
        }

        /// <summary>
        /// Disables the specified ConditionalJumpTrigger by configuring it to None.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="conditionalJumpTriggerId">The ID of the of ConditionalJumpTrigger to configure.
        /// Valid values include: 0, 1, 2, and 3,
        /// which all equate to conditionalJumpTrigger0, conditionalJumpTrigger1, conditionalJumpTrigger2, and conditionalJumpTrigger3, respectively.</param>
        public static void DisableConditionalJumpTrigger(this DigitalSessionsBundle sessionsBundle, int conditionalJumpTriggerId)
        {
            sessionsBundle.Do((sessionInfo) =>
            {
                sessionInfo.Session.Trigger.ConditionalJumpTriggers[conditionalJumpTriggerId].Disable();
            });
        }

        /// <summary>
        /// Disables all ConditionalJumpTriggers by configuring them to None.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        public static void DisableConditionalJumpTriggers(this DigitalSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do((sessionInfo) =>
            {
                foreach (var trigger in sessionInfo.Session.Trigger.ConditionalJumpTriggers)
                {
                    trigger.Disable();
                }
            });
        }

        /// <summary>
        /// Disables StartTrigger trigger by configuring it to None.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        public static void DisableStartTrigger(this DigitalSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do((sessionInfo) =>
            {
                sessionInfo.Session.Trigger.StartTrigger.Disable();
            });
        }

        /// <summary>
        /// Routes trigger and event signals to the specified outputTerminal.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="signalType">The type of signal to export.</param>
        /// <param name="signalID">The instance of the selected signal to export. Possible values include "patternOpcodeEvent0", "patternOpcodeEvent1", "patternOpcodeEvent2", or "patternOpcodeEvent3". </param>
        /// <param name="outputTerminal">The terminal to which to export the signal. Possible values include but are not limited to "PXI_Trig0", "PXI_Trig1", "PXI_Trig2", "PXI_Trig3", "PXI_Trig4", "PXI_Trig5", "PXI_Trig6", or "PXI_Trig7". </param>
        /// <exception cref="ArgumentException">The value for one or all of the parameters, signalType, signalID, or outputTerminal, is invalid.</exception>
        /// <exception cref="viCDriverException">The underlying NI-Digital Pattern Driver returned an error.</exception>
        public static void ExportSignal(this DigitalSessionsBundle sessionsBundle, SignalType signalType, string signalID, string outputTerminal)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.ExportSignal(signalType, signalID, outputTerminal);
            });
        }

        /// <summary>
        /// Sends the software start trigger to a digital pattern instrument, forcing the start trigger to assert, regardless of how the start trigger is configured.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        public static void SendSoftwareEdgeStartTrigger(this DigitalSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.StartTrigger.Software.Send();
            });
        }

        /// <summary>
        /// Sends the Software Conditional Jump Trigger to a digital pattern instrument, forcing the Conditional Jump Trigger to assert, regardless of how the Conditional Jump Trigger is configured.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="conditionalJumpTriggerId">The ID of the of ConditionalJumpTrigger to configure.
        /// Valid values include: 0, 1, 2, and 3,
        /// which all equate to conditionalJumpTrigger0, conditionalJumpTrigger1, conditionalJumpTrigger2, and conditionalJumpTrigger3, respectively.</param>
        public static void SendSoftwareEdgeConditionalJumpTrigger(this DigitalSessionsBundle sessionsBundle, int conditionalJumpTriggerId)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.ConditionalJumpTriggers[conditionalJumpTriggerId].Software.Send();
            });
        }
    }
}
