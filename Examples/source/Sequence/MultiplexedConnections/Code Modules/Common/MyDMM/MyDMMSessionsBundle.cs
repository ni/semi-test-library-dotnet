using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.MultiplexedConnections.Common.MyDMM
{
    /// <summary>
    /// Defines an object that contains one or more <see cref="MyDMMSessionInformation"/> objects.
    /// </summary>
    public class MyDMMSessionsBundle : ISessionsBundle<MyDMMSessionInformation>
    {
        /// <inheritdoc/>
        public ISemiconductorModuleContext TSMContext { get; }

        /// <inheritdoc/>
        public IEnumerable<MyDMMSessionInformation> InstrumentSessions { get; }

        /// <inheritdoc/>
        public IList<SitePinInfo> AggregateSitePinList { get; } = new List<SitePinInfo>();

        /// <summary>
        /// Constructs a sessions bundle object that represents a bunch of <see cref="MyDMMSessionInformation"/>s.
        /// </summary>
        /// <param name="tsmContext">The associated <see cref="ISemiconductorModuleContext"/>.</param>
        /// <param name="allSessionInfo">An enumerable of <see cref="MyDMMSessionInformation"/>s.</param>
        public MyDMMSessionsBundle(ISemiconductorModuleContext tsmContext, IEnumerable<MyDMMSessionInformation> allSessionInfo)
        {
            TSMContext = tsmContext;
            InstrumentSessions = allSessionInfo;
            allSessionInfo.SafeForEach(sessionInfo => AggregateSitePinList.AddRange(sessionInfo.AssociatedSitePinList));
        }

        /// <summary>
        /// Filters current <see cref="MyDMMSessionsBundle"/> and returns a new one with the requested site.
        /// </summary>
        /// <param name="requestedSite">The requested site.</param>
        /// <returns>A new <see cref="MyDMMSessionsBundle"/> object with the requested site.</returns>
        public MyDMMSessionsBundle FilterBySite(int requestedSite)
        {
            return FilterBySite(new[] { requestedSite });
        }

        /// <summary>
        /// Filters current <see cref="MyDMMSessionsBundle"/> and returns a new one with requested sites.
        /// </summary>
        /// <param name="requestedSites">The requested sites.</param>
        /// <returns>A new <see cref="MyDMMSessionsBundle"/> object with requested sites.</returns>
        public MyDMMSessionsBundle FilterBySite(int[] requestedSites)
        {
            return new MyDMMSessionsBundle(TSMContext.GetSemiconductorModuleContextWithSites(requestedSites), Filter(
                InstrumentSessions,
                includeSitePinPredicate: sitePin => requestedSites.Contains(sitePin.SiteNumber),
                createSessionInformation: (sessionInfo, filteredSitePinList) => new MyDMMSessionInformation(sessionInfo.Session, filteredSitePinList)));
        }

        /// <summary>
        /// Filter current <see cref="MyDMMSessionsBundle"/> and returns a new one with the requested pin.
        /// </summary>
        /// <param name="requestedPin">The requested pin.</param>
        /// <returns>A new <see cref="MyDMMSessionsBundle"/> object with the requested pin.</returns>
        public MyDMMSessionsBundle FilterByPin(string requestedPin)
        {
            return FilterByPin(new[] { requestedPin });
        }

        /// <summary>
        /// Filters current <see cref="MyDMMSessionsBundle"/> and returns a new one with requested pins.
        /// </summary>
        /// <param name="requestedPins">The requested pins.</param>
        /// <returns>A new <see cref="MyDMMSessionsBundle"/> object with requested pins.</returns>
        public MyDMMSessionsBundle FilterByPin(string[] requestedPins)
        {
            ValidatePins(AggregateSitePinList, requestedPins);
            return new MyDMMSessionsBundle(TSMContext, Filter(
                InstrumentSessions,
                includeSitePinPredicate: sitePin => requestedPins.Contains(sitePin.PinName),
                createSessionInformation: (sessionInfo, filteredSitePinList) => new MyDMMSessionInformation(sessionInfo.Session, filteredSitePinList)));
        }

        private void ValidatePins(IList<SitePinInfo> aggregateSitePinList, string[] requestedPins)
        {
            var invalidPins = requestedPins.Except(aggregateSitePinList.Select(sitePin => sitePin.PinName));
            if (invalidPins.Any())
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, "SitePinFilter_RequestedPinsInvalid", string.Join(", ", invalidPins)));
            }
        }

        private  static IEnumerable<TSessionInformation> Filter<TSessionInformation>(
            IEnumerable<TSessionInformation> instrumentSessions,
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
