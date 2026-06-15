using System.Runtime.CompilerServices;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.TMU
{
    /// <summary>
    /// Provides extension methods for <see cref="NIDigital"/> to manage and perform Time Measurement Unit (TMU) operations.
    /// session.
    /// </summary>
    public static class NIDigitalTmuExtensions
    {
        /// <summary>
        /// Gets the TMU contexts of all disabled TMU resources available within the <see cref="NIDigital"/> session.
        /// </summary>
        /// <remarks>
        /// Calling this will cause attribute verification for all TMU attributes if they are not already verified. Ensure the attribute configuration for all TMUs is valid before calling.
        /// </remarks>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <returns>
        /// Returns a comma-separated list of the TMU context strings of all disabled TMUs.  Returns an empty string if all TMUs are enabled.
        /// </returns>
        public static string GetAllDisabledTMUContexts(this NIDigital session)
        {
            var vi = GetSessionHandle(session);
            int status = NativeMethods.GetDisabledTMUContexts(vi, out string tmuContexts);
            CheckStatus(status);
            return tmuContexts;
        }

        /// <summary>
        /// Initiates the TMU measurement for the specified TMU context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method validates TMU configuration, clears the measurement buffers for the specified TMU,
        /// and prepares the hardware for making a TMU measurement.
        /// </para>
        /// <para>
        /// All TMU configuration attributes must be set before calling this method.
        /// The driver validates and commits TMU configuration parameters when this method is called,
        /// not when <c>niDigital_Commit()</c> is called.
        /// </para>
        /// <para>
        /// This method returns immediately; it does not wait for the measurement to complete.
        /// Once a measurement is initiated, any attempt to modify TMU configuration will fail
        /// until the measurement completes or is aborted.
        /// </para>
        /// <para><b>Error Conditions:</b></para>
        /// <list type="bullet">
        /// <item><description>Returns an error if the specified TMU is enabled but has invalid configuration (such as missing or incompatible start/stop sources).</description></item>
        /// <item><description>Returns an error if the specified TMU is not enabled.</description></item>
        /// <item><description>Returns an error if a measurement is already in progress on the specified TMU.</description></item>
        /// <item><description>Returns an error if resource conflicts exist (e.g., hysteresis conflicts on adjacent TMUs).</description></item>
        /// </list>
        /// </remarks>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <param name="tmuContext">A single TMU repeated capability context (e.g., "PXI1Slot2/tmu0").</param>
        public static void TMUInitiate(this NIDigital session, string tmuContext)
        {
            var vi = GetSessionHandle(session);
            int status = NativeMethods.TMUInitiate(vi, tmuContext);
            CheckStatus(status);
        }

        /// <summary>
        /// Aborts any in-progress TMU measurement(s) on the specified TMUs.
        /// </summary>
        /// <remarks>
        /// If no measurement is in progress on the specified TMU, this function has no effect for that TMU.
        /// This function does not affect TMU configuration or reservation state.
        /// </remarks>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <param name="tmuContext">A single TMU repeated capability context (e.g., "PXI1Slot2/tmu0").</param>
        public static void TMUAbort(this NIDigital session, string tmuContext)
        {
            var vi = GetSessionHandle(session);
            int status = NativeMethods.TMUAbort(vi, tmuContext);
            CheckStatus(status);
        }

        /// <summary>
        /// Fetches averaged measurement from the specified TMU. Optionally retrieves minimum and maximum sample values if the corresponding output parameters are not NULL. The function waits up to timeout seconds for the measurement to complete.
        /// </summary>
        /// <remarks>
        /// This function is only allowed if the measurement format for the specified TMU is set to NIDIGITAL_VAL_TMU_AVERAGED_MEASUREMENT(default value).<br/>
        /// If a TMU arm or sample timeout occurs during measurement, or if the measurement was aborted before it completed, an error is returned and no measurement values are provided.
        /// </remarks>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <param name="tmuContext">A single TMU repeated capability context (e.g., "PXI1Slot2/tmu0").</param>
        /// <param name="timeout">Maximum time (in seconds) to wait for the measurement to complete.</param>
        /// <returns>
        /// The averaged measurement value from the specified TMU.
        /// </returns>
        public static double TMUFetchAveragedMeasurement(this NIDigital session, string tmuContext, double timeout)
        {
            var vi = GetSessionHandle(session);
            int status = NativeMethods.TMUFetchAveragedMeasurement(vi, tmuContext, timeout, out double measurement);
            CheckStatus(status);
            return measurement;
        }

        /// <summary>
        /// Sets the specified TMU attribute.
        /// </summary>
        /// <param name="session">The <see cref="NIDigital"/> session.</param>
        /// <param name="channelName">
        /// List of channel names or list of pins.
        /// Do not pass a mix of channel names and pin names.
        /// An empty string denotes all digital pattern instrument channels.
        /// <br/>
        /// Pin names and pin groups apply to all enabled sites, unless the pin name explicitly specifies the site.
        /// You can specify a pin in a specific site using the form siteN/pinName, where N is the site number.
        /// This function ignores pins that are not mapped to the digital pattern instrument.
        /// <br/>
        /// Specify channel names using the form PXI1Slot3/0,2-3 or PXI1Slot3/0,PXI1Slot3/2-3, where PXI1Slot3 is the instrument resource name and 0, 2, 3 are channel names.
        /// To specify channels from multiple instruments, use the form PXI1Slot3/0,PXI1Slot3/2-3,PXI1Slot4/2-3.
        /// The instruments must be in the same chassis.
        /// </param>
        /// <param name="attribute">The ID of an attribute.</param>
        /// <param name="value">The value to which you want to set the attribute; some of the values might not be valid depending on the current settings of the instrument session.</param>
        public static void SetTMUAttribute(this NIDigital session, string channelName, int attribute, int value)
        {
            var vi = GetSessionHandle(session);
            int status = NativeMethods.SetTMUAttribute(vi, channelName, attribute, value);
            CheckStatus(status);
        }

        /// <inheritdoc cref="SetTMUAttribute(NIDigital, string, int, int)"/>
        public static void SetTMUAttribute(this NIDigital session, string channelName, int attribute, long value)
        {
            var vi = GetSessionHandle(session);
            int status = NativeMethods.SetTMUAttribute(vi, channelName, attribute, value);
            CheckStatus(status);
        }

        /// <inheritdoc cref="SetTMUAttribute(NIDigital, string, int, int)"/>
        public static void SetTMUAttribute(this NIDigital session, string channelName, int attribute, bool value)
        {
            var vi = GetSessionHandle(session);
            int status = NativeMethods.SetTMUAttribute(vi, channelName, attribute, value);
            CheckStatus(status);
        }

        /// <inheritdoc cref="SetTMUAttribute(NIDigital, string, int, int)"/>
        public static void SetTMUAttribute(this NIDigital session, string channelName, int attribute, string value)
        {
            var vi = GetSessionHandle(session);
            int status = NativeMethods.SetTMUAttribute(vi, channelName, attribute, value);
            CheckStatus(status);
        }

        /// <inheritdoc cref="SetTMUAttribute(NIDigital, string, int, int)"/>
        public static void SetTMUAttribute(this NIDigital session, string channelName, int attribute, double value)
        {
            var vi = GetSessionHandle(session);
            int status = NativeMethods.SetTMUAttribute(vi, channelName, attribute, value);
            CheckStatus(status);
        }

        private static uint GetSessionHandle(NIDigital session)
        {
            var digitalHandle = session.GetInstrumentHandle();
            return (uint)digitalHandle.DangerousGetHandle();
        }

        private static void CheckStatus(int status, [CallerMemberName] string callerName = "")
        {
            if (status < 0)
            {
                throw new NISemiconductorTestException($"Error in {callerName}: Status code {status}");
            }
        }
    }
}
