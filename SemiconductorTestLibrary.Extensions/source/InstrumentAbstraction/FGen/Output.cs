using System;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Output class for controlling singnal output.
    /// </summary>
    public static class Output
    {
        /// <summary>
        /// Output enable.
        /// </summary>
        /// <param name="enable">The enable state.</param>
        public static void OutputEnable(bool enable)
        { }

        /// <summary>
        /// Output enable.
        /// </summary>
        /// <param name="enable">The enable state.</param>
        public static void OutputEnable(SiteData<bool> enable)
        { }

        /// <summary>
        /// Output enable.
        /// </summary>
        /// <param name="enable">The enable state.</param>
        public static void OutputEnable(PinSiteData<bool> enable)
        { }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="impedance">The impedance value.</param>
        public static void OutputImpedance(double impedance = 50)
        { }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="impedance">The impedance value.</param>
        public static void OutputImpedance(SiteData<double> impedance)
        { }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="impedance">The impedance value.</param>
        public static void OutputImpedance(PinSiteData<double> impedance)
        { }
    }
}
