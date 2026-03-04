using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Provides general helper methods.
    /// </summary>
    public static class Utilities
    {
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
