using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    internal static class Utilities
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
        /// <remarks>
        /// This cache is populated on first access and eliminates the expensive reflection operations (GetProperties() and Enum.TryParse()) from being executed repeatedly. The cache includes
        /// properties that have a matching enum value.
        /// Thread Safety: Uses LazyThreadSafetyMode.ExecutionAndPublication to ensure the initialization function executes only once across all threads, with all threads seeing the same cached result.
        /// This makes it safe for parallel access without additional locking.
        /// </remarks>
        private static readonly Lazy<(PropertyInfo Property, DCPowerAdvancedSequenceProperty EnumValue)[]> _propertyMappingsCache =
            new Lazy<(PropertyInfo Property, DCPowerAdvancedSequenceProperty EnumValue)[]>(
                () =>
                typeof(DCPowerAdvancedSequenceStepProperties)
                    .GetProperties()
                    .Select(prop => (
                        Property: prop,
                        EnumValue: Enum.TryParse(prop.Name, out DCPowerAdvancedSequenceProperty enumValue) ? enumValue : (DCPowerAdvancedSequenceProperty?)null))
                    .Where(x => x.EnumValue.HasValue)
                    .Select(x => (x.Property, x.EnumValue.Value))
                    .ToArray(),
                LazyThreadSafetyMode.ExecutionAndPublication);

        public static string ExcludeSpecificChannel(this string channelString, string channelToExclude)
        {
            return string.Join(",", channelString.Split(',').Where(s => !s.Contains($"/{channelToExclude}")));
        }

        public static IEnumerable<TriggerType> GetUnsupportedTriggerTypes(string modelString)
        {
            foreach (TriggerType triggerType in _triggerTypeToUnsupportedModelStringMap.Keys)
            {
                if (_triggerTypeToUnsupportedModelStringMap[triggerType].Contains(modelString))
                {
                    yield return triggerType;
                }
            }
        }

        public static DCPowerAdvancedSequenceProperty[] GetAdvancedSequencePropertiesToConfigure(IEnumerable<DCPowerAdvancedSequenceStepProperties> perStepProperties)
        {
            var result = new HashSet<DCPowerAdvancedSequenceProperty>();
            foreach (var stepProperties in perStepProperties)
            {
                foreach (var (property, enumValue) in _propertyMappingsCache.Value)
                {
                    if (property.GetValue(stepProperties) != null)
                    {
                        result.Add(enumValue);
                    }
                }
            }

            return result.ToArray();
        }

        public static void ApplyStepProperties(
            DCPowerOutput channelOutput,
            DCPowerAdvancedSequenceStepProperties stepProperties,
            string modelString)
        {
            // Measurement Properties
            TrySetProperty(
                () =>
                {
                    if (stepProperties.ApertureTime.HasValue)
                    {
                        channelOutput.Measurement.ApertureTime = stepProperties.ApertureTime.Value;
                    }
                },
                "ApertureTime",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.DCNoiseRejection.HasValue)
                    {
                        channelOutput.Measurement.NoiseRejection = stepProperties.DCNoiseRejection.Value;
                    }
                },
                "DCNoiseRejection",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.Autorange.HasValue)
                    {
                        channelOutput.Measurement.Autorange = stepProperties.Autorange.Value;
                    }
                },
                "Autorange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.Sense.HasValue)
                    {
                        channelOutput.Measurement.Sense = stepProperties.Sense.Value;
                    }
                },
                "Sense",
                modelString);

            // DC Voltage Properties
            TrySetProperty(
                () =>
                {
                    if (stepProperties.VoltageLevel.HasValue)
                    {
                        channelOutput.Source.Voltage.VoltageLevel = stepProperties.VoltageLevel.Value;
                    }
                },
                "VoltageLevel",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.VoltageLevelRange.HasValue)
                    {
                        channelOutput.Source.Voltage.VoltageLevelRange = stepProperties.VoltageLevelRange.Value;
                    }
                },
                "VoltageLevelRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.VoltageLimit.HasValue)
                    {
                        channelOutput.Source.Voltage.CurrentLimit = stepProperties.CurrentLimit.Value;
                    }
                },
                "CurrentLimit",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.VoltageLimitRange.HasValue)
                    {
                        channelOutput.Source.Voltage.CurrentLimitRange = stepProperties.CurrentLimitRange.Value;
                    }
                },
                "CurrentLimitRange",
                modelString);

            // DC Current Properties
            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentLevel.HasValue)
                    {
                        channelOutput.Source.Current.CurrentLevel = stepProperties.CurrentLevel.Value;
                    }
                },
                "CurrentLevel",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentLevelRange.HasValue)
                    {
                        channelOutput.Source.Current.CurrentLevelRange = stepProperties.CurrentLevelRange.Value;
                    }
                },
                "CurrentLevelRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentLimit.HasValue)
                    {
                        channelOutput.Source.Current.VoltageLimit = stepProperties.VoltageLimit.Value;
                    }
                },
                "VoltageLimit",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentLimitRange.HasValue)
                    {
                        channelOutput.Source.Current.VoltageLimitRange = stepProperties.VoltageLimitRange.Value;
                    }
                },
                "VoltageLimitRange",
                modelString);

            // Pulse Voltage Properties
            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseVoltageLevel.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.VoltageLevel = stepProperties.PulseVoltageLevel.Value;
                    }
                },
                "PulseVoltageLevel",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseVoltageLevelRange.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.VoltageLevelRange = stepProperties.PulseVoltageLevelRange.Value;
                    }
                },
                "PulseVoltageLevelRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseVoltageLimit.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.CurrentLimit = stepProperties.PulseVoltageLimit.Value;
                    }
                },
                "PulseCurrentLimit",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseVoltageLimitRange.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.CurrentLimitRange = stepProperties.PulseVoltageLimitRange.Value;
                    }
                },
                "PulseCurrentLimitRange",
                modelString);

            // Pulse Current Properties
            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseCurrentLevel.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.CurrentLevel = stepProperties.PulseCurrentLevel.Value;
                    }
                },
                "PulseCurrentLevel",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseCurrentLevelRange.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.CurrentLevelRange = stepProperties.PulseCurrentLevelRange.Value;
                    }
                },
                "PulseCurrentLevelRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseCurrentLimit.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.VoltageLimit = stepProperties.PulseCurrentLimit.Value;
                    }
                },
                "PulseVoltageLimit",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseCurrentLimitRange.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.VoltageLimitRange = stepProperties.PulseCurrentLimitRange.Value;
                    }
                },
                "PulseVoltageLimitRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseBiasVoltageLevel.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.BiasVoltageLevel = stepProperties.PulseBiasVoltageLevel.Value;
                    }
                },
                "PulseBiasVoltageLevel",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseBiasCurrentLevel.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.BiasCurrentLevel = stepProperties.PulseBiasCurrentLevel.Value;
                    }
                },
                "PulseBiasCurrentLevel",
                modelString);

            // Source Delay
            TrySetProperty(
                () =>
                {
                    if (stepProperties.SourceDelay.HasValue)
                    {
                        channelOutput.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(stepProperties.SourceDelay.Value);
                    }
                },
                "SourceDelay",
                modelString);

            // Transient Response
            TrySetProperty(
                () =>
                {
                    if (stepProperties.TransientResponse.HasValue)
                    {
                        channelOutput.Source.TransientResponse = stepProperties.TransientResponse.Value;
                    }
                },
                "TransientResponse",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.MeasureRecordLength.HasValue)
                    {
                        channelOutput.Measurement.RecordLength = stepProperties.MeasureRecordLength.Value;
                    }
                },
                "MeasureRecordLength",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.OutputEnabled.HasValue)
                    {
                        channelOutput.Source.Output.Enabled = stepProperties.OutputEnabled.Value;
                    }
                },
                "OutputEnabled",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.OutputFunction.HasValue)
                    {
                        channelOutput.Source.Output.Function = stepProperties.OutputFunction.Value;
                    }
                },
                "OutputFunction",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ComplianceLimitSymmetry.HasValue)
                    {
                        channelOutput.Source.ComplianceLimitSymmetry = stepProperties.ComplianceLimitSymmetry.Value;
                    }
                },
                "ComplianceLimitSymmetry",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.OvpEnabled.HasValue)
                    {
                        channelOutput.Source.OvpEnabled = stepProperties.OvpEnabled.Value;
                    }
                },
                "OvpEnabled",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.OvpLimit.HasValue)
                    {
                        channelOutput.Source.OvpLimit = stepProperties.OvpLimit.Value;
                    }
                },
                "OvpLimit",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrCurrentAmplitude.HasValue)
                    {
                        channelOutput.LCR.CurrentAmplitude = stepProperties.LcrCurrentAmplitude.Value;
                    }
                },
                "LcrCurrentAmplitude",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrVoltageAmplitude.HasValue)
                    {
                        channelOutput.LCR.VoltageAmplitude = stepProperties.LcrVoltageAmplitude.Value;
                    }
                },
                "LcrVoltageAmplitude",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrFrequency.HasValue)
                    {
                        channelOutput.LCR.Frequency = stepProperties.LcrFrequency.Value;
                    }
                },
                "LcrFrequency",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrDcBiasCurrentLevel.HasValue)
                    {
                        channelOutput.LCR.DCBiasCurrentLevel = stepProperties.LcrDcBiasCurrentLevel.Value;
                    }
                },
                "LcrDcBiasCurrentLevel",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrDcBiasVoltageLevel.HasValue)
                    {
                        channelOutput.LCR.DCBiasVoltageLevel = stepProperties.LcrDcBiasVoltageLevel.Value;
                    }
                },
                "LcrDcBiasVoltageLevel",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrImpedanceRange.HasValue)
                    {
                        channelOutput.LCR.ImpedanceRange = stepProperties.LcrImpedanceRange.Value;
                    }
                },
                "LcrImpedanceRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrImpedanceRangeSource.HasValue)
                    {
                        channelOutput.LCR.ImpedanceRangeSource = stepProperties.LcrImpedanceRangeSource.Value;
                    }
                },
                "LcrImpedanceRangeSource",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrMeasurementTime.HasValue)
                    {
                        channelOutput.LCR.MeasurementTime = stepProperties.LcrMeasurementTime.Value;
                    }
                },
                "LcrMeasurementTime",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrCustomMeasurementTime.HasValue)
                    {
                        channelOutput.LCR.CustomMeasurementTime = stepProperties.LcrCustomMeasurementTime.Value;
                    }
                },
                "LcrCustomMeasurementTime",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrSourceApertureTime.HasValue)
                    {
                        channelOutput.LCR.Advanced.SourceApertureTime = stepProperties.LcrSourceApertureTime.Value;
                    }
                },
                "LcrSourceApertureTime",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrStimulusFunction.HasValue)
                    {
                        channelOutput.LCR.StimulusFunction = stepProperties.LcrStimulusFunction.Value;
                    }
                },
                "LcrStimulusFunction",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrDcBiasSource.HasValue)
                    {
                        channelOutput.LCR.DCBiasSource = stepProperties.LcrDcBiasSource.Value;
                    }
                },
                "LcrDcBiasSource",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrOpenCompensationEnabled.HasValue)
                    {
                        channelOutput.LCR.Compensation.OpenCompensationEnabled = stepProperties.LcrOpenCompensationEnabled.Value;
                    }
                },
                "LcrOpenCompensationEnabled",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrShortCompensationEnabled.HasValue)
                    {
                        channelOutput.LCR.Compensation.ShortCompensationEnabled = stepProperties.LcrShortCompensationEnabled.Value;
                    }
                },
                "LcrShortCompensationEnabled",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrLoadCompensationEnabled.HasValue)
                    {
                        channelOutput.LCR.Compensation.LoadCompensationEnabled = stepProperties.LcrLoadCompensationEnabled.Value;
                    }
                },
                "LcrLoadCompensationEnabled",
                modelString);
            TrySetProperty(
                () =>
                {
                    if (stepProperties.AutorangeThresholdMode.HasValue)
                    {
                        channelOutput.Measurement.AutorangeThresholdMode = stepProperties.AutorangeThresholdMode.Value;
                    }
                },
                "AutorangeThresholdMode",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.AutorangeApertureTimeMode.HasValue)
                    {
                        channelOutput.Measurement.AutorangeApertureTimeMode = stepProperties.AutorangeApertureTimeMode.Value;
                    }
                },
                "AutorangeApertureTimeMode",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.AutorangeBehavior.HasValue)
                    {
                        channelOutput.Measurement.AutorangeBehavior = stepProperties.AutorangeBehavior.Value;
                    }
                },
                "AutorangeBehavior",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.AutorangeMaximumDelayAfterRangeChange.HasValue)
                    {
                        channelOutput.Measurement.AutorangeMaximumDelayAfterRangeChange = stepProperties.AutorangeMaximumDelayAfterRangeChange.Value;
                    }
                },
                "AutorangeMaximumDelayAfterRangeChange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.AutorangeMinimumApertureTime.HasValue)
                    {
                        channelOutput.Measurement.AutorangeMinimumApertureTime = stepProperties.AutorangeMinimumApertureTime.Value;
                    }
                },
                "AutorangeMinimumApertureTime",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.AutorangeMinimumCurrentRange.HasValue)
                    {
                        channelOutput.Measurement.AutorangeMinimumCurrentRange = stepProperties.AutorangeMinimumCurrentRange.Value;
                    }
                },
                "AutorangeMinimumCurrentRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.AutorangeMinimumVoltageRange.HasValue)
                    {
                        channelOutput.Measurement.AutorangeMinimumVoltageRange = stepProperties.AutorangeMinimumVoltageRange.Value;
                    }
                },
                "AutorangeMinimumVoltageRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.AutorangeMinimumApertureTimeUnits.HasValue)
                    {
                        channelOutput.Measurement.AutorangeMinimumApertureTimeUnits = stepProperties.AutorangeMinimumApertureTimeUnits.Value;
                    }
                },
                "AutorangeMinimumApertureTimeUnits",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.VoltageLimitHigh.HasValue)
                    {
                        channelOutput.Source.Voltage.CurrentLimitHigh = stepProperties.VoltageLimitHigh.Value;
                    }
                },
                "CurrentLimitHigh",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.VoltageLimitLow.HasValue)
                    {
                        channelOutput.Source.Voltage.CurrentLimitLow = stepProperties.VoltageLimitLow.Value;
                    }
                },
                "CurrentLimitLow",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.VoltageCompensationFrequency.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.Voltage.CompensationFrequency = stepProperties.VoltageCompensationFrequency.Value;
                    }
                },
                "VoltageCompensationFrequency",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.VoltageGainBandwidth.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.Voltage.GainBandwidth = stepProperties.VoltageGainBandwidth.Value;
                    }
                },
                "VoltageGainBandwidth",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.VoltagePoleZeroRatio.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.Voltage.PoleZeroRatio = stepProperties.VoltagePoleZeroRatio.Value;
                    }
                },
                "VoltagePoleZeroRatio",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentLimitHigh.HasValue)
                    {
                        channelOutput.Source.Voltage.CurrentLimitHigh = stepProperties.CurrentLimitHigh.Value;
                    }
                },
                "VoltageLimitHigh",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentLimitLow.HasValue)
                    {
                        channelOutput.Source.Voltage.CurrentLimitLow = stepProperties.CurrentLimitLow.Value;
                    }
                },
                "VoltageLimitLow",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentCompensationFrequency.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.Current.CompensationFrequency = stepProperties.CurrentCompensationFrequency.Value;
                    }
                },
                "CurrentCompensationFrequency",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentGainBandwidth.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.Current.GainBandwidth = stepProperties.CurrentGainBandwidth.Value;
                    }
                },
                "CurrentGainBandwidth",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentPoleZeroRatio.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.Current.PoleZeroRatio = stepProperties.CurrentPoleZeroRatio.Value;
                    }
                },
                "CurrentPoleZeroRatio",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentLevelRisingSlewRate.HasValue)
                    {
                        channelOutput.Source.Current.CurrentLevelRisingSlewRate = stepProperties.CurrentLevelRisingSlewRate.Value;
                    }
                },
                "CurrentLevelRisingSlewRate",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.CurrentLevelFallingSlewRate.HasValue)
                    {
                        channelOutput.Source.Current.CurrentLevelFallingSlewRate = stepProperties.CurrentLevelFallingSlewRate.Value;
                    }
                },
                "CurrentLevelFallingSlewRate",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseVoltageLimitHigh.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.VoltageLimitHigh = stepProperties.PulseVoltageLimitHigh.Value;
                    }
                },
                "PulseVoltageLimitHigh",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseVoltageLimitLow.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.VoltageLimitLow = stepProperties.PulseVoltageLimitLow.Value;
                    }
                },
                "PulseVoltageLimitLow",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseBiasVoltageLimit.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.BiasVoltageLimit = stepProperties.PulseBiasVoltageLimit.Value;
                    }
                },
                "PulseBiasVoltageLimit",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseBiasVoltageLimitHigh.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.BiasVoltageLimitHigh = stepProperties.PulseBiasVoltageLimitHigh.Value;
                    }
                },
                "PulseBiasVoltageLimitHigh",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseBiasVoltageLimitLow.HasValue)
                    {
                        channelOutput.Source.PulseCurrent.BiasVoltageLimitLow = stepProperties.PulseBiasVoltageLimitLow.Value;
                    }
                },
                "PulseBiasVoltageLimitLow",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseCurrentLimitHigh.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.CurrentLimitHigh = stepProperties.PulseCurrentLimitHigh.Value;
                    }
                },
                "PulseCurrentLimitHigh",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseCurrentLimitLow.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.CurrentLimitLow = stepProperties.PulseCurrentLimitLow.Value;
                    }
                },
                "PulseCurrentLimitLow",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseBiasCurrentLimit.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.BiasCurrentLimit = stepProperties.PulseBiasCurrentLimit.Value;
                    }
                },
                "PulseBiasCurrentLimit",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseBiasCurrentLimitHigh.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.BiasCurrentLimitHigh = stepProperties.PulseBiasCurrentLimitHigh.Value;
                    }
                },
                "PulseBiasCurrentLimitHigh",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseBiasCurrentLimitLow.HasValue)
                    {
                        channelOutput.Source.PulseVoltage.BiasCurrentLimitLow = stepProperties.PulseBiasCurrentLimitLow.Value;
                    }
                },
                "PulseBiasCurrentLimitLow",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseOnTime.HasValue)
                    {
                        channelOutput.Source.PulseOnTime = stepProperties.PulseOnTime.Value;
                    }
                },
                "PulseOnTime",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseOffTime.HasValue)
                    {
                        channelOutput.Source.PulseOffTime = stepProperties.PulseOffTime.Value;
                    }
                },
                "PulseOffTime",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.PulseBiasDelay.HasValue)
                    {
                        channelOutput.Source.PulseBiasDelay = PrecisionTimeSpan.FromSeconds(stepProperties.PulseBiasDelay.Value);
                    }
                },
                "PulseBiasDelay",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.SequenceStepDeltaTime.HasValue)
                    {
                        channelOutput.Source.SequenceStepDeltaTime = PrecisionTimeSpan.FromSeconds(stepProperties.SequenceStepDeltaTime.Value);
                    }
                },
                "SequenceStepDeltaTime",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.OutputResistance.HasValue)
                    {
                        channelOutput.Source.Output.Resistance = stepProperties.OutputResistance.Value;
                    }
                },
                "OutputResistance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConductionVoltageMode.HasValue)
                    {
                        channelOutput.Source.ConductionVoltageMode = stepProperties.ConductionVoltageMode.Value;
                    }
                },
                "ConductionVoltageMode",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConductionVoltageOffThreshold.HasValue)
                    {
                        channelOutput.Source.ConductionVoltageOffThreshold = stepProperties.ConductionVoltageOffThreshold.Value;
                    }
                },
                "ConductionVoltageOffThreshold",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConductionVoltageOnThreshold.HasValue)
                    {
                        channelOutput.Source.ConductionVoltageOnThreshold = stepProperties.ConductionVoltageOnThreshold.Value;
                    }
                },
                "ConductionVoltageOnThreshold",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantPowerCompensationFrequency.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.ConstantPower.CompensationFrequency = stepProperties.ConstantPowerCompensationFrequency.Value;
                    }
                },
                "ConstantPowerCompensationFrequency",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantPowerCurrentLimit.HasValue)
                    {
                        channelOutput.Source.ConstantPower.CurrentLimit = stepProperties.ConstantPowerCurrentLimit.Value;
                    }
                },
                "ConstantPowerCurrentLimit",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantPowerGainBandwidth.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.ConstantPower.GainBandwidth = stepProperties.ConstantPowerGainBandwidth.Value;
                    }
                },
                "ConstantPowerGainBandwidth",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantPowerLevel.HasValue)
                    {
                        channelOutput.Source.ConstantPower.Level = stepProperties.ConstantPowerLevel.Value;
                    }
                },
                "ConstantPowerLevel",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantPowerLevelRange.HasValue)
                    {
                        channelOutput.Source.ConstantPower.LevelRange = stepProperties.ConstantPowerLevelRange.Value;
                    }
                },
                "ConstantPowerLevelRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantPowerPoleZeroRatio.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.ConstantPower.PoleZeroRatio = stepProperties.ConstantPowerPoleZeroRatio.Value;
                    }
                },
                "ConstantPowerPoleZeroRatio",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantResistanceCompensationFrequency.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.ConstantResistance.CompensationFrequency = stepProperties.ConstantResistanceCompensationFrequency.Value;
                    }
                },
                "ConstantResistanceCompensationFrequency",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantResistanceCurrentLimit.HasValue)
                    {
                        channelOutput.Source.ConstantResistance.CurrentLimit = stepProperties.ConstantResistanceCurrentLimit.Value;
                    }
                },
                "ConstantResistanceCurrentLimit",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantResistanceGainBandwidth.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.ConstantResistance.GainBandwidth = stepProperties.ConstantResistanceGainBandwidth.Value;
                    }
                },
                "ConstantResistanceGainBandwidth",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantResistanceLevel.HasValue)
                    {
                        channelOutput.Source.ConstantResistance.Level = stepProperties.ConstantResistanceLevel.Value;
                    }
                },
                "ConstantResistanceLevel",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantResistanceLevelRange.HasValue)
                    {
                        channelOutput.Source.ConstantResistance.LevelRange = stepProperties.ConstantResistanceLevelRange.Value;
                    }
                },
                "ConstantResistanceLevelRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.ConstantResistancePoleZeroRatio.HasValue)
                    {
                        channelOutput.Source.CustomTransientResponse.ConstantResistance.PoleZeroRatio = stepProperties.ConstantResistancePoleZeroRatio.Value;
                    }
                },
                "ConstantResistancePoleZeroRatio",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.OutputShorted.HasValue)
                    {
                        channelOutput.Source.OutputShorted = stepProperties.OutputShorted.Value;
                    }
                },
                "OutputShorted",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrActualLoadReactance.HasValue)
                    {
                        channelOutput.LCR.Compensation.ActualLoadReactance = stepProperties.LcrActualLoadReactance.Value;
                    }
                },
                "LcrActualLoadReactance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrActualLoadResistance.HasValue)
                    {
                        channelOutput.LCR.Compensation.ActualLoadResistance = stepProperties.LcrActualLoadResistance.Value;
                    }
                },
                "LcrActualLoadResistance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrCurrentRange.HasValue)
                    {
                        channelOutput.LCR.Advanced.CurrentRange = stepProperties.LcrCurrentRange.Value;
                    }
                },
                "LcrCurrentRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrDcBiasCurrentRange.HasValue)
                    {
                        channelOutput.LCR.Advanced.DCBiasCurrentRange = stepProperties.LcrDcBiasCurrentRange.Value;
                    }
                },
                "LcrDcBiasCurrentRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrDcBiasTransientResponse.HasValue)
                    {
                        channelOutput.LCR.Advanced.DCBiasTransientResponse = stepProperties.LcrDcBiasTransientResponse.Value;
                    }
                },
                "LcrDcBiasTransientResponse",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrDcBiasVoltageRange.HasValue)
                    {
                        channelOutput.LCR.Advanced.DCBiasVoltageRange = stepProperties.LcrDcBiasVoltageRange.Value;
                    }
                },
                "LcrDcBiasVoltageRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrImpedanceAutoRange.HasValue)
                    {
                        channelOutput.LCR.ImpedanceAutoRange = stepProperties.LcrImpedanceAutoRange.Value;
                    }
                },
                "LcrImpedanceAutoRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrLoadCapacitance.HasValue)
                    {
                        channelOutput.LCR.LoadCapacitance = stepProperties.LcrLoadCapacitance.Value;
                    }
                },
                "LcrLoadCapacitance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrLoadInductance.HasValue)
                    {
                        channelOutput.LCR.LoadInductance = stepProperties.LcrLoadInductance.Value;
                    }
                },
                "LcrLoadInductance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrLoadResistance.HasValue)
                    {
                        channelOutput.LCR.LoadResistance = stepProperties.LcrLoadResistance.Value;
                    }
                },
                "LcrLoadResistance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrMeasuredLoadReactance.HasValue)
                    {
                        channelOutput.LCR.Compensation.MeasuredLoadReactance = stepProperties.LcrMeasuredLoadReactance.Value;
                    }
                },
                "LcrMeasuredLoadReactance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrMeasuredLoadResistance.HasValue)
                    {
                        channelOutput.LCR.Compensation.MeasuredLoadResistance = stepProperties.LcrMeasuredLoadResistance.Value;
                    }
                },
                "LcrMeasuredLoadResistance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrOpenConductance.HasValue)
                    {
                        channelOutput.LCR.Compensation.OpenConductance = stepProperties.LcrOpenConductance.Value;
                    }
                },
                "LcrOpenConductance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrOpenSusceptance.HasValue)
                    {
                        channelOutput.LCR.Compensation.OpenSusceptance = stepProperties.LcrOpenSusceptance.Value;
                    }
                },
                "LcrOpenSusceptance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrShortReactance.HasValue)
                    {
                        channelOutput.LCR.Compensation.ShortReactance = stepProperties.LcrShortReactance.Value;
                    }
                },
                "LcrShortReactance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrShortResistance.HasValue)
                    {
                        channelOutput.LCR.Compensation.ShortResistance = stepProperties.LcrShortResistance.Value;
                    }
                },
                "LcrShortResistance",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrSourceDelayMode.HasValue)
                    {
                        channelOutput.LCR.SourceDelayMode = stepProperties.LcrSourceDelayMode.Value;
                    }
                },
                "LcrSourceDelayMode",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.LcrVoltageRange.HasValue)
                    {
                        channelOutput.LCR.Advanced.VoltageRange = stepProperties.LcrVoltageRange.Value;
                    }
                },
                "LcrVoltageRange",
                modelString);

            TrySetProperty(
                () =>
                {
                    if (stepProperties.InstrumentMode.HasValue)
                    {
                        channelOutput.InstrumentMode = stepProperties.InstrumentMode.Value;
                    }
                },
                "InstrumentMode",
                modelString);
        }

        /// <summary>
        /// Attempts to set a property, catching exceptions for unsupported properties.
        /// </summary>
        private static void TrySetProperty(Action setPropertyAction, string propertyName, string modelString)
        {
            try
            {
                setPropertyAction();
            }
            catch (Exception ex) when (ex is Ivi.Driver.IviCDriverException)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPowerDeviceNotSupported, propertyName, modelString));
            }
        }
    }
}
