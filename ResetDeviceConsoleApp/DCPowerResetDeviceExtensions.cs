using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;

namespace ResetDeviceConsoleApp
{
    /// <summary>
    /// The DCPower ResetDevice extension, kept local to this research app because the published Extensions project builds
    /// against an Abstractions DLL that does not yet contain <see cref="DCPowerSessionsBundle.ResetDevices(bool)"/>.
    /// </summary>
    internal static class DCPowerResetDeviceExtensions
    {
        /// <summary>
        /// Hard-resets the device(s) behind the bundle. The bundle's own channel groups stay in the reset state; every other
        /// channel group on the reset device(s) is reinitialized and, when requested, has its configuration restored.
        /// </summary>
        public static void ResetDevice(this DCPowerSessionsBundle sessionsBundle, bool restoreConfiguration = true)
        {
            sessionsBundle.ResetDevices(restoreConfiguration);
        }
    }
}
