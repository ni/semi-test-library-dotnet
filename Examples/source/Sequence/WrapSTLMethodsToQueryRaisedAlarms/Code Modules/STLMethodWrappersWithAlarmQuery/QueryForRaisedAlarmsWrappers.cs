using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery
{
    internal static class QueryForRaisedAlarmsWrappers
    {
        /// <summary>
        /// Wraps a method call to query for raised alarms before and after the target method call.
        /// If any alarms are raised, an exception is thrown with the details of the raised alarms.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        /// <param name="method">The method to be wrapped.</param>
        /// <exception cref="AlarmOccurredException">Thrown when an alarm occurrence has been detected.</exception>
        internal static void WrapMethodToQueryForRaisedAlarmsBeforeAndAfterInvoke(ISemiconductorModuleContext semiconductorModuleContext, bool clearAlarm, System.Action method)
        {
            string details;
            if (semiconductorModuleContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new AlarmOccurredException(
                    $"Alarm(s) raised before the measurement. Details: {details}");
            }
            method();
            if (semiconductorModuleContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new AlarmOccurredException(
                    $"Alarm(s) raised after the measurement. Details: {details}");
            }
        }

        /// <inheritdoc cref="WrapMethodToQueryForRaisedAlarmsBeforeAndAfterInvoke(ISemiconductorModuleContext, bool, System.Action)"/>
        /// <typeparam name="TReturn">The return type of the method being wrapped.</typeparam>
        /// <param name="semiconductorModuleContext"/>
        /// <param name="clearAlarm"/>
        /// <param name="method"/>
        /// <returns>The result of the wrapped method.</returns>
        internal static TReturn WrapMethodToQueryForRaisedAlarmsBeforeAndAfterInvoke<TReturn>(ISemiconductorModuleContext semiconductorModuleContext, bool clearAlarm, System.Func<TReturn> method)
        {
            string details;
            if (semiconductorModuleContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new AlarmOccurredException(
                    $"Alarm(s) raised before the measurement. Details: {details}");
            }
            var measurement = method();
            if (semiconductorModuleContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new AlarmOccurredException(
                    $"Alarm(s) raised after the measurement. Details: {details}");
            }
            return measurement;
        }

        /// <summary>
        /// Wraps a method call to query for raised alarms before the target method call.
        /// If any alarms are raised, an exception is thrown with the details of the raised alarms.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        /// <param name="method">The method to be wrapped.</param>
        /// <exception cref="AlarmOccurredException">Thrown when an alarm occurrence has been detected.</exception>
        internal static void WrapMethodToQueryForRaisedAlarmsBeforeInvoke(ISemiconductorModuleContext semiconductorModuleContext, bool clearAlarm, System.Action method)
        {
            string details;
            if (semiconductorModuleContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new AlarmOccurredException(
                    $"Alarm(s) raised before the measurement. Details: {details}");
            }
            method();
        }

        /// <inheritdoc cref="WrapMethodToQueryForRaisedAlarmsBeforeInvoke(ISemiconductorModuleContext, bool, System.Action)"/>
        /// <typeparam name="TReturn">The return type of the method being wrapped.</typeparam>
        /// <param name="semiconductorModuleContext"/>
        /// <param name="clearAlarm"/>
        /// <param name="method"/>
        /// <returns>The result of the wrapped method.</returns>
        internal static TReturn WrapMethodToQueryForRaisedAlarmsBeforeInvoke<TReturn>(ISemiconductorModuleContext semiconductorModuleContext, bool clearAlarm, System.Func<TReturn> method)
        {
            string details;
            if (semiconductorModuleContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new AlarmOccurredException(
                    $"Alarm(s) raised before the measurement. Details: {details}");
            }
            return method();
        }

        /// <summary>
        /// Wraps a method call to query for raised alarms after the target method call.
        /// If any alarms are raised, an exception is thrown with the details of the raised alarms.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        /// <param name="method">The method to be wrapped.</param>
        /// <exception cref="AlarmOccurredException">Thrown when an alarm occurrence has been detected.</exception>
        internal static void WrapMethodToQueryForRaisedAlarmsAfterInvoke(ISemiconductorModuleContext semiconductorModuleContext, bool clearAlarm, System.Action method)
        {
            string details;
            method();
            if (semiconductorModuleContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new AlarmOccurredException(
                    $"Alarm(s) raised after the measurement. Details: {details}");
            }
        }

        /// <inheritdoc cref="WrapMethodToQueryForRaisedAlarmsAfter(ISemiconductorModuleContext, bool, System.Action)"/>
        /// <typeparam name="TReturn">The return type of the method being wrapped.</typeparam>
        /// <param name="semiconductorModuleContext"/>
        /// <param name="clearAlarm"/>
        /// <param name="method"/>
        /// <returns>The result of the wrapped method.</returns>
        internal static TReturn WrapMethodToQueryForRaisedAlarmsAfterInvoke<TReturn>(ISemiconductorModuleContext semiconductorModuleContext, bool clearAlarm, System.Func<TReturn> method)
        {
            string details;
            var measurement = method();
            if (semiconductorModuleContext.QueryForRaisedAlarms(clearAlarm, out details))
            {
                throw new AlarmOccurredException(
                    $"Alarm(s) raised after the measurement. Details: {details}");
            }
            return measurement;
        }
    }
}
