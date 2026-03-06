using System;
using System.Linq;
using System.Reflection;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Provides general helper methods.
    /// </summary>
    public static class Utilities
    {
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
    }
}
