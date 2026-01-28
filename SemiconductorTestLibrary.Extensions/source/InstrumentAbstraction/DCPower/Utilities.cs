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
            DCPowerAdvancedSequenceStepProperties stepProperties)
        {
            // Measurement Properties
            if (stepProperties.ApertureTime.HasValue)
            {
                channelOutput.Measurement.ApertureTime = stepProperties.ApertureTime.Value;
            }
            if (stepProperties.DCNoiseRejection.HasValue)
            {
                channelOutput.Measurement.NoiseRejection = stepProperties.DCNoiseRejection.Value;
            }
            if (stepProperties.Autorange.HasValue)
            {
                channelOutput.Measurement.Autorange = stepProperties.Autorange.Value;
            }
            if (stepProperties.Sense.HasValue)
            {
                channelOutput.Measurement.Sense = stepProperties.Sense.Value;
            }
            if (stepProperties.VoltageLevel.HasValue)
            {
                channelOutput.Source.Voltage.VoltageLevel = stepProperties.VoltageLevel.Value;
            }
            if (stepProperties.VoltageLevelRange.HasValue)
            {
                channelOutput.Source.Voltage.VoltageLevelRange = stepProperties.VoltageLevelRange.Value;
            }
            if (stepProperties.CurrentLimit.HasValue)
            {
                channelOutput.Source.Voltage.CurrentLimit = stepProperties.CurrentLimit.Value;
            }
            if (stepProperties.CurrentLimitRange.HasValue)
            {
                channelOutput.Source.Voltage.CurrentLimitRange = stepProperties.CurrentLimitRange.Value;
            }
            // DC Current Properties
            if (stepProperties.CurrentLevel.HasValue)
            {
                channelOutput.Source.Current.CurrentLevel = stepProperties.CurrentLevel.Value;
            }
            if (stepProperties.CurrentLevelRange.HasValue)
            {
                channelOutput.Source.Current.CurrentLevelRange = stepProperties.CurrentLevelRange.Value;
            }
            if (stepProperties.VoltageLimit.HasValue)
            {
                channelOutput.Source.Current.VoltageLimit = stepProperties.VoltageLimit.Value;
            }
            if (stepProperties.VoltageLimitRange.HasValue)
            {
                channelOutput.Source.Current.VoltageLimitRange = stepProperties.VoltageLimitRange.Value;
            }
            if (stepProperties.PulseVoltageLevel.HasValue)
            {
                channelOutput.Source.PulseVoltage.VoltageLevel = stepProperties.PulseVoltageLevel.Value;
            }
            if (stepProperties.PulseVoltageLevelRange.HasValue)
            {
                channelOutput.Source.PulseVoltage.VoltageLevelRange = stepProperties.PulseVoltageLevelRange.Value;
            }
            if (stepProperties.PulseVoltageLimit.HasValue)
            {
                channelOutput.Source.PulseVoltage.CurrentLimit = stepProperties.PulseVoltageLimit.Value;
            }
            if (stepProperties.PulseVoltageLimitRange.HasValue)
            {
                channelOutput.Source.PulseVoltage.CurrentLimitRange = stepProperties.PulseVoltageLimitRange.Value;
            }

            // Pulse Current Properties

            if (stepProperties.PulseCurrentLevel.HasValue)
            {
                channelOutput.Source.PulseCurrent.CurrentLevel = stepProperties.PulseCurrentLevel.Value;
            }
            if (stepProperties.PulseCurrentLevelRange.HasValue)
            {
                channelOutput.Source.PulseCurrent.CurrentLevelRange = stepProperties.PulseCurrentLevelRange.Value;
            }
            if (stepProperties.PulseCurrentLimit.HasValue)
            {
                channelOutput.Source.PulseCurrent.VoltageLimit = stepProperties.PulseCurrentLimit.Value;
            }
            if (stepProperties.PulseCurrentLimitRange.HasValue)
            {
                channelOutput.Source.PulseCurrent.VoltageLimitRange = stepProperties.PulseCurrentLimitRange.Value;
            }
            if (stepProperties.PulseBiasVoltageLevel.HasValue)
            {
                channelOutput.Source.PulseVoltage.BiasVoltageLevel = stepProperties.PulseBiasVoltageLevel.Value;
            }
            if (stepProperties.PulseBiasCurrentLevel.HasValue)
            {
                channelOutput.Source.PulseCurrent.BiasCurrentLevel = stepProperties.PulseBiasCurrentLevel.Value;
            }
            if (stepProperties.SourceDelay.HasValue)
            {
                channelOutput.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(stepProperties.SourceDelay.Value);
            }
            if (stepProperties.TransientResponse.HasValue)
            {
                channelOutput.Source.TransientResponse = stepProperties.TransientResponse.Value;
            }
            if (stepProperties.MeasureRecordLength.HasValue)
            {
                channelOutput.Measurement.RecordLength = stepProperties.MeasureRecordLength.Value;
            }
            if (stepProperties.OutputEnabled.HasValue)
            {
                channelOutput.Source.Output.Enabled = stepProperties.OutputEnabled.Value;
            }
            if (stepProperties.OutputFunction.HasValue)
            {
                channelOutput.Source.Output.Function = stepProperties.OutputFunction.Value;
            }
            if (stepProperties.ComplianceLimitSymmetry.HasValue)
            {
                channelOutput.Source.ComplianceLimitSymmetry = stepProperties.ComplianceLimitSymmetry.Value;
            }
            if (stepProperties.OvpEnabled.HasValue)
            {
                channelOutput.Source.OvpEnabled = stepProperties.OvpEnabled.Value;
            }
            if (stepProperties.OvpLimit.HasValue)
            {
                channelOutput.Source.OvpLimit = stepProperties.OvpLimit.Value;
            }
            if (stepProperties.LcrCurrentAmplitude.HasValue)
            {
                channelOutput.LCR.CurrentAmplitude = stepProperties.LcrCurrentAmplitude.Value;
            }
            if (stepProperties.LcrVoltageAmplitude.HasValue)
            {
                channelOutput.LCR.VoltageAmplitude = stepProperties.LcrVoltageAmplitude.Value;
            }
            if (stepProperties.LcrFrequency.HasValue)
            {
                channelOutput.LCR.Frequency = stepProperties.LcrFrequency.Value;
            }
            if (stepProperties.LcrDcBiasCurrentLevel.HasValue)
            {
                channelOutput.LCR.DCBiasCurrentLevel = stepProperties.LcrDcBiasCurrentLevel.Value;
            }
            if (stepProperties.LcrDcBiasVoltageLevel.HasValue)
            {
                channelOutput.LCR.DCBiasVoltageLevel = stepProperties.LcrDcBiasVoltageLevel.Value;
            }
            if (stepProperties.LcrImpedanceRange.HasValue)
            {
                channelOutput.LCR.ImpedanceRange = stepProperties.LcrImpedanceRange.Value;
            }
            if (stepProperties.LcrImpedanceRangeSource.HasValue)
            {
                channelOutput.LCR.ImpedanceRangeSource = stepProperties.LcrImpedanceRangeSource.Value;
            }
            if (stepProperties.LcrMeasurementTime.HasValue)
            {
                channelOutput.LCR.MeasurementTime = stepProperties.LcrMeasurementTime.Value;
            }
            if (stepProperties.LcrCustomMeasurementTime.HasValue)
            {
                channelOutput.LCR.CustomMeasurementTime = stepProperties.LcrCustomMeasurementTime.Value;
            }
            if (stepProperties.LcrSourceApertureTime.HasValue)
            {
                channelOutput.LCR.Advanced.SourceApertureTime = stepProperties.LcrSourceApertureTime.Value;
            }
            if (stepProperties.LcrStimulusFunction.HasValue)
            {
                channelOutput.LCR.StimulusFunction = stepProperties.LcrStimulusFunction.Value;
            }
            if (stepProperties.LcrDcBiasSource.HasValue)
            {
                channelOutput.LCR.DCBiasSource = stepProperties.LcrDcBiasSource.Value;
            }
            if (stepProperties.LcrOpenCompensationEnabled.HasValue)
            {
                channelOutput.LCR.Compensation.OpenCompensationEnabled = stepProperties.LcrOpenCompensationEnabled.Value;
            }
            if (stepProperties.LcrShortCompensationEnabled.HasValue)
            {
                channelOutput.LCR.Compensation.ShortCompensationEnabled = stepProperties.LcrShortCompensationEnabled.Value;
            }
            if (stepProperties.LcrLoadCompensationEnabled.HasValue)
            {
                channelOutput.LCR.Compensation.LoadCompensationEnabled = stepProperties.LcrLoadCompensationEnabled.Value;
            }
            if (stepProperties.AutorangeThresholdMode.HasValue)
            {
                channelOutput.Measurement.AutorangeThresholdMode = stepProperties.AutorangeThresholdMode.Value;
            }
            if (stepProperties.AutorangeApertureTimeMode.HasValue)
            {
                channelOutput.Measurement.AutorangeApertureTimeMode = stepProperties.AutorangeApertureTimeMode.Value;
            }
            if (stepProperties.AutorangeBehavior.HasValue)
            {
                channelOutput.Measurement.AutorangeBehavior = stepProperties.AutorangeBehavior.Value;
            }
            if (stepProperties.AutorangeMaximumDelayAfterRangeChange.HasValue)
            {
                channelOutput.Measurement.AutorangeMaximumDelayAfterRangeChange = stepProperties.AutorangeMaximumDelayAfterRangeChange.Value;
            }
            if (stepProperties.AutorangeMinimumApertureTime.HasValue)
            {
                channelOutput.Measurement.AutorangeMinimumApertureTime = stepProperties.AutorangeMinimumApertureTime.Value;
            }
            if (stepProperties.AutorangeMinimumCurrentRange.HasValue)
            {
                channelOutput.Measurement.AutorangeMinimumCurrentRange = stepProperties.AutorangeMinimumCurrentRange.Value;
            }
            if (stepProperties.AutorangeMinimumVoltageRange.HasValue)
            {
                channelOutput.Measurement.AutorangeMinimumVoltageRange = stepProperties.AutorangeMinimumVoltageRange.Value;
            }
            if (stepProperties.AutorangeMinimumApertureTimeUnits.HasValue)
            {
                channelOutput.Measurement.AutorangeMinimumApertureTimeUnits = stepProperties.AutorangeMinimumApertureTimeUnits.Value;
            }
            if (stepProperties.VoltageLimitHigh.HasValue)
            {
                channelOutput.Source.Current.VoltageLimitHigh = stepProperties.VoltageLimitHigh.Value;
            }
            if (stepProperties.VoltageLimitLow.HasValue)
            {
                channelOutput.Source.Current.VoltageLimitLow = stepProperties.VoltageLimitLow.Value;
            }
            if (stepProperties.VoltageCompensationFrequency.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Voltage.CompensationFrequency = stepProperties.VoltageCompensationFrequency.Value;
            }
            if (stepProperties.VoltageGainBandwidth.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Voltage.GainBandwidth = stepProperties.VoltageGainBandwidth.Value;
            }
            if (stepProperties.VoltagePoleZeroRatio.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Voltage.PoleZeroRatio = stepProperties.VoltagePoleZeroRatio.Value;
            }
            if (stepProperties.CurrentLimitHigh.HasValue)
            {
                channelOutput.Source.Voltage.CurrentLimitHigh = stepProperties.CurrentLimitHigh.Value;
            }
            if (stepProperties.CurrentLimitLow.HasValue)
            {
                channelOutput.Source.Voltage.CurrentLimitLow = stepProperties.CurrentLimitLow.Value;
            }
            if (stepProperties.CurrentCompensationFrequency.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Current.CompensationFrequency = stepProperties.CurrentCompensationFrequency.Value;
            }
            if (stepProperties.CurrentGainBandwidth.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Current.GainBandwidth = stepProperties.CurrentGainBandwidth.Value;
            }
            if (stepProperties.CurrentPoleZeroRatio.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Current.PoleZeroRatio = stepProperties.CurrentPoleZeroRatio.Value;
            }
            if (stepProperties.CurrentLevelRisingSlewRate.HasValue)
            {
                channelOutput.Source.Current.CurrentLevelRisingSlewRate = stepProperties.CurrentLevelRisingSlewRate.Value;
            }
            if (stepProperties.CurrentLevelFallingSlewRate.HasValue)
            {
                channelOutput.Source.Current.CurrentLevelFallingSlewRate = stepProperties.CurrentLevelFallingSlewRate.Value;
            }
            if (stepProperties.PulseVoltageLimitHigh.HasValue)
            {
                channelOutput.Source.PulseCurrent.VoltageLimitHigh = stepProperties.PulseVoltageLimitHigh.Value;
            }
            if (stepProperties.PulseVoltageLimitLow.HasValue)
            {
                channelOutput.Source.PulseCurrent.VoltageLimitLow = stepProperties.PulseVoltageLimitLow.Value;
            }
            if (stepProperties.PulseBiasVoltageLimit.HasValue)
            {
                channelOutput.Source.PulseCurrent.BiasVoltageLimit = stepProperties.PulseBiasVoltageLimit.Value;
            }
            if (stepProperties.PulseBiasVoltageLimitHigh.HasValue)
            {
                channelOutput.Source.PulseCurrent.BiasVoltageLimitHigh = stepProperties.PulseBiasVoltageLimitHigh.Value;
            }
            if (stepProperties.PulseBiasVoltageLimitLow.HasValue)
            {
                channelOutput.Source.PulseCurrent.BiasVoltageLimitLow = stepProperties.PulseBiasVoltageLimitLow.Value;
            }
            if (stepProperties.PulseCurrentLimitHigh.HasValue)
            {
                channelOutput.Source.PulseVoltage.CurrentLimitHigh = stepProperties.PulseCurrentLimitHigh.Value;
            }
            if (stepProperties.PulseCurrentLimitLow.HasValue)
            {
                channelOutput.Source.PulseVoltage.CurrentLimitLow = stepProperties.PulseCurrentLimitLow.Value;
            }
            if (stepProperties.PulseBiasCurrentLimit.HasValue)
            {
                channelOutput.Source.PulseVoltage.BiasCurrentLimit = stepProperties.PulseBiasCurrentLimit.Value;
            }
            if (stepProperties.PulseBiasCurrentLimitHigh.HasValue)
            {
                channelOutput.Source.PulseVoltage.BiasCurrentLimitHigh = stepProperties.PulseBiasCurrentLimitHigh.Value;
            }
            if (stepProperties.PulseBiasCurrentLimitLow.HasValue)
            {
                channelOutput.Source.PulseVoltage.BiasCurrentLimitLow = stepProperties.PulseBiasCurrentLimitLow.Value;
            }
            if (stepProperties.PulseOnTime.HasValue)
            {
                channelOutput.Source.PulseOnTime = stepProperties.PulseOnTime.Value;
            }
            if (stepProperties.PulseOffTime.HasValue)
            {
                channelOutput.Source.PulseOffTime = stepProperties.PulseOffTime.Value;
            }
            if (stepProperties.PulseBiasDelay.HasValue)
            {
                channelOutput.Source.PulseBiasDelay = PrecisionTimeSpan.FromSeconds(stepProperties.PulseBiasDelay.Value);
            }
            if (stepProperties.SequenceStepDeltaTime.HasValue)
            {
                channelOutput.Source.SequenceStepDeltaTime = PrecisionTimeSpan.FromSeconds(stepProperties.SequenceStepDeltaTime.Value);
            }
            if (stepProperties.OutputResistance.HasValue)
            {
                channelOutput.Source.Output.Resistance = stepProperties.OutputResistance.Value;
            }
            if (stepProperties.ConductionVoltageMode.HasValue)
            {
                channelOutput.Source.ConductionVoltageMode = stepProperties.ConductionVoltageMode.Value;
            }
            if (stepProperties.ConductionVoltageOffThreshold.HasValue)
            {
                channelOutput.Source.ConductionVoltageOffThreshold = stepProperties.ConductionVoltageOffThreshold.Value;
            }
            if (stepProperties.ConductionVoltageOnThreshold.HasValue)
            {
                channelOutput.Source.ConductionVoltageOnThreshold = stepProperties.ConductionVoltageOnThreshold.Value;
            }
            if (stepProperties.ConstantPowerCompensationFrequency.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.ConstantPower.CompensationFrequency = stepProperties.ConstantPowerCompensationFrequency.Value;
            }
            if (stepProperties.ConstantPowerCurrentLimit.HasValue)
            {
                channelOutput.Source.ConstantPower.CurrentLimit = stepProperties.ConstantPowerCurrentLimit.Value;
            }
            if (stepProperties.ConstantPowerGainBandwidth.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.ConstantPower.GainBandwidth = stepProperties.ConstantPowerGainBandwidth.Value;
            }
            if (stepProperties.ConstantPowerLevel.HasValue)
            {
                channelOutput.Source.ConstantPower.Level = stepProperties.ConstantPowerLevel.Value;
            }
            if (stepProperties.ConstantPowerLevelRange.HasValue)
            {
                channelOutput.Source.ConstantPower.LevelRange = stepProperties.ConstantPowerLevelRange.Value;
            }
            if (stepProperties.ConstantPowerPoleZeroRatio.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.ConstantPower.PoleZeroRatio = stepProperties.ConstantPowerPoleZeroRatio.Value;
            }
            if (stepProperties.ConstantResistanceCompensationFrequency.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.ConstantResistance.CompensationFrequency = stepProperties.ConstantResistanceCompensationFrequency.Value;
            }
            if (stepProperties.ConstantResistanceCurrentLimit.HasValue)
            {
                channelOutput.Source.ConstantResistance.CurrentLimit = stepProperties.ConstantResistanceCurrentLimit.Value;
            }
            if (stepProperties.ConstantResistanceGainBandwidth.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.ConstantResistance.GainBandwidth = stepProperties.ConstantResistanceGainBandwidth.Value;
            }
            if (stepProperties.ConstantResistanceLevel.HasValue)
            {
                channelOutput.Source.ConstantResistance.Level = stepProperties.ConstantResistanceLevel.Value;
            }
            if (stepProperties.ConstantResistanceLevelRange.HasValue)
            {
                channelOutput.Source.ConstantResistance.LevelRange = stepProperties.ConstantResistanceLevelRange.Value;
            }
            if (stepProperties.ConstantResistancePoleZeroRatio.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.ConstantResistance.PoleZeroRatio = stepProperties.ConstantResistancePoleZeroRatio.Value;
            }
            if (stepProperties.OutputShorted.HasValue)
            {
                channelOutput.Source.OutputShorted = stepProperties.OutputShorted.Value;
            }
            if (stepProperties.LcrActualLoadReactance.HasValue)
            {
                channelOutput.LCR.Compensation.ActualLoadReactance = stepProperties.LcrActualLoadReactance.Value;
            }
            if (stepProperties.LcrActualLoadResistance.HasValue)
            {
                channelOutput.LCR.Compensation.ActualLoadResistance = stepProperties.LcrActualLoadResistance.Value;
            }
            if (stepProperties.LcrCurrentRange.HasValue)
            {
                channelOutput.LCR.Advanced.CurrentRange = stepProperties.LcrCurrentRange.Value;
            }
            if (stepProperties.LcrDcBiasCurrentRange.HasValue)
            {
                channelOutput.LCR.Advanced.DCBiasCurrentRange = stepProperties.LcrDcBiasCurrentRange.Value;
            }
            if (stepProperties.LcrDcBiasTransientResponse.HasValue)
            {
                channelOutput.LCR.Advanced.DCBiasTransientResponse = stepProperties.LcrDcBiasTransientResponse.Value;
            }
            if (stepProperties.LcrDcBiasVoltageRange.HasValue)
            {
                channelOutput.LCR.Advanced.DCBiasVoltageRange = stepProperties.LcrDcBiasVoltageRange.Value;
            }
            if (stepProperties.LcrImpedanceAutoRange.HasValue)
            {
                channelOutput.LCR.ImpedanceAutoRange = stepProperties.LcrImpedanceAutoRange.Value;
            }
            if (stepProperties.LcrLoadCapacitance.HasValue)
            {
                channelOutput.LCR.LoadCapacitance = stepProperties.LcrLoadCapacitance.Value;
            }
            if (stepProperties.LcrLoadInductance.HasValue)
            {
                channelOutput.LCR.LoadInductance = stepProperties.LcrLoadInductance.Value;
            }
            if (stepProperties.LcrLoadResistance.HasValue)
            {
                channelOutput.LCR.LoadResistance = stepProperties.LcrLoadResistance.Value;
            }
            if (stepProperties.LcrMeasuredLoadReactance.HasValue)
            {
                channelOutput.LCR.Compensation.MeasuredLoadReactance = stepProperties.LcrMeasuredLoadReactance.Value;
            }
            if (stepProperties.LcrMeasuredLoadResistance.HasValue)
            {
                channelOutput.LCR.Compensation.MeasuredLoadResistance = stepProperties.LcrMeasuredLoadResistance.Value;
            }
            if (stepProperties.LcrOpenConductance.HasValue)
            {
                channelOutput.LCR.Compensation.OpenConductance = stepProperties.LcrOpenConductance.Value;
            }
            if (stepProperties.LcrOpenSusceptance.HasValue)
            {
                channelOutput.LCR.Compensation.OpenSusceptance = stepProperties.LcrOpenSusceptance.Value;
            }
            if (stepProperties.LcrShortReactance.HasValue)
            {
                channelOutput.LCR.Compensation.ShortReactance = stepProperties.LcrShortReactance.Value;
            }
            if (stepProperties.LcrShortResistance.HasValue)
            {
                channelOutput.LCR.Compensation.ShortResistance = stepProperties.LcrShortResistance.Value;
            }
            if (stepProperties.LcrSourceDelayMode.HasValue)
            {
                channelOutput.LCR.SourceDelayMode = stepProperties.LcrSourceDelayMode.Value;
            }
            if (stepProperties.LcrVoltageRange.HasValue)
            {
                channelOutput.LCR.Advanced.VoltageRange = stepProperties.LcrVoltageRange.Value;
            }
            if (stepProperties.InstrumentMode.HasValue)
            {
                channelOutput.InstrumentMode = stepProperties.InstrumentMode.Value;
            }
        }
    }
}
