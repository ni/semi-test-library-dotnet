using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for controlling the NI-Scope session.
    /// </summary>
    public static class Control
    {
        #region Methods on ScopeSessionsBundle

        /// <summary>
        /// Aborts an in-progress acquisition on all sessions in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        public static void Abort(this ScopeSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Measurement.Abort();
            });
        }

        /// <summary>
        /// Automatically configures all the oscilloscope sessions in the bundle.
        /// <remarks> Oscilloscope senses the input signal and automatically
        /// configures many of the instrument settings. If a signal is detected on a pin,
        /// the driver chooses the smallest available vertical range that is larger than the signal range.
        /// For example, if the signal is a 1.2 Vpk-pk sine wave, and the device supports 1 V and 2 V
        /// vertical ranges, the driver will choose the 2 V vertical range for that channel.
        /// If no signal is found on any analog input pin, a warning is returned and all channels are enabled.
        /// A channel is considered to have a signal present if the signal is at least 10% of the smallest
        /// vertical range available for that channel.
        /// </remarks>
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        public static void AutoSetup(this ScopeSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Measurement.AutoSetup();
            });
        }

        /// <summary>
        /// Commits to hardware all the parameter settings associated with the bundle.
        /// Use this Method if you want a parameter change to be immediately reflected in the hardware.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        public static void Commit(this ScopeSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Measurement.Commit();
            });
        }

        /// <summary>
        /// Initiates a waveform acquisition on all sessions in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        public static void Initiate(this ScopeSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Measurement.Initiate();
            });
        }
        #endregion
    }
}
