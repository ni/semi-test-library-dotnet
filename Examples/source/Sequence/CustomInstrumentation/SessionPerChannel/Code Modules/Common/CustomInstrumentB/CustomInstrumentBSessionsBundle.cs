using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannel.Common.CustomInstrumentB
{
    /// <summary>
    /// Defines an object that contains one or more <see cref="CustomInstrumentB"/> objects.
    /// </summary>
    public class CustomInstrumentBSessionsBundle : ISessionsBundle<CustomInstrumentBSessionInformation>
    {
        /// <inheritdoc/>
        public ISemiconductorModuleContext TSMContext { get; }

        /// <inheritdoc/>
        public IEnumerable<CustomInstrumentBSessionInformation> InstrumentSessions { get; }

        /// <inheritdoc/>
        public IList<SitePinInfo> AggregateSitePinList { get; } = new List<SitePinInfo>();

        /// <summary>
        /// Constructs a sessions bundle object that represents a bunch of <see cref="CustomInstrumentBSessionInformation"/>s.
        /// </summary>
        /// <param name="tsmContext">The associated <see cref="ISemiconductorModuleContext"/>.</param>
        /// <param name="allSessionInfo">An enumerable of <see cref="CustomInstrumentBSessionInformation"/>s.</param>
        public CustomInstrumentBSessionsBundle(ISemiconductorModuleContext tsmContext, IEnumerable<CustomInstrumentBSessionInformation> allSessionInfo)
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
        /// Filters current <see cref="CustomInstrumentBSessionsBundle"/> and returns a new one with the requested site.
        /// </summary>
        /// <param name="requestedSite">The requested site.</param>
        /// <returns>A new <see cref="CustomInstrumentBSessionsBundle"/> object with the requested site.</returns>
        public CustomInstrumentBSessionsBundle FilterBySite(int requestedSite)
        {
            return FilterBySite(new[] { requestedSite });
        }

        /// <summary>
        /// Filters current <see cref="CustomInstrumentBSessionsBundle"/> and returns a new one with requested sites.
        /// </summary>
        /// <param name="requestedSites">The requested sites.</param>
        /// <returns>A new <see cref="CustomInstrumentBSessionsBundle"/> object with requested sites.</returns>
        public CustomInstrumentBSessionsBundle FilterBySite(int[] requestedSites)
        {
            return new CustomInstrumentBSessionsBundle(TSMContext.GetSemiconductorModuleContextWithSites(requestedSites), Filter(
                InstrumentSessions,
                includeSitePinPredicate: sitePin => requestedSites.Contains(sitePin.SiteNumber),
                createSessionInformation: (sessionInfo, filteredSitePinList) => new CustomInstrumentBSessionInformation(sessionInfo.Session, filteredSitePinList)));
        }

        /// <summary>
        /// Filter current <see cref="CustomInstrumentBSessionsBundle"/> and returns a new one with the requested pin.
        /// </summary>
        /// <param name="requestedPin">The requested pin.</param>
        /// <returns>A new <see cref="CustomInstrumentBSessionsBundle"/> object with the requested pin.</returns>
        public CustomInstrumentBSessionsBundle FilterByPin(string requestedPin)
        {
            return FilterByPin(new[] { requestedPin });
        }

        /// <summary>
        /// Filters current <see cref="CustomInstrumentBSessionsBundle"/> and returns a new one with requested pins.
        /// </summary>
        /// <param name="requestedPins">The requested pins.</param>
        /// <returns>A new <see cref="CustomInstrumentBSessionsBundle"/> object with requested pins.</returns>
        public CustomInstrumentBSessionsBundle FilterByPin(string[] requestedPins)
        {
            ValidatePins(AggregateSitePinList, requestedPins);
            return new CustomInstrumentBSessionsBundle(TSMContext, Filter(
                InstrumentSessions,
                includeSitePinPredicate: sitePin => requestedPins.Contains(sitePin.PinName),
                createSessionInformation: (sessionInfo, filteredSitePinList) => new CustomInstrumentBSessionInformation(sessionInfo.Session, filteredSitePinList)));
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
