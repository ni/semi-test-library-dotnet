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
        /// Causes a transition of the underlying device(s) to the committed state.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <remarks>
        /// This method verifies attribute values, reserves the device(s), and commits the attribute values to the device(s) for each session in the bundle.
        /// If the attribute values are all valid, the hardware configuration of each underlying device is set to match the configuration of its corresponding session in the bundle.
        /// <para>
        /// If any session properties are changed afterwards, the underlying device(s) implicitly transition back to the idle state, where you can program all session properties before applying them to the device(s).
        /// </para>
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">
        /// The operation cannot be completed because the device is not configurable while it is generating a signal.
        /// </exception>
        public static void Commit(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Commit();
            });
        }

        /// <summary>
        /// Initiates signal generation on the underlying device(s).
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <remarks>
        /// If you want to abort signal generation, call <see cref="Abort"/>.
        /// After the signal generation is aborted, you can call <see cref="Initiate"/> to cause the signal generator to produce a signal again.
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">"
        /// The operation cannot be completed because the device is not configurable while it is generating a signal.
        /// </exception>
        public static void Initiate(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.InitiateGeneration();
            });
        }

        /// <summary>
        /// Gets a value indicating whether the current generation is complete for each device(s) in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <returns>
        /// An array of generation status values, one per device in the bundle.
        /// Each element is <c>true</c> if generation is complete for the corresponding device; otherwise, <c>false</c>.
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
        /// Waits until all underlying device(s) are done generating or until the timeout has expired.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="timeout">
        /// The maximum time, in milliseconds, to wait for generation to complete.
        /// The value must be non-negative, or -1 to wait indefinitely.
        /// </param>
        /// <remarks>
        /// Call this method after calling <see cref="Initiate"/>.
        /// <para>
        /// If <paramref name="timeout"/> is set to -1, this method waits indefinitely until generation is complete for all sessions in the bundle.
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
        /// Aborts any previously initiated signal generation on the underlying device(s).
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