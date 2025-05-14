using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.CustomInstrument.Common
{
    /// <summary>
    /// Defines an object that contains an individual <see cref="MyCustomInstrument"/> session and its related information.
    /// </summary>
    public class MyCustomInstrumentSessionInformation : ISessionInformation
    {
        private static readonly Dictionary<string, IList<SitePinInfo>> _resourceDescriptorToSitePinDictionary = new Dictionary<string, IList<SitePinInfo>>();

        /// <summary>
        /// The <see cref="MyCustomInstrument"/> session.
        /// </summary>
        public MyCustomInstrument Session { get; }

        /// <inheritdoc/>
        public IList<SitePinInfo> AssociatedSitePinList { get; } = new List<SitePinInfo>();

        /// <summary>
        /// Constructs a session information object that associates with a <see cref="NIDmm"/> instrument session.
        /// </summary>
        /// <param name="session">The <see cref="MyCustomInstrument"/> session.</param>
        public MyCustomInstrumentSessionInformation(MyCustomInstrument session)
        {
            Session = session;
            AssociatedSitePinList.Concat(_resourceDescriptorToSitePinDictionary[session.ResourceName]);
        }

        /// <summary>
        /// Constructs a session information object.
        /// </summary>
        /// <param name="session">The <see cref="MyCustomInstrument"/> session.</param>
        /// <param name="associatedSitePinList">The specified subset of channels associated with the session.</param>
        public MyCustomInstrumentSessionInformation(MyCustomInstrument session, IList<SitePinInfo> associatedSitePinList)
        {
            Session = session;
            AssociatedSitePinList = associatedSitePinList;
        }

        /// <summary>
        /// Generates a dictionary that takes a resource descriptor and returns the associated site-pin pair information.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void GenerateResourceDescriptorToSitePinDictionary(ISemiconductorModuleContext tsmContext)
        {
            _resourceDescriptorToSitePinDictionary.Clear();
            tsmContext.GetPins(MyCustomInstrument.InstrumentTypeId, out var dutPins, out var systemPins);

            foreach (var dutPin in dutPins)
            {
                foreach (var tsmSiteContext in tsmContext.GetSiteSemiconductorModuleContexts())
                {
                    tsmSiteContext.GetCustomSession(MyCustomInstrument.InstrumentTypeId, dutPin, out var session, out _, out _);
                    var resourceName = (session as MyCustomInstrument).ResourceName;
                    if (!_resourceDescriptorToSitePinDictionary.ContainsKey(resourceName))
                    {
                        _resourceDescriptorToSitePinDictionary.Add(
                            resourceName,
                            new List<SitePinInfo> { new SitePinInfo(tsmSiteContext.SiteNumbers.First(), dutPin) });
                    }
                    else
                    {
                        _resourceDescriptorToSitePinDictionary[resourceName]
                            .Add(new SitePinInfo(tsmSiteContext.SiteNumbers.First(), dutPin));
                    }
                }
            }
            foreach (var systemPin in systemPins)
            {
                tsmContext.GetCustomSession(MyCustomInstrument.InstrumentTypeId, systemPin, out var session, out _, out _);
                var resourceName = (session as MyCustomInstrument).ResourceName;
                if (!_resourceDescriptorToSitePinDictionary.ContainsKey(resourceName))
                {
                    _resourceDescriptorToSitePinDictionary.Add(
                        resourceName,
                        new List<SitePinInfo> { new SitePinInfo(SitePinInfo.SiteNumberNone, systemPin) });
                }
                else
                {
                    _resourceDescriptorToSitePinDictionary[resourceName]
                        .Add(new SitePinInfo(SitePinInfo.SiteNumberNone, systemPin));
                }
            }
        }
    }
}
