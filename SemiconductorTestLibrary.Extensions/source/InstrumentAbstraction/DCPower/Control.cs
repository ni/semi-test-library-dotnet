using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for get set DCPower configurations.
    /// </summary>
    public static class Control
    {
        #region methods on DCPowerSessionsBundle

        /// <summary>
        /// Transitions the NI-DCPower session from the Running state to the Uncommitted state.
        /// If a sequence is running, then the NI-DCPower session is stopped.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <remarks>
        /// Note, this is a lower level function for control over the niDCPower driver session.
        /// Any low level driver property updated after this method would not be applied until a the next sourcing operation,
        /// or by explicitly calling the Commit() method./>.
        /// If power output is enabled when you call the Abort method, the channels remain in their current state and continue providing power.
        /// Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// Use the OutputEnable method to disable power output on a per channel basis. Use the Reset method to disable output on all channels.
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
        /// Note, this is a lower level function for control over the niDCPower driver session.
        /// Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// </remarks>
        public static void Commit(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Commit();
            });
        }

        /// <summary>
        /// Starts generation or acquisition, causing the underlying devices channel(s) to leave the Uncommitted state or Committed state and enter the Running state.
        /// To return to the Uncommitted state, call the Abort method.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <remarks>
        /// Note, this is a lower level function for control over the niDCPower driver session.
        /// Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// </remarks>
        public static void Initiate(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Initiate();
            });
        }
        #endregion methods on DCPowerSessionsBundle
    }
}
