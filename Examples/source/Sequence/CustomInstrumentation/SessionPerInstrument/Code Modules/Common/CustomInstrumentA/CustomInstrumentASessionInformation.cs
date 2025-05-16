using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerInstrument.Common.CustomInstrumentA.CustomInstrumentATSMExtensions;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerInstrument.Common.CustomInstrumentA
{
    /// <summary>
    /// Defines an object that contains an individual <see cref="CustomInstrumentA"/> session and its related information.
    /// </summary>
    public class CustomInstrumentASessionInformation : ISessionInformation
    {
        private static readonly Dictionary<string, IList<SitePinInfo>> _resourceDescriptorToSitePinDictionary = new Dictionary<string, IList<SitePinInfo>>();

        /// <summary>
        /// The <see cref="CustomInstrumentA"/> session.
        /// </summary>
        public CustomInstrumentA Session { get; }

        /// <inheritdoc/>
        public IList<SitePinInfo> AssociatedSitePinList { get; } = new List<SitePinInfo>();

        /// <summary>
        /// Constructs a session information object that associates with a <see cref="NIDmm"/> instrument session.
        /// </summary>
        /// <param name="session">The <see cref="CustomInstrumentA"/> session.</param>
        public CustomInstrumentASessionInformation(CustomInstrumentA session)
        {
            Session = session;
            foreach (var item in _resourceDescriptorToSitePinDictionary[session.ResourceName])
            {
                AssociatedSitePinList.Add(item);
            }
        }

        /// <summary>
        /// Constructs a session information object.
        /// </summary>
        /// <param name="session">The <see cref="CustomInstrumentA"/> session.</param>
        /// <param name="associatedSitePinList">The specified subset of channels associated with the session.</param>
        public CustomInstrumentASessionInformation(CustomInstrumentA session, IList<SitePinInfo> associatedSitePinList)
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
            tsmContext.GetPins(InstrumentTypeId, out var dutPins, out var systemPins);

            foreach (var dutPin in dutPins)
            {
                foreach (var tsmSiteContext in tsmContext.GetSiteSemiconductorModuleContexts())
                {
                    tsmSiteContext.GetCustomSession(InstrumentTypeId, dutPin, out var session, out _, out _);
                    var resourceName = (session as CustomInstrumentA).ResourceName;
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
                tsmContext.GetCustomSession(InstrumentTypeId, systemPin, out var session, out _, out _);
                var resourceName = (session as CustomInstrumentA).ResourceName;
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
