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
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have been already initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class PublishingExamples
    {
        internal static void PublishingExample(ISemiconductorModuleContext tsmContext)
        {
            string vddPinName = "VDD", sdoPinName = "SDO";
            TSMSessionManager sessionmanager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle vdd = sessionmanager.DCPower(vddPinName);
            DigitalSessionsBundle sdo = sessionmanager.Digital(sdoPinName);

            vdd.ForceVoltage(3.3, currentLimit: 0.1);
            sdo.WriteStatic(PinState._1);

            vdd.MeasureAndPublishCurrent(publishedDataId: "VDD_Current");
            sdo.MeasureAndPublishVoltage(publishedDataId: "SDO_Voltage");

            // Publish per-site per-pin current measurements
            PinSiteData<double> currentMeasurements = vdd.MeasureCurrent();
            tsmContext.PublishResults(currentMeasurements, publishedDataId: "VDD_Current");

            // Publish per site pass/fail count
            SiteData<bool> passFail = sdo.GetSitePassFail();
            tsmContext.PublishResults(passFail, publishedDataId: "SDO_PassFailCount");

            // Publish site number
            foreach (var site in tsmContext.SiteNumbers)
            {
                var siteSpecificContext = tsmContext.GetSemiconductorModuleContextWithSites(new int[] { site });
                siteSpecificContext.PublishSingleSiteResult(site, publishedDataId: "SiteNumber");
            }
        }
    }
}
