using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.CustomInstrument.Common
{
    /// <summary>
    /// Defines an object that contains one or more <see cref="MyCustomInstrument"/> objects.
    /// </summary>
    public class MyCustomInstrumentSessionsBundle : ISessionsBundle<MyCustomInstrumentSessionInformation>
    {
        /// <inheritdoc/>
        public ISemiconductorModuleContext TSMContext { get; }

        /// <inheritdoc/>
        public IEnumerable<MyCustomInstrumentSessionInformation> InstrumentSessions { get; }

        /// <inheritdoc/>
        public IList<SitePinInfo> AggregateSitePinList { get; } = new List<SitePinInfo>();

        /// <summary>
        /// Constructs a sessions bundle object that represents a bunch of <see cref="MyCustomInstrumentSessionInformation"/>s.
        /// </summary>
        /// <param name="tsmContext">The associated <see cref="ISemiconductorModuleContext"/>.</param>
        /// <param name="allSessionInfo">An enumerable of <see cref="MyCustomInstrumentSessionInformation"/>s.</param>
        public MyCustomInstrumentSessionsBundle(ISemiconductorModuleContext tsmContext, IEnumerable<MyCustomInstrumentSessionInformation> allSessionInfo)
        {
            TSMContext = tsmContext;
            InstrumentSessions = allSessionInfo;
            allSessionInfo.SafeForEach(sessionInfo => AggregateSitePinList.AddRange(sessionInfo.AssociatedSitePinList));
        }

        /// <summary>
        /// Filters current <see cref="MyCustomInstrumentSessionsBundle"/> and returns a new one with the requested site.
        /// </summary>
        /// <param name="requestedSite">The requested site.</param>
        /// <returns>A new <see cref="MyCustomInstrumentSessionsBundle"/> object with the requested site.</returns>
        public MyCustomInstrumentSessionsBundle FilterBySite(int requestedSite)
        {
            return FilterBySite(new[] { requestedSite });
        }

        /// <summary>
        /// Filters current <see cref="MyCustomInstrumentSessionsBundle"/> and returns a new one with requested sites.
        /// </summary>
        /// <param name="requestedSites">The requested sites.</param>
        /// <returns>A new <see cref="MyCustomInstrumentSessionsBundle"/> object with requested sites.</returns>
        public MyCustomInstrumentSessionsBundle FilterBySite(int[] requestedSites)
        {
            return new MyCustomInstrumentSessionsBundle(TSMContext.GetSemiconductorModuleContextWithSites(requestedSites), InstrumentSessions.Filter(
                includeSitePinPredicate: sitePin => requestedSites.Contains(sitePin.SiteNumber),
                createSessionInformation: (sessionInfo, filteredSitePinList) => new MyCustomInstrumentSessionInformation(sessionInfo.Session, filteredSitePinList)));
        }

        /// <summary>
        /// Filter current <see cref="MyCustomInstrumentSessionsBundle"/> and returns a new one with the requested pin.
        /// </summary>
        /// <param name="requestedPin">The requested pin.</param>
        /// <returns>A new <see cref="MyCustomInstrumentSessionsBundle"/> object with the requested pin.</returns>
        public MyCustomInstrumentSessionsBundle FilterByPin(string requestedPin)
        {
            return FilterByPin(new[] { requestedPin });
        }

        /// <summary>
        /// Filters current <see cref="MyCustomInstrumentSessionsBundle"/> and returns a new one with requested pins.
        /// </summary>
        /// <param name="requestedPins">The requested pins.</param>
        /// <returns>A new <see cref="MyCustomInstrumentSessionsBundle"/> object with requested pins.</returns>
        public MyCustomInstrumentSessionsBundle FilterByPin(string[] requestedPins)
        {
            ValidatePins(AggregateSitePinList, requestedPins);
            return new MyCustomInstrumentSessionsBundle(TSMContext, InstrumentSessions.Filter(
                includeSitePinPredicate: sitePin => requestedPins.Contains(sitePin.PinName),
                createSessionInformation: (sessionInfo, filteredSitePinList) => new MyCustomInstrumentSessionInformation(sessionInfo.Session, filteredSitePinList)));
        }

        private static void ValidatePins(IList<SitePinInfo> aggregateSitePinList, string[] requestedPins)
        {
            var invalidPins = requestedPins.Except(aggregateSitePinList.Select(sitePin => sitePin.PinName));
            if (invalidPins.Any())
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, "SitePinFilter_RequestedPinsInvalid", string.Join(", ", invalidPins)));
            }
        }
    }
}
