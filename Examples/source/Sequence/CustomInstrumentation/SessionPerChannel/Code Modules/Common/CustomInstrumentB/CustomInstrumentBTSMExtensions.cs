using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannel.Common.CustomInstrumentB
{
    /// <summary>
    /// Class containing extensions to the <see cref="ISemiconductorModuleContext"/> interface
    /// to support instruments with an InstrumentTypeId of "CustomInstrumentB" declared in a pin map.
    /// Such instruments are expected to have a separate hardware session per instrument channel.
    /// </summary>
    public static class CustomInstrumentBTSMExtensions
    {
        public static readonly string InstrumentTypeId = "CustomInstrumentB";

        public static void GetAllCustomInstrumentBNames(
            this ISemiconductorModuleContext tsmContext,
            out string[] instrumentNames,
            out string[] channelGroups,
            out string[] channelLists)
        {
            // Get all instrument definitions of the specified instrument type.
            tsmContext.GetCustomInstrumentNames(
                InstrumentTypeId,
                out instrumentNames,
                out channelGroups,
                out channelLists);
            // Validate that the pin map XML is structured properly.
            // Each channel group must have a single channel with the same ID.
            for (int i = 0; i < channelGroups.Length; i++)
            {
                var channelGroup = channelGroups[i];
                var channelList = channelLists[i];
                if (channelList.Split(',').Length != 1 | channelGroup != channelList)
                {
                    throw new Exception($"Error in pin map XML. Expected a single channel per channel group \"{channelGroup}\".");
                }
            }
        }

        public static void SetCustomInstrumentBSessions(this ISemiconductorModuleContext tsmContext, string instrumentName, string channelGroupId, CustomInstrumentB session)
        {
            tsmContext.SetCustomSession(InstrumentTypeId, instrumentName, channelGroupId, session);
        }

        public static void GetAllCustomInstrumentBSessions(this ISemiconductorModuleContext tsmContext, out CustomInstrumentB[] sessions)
        {
            tsmContext.GetAllCustomSessions(InstrumentTypeId, out object[] customInstrumentSessions, out _, out _);
            sessions = Array.ConvertAll(customInstrumentSessions, x => (CustomInstrumentB)x);
        }

        public static void PinsToCustomInstrumentBSessions(
            this ISemiconductorModuleContext tsmContext,
            string[] pins,
            out CustomInstrumentB[] sessions,
            out string[] channelLists)
        {
            tsmContext.GetCustomSessions(
                InstrumentTypeId,
                pins,
                out var customInstrumentSessions,
                out _,
                out channelLists);
            sessions = Array.ConvertAll(customInstrumentSessions, x => (CustomInstrumentB)x);
        }
    }
}
