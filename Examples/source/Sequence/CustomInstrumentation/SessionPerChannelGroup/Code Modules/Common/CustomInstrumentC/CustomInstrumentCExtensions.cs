using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using System.Linq;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannelGroup.Common.CustomInstrumentC
{
    public static class CustomInstrumentCExtensions
    {

        public static void Configure(this CustomInstrumentCSessionsBundle sessionsBundle, double range)
        {
            sessionsBundle.Do(x => x.Session.Configure(range));
        }

        public static PinSiteData<double> Measure(this CustomInstrumentCSessionsBundle sessionsBundle)
        {
            return new PinSiteData<double>(
                sessionsBundle.AggregateSitePinList.Select(x => x.PinName).Distinct().ToArray(),
                sessionsBundle.AggregateSitePinList.Select(x => x.SiteNumber).Distinct().ToArray(),
                99);
        }
    }
}
