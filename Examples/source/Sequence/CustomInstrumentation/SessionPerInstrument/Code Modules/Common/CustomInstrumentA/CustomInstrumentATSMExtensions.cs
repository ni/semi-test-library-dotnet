using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System;
using System.Linq;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerInstrument.Common.CustomInstrumentA
{

    /// <summary>
    /// Class containing extensions to the <see cref="ISemiconductorModuleContext"/> interface
    /// to support instruments with an InstrumentTypeId of "CustomInstrumentA" declared in a pin map.
    /// Such instruments are expected to have a separate hardware session per instrument device/module.
    /// </summary>
    public static class CustomInstrumentATSMExtensions
    {
        public static readonly string InstrumentTypeId = "CustomInstrumentA";
        private static readonly string ExpectedChannelGroupId = "allChannels";

        public static void GetAllCustomInstrumentANames(this ISemiconductorModuleContext tsmContext, out string[] instrumentNames, out string[] channelLists)
        {
            // Get all instrument definitions of the specified instrument type.
            tsmContext.GetCustomInstrumentNames(
                InstrumentTypeId,
                out instrumentNames,
                out string[] channelGroups,
                out channelLists);
            // Validate that the pin map XML is structured properly.
            // Each instrument must contain a single channel group with an ID of 'allChannels'.
            if (channelGroups.Any(x => x != ExpectedChannelGroupId))
            {
                throw new Exception($"Error in pin map XML. Expected a single channel group with id \"{ExpectedChannelGroupId}\".");
            }
        }

        public static void SetCustomInstrumentASessions(this ISemiconductorModuleContext tsmContext, string instrumentName, CustomInstrumentA session)
        {
            tsmContext.SetCustomSession(InstrumentTypeId, instrumentName, ExpectedChannelGroupId, session);
        }

        public static void GetAllCustomInstrumentASessions(this ISemiconductorModuleContext tsmContext, out CustomInstrumentA[] sessions)
        {
            tsmContext.GetAllCustomSessions(InstrumentTypeId, out object[] customInstrumentSessions, out _, out _);
            sessions = Array.ConvertAll(customInstrumentSessions, x => (CustomInstrumentA)x );
        }

        public static void PinsToCustomInstrumentASessions(
            this ISemiconductorModuleContext tsmContext,
            string[] pins,
            out CustomInstrumentA[] sessions,
            out string[] channelLists)
        {
            tsmContext.GetCustomSessions(
                InstrumentTypeId,
                pins,
                out var customInstrumentSessions,
                out _,
                out channelLists);
            sessions = Array.ConvertAll(customInstrumentSessions, x => (CustomInstrumentA)x );
        }
    }
}
