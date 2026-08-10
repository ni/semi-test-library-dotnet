using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery
{
    internal static class QueryForRaisedAlarmsWrappers
    {
        /// <summary>
        /// Wraps a method call to query for raised alarms before and after the measurement.
        /// If any alarms are raised, an exception is thrown with the details of the raised alarms.
        /// </summary>
        /// <typeparam name="TReturn">The return type of the method being wrapped.</typeparam>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        /// <param name="method">The method to be wrapped.</param>
        /// <returns>The result of the wrapped method.</returns>
        /// <exception cref="NISemiconductorTestException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "NI1704:Identifiers should be spelled correctly", Justification = "TSM valid acronym")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "NI1704:Identifiers should be spelled correctly", Justification = "TSM valid acronym")]
        internal static TReturn WrapMethodWithQueryForRaisedAlarms<TReturn>(ISemiconductorModuleContext tsmContext, bool clearAlarm, System.Func<TReturn> method)
        {
            string details;
            if (tsmContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new NISemiconductorTestException(
                    $"Alarm(s) raised before the measurement. Details: {details}");
            }
            var measurement = method();
            if (tsmContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new NISemiconductorTestException(
                    $"Alarm(s) raised after the measurement. Details: {details}");
            }
            return measurement;
        }

        /// <summary>
        /// Wraps a method call to query for raised alarms before and after the measurement.
        /// If any alarms are raised, an exception is thrown with the details of the raised alarms.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        /// <param name="method">The method to be wrapped.</param>
        /// <exception cref="NISemiconductorTestException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "NI1704:Identifiers should be spelled correctly", Justification = "TSM valid acronym")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "NI1704:Identifiers should be spelled correctly", Justification = "TSM valid acronym")]
        internal static void WrapMethodWithQueryForRaisedAlarms(ISemiconductorModuleContext tsmContext, bool clearAlarm, System.Action method)
        {
            string details;
            if (tsmContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new NISemiconductorTestException(
                    $"Alarm(s) raised before the measurement. Details: {details}");
            }
            method();
            if (tsmContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new NISemiconductorTestException(
                    $"Alarm(s) raised after the measurement. Details: {details}");
            }
        }
    }
}
