using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery.QueryForRaisedAlarmsWrappers;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery
{
    /// <summary>
    /// Extension methods to wrap publish calls to query for raised alarms when publishing measurement results.
    /// </summary>
    public static class STLPublishExtensions
    {
        /// <inheritdoc cref="Publish.PublishResults{T}(ISemiconductorModuleContext, PinSiteData{T}, string)"/>
        /// <param name="semiconductorModuleContext"/>
        /// <param name="results"/>
        /// <param name="publishedDataId"/>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        public static void PublishResults<T>(this ISemiconductorModuleContext semiconductorModuleContext, PinSiteData<T> results, string publishedDataId, bool clearAlarm = true)
        {
            WrapMethodWithQueryForRaisedAlarms(semiconductorModuleContext, clearAlarm, () =>
            Publish.PublishResults(semiconductorModuleContext, results, publishedDataId));
        }

        /// <inheritdoc cref="Publish.PublishResults{T}(ISemiconductorModuleContext, SiteData{T}, string, string)"/>
        /// <param name="semiconductorModuleContext"/>
        /// <param name="results"/>
        /// <param name="publishedDataId"/>
        /// <param name="pin"/>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        public static void PublishResults<T>(this ISemiconductorModuleContext semiconductorModuleContext, SiteData<T> results, string publishedDataId, string pin = "", bool clearAlarm = true)
        {
            WrapMethodWithQueryForRaisedAlarms(semiconductorModuleContext, clearAlarm, () =>
            Publish.PublishResults(semiconductorModuleContext, results, publishedDataId, pin));
        }

        /// <inheritdoc cref="Publish.PublishResult{T}(ISemiconductorModuleContext, T, string, string)"/>
        /// <param name="semiconductorModuleContext"/>
        /// <param name="result"/>
        /// <param name="publishedDataId"/>
        /// <param name="pin"/>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        public static void PublishResult<T>(this ISemiconductorModuleContext semiconductorModuleContext, T result, string publishedDataId, string pin = "", bool clearAlarm = true)
        {
            WrapMethodWithQueryForRaisedAlarms(semiconductorModuleContext, clearAlarm, () =>
            Publish.PublishResult(semiconductorModuleContext, result, publishedDataId, pin));
        }

        /// <inheritdoc cref="Publish.PublishSingleSiteResult{T}(ISemiconductorModuleContext, T, string, string)"/>
        /// <param name="singleSiteSemiconductorModuleContext"/>
        /// <param name="result"/>
        /// <param name="publishedDataId"/>
        /// <param name="pin"/>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        public static void PublishSingleSiteResult<T>(this ISemiconductorModuleContext singleSiteSemiconductorModuleContext, T result, string publishedDataId, string pin = "", bool clearAlarm = true)
        {
            WrapMethodWithQueryForRaisedAlarms(singleSiteSemiconductorModuleContext, clearAlarm, () =>
            Publish.PublishSingleSiteResult(singleSiteSemiconductorModuleContext, result, publishedDataId, pin));
        }

        /// <inheritdoc cref="Publish.PublishResults{TSessionInformation, TData}(ISessionsBundle{TSessionInformation}, TData[][], string)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="results"/>
        /// <param name="publishedDataId"/>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        public static void PublishResults<TSessionInformation, TData>(this ISessionsBundle<TSessionInformation> sessionsBundle, TData[][] results, string publishedDataId, bool clearAlarm = true)
        {
            WrapMethodWithQueryForRaisedAlarms(sessionsBundle.TSMContext, clearAlarm, () =>
            Publish.PublishResults(sessionsBundle, results, publishedDataId));
        }

        /// <inheritdoc cref="Publish.PublishResults{TSessionInformation, TData}(ISessionsBundle{TSessionInformation}, TData[], string)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="results"/>
        /// <param name="publishedDataId"/>
        /// <param name="clearAlarm">A boolean indicating whether to clear the alarm.</param>
        public static void PublishResults<TSessionInformation, TData>(this ISessionsBundle<TSessionInformation> sessionsBundle, TData[] results, string publishedDataId, bool clearAlarm = true)
        {
            WrapMethodWithQueryForRaisedAlarms(sessionsBundle.TSMContext, clearAlarm, () =>
            Publish.PublishResults(sessionsBundle, results, publishedDataId));
        }
    }
}
