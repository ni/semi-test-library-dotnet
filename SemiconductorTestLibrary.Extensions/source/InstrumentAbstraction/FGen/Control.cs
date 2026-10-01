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
        /// Gets a value indicating whether the current generation is complete for each session in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <returns>
        /// An array of generation status values, one per session in the bundle.
        /// Each element is <c>true</c> if generation is complete for the corresponding session; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If a session is in the idle or committed state, the corresponding element is <c>true</c>.
        /// </remarks>
        public static bool[] IsDone(this FgenSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo) =>
            {
                return sessionInfo.Session.IsDone;
            });
        }

        /// <summary>
        /// Waits until the device is done generating or until the timeout has expired.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="timeout">The maximum time, in milliseconds, to wait for generation to complete. The value must be non-negative.</param>
        /// <remarks>
        /// Call this method after calling <see cref="Initiate"/>.
        /// <para>
        /// Throws exception when the generation initiated by <see cref="Initiate"/> did not complete within the specified timeout for one or more sessions in the bundle,
        /// or the specified <paramref name="timeout"/> value is invalid.
        /// </para>
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">
        /// The generation initiated by <see cref="Initiate"/> did not complete within the specified timeout for one or more sessions in the bundle,
        /// or the specified <paramref name="timeout"/> value is invalid.
        /// </exception>
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