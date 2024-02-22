using NationalInstruments.ModularInstruments.NIDCPower;

namespace NationalInstruments.MixedSignalLibrary
{
    /// <summary>
    /// Defines custom data types.
    /// </summary>
    public static class TypeDefinitions
    {
        /// <summary>
        /// Defines what to measure, just voltage, just current, or both of them.
        /// </summary>
        public enum MeasurementType
        {
            /// <summary>
            /// Measure voltage
            /// </summary>
            Voltage,

            /// <summary>
            /// Measure current
            /// </summary>
            Current,

            /// <summary>
            /// Measure both voltage and current
            /// </summary>
            Both
        }

        internal sealed class MixedSignalParameters
        {
            public double[] Supplies { get; }
            public double[] Limits { get; }
            public double[] ApertureTimes { get; }
            public double[] SettlingTimes { get; }
            public DCPowerMeasurementSense[] DCPowerMeasurementSenses { get; }
            public DCPowerSourceTransientResponse[] DCPowerSourceTransientResponses { get; }

            internal MixedSignalParameters()
            {
            }

            public MixedSignalParameters(
                double[] supplies,
                double[] limits,
                double[] apertureTimes,
                double[] settlingTimes,
                DCPowerMeasurementSense[] dcPowerMeasurementSenses,
                DCPowerSourceTransientResponse[] dcPowerSourceTransientResponses)
            {
                Supplies = supplies;
                Limits = limits;
                ApertureTimes = apertureTimes;
                SettlingTimes = settlingTimes;
                DCPowerMeasurementSenses = dcPowerMeasurementSenses;
                DCPowerSourceTransientResponses = dcPowerSourceTransientResponses;
            }

            public MixedSignalParameters(
                double[] supplies,
                double[] limits,
                double[] apertureTimes,
                double[] settlingTimes,
                DCPowerMeasurementSense[] dcPowerMeasurementSenses,
                DCPowerSourceTransientResponse[] dcPowerSourceTransientResponses,
                int expectedSize)
                : this(supplies, limits, apertureTimes, settlingTimes, dcPowerMeasurementSenses, dcPowerSourceTransientResponses)
            {
                GeneralUtilities.VerifySizeOfArrayParameters(
                    expectedSize,
                    supplies,
                    limits,
                    apertureTimes,
                    settlingTimes,
                    dcPowerMeasurementSenses,
                    dcPowerSourceTransientResponses);
            }
        }
    }
}
