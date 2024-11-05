using System.Globalization;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    /// <summary>
    /// This file contains shared methods and other definitions used by multiple TestStand steps.
    /// </summary>
    public static partial class CommonSteps
    {
        private static void VerifySizeOfArrayInputs(string arrayNames, string[] pinsOrPinGroups, params double[][] perPinOrPinGroupValues)
        {
            var valuesArraysDistinctSizes = perPinOrPinGroupValues.Select(item => item.Length).Distinct();
            if (valuesArraysDistinctSizes.Count() != 1 || valuesArraysDistinctSizes.Single() != pinsOrPinGroups.Length)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Parameter_ArraySizeMismatch, arrayNames));
            }
        }
    }

    /// <summary>
    /// Defines NI instrument types the NI Semiconductor Test Library supports.
    /// </summary>
    public enum NIInstrumentType
    {
        /// <summary>
        /// All NI instruments.
        /// </summary>
        All,

        /// <summary>
        /// An NI-DCPower instrument.
        /// </summary>
        NIDCPower,

        /// <summary>
        /// An NI-Digital Pattern instrument.
        /// </summary>
        NIDigitalPattern,

        /// <summary>
        /// A relay driver module (NI-SWITCH instrument).
        /// </summary>
        NIRelayDriver,

        /// <summary>
        /// An NI-DAQmx task.
        /// </summary>
        NIDAQmx,

        /// <summary>
        /// An NI-DMM instrument.
        /// </summary>
        NIDMM,

        /// <summary>
        /// An NI-FGEN instrument.
        /// </summary>
        NIFGen,

        /// <summary>
        /// An NI-SCOPE instrument.
        /// </summary>
        NIScope,

        /// <summary>
        /// An NI-Sync instrument.
        /// </summary>
        NISync
    }
}
