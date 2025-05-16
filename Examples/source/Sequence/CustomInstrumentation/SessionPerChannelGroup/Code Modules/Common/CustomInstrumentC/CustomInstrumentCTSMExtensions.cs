using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System;
using System.Linq;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannelGroup.Common.CustomInstrumentC
{
    /// <summary>
    /// Class containing extensions to the <see cref="ISemiconductorModuleContext"/> interface
    /// to support instruments with an InstrumentTypeId of "CustomInstrumentC" declared in a pin map.
    /// Such instruments are expected to have a separate hardware session per group of channels.
    /// </summary>
    public static class CustomInstrumentCTSMExtensions
    {
        public static readonly string InstrumentTypeId = "CustomInstrumentC";

        public static void GetAllCustomInstrumentCNames(
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
        }

        public static void SetCustomInstrumentCSessions(this ISemiconductorModuleContext tsmContext, string instrumentName, string channelGroupId, CustomInstrumentC session)
        {
            tsmContext.SetCustomSession(InstrumentTypeId, instrumentName, channelGroupId, session);
        }

        public static void GetAllCustomInstrumentCSessions(this ISemiconductorModuleContext tsmContext, out CustomInstrumentC[] sessions)
        {
            tsmContext.GetAllCustomSessions(InstrumentTypeId, out object[] customInstrumentSessions, out _, out _);
            sessions = Array.ConvertAll(customInstrumentSessions, x => (CustomInstrumentC)x);
        }

        public static void PinsToCustomInstrumentCSessions(
            this ISemiconductorModuleContext tsmContext,
            string[] pins,
            out CustomInstrumentC[] sessions,
            out string[] channelGroups,
            out string[] channelLists)
        {
            tsmContext.GetCustomSessions(
                InstrumentTypeId,
                pins,
                out var customInstrumentSessions,
                out channelGroups,
                out channelLists);
            sessions = Array.ConvertAll(customInstrumentSessions, x => (CustomInstrumentC)x);
        }
    }
}
