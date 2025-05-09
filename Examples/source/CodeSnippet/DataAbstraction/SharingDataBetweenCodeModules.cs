using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.DataAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Data Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to use the PinStieData objects.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// that has already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class SharingDataBetweenCodeModules
    {
        internal static void StoreSiteDataForLaterRetrival(ISemiconductorModuleContext tsmContext)
        {
            SiteData<double> measurementA = SiteDataExamples.PerSiteMeasure();
            tsmContext.SetGlobalSiteData("MeasurementsFromCodeModuleA", measurementA);
        }

        internal static void GetSiteDataFromAnotherCodeModule(ISemiconductorModuleContext tsmContext)
        {
            SiteData<double> measurments = tsmContext.GetGlobalSiteData<double>("MeasurementsFromCodeModuleA");
        }

        internal static void StorePinSiteDataForLaterRetrival(ISemiconductorModuleContext tsmContext)
        {
            PinSiteData<double> measurementsAB = PinSiteDataExamples.Measure();
            tsmContext.SetGlobalPinSiteData("MeasurementsAB", measurementsAB);
        }

        internal static void GetPinSiteDataFromAnotherCodeModule(ISemiconductorModuleContext tsmContext)
        {
            PinSiteData<double> measurmentsAB = tsmContext.GetGlobalPinSiteData<double>("MeasurementsAB");
        }
    }
}
