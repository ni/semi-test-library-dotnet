using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using SemiconductorTestLibrary.Examples.MultiplexedConnections.Common.MyDMM;
using System.Collections.Generic;
using System.Linq;

namespace SemiconductorTestLibrary.Examples.MultiplexedConnections.Common
{
    public class MyPinSiteData<T> : PinSiteData<T>
    {
        public MyPinSiteData(string[] pinNames, SiteData<T>[] perPinSiteData) : base(pinNames, perPinSiteData)
        {
        }

        public MyPinSiteData(Dictionary<string, IDictionary<int, T>> pinSiteData) : base(pinSiteData)
        {
        }

        public MyPinSiteData(int[] siteNumbers, Dictionary<string, T> pinData) : base(siteNumbers, pinData)
        {
        }

        public MyPinSiteData(string[] pinNames, int[] siteNumbers, T data) : base(pinNames, siteNumbers, data)
        {
        }

        public MyPinSiteData(string[] pinNames, int[] siteNumbers, T[] perPinData) : base(pinNames, siteNumbers, perPinData)
        {
        }

        public MyPinSiteData(int[] siteNumbers, string[] pinNames, T[] perSiteData) : base(siteNumbers, pinNames, perSiteData)
        {
        }

        public MyPinSiteData(string[] pinNames, int[] siteNumbers, T[][] perPinPerSiteData) : base(pinNames, siteNumbers, perPinPerSiteData)
        {
        }

        public MyPinSiteData(int[] siteNumbers, string[] pinNames, T[][] perSitePerPinData) : base(siteNumbers, pinNames, perSitePerPinData)
        {
        }

        public void MyNewFunction()
        {
            
        }
    }
}
