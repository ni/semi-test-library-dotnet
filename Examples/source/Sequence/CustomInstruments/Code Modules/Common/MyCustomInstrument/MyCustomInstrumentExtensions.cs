using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using System.Collections.Generic;
using System.Linq;
using System;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace SemiconductorTestLibrary.Examples.CustomInstrument.Common
{
    public static class MyCustomInstrumentExtensions
    {

        public static void Configure(this MyCustomInstrumentSessionsBundle sessionsBundle, double range)
        {
            sessionsBundle.Do(x => x.Session.Configure(range));
        }

        public static PinSiteData<double> Measure(this MyCustomInstrumentSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(x => new[] { x.Session.Measure() });
        }

        internal static IEnumerable<TSessionInformation> Filter<TSessionInformation>(
            this IEnumerable<TSessionInformation> instrumentSessions,
            Func<SitePinInfo, bool> includeSitePinPredicate,
            Func<TSessionInformation, IList<SitePinInfo>, TSessionInformation> createSessionInformation)
        {
            var sessions = new List<TSessionInformation>();
            foreach (var sessionInfo in instrumentSessions)
            {
                var filteredSitePinList = (sessionInfo as ISessionInformation).AssociatedSitePinList.Where(includeSitePinPredicate);
                if (filteredSitePinList.Any())
                {
                    sessions.Add(createSessionInformation(sessionInfo, filteredSitePinList.ToList()));
                }
            }
            return sessions;
        }
    }
}
