using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples
{
    /// <summary>
    /// This class contains examples of how to publish results using the Semiconductor Test Library.
    /// This class, and it's methods are intended for example purposes only,
    /// and are therefore intentionally marked as internal to prevent them from be directly invoked.
    /// </summary>
    internal static class PublishingExamples
    {
        internal static void PublishingExample(ISemiconductorModuleContext tsmContext)
        {
            string vddPinName = "VDD", sdoPinName = "SDO";
            TSMSessionManager sessionmanager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle vdd = sessionmanager.DCPower(vddPinName);
            DigitalSessionsBundle sdo = sessionmanager.Digital(sdoPinName);

            vdd.ForceVoltage(3.3, 0.1);
            sdo.WriteStatic(PinState._1);

            vdd.MeasureAndPublishCurrent("VDD_Current");
            sdo.MeasureAndPublishVoltage("SDO_Voltage");

            // Publish per-site per-pin current measurements
            PinSiteData<double> currentMeasurements = vdd.MeasureCurrent();
            tsmContext.PublishResults(currentMeasurements, "VDD_Current");

            // Publish per site pass/fail count
            SiteData<bool> passFail = sdo.GetSitePassFail();
            tsmContext.PublishResults(passFail, "SDO_PassFailCount");

            // Publish site number
            foreach (var site in tsmContext.SiteNumbers)
            {
                var siteSpecificContext = tsmContext.GetSemiconductorModuleContextWithSites(new int[] { site });
                siteSpecificContext.PublishSingleSiteResult(site, "SiteNumber");
            }
        }
    }
}
