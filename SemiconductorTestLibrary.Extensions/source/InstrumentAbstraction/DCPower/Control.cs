using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for controlling the NI-DCPower session.
    /// </summary>
    public static class Control
    {
        #region methods on DCPowerSessionsBundle

        /// <summary>
        /// Transitions the NI-DCPower session from the Running state to the Uncommitted state.
        /// If a sequence is running, the NI-DCPower session is stopped.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <remarks>
        /// Note: This is a lower level function for controlling over the NI-DCPower session.
        /// Any low level driver property updated after this method will not be applied until the next sourcing operation,
        /// or when the Commit method is explicitly called.
        /// If power output is enabled when you call the Abort method, the channels remain in their current state and continue providing power.
        /// Refer to the Programming States topic in the NI-DCPower User Manual and the document of your SMU model for information about the specific NI-DCPower software states.
        /// Use the OutputEnable method to disable power output per channel. Use the Reset method to disable output on all channels.
        /// </remarks>
        public static void Abort(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Abort();
            });
        }

        /// <summary>
        /// Applies previously configured settings to the underlying device channel(s).
        /// Calling this methods moves the underlying channel(s) from the Uncommitted state into the Committed state.
        /// After calling this method, modifying any property reverts the underlying device channel(s) to the Uncommitted state.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <remarks>
        /// Note: This is a lower level function for controlling over the NI-DCPower session.
        /// Refer to the Programming States topic in the NI-DCPower User Manual and the document of your SMU model for information about the specific NI-DCPower software states.
        /// </remarks>
        public static void Commit(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Commit();
            });
        }

        /// <summary>
        /// Starts generation or acquisition and moves the underlying devices channel(s) from the Uncommitted state or Committed state to the Running state.
        /// To return to the Uncommitted state, call the Abort method.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <remarks>
        /// Note: This is a lower level function for controlling over the NI-DCPower session.
        /// Refer to the Programming States topic in the NI-DCPower User Manual and the document of your SMU model for information about the specific NI-DCPower software states.
        /// </remarks>
        public static void Initiate(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Initiate();
            });
        }

        /// <summary>
        /// Initiates the specified advanced sequence on all sessions in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object..</param>
        /// <param name="sequenceName">The name of the advanced sequence to initiate.</param>
        /// <param name="waitForSequenceCompletion"><see langword="true"/> to wait for the sequence to complete before returning; <see langword="false"/> to
        /// return immediately after initiating the sequence.</param>
        /// <param name="sequenceTimeoutInSeconds">The maximum time, in seconds, to wait for the sequence to complete. Used only if <paramref name="waitForSequenceCompletion"/> is <see langword="true"/>. Must be greater then zero.</param>
        public static void InitiateAdvancedSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName;
                if (waitForSequenceCompletion)
                {
                    channelOutput.Events.SourceCompleteEvent.WaitForEvent(PrecisionTimeSpan.FromSeconds(sequenceTimeoutInSeconds));
                }
            });
        }
        #endregion methods on DCPowerSessionsBundle
    }
}
