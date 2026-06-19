using System;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;

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
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="enable">The enable state.</param>
        public static void OutputEnable(this FgenSessionsBundle sessionsBundle, bool enable)
        { }

        /// <summary>
        /// Output enable.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="enable">The enable state.</param>
        public static void OutputEnable(this FgenSessionsBundle sessionsBundle, SiteData<bool> enable)
        { }

        /// <summary>
        /// Output enable.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="enable">The enable state.</param>
        public static void OutputEnable(this FgenSessionsBundle sessionsBundle, PinSiteData<bool> enable)
        { }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="impedance">The impedance value.</param>
        public static void OutputImpedance(this FgenSessionsBundle sessionsBundle, double impedance = 50)
        { }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="impedance">The impedance value.</param>
        public static void OutputImpedance(this FgenSessionsBundle sessionsBundle, SiteData<double> impedance)
        { }

        /// <summary>
        /// Output Impedence.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="impedance">The impedance value.</param>
        public static void OutputImpedance(this FgenSessionsBundle sessionsBundle, PinSiteData<double> impedance)
        { }

        /// <summary>
        /// Configure Active Channel.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <remarks>
        /// Active channel should be configured if session is opened for whole device instead of specific channel. All the control operations called after that are applied to the active channel.
        /// </remarks>
        public static void ConfigureChannel(this FgenSessionsBundle sessionsBundle)
        { }
    }
}
