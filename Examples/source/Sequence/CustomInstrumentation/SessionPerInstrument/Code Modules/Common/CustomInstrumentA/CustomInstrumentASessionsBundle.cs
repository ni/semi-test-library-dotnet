using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerInstrument.Common.CustomInstrumentA
{
    /// <summary>
    /// Defines an object that contains one or more <see cref="CustomInstrumentA"/> objects.
    /// </summary>
    public class CustomInstrumentASessionsBundle : ISessionsBundle<CustomInstrumentASessionInformation>
    {
        /// <inheritdoc/>
        public ISemiconductorModuleContext TSMContext { get; }

        /// <inheritdoc/>
        public IEnumerable<CustomInstrumentASessionInformation> InstrumentSessions { get; }

        /// <inheritdoc/>
        public IList<SitePinInfo> AggregateSitePinList { get; } = new List<SitePinInfo>();

        /// <summary>
        /// Constructs a sessions bundle object that represents a bunch of <see cref="CustomInstrumentASessionInformation"/>s.
        /// </summary>
        /// <param name="tsmContext">The associated <see cref="ISemiconductorModuleContext"/>.</param>
        /// <param name="allSessionInfo">An enumerable of <see cref="CustomInstrumentASessionInformation"/>s.</param>
        public CustomInstrumentASessionsBundle(ISemiconductorModuleContext tsmContext, IEnumerable<CustomInstrumentASessionInformation> allSessionInfo)
        {
            TSMContext = tsmContext;
            InstrumentSessions = allSessionInfo;
            foreach (var sessionInfo in allSessionInfo)
            {
                foreach (var sitePinInfo in sessionInfo.AssociatedSitePinList)
                {
                    AggregateSitePinList.Add(sitePinInfo);
                }
            }
        }

        /// <summary>
        /// Filters current <see cref="CustomInstrumentASessionsBundle"/> and returns a new one with the requested site.
        /// </summary>
        /// <param name="requestedSite">The requested site.</param>
        /// <returns>A new <see cref="CustomInstrumentASessionsBundle"/> object with the requested site.</returns>
        public CustomInstrumentASessionsBundle FilterBySite(int requestedSite)
        {
            return FilterBySite(new[] { requestedSite });
        }

        /// <summary>
        /// Filters current <see cref="CustomInstrumentASessionsBundle"/> and returns a new one with requested sites.
        /// </summary>
        /// <param name="requestedSites">The requested sites.</param>
        /// <returns>A new <see cref="CustomInstrumentASessionsBundle"/> object with requested sites.</returns>
        public CustomInstrumentASessionsBundle FilterBySite(int[] requestedSites)
        {
            return new CustomInstrumentASessionsBundle(TSMContext.GetSemiconductorModuleContextWithSites(requestedSites), Filter(
                InstrumentSessions,
                includeSitePinPredicate: sitePin => requestedSites.Contains(sitePin.SiteNumber),
                createSessionInformation: (sessionInfo, filteredSitePinList) => new CustomInstrumentASessionInformation(sessionInfo.Session, filteredSitePinList)));
        }

        /// <summary>
        /// Filter current <see cref="CustomInstrumentASessionsBundle"/> and returns a new one with the requested pin.
        /// </summary>
        /// <param name="requestedPin">The requested pin.</param>
        /// <returns>A new <see cref="CustomInstrumentASessionsBundle"/> object with the requested pin.</returns>
        public CustomInstrumentASessionsBundle FilterByPin(string requestedPin)
        {
            return FilterByPin(new[] { requestedPin });
        }

        /// <summary>
        /// Filters current <see cref="CustomInstrumentASessionsBundle"/> and returns a new one with requested pins.
        /// </summary>
        /// <param name="requestedPins">The requested pins.</param>
        /// <returns>A new <see cref="CustomInstrumentASessionsBundle"/> object with requested pins.</returns>
        public CustomInstrumentASessionsBundle FilterByPin(string[] requestedPins)
        {
            ValidatePins(AggregateSitePinList, requestedPins);
            return new CustomInstrumentASessionsBundle(TSMContext, Filter(
                InstrumentSessions,
                includeSitePinPredicate: sitePin => requestedPins.Contains(sitePin.PinName),
                createSessionInformation: (sessionInfo, filteredSitePinList) => new CustomInstrumentASessionInformation(sessionInfo.Session, filteredSitePinList)));
        }

        private static void ValidatePins(IList<SitePinInfo> aggregateSitePinList, string[] requestedPins)
        {
            var invalidPins = requestedPins.Except(aggregateSitePinList.Select(sitePin => sitePin.PinName));
            if (invalidPins.Any())
            {
                throw new NISemiconductorTestException(string.Format(
                    CultureInfo.InvariantCulture,
                    "SitePinFilter_RequestedPinsInvalid",
                    string.Join(", ", invalidPins)));
            }
        }

        private static IEnumerable<TSessionInformation> Filter<TSessionInformation>(
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
