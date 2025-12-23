using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;

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

        public static DCPowerAdvancedSequenceProperty[] ExtractAdvancedSequencePropertiesArray(
            DCPowerAdvancedSequenceStepProperties stepProperties)
        {
            var properties = new HashSet<DCPowerAdvancedSequenceProperty>();

            if (stepProperties.Autorange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.Autorange);
            }
            if (stepProperties.AutorangeApertureTimeMode.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.AutorangeApertureTimeMode);
            }
            if (stepProperties.AutorangeBehavior.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.AutorangeBehavior);
            }
            if (stepProperties.AutorangeMaximumDelayAfterRangeChange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.AutorangeMaximumDelayAfterRangeChange);
            }
            if (stepProperties.AutorangeMinimumApertureTime.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.AutorangeMinimumApertureTime);
            }
            if (stepProperties.AutorangeMinimumCurrentRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.AutorangeMinimumCurrentRange);
            }
            if (stepProperties.AutorangeMinimumVoltageRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.AutorangeMinimumVoltageRange);
            }
            if (stepProperties.DCNoiseRejection.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.DCNoiseRejection);
            }
            if (stepProperties.MeasureRecordLength.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.MeasureRecordLength);
            }
            if (stepProperties.Sense.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.Sense);
            }
            if (stepProperties.VoltageLevel.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.VoltageLevel);
            }
            if (stepProperties.VoltageLevelRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.VoltageLevelRange);
            }
            if (stepProperties.VoltageLimit.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.VoltageLimit);
            }
            if (stepProperties.VoltageLimitHigh.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.VoltageLimitHigh);
            }
            if (stepProperties.VoltageLimitLow.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.VoltageLimitLow);
            }
            if (stepProperties.VoltageLimitRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.VoltageLimitRange);
            }
            if (stepProperties.VoltageCompensationFrequency.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.VoltageCompensationFrequency);
            }
            if (stepProperties.VoltageGainBandwidth.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.VoltageGainBandwidth);
            }
            if (stepProperties.VoltagePoleZeroRatio.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.VoltagePoleZeroRatio);
            }
            if (stepProperties.CurrentLevel.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentLevel);
            }
            if (stepProperties.CurrentLevelRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentLevelRange);
            }
            if (stepProperties.CurrentLimit.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentLimit);
            }
            if (stepProperties.CurrentLimitHigh.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentLimitHigh);
            }
            if (stepProperties.CurrentLimitLow.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentLimitLow);
            }
            if (stepProperties.CurrentLimitRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentLimitRange);
            }
            if (stepProperties.CurrentCompensationFrequency.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentCompensationFrequency);
            }
            if (stepProperties.CurrentGainBandwidth.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentGainBandwidth);
            }
            if (stepProperties.CurrentPoleZeroRatio.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentPoleZeroRatio);
            }
            if (stepProperties.CurrentLevelRisingSlewRate.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentLevelRisingSlewRate);
            }
            if (stepProperties.CurrentLevelFallingSlewRate.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.CurrentLevelFallingSlewRate);
            }
            if (stepProperties.PulseVoltageLevel.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseVoltageLevel);
            }
            if (stepProperties.PulseVoltageLevelRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseVoltageLevelRange);
            }
            if (stepProperties.PulseVoltageLimit.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseVoltageLimit);
            }
            if (stepProperties.PulseVoltageLimitHigh.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseVoltageLimitHigh);
            }
            if (stepProperties.PulseVoltageLimitLow.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseVoltageLimitLow);
            }
            if (stepProperties.PulseVoltageLimitRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseVoltageLimitRange);
            }
            if (stepProperties.PulseBiasVoltageLevel.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasVoltageLevel);
            }
            if (stepProperties.PulseBiasVoltageLimit.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasVoltageLimit);
            }
            if (stepProperties.PulseBiasVoltageLimitHigh.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasVoltageLimitHigh);
            }
            if (stepProperties.PulseBiasVoltageLimitLow.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasVoltageLimitLow);
            }
            if (stepProperties.PulseCurrentLevel.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseCurrentLevel);
            }
            if (stepProperties.PulseCurrentLevelRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseCurrentLevelRange);
            }
            if (stepProperties.PulseCurrentLimit.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseCurrentLimit);
            }
            if (stepProperties.PulseCurrentLimitHigh.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseCurrentLimitHigh);
            }
            if (stepProperties.PulseCurrentLimitLow.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseCurrentLimitLow);
            }
            if (stepProperties.PulseCurrentLimitRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseCurrentLimitRange);
            }
            if (stepProperties.PulseBiasCurrentLevel.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasCurrentLevel);
            }
            if (stepProperties.PulseBiasCurrentLimit.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasCurrentLimit);
            }
            if (stepProperties.PulseBiasCurrentLimitHigh.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasCurrentLimitHigh);
            }
            if (stepProperties.PulseBiasCurrentLimitLow.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasCurrentLimitLow);
            }
            if (stepProperties.PulseOnTime.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseOnTime);
            }
            if (stepProperties.PulseOffTime.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseOffTime);
            }
            if (stepProperties.PulseBiasDelay.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasDelay);
            }
            if (stepProperties.SourceDelay.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.SourceDelay);
            }
            if (stepProperties.SequenceStepDeltaTime.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.SequenceStepDeltaTime);
            }
            if (stepProperties.TransientResponse.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.TransientResponse);
            }
            if (stepProperties.OutputEnabled.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.OutputEnabled);
            }
            if (stepProperties.OutputFunction.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.OutputFunction);
            }
            if (stepProperties.OutputResistance.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.OutputResistance);
            }
            if (stepProperties.ComplianceLimitSymmetry.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.ComplianceLimitSymmetry);
            }
            if (stepProperties.OvpEnabled.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.OvpEnabled);
            }
            if (stepProperties.OvpLimit.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.OvpLimit);
            }
            if (stepProperties.LcrCurrentAmplitude.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrCurrentAmplitude);
            }
            if (stepProperties.LcrVoltageAmplitude.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrVoltageAmplitude);
            }
            if (stepProperties.LcrFrequency.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrFrequency);
            }
            if (stepProperties.LcrDcBiasCurrentLevel.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrDcBiasCurrentLevel);
            }
            if (stepProperties.LcrDcBiasVoltageLevel.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrDcBiasVoltageLevel);
            }
            if (stepProperties.LcrImpedanceRange.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrImpedanceRange);
            }
            if (stepProperties.LcrImpedanceRangeSource.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrImpedanceRangeSource);
            }
            if (stepProperties.LcrMeasurementTime.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrMeasurementTime);
            }
            if (stepProperties.LcrCustomMeasurementTime.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrCustomMeasurementTime);
            }
            if (stepProperties.LcrSourceApertureTime.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrSourceApertureTime);
            }
            if (stepProperties.LcrStimulusFunction.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrStimulusFunction);
            }
            if (stepProperties.LcrDcBiasSource.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrDcBiasSource);
            }
            if (stepProperties.LcrOpenCompensationEnabled.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrOpenCompensationEnabled);
            }
            if (stepProperties.LcrShortCompensationEnabled.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrShortCompensationEnabled);
            }
            if (stepProperties.LcrLoadCompensationEnabled.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.LcrLoadCompensationEnabled);
            }
            if (stepProperties.InstrumentMode.HasValue)
            {
                properties.Add(DCPowerAdvancedSequenceProperty.InstrumentMode);
            }
            return properties.ToArray();
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
                    channelOutput.Measurement.ApertureTime = stepProperties.ApertureTime.Value;
            }, "ApertureTime", modelString);

            TrySetProperty(
                () =>
            {
                if (stepProperties.Autorange.HasValue)
                    channelOutput.Measurement.Autorange = stepProperties.Autorange.Value;
            }, "Autorange", modelString);

            TrySetProperty(
                () =>
            {
                if (stepProperties.Sense.HasValue)
                    channelOutput.Measurement.Sense = stepProperties.Sense.Value;
            }, "Sense", modelString);

            // DC Voltage Properties
            TrySetProperty(
                () =>
            {
                if (stepProperties.VoltageLevel.HasValue)
                    channelOutput.Source.Voltage.VoltageLevel = stepProperties.VoltageLevel.Value;
            }, "VoltageLevel", modelString);

            TrySetProperty(
                () =>
            {
                if (stepProperties.VoltageLevelRange.HasValue)
                    channelOutput.Source.Voltage.VoltageLevelRange = stepProperties.VoltageLevelRange.Value;
            }, "VoltageLevelRange", modelString);

            TrySetProperty(
                () =>
            {
                if (stepProperties.VoltageLimit.HasValue)
                    channelOutput.Source.Voltage.CurrentLimit = stepProperties.VoltageLimit.Value;
            }, "CurrentLimit", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.VoltageLimitRange.HasValue)
                    channelOutput.Source.Voltage.CurrentLimitRange = stepProperties.VoltageLimitRange.Value;
            }, "CurrentLimitRange", modelString);

            // DC Current Properties
            TrySetProperty(() =>
            {
                if (stepProperties.CurrentLevel.HasValue)
                    channelOutput.Source.Current.CurrentLevel = stepProperties.CurrentLevel.Value;
            }, "CurrentLevel", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.CurrentLevelRange.HasValue)
                    channelOutput.Source.Current.CurrentLevelRange = stepProperties.CurrentLevelRange.Value;
            }, "CurrentLevelRange", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.CurrentLimit.HasValue)
                    channelOutput.Source.Current.VoltageLimit = stepProperties.CurrentLimit.Value;
            }, "VoltageLimit", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.CurrentLimitRange.HasValue)
                    channelOutput.Source.Current.VoltageLimitRange = stepProperties.CurrentLimitRange.Value;
            }, "VoltageLimitRange", modelString);

            // Pulse Properties
            TrySetProperty(() =>
            {
                if (stepProperties.PulseVoltageLevel.HasValue)
                    channelOutput.Source.PulseVoltage.VoltageLevel = stepProperties.PulseVoltageLevel.Value;
            }, "PulseVoltageLevel", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.PulseVoltageLevelRange.HasValue)
                    channelOutput.Source.PulseVoltage.VoltageLevelRange = stepProperties.PulseVoltageLevelRange.Value;
            }, "PulseVoltageLevelRange", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.PulseVoltageLimit.HasValue)
                    channelOutput.Source.PulseVoltage.CurrentLimit = stepProperties.PulseVoltageLimit.Value;
            }, "PulseCurrentLimit", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.PulseVoltageLimitRange.HasValue)
                    channelOutput.Source.PulseVoltage.CurrentLimitRange = stepProperties.PulseVoltageLimitRange.Value;
            }, "PulseCurrentLimitRange", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.PulseCurrentLevel.HasValue)
                    channelOutput.Source.PulseCurrent.CurrentLevel = stepProperties.PulseCurrentLevel.Value;
            }, "PulseCurrentLevel", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.PulseCurrentLevelRange.HasValue)
                    channelOutput.Source.PulseCurrent.CurrentLevelRange = stepProperties.PulseCurrentLevelRange.Value;
            }, "PulseCurrentLevelRange", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.PulseCurrentLimit.HasValue)
                    channelOutput.Source.PulseCurrent.VoltageLimit = stepProperties.PulseCurrentLimit.Value;
            }, "PulseVoltageLimit", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.PulseCurrentLimitRange.HasValue)
                    channelOutput.Source.PulseCurrent.VoltageLimitRange = stepProperties.PulseCurrentLimitRange.Value;
            }, "PulseVoltageLimitRange", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.PulseBiasVoltageLevel.HasValue)
                    channelOutput.Source.PulseVoltage.BiasVoltageLevel = stepProperties.PulseBiasVoltageLevel.Value;
            }, "PulseBiasVoltageLevel", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.PulseBiasCurrentLevel.HasValue)
                    channelOutput.Source.PulseCurrent.BiasCurrentLevel = stepProperties.PulseBiasCurrentLevel.Value;
            }, "PulseBiasCurrentLevel", modelString);

            // Source Delay
            TrySetProperty(() =>
            {
                if (stepProperties.SourceDelay.HasValue)
                    channelOutput.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(stepProperties.SourceDelay.Value);
            }, "SourceDelay", modelString);

            // Transient Response
            TrySetProperty(() =>
            {
                if (stepProperties.TransientResponse.HasValue)
                    channelOutput.Source.TransientResponse = stepProperties.TransientResponse.Value;
            }, "TransientResponse", modelString);

            // Measurement Record Length
            TrySetProperty(() =>
            {
                if (stepProperties.MeasureRecordLength.HasValue)
                    channelOutput.Measurement.Configuration.MeasureRecordLength = stepProperties.MeasureRecordLength.Value;
            }, "MeasureRecordLength", modelString);

            // Output Properties
            TrySetProperty(() =>
            {
                if (stepProperties.OutputEnabled.HasValue)
                    channelOutput.Source.Output.Enabled = stepProperties.OutputEnabled.Value;
            }, "OutputEnabled", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.OutputFunction.HasValue)
                    channelOutput.Source.Output.Function = stepProperties.OutputFunction.Value;
            }, "OutputFunction", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.ComplianceLimitSymmetry.HasValue)
                    channelOutput.Source.ComplianceLimitSymmetry = stepProperties.ComplianceLimitSymmetry.Value;
            }, "ComplianceLimitSymmetry", modelString);

            // OVP Properties
            TrySetProperty(() =>
            {
                if (stepProperties.OvpEnabled.HasValue)
                    channelOutput.OverVoltageProtection.Enabled = stepProperties.OvpEnabled.Value;
            }, "OvpEnabled", modelString);

            TrySetProperty(() =>
            {
                if (stepProperties.OvpLimit.HasValue)
                    channelOutput.OverVoltageProtection.Limit = stepProperties.OvpLimit.Value;
            }, "OvpLimit", modelString);

            // Note: LCR properties and other advanced properties would be set similarly
            // but may require model-specific checks for availability
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
            catch (Exception ex) when (ex is Ivi.Driver.IviCDriverException ||
                                       ex is System.Runtime.InteropServices.COMException ||
                                       ex is NotSupportedException)
            {
                // Log unsupported property - in production use a logging framework
                System.Diagnostics.Debug.WriteLine(
                    $"Property '{propertyName}' is not supported on instrument model '{modelString}'. " +
                    $"Skipping. Error: {ex.Message}");

                // Only rethrow if it's a critical error
                if (ex.Message.Contains("required") || ex.Message.Contains("critical"))
                {
                    throw;
                }
            }
        }

    }
}
