using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerInstrument.Common.CustomInstrumentA
{
    public static class CustomInstrumentAExtensions
    {

        public static void Configure(this CustomInstrumentASessionsBundle sessionsBundle, double range)
        {
            sessionsBundle.Do(x => x.Session.Configure(range));
        }

        public static PinSiteData<double> Measure(this CustomInstrumentASessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(x => x.Session.Measure());
        }
    }
}
