using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument
{
    /// <summary>
    /// Represents information about a custom instrument session.
    /// </summary>
    public class CustomInstrumentSessionInformation : ISessionInformation
    {
        // [Alternate] We can also update the dictory to be "dictionary<InstrumentName, dictionary<ChannelGroupId, SitePinInfo>>" and then store the instrument name and channel group id seperately in the session rather than in the resource name.
        private static readonly Dictionary<string, SitePinInfo> _resourceNameToSitePinDictionary = new Dictionary<string, SitePinInfo>();

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ICustomInstrument Session { get; }

        public IList<SitePinInfo> AssociatedSitePinList { get; } = new List<SitePinInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomInstrumentSessionInformation"/> class.
        /// </summary>
        /// <param name="session">The custom instrument session.</param>
        /// <param name="associatedSitePinList">The associated site pin list.</param>
        public CustomInstrumentSessionInformation(ICustomInstrument session, IList<SitePinInfo> associatedSitePinList)
        {
            Session = session;
            AssociatedSitePinList = associatedSitePinList;
        }

        public CustomInstrumentSessionInformation(ICustomInstrument session)
        {
            Session = session;
            AssociatedSitePinList.Add(_resourceNameToSitePinDictionary[session.ResourceName]);
        }

        public static void GenerateInstrumentChannelToSitePinDictionary(ISemiconductorModuleContext tsmContext, string instrumentTypeId)
        {
            // We should not clear the dictory here, because we may have multiple custom instrument types which share the same dictionary.
            tsmContext.GetPins(instrumentTypeId, out var dutPins, out var systemPins);
            foreach (var dutPin in dutPins)
            {
                foreach (var tsmSiteContext in tsmContext.GetSiteSemiconductorModuleContexts())
                {
                    tsmSiteContext.GetCustomSession(instrumentTypeId, dutPin, out var sessionData, out var channelGroupId, out var channelList);
                    var session = sessionData as ICustomInstrument;
                    if (_resourceNameToSitePinDictionary.ContainsKey(session.ResourceName)) // Optional
                    {
                        _resourceNameToSitePinDictionary.Remove(session.ResourceName);
                    }
                    _resourceNameToSitePinDictionary.Add(session.ResourceName, new SitePinInfo(tsmSiteContext.SiteNumbers.First(), dutPin));
                }
            }
            foreach (var systemPin in systemPins)
            {
                tsmContext.GetCustomSession(instrumentTypeId, systemPin, out var sessionData, out var channelGroupId, out var channelList);
                var session = sessionData as ICustomInstrument;
                _resourceNameToSitePinDictionary.Add(session.ResourceName, new SitePinInfo(0, systemPin));
            }
        }
    }
}
