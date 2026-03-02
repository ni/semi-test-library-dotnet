using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Provides general helper methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Returns the trigger name for a given site pin, leader channel string, and trigger type.
        /// </summary>
        /// <param name="sitePinInfo">The site pin information object.</param>
        /// <param name="leaderChannelString">Channel string of the leader channel used for ganging.</param>
        /// <param name="triggerType">The type of trigger to generate the name for. Defaults to "Source".</param>
        /// <returns>
        /// The trigger name string for the specified site pin and trigger type, or an empty string if not applicable.
        /// </returns>
        public static string GetTriggerName(SitePinInfo sitePinInfo, string leaderChannelString, string triggerType = "Source")
        {
            var channel = sitePinInfo.IndividualChannelString;
            var leaderChannel = leaderChannelString.Split('/');
            var leaderChannelSlot = leaderChannel[0];
            var leaderChannelNumber = leaderChannel[leaderChannel.Length - 1];

            if (sitePinInfo.CascadingInfo is GangingInfo gangingInfo && gangingInfo.IsFollower)
            {
                return $"/{leaderChannelSlot}/Engine{leaderChannelNumber}/{triggerType}Trigger";
            }
            if (channel.Contains("SMU_4147") && triggerType == "Source")
            {
                return $"/{channel.Remove(channel.Length - 2)}/Immediate";
            }
            return string.Empty;
        }

        private static readonly Dictionary<TriggerType, IList<string>> _triggerTypeToUnsupportedModelStringMap = new Dictionary<TriggerType, IList<string>>
        {
            {
                TriggerType.MeasureTrigger, new List<string>
                {
                    DCPowerModelStrings.PXI_4110,
                    DCPowerModelStrings.PXI_4130
                }
            },
            {
                TriggerType.PulseTrigger, new List<string>
                {
                    DCPowerModelStrings.PXI_4110,
                    DCPowerModelStrings.PXIe_4112,
                    DCPowerModelStrings.PXIe_4113,
                    DCPowerModelStrings.PXI_4130,
                    DCPowerModelStrings.PXIe_4141,
                    DCPowerModelStrings.PXIe_4143,
                    DCPowerModelStrings.PXIe_4145,
                    DCPowerModelStrings.PXIe_4147,
                    DCPowerModelStrings.PXIe_4154,
                    DCPowerModelStrings.PXIe_4162,
                    DCPowerModelStrings.PXIe_4163,
                    DCPowerModelStrings.PXIe_4190
                }
            },
            {
                TriggerType.SequenceAdvanceTrigger, new List<string>
                {
                    DCPowerModelStrings.PXI_4110,
                    DCPowerModelStrings.PXI_4130
                }
            },
            {
                TriggerType.SourceTrigger, new List<string>
                {
                    DCPowerModelStrings.PXI_4110,
                    DCPowerModelStrings.PXI_4130
                }
            },
            {
                TriggerType.StartTrigger, new List<string>
                {
                    DCPowerModelStrings.PXI_4110,
                    DCPowerModelStrings.PXI_4130,
                    DCPowerModelStrings.PXIe_4145
                }
            },
            {
                TriggerType.ShutdownTrigger, new List<string>
                {
                    DCPowerModelStrings.PXI_4110,
                    DCPowerModelStrings.PXIe_4112,
                    DCPowerModelStrings.PXIe_4113,
                    DCPowerModelStrings.PXI_4130,
                    DCPowerModelStrings.PXIe_4135,
                    DCPowerModelStrings.PXIe_4137,
                    DCPowerModelStrings.PXIe_4141,
                    DCPowerModelStrings.PXIe_4143,
                    DCPowerModelStrings.PXIe_4145,
                    DCPowerModelStrings.PXIe_4147,
                    DCPowerModelStrings.PXIe_4154,
                    DCPowerModelStrings.PXIe_4162,
                    DCPowerModelStrings.PXIe_4163,
                    DCPowerModelStrings.PXIe_4190
                }
            }
        };

        /// <summary>
        /// Caches mappings between properties of DCPowerAdvancedSequenceStepProperties and their corresponding DCPowerAdvancedSequenceProperty enum values.
        /// </summary>
        internal static (PropertyInfo Property, DCPowerAdvancedSequenceProperty EnumValue)[] PropertyMappingsCache;

        /// <summary>
        /// Initializes the cache for DC power advanced sequence property mappings, so that it can be called in while SetupDCPowerInstrumentation reducing the first-call latency when the cache is needed later.
        /// </summary>
        public static void CreateDCPowerAdvancedSequencePropertyMappingsCache()
        {
            PropertyMappingsCache = typeof(DCPowerAdvancedSequenceStepProperties)
                .GetProperties()
                .Select(prop => (
                    Property: prop,
                    EnumValue: Enum.TryParse(prop.Name, out DCPowerAdvancedSequenceProperty enumValue) ? enumValue : (DCPowerAdvancedSequenceProperty?)null))
                .Where(x => x.EnumValue.HasValue)
                .Select(x => (x.Property, x.EnumValue.Value))
                .ToArray();
        }

        internal static string ExcludeSpecificChannel(this string channelString, string channelToExclude)
        {
            return string.Join(",", channelString.Split(',').Where(s => !s.Contains($"/{channelToExclude}")));
        }

        internal static IEnumerable<TriggerType> GetUnsupportedTriggerTypes(string modelString)
        {
            foreach (TriggerType triggerType in _triggerTypeToUnsupportedModelStringMap.Keys)
            {
                if (_triggerTypeToUnsupportedModelStringMap[triggerType].Contains(modelString))
                {
                    yield return triggerType;
                }
            }
        }
    }
}
