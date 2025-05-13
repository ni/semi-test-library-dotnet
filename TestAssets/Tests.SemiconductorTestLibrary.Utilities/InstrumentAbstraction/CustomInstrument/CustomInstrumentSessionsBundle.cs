using System.Collections.Generic;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument
{
    /// <summary>
    /// Represents a bundle of custom instrument sessions cooresponding to a custom instrument type.
    /// </summary>
    public class CustomInstrumentSessionsBundle : ISessionsBundle<CustomInstrumentSessionInformation>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ISemiconductorModuleContext TSMContext { get; }

        public IEnumerable<CustomInstrumentSessionInformation> InstrumentSessions { get; }

        public IList<SitePinInfo> AggregateSitePinList => new List<SitePinInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomInstrumentSessionsBundle"/> class.
        /// </summary>
        /// <param name="tsmContext">The associated <see cref="ISemiconductorModuleContext"/>.</param>
        /// <param name="allSessionInfo">An enumerable of <see cref="DMMSessionInformation"/>s.</param>
        public CustomInstrumentSessionsBundle(ISemiconductorModuleContext tsmContext, IEnumerable<CustomInstrumentSessionInformation> allSessionInfo)
        {
            TSMContext = tsmContext;
            InstrumentSessions = allSessionInfo;
            allSessionInfo.SafeForEach(sessionInfo => AggregateSitePinList.AddRange(sessionInfo.AssociatedSitePinList));
        }
    }
}
