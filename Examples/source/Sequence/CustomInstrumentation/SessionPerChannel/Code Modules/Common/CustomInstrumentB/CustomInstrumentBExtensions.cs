using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using System.Linq;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannel.Common.CustomInstrumentB
{
    public static class CustomInstrumentBExtensions
    {

        public static void Configure(this CustomInstrumentBSessionsBundle sessionsBundle, double range)
        {
            sessionsBundle.Do(x => x.Session.Configure(range));
        }

        public static PinSiteData<double> Measure(this CustomInstrumentBSessionsBundle sessionsBundle)
        {
            return new PinSiteData<double>(
                sessionsBundle.AggregateSitePinList.Select(x => x.PinName).Distinct().ToArray(),
                sessionsBundle.AggregateSitePinList.Select(x => x.SiteNumber).Distinct().ToArray(),
                99);
        }
    }
}
