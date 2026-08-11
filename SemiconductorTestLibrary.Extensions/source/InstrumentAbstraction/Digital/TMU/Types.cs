using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.TMU;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// TMU Arm settings
    /// </summary>
    public abstract class TmuArmSettings
    {
        /// <summary>Prevents derivation outside this assembly.</summary>
        internal TmuArmSettings()
        {
        }

        /// <summary>Defines the type of signal used to arm the TMU measurement.</summary>
        public abstract TmuArmType ArmType { get; }

        /// <summary>Applies these arm settings to the specified TMU.</summary>
        /// <param name="tmu">The <see cref="DigitalTmu"/> object.</param>
        /// <param name="source">The arm source terminal.</param>
        internal abstract void ApplyTo(DigitalTmu tmu, string source);

        /// <summary>Creates arm settings that arm each sample immediately.</summary>
        public static TmuArmSettings Immediate()
        {
            return new TmuImmediateArmSettings();
        }

        /// <summary>Creates arm settings that arm each sample on an edge of the specified source.</summary>
        /// <param name="source">The arm source terminal.</param>
        /// <param name="sourceEvent">The digital event used to arm the TMU. When <c>null</c>, the driver default is used.</param>
        /// <param name="polarity">The edge polarity of the arm input. When <c>null</c>, the driver default is used.</param>
        public static TmuArmSettings Edge(string source, TmuSourceEvent? sourceEvent = null, TmuPolarity? polarity = null)
        {
            return new TmuEdgeArmSettings(source, sourceEvent, polarity);
        }
    }

    /// <summary>
    /// Defines the TMU arm settings used to arm each sample immediately.
    /// </summary>
    public sealed class TmuImmediateArmSettings : TmuArmSettings
    {
        internal TmuImmediateArmSettings()
        {
        }

        /// <inheritdoc/>
        public override TmuArmType ArmType => TmuArmType.Immediate;

        /// <inheritdoc/>
        internal override void ApplyTo(DigitalTmu tmu, string source)
        {
            tmu.ArmType = TmuArmType.Immediate;
        }
    }

    /// <summary>
    /// Defines the TMU arm settings used to arm each sample on an edge of a specified source.
    /// </summary>
    public sealed class TmuEdgeArmSettings : TmuArmSettings
    {
        internal TmuEdgeArmSettings(string sourcePin, TmuSourceEvent? sourceEvent, TmuPolarity? polarity)
        {
            if (string.IsNullOrWhiteSpace(sourcePin))
            {
                throw new ArgumentException("The TMU edge arm source cannot be null or empty.", nameof(sourcePin));
            }
            SourcePin = sourcePin;
            SourceEvent = sourceEvent;
            Polarity = polarity;
        }

        /// <inheritdoc/>
        public override TmuArmType ArmType => TmuArmType.Edge;

        /// <summary>Defines the arm source terminal.</summary>
        public string SourcePin { get; }

        /// <summary>Defines the digital event used to arm the TMU. When <c>null</c>, the driver default is used.</summary>
        public TmuSourceEvent? SourceEvent { get; }

        /// <summary>Defines the edge polarity of the arm input. When <c>null</c>, the driver default is used.</summary>
        public TmuPolarity? Polarity { get; }

        /// <inheritdoc/>
        internal override void ApplyTo(DigitalTmu tmu, string source)
        {
            tmu.ArmType = TmuArmType.Edge;
            tmu.EdgeArm.Source = source;
            if (SourceEvent.HasValue)
            {
                tmu.EdgeArm.SourceEvent = SourceEvent.Value;
            }
            if (Polarity.HasValue)
            {
                tmu.EdgeArm.Polarity = Polarity.Value;
            }
        }
    }
}
