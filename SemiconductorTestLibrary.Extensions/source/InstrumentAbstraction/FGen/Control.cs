using System;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    ///  Defines methods for waveform control operations.
    /// </summary>
    public static class Control
    {
        /// <summary>
        /// Causes a transition to the committed state.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <remarks>
        /// This method verifies driver attribute values, reserves the device, and commits the attribute values to the device.
        /// If the attribute values are all valid, NI-FGEN sets the device hardware configuration to match the session configuration.
        /// <para>
        /// In the committed state, you can load waveforms, scripts, and sequences into memory.
        /// If any driver attributes are changed, NI-FGEN implicitly transitions back to the idle state, where you can program all session properties before applying them to the device.
        /// This method has no effect if the device is already in the committed or generating state.
        /// </para>
        /// </remarks>
        public static void Commit(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Commit();
            });
        }

        /// <summary>
        /// Initiates signal generation.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <remarks>
        /// If you want to abort signal generation, call <see cref="Abort"/>.
        /// After the signal generation is aborted, you can call <see cref="Initiate"/> to cause the signal generator to produce a signal again.
        /// </remarks>
        public static void Initiate(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.InitiateGeneration();
            });
        }

        /// <summary>
        /// Gets a value indicating whether the current generation is complete.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <remarks>
        /// If the session is in the idle or committed states, this property returns 'True'.
        /// </remarks>
        public static bool[] IsDone(this FgenSessionsBundle sessionsBundle)
        {
            // Returning array of bool for each session in the bundle, indicating whether each session is done.
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo) =>
            {
                return sessionInfo.Session.IsDone;
            });
        }

        /// <summary>
        /// Waits until the device is done generating or until the timeout has expired.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="timeout">Max wait time in milliseconds.</param>
        /// <remarks>
        /// Call this method after calling <see cref="Initiate"/>.
        /// </remarks>
        public static void WaitUntilDone(this FgenSessionsBundle sessionsBundle, int timeout = 10000)
        {
            TimeSpan timeoutSpan = TimeSpan.FromMilliseconds(timeout);
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.WaitUntilDone(timeoutSpan);
            });
         }

        /// <summary>
        /// Aborts any previously initiated signal generation.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <remarks>
        /// Call <see cref="Initiate"/> to cause the signal generator to produce a signal again.
        /// </remarks>
        public static void Abort(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.AbortGeneration();
            });
        }
    }
}
