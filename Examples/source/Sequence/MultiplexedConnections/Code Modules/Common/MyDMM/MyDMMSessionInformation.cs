using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.MultiplexedConnections.Common.MyDMM
{
    /// <summary>
    /// Defines an object that contains an individual <see cref="NIDmm"/> session and its related information.
    /// </summary>
    public class MyDMMSessionInformation : ISessionInformation
    {
        private static readonly Dictionary<string, IList<SitePinInfo>> _resourceDescriptorToSitePinDictionary = new Dictionary<string, IList<SitePinInfo>>();

        /// <summary>
        /// The <see cref="NIDmm"/> session.
        /// </summary>
        public NIDmm Session { get; }

        /// <inheritdoc/>
        public IList<SitePinInfo> AssociatedSitePinList { get; } = new List<SitePinInfo>();

        /// <summary>
        /// Constructs a session information object that associates with a <see cref="NIDmm"/> instrument session.
        /// </summary>
        /// <param name="session">The <see cref="NIDmm"/> session.</param>
        public MyDMMSessionInformation(NIDmm session)
        {
            Session = session;
            AssociatedSitePinList.AddRange(_resourceDescriptorToSitePinDictionary[session.DriverOperation.IOResourceDescriptor]);
        }

        /// <summary>
        /// Constructs a session information object.
        /// </summary>
        /// <param name="session">The <see cref="NIDmm"/> session.</param>
        /// <param name="associatedSitePinList">The specified subset of channels associated with the session.</param>
        public MyDMMSessionInformation(NIDmm session, IList<SitePinInfo> associatedSitePinList)
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
            tsmContext.GetPins(InstrumentTypeIdConstants.NIDmm, out var dutPins, out var systemPins);

            foreach (var dutPin in dutPins)
            {
                foreach (var tsmSiteContext in tsmContext.GetSiteSemiconductorModuleContexts())
                {
                    tsmSiteContext.GetNIDmmSession(dutPin, out var session);
                    if (!_resourceDescriptorToSitePinDictionary.ContainsKey(session.DriverOperation.IOResourceDescriptor))
                    {
                        _resourceDescriptorToSitePinDictionary.Add(
                            session.DriverOperation.IOResourceDescriptor,
                            new List<SitePinInfo> { new SitePinInfo(tsmSiteContext.SiteNumbers.First(), dutPin) });
                    }
                    else
                    {
                        _resourceDescriptorToSitePinDictionary[session.DriverOperation.IOResourceDescriptor]
                            .Add(new SitePinInfo(tsmSiteContext.SiteNumbers.First(), dutPin));
                    }
                }
            }
            foreach (var systemPin in systemPins)
            {
                tsmContext.GetNIDmmSession(systemPin, out var session);
                if (!_resourceDescriptorToSitePinDictionary.ContainsKey(session.DriverOperation.IOResourceDescriptor))
                {
                    _resourceDescriptorToSitePinDictionary.Add(
                        session.DriverOperation.IOResourceDescriptor,
                        new List<SitePinInfo> { new SitePinInfo(SitePinInfo.SiteNumberNone, systemPin) });
                }
                else
                {
                    _resourceDescriptorToSitePinDictionary[session.DriverOperation.IOResourceDescriptor]
                        .Add(new SitePinInfo(SitePinInfo.SiteNumberNone, systemPin));
                }
            }
        }
    }
}
