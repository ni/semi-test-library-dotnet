using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.DataAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Data Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to use the PinStieData objects.
    /// This class, and it's methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// that has already been initiated and configured prior.
    /// Additionally, they are intentionally marked as internal to prevent them from be directly invoked from code outside of this project.
    /// </summary>
    internal static class SharingDataBetweenCodeModules
    {
        internal static void StoreSiteDataForLaterRetrival(ISemiconductorModuleContext tsmContext)
        {
            SiteData<double> measurementA = SiteDataExamples.PerSiteMeasure();

            // The Semiconductor Module Context does not currently support being passed SiteData objects.
            // Therefore, SiteData must first be converted into a 1D array of per-site values.
            // Where, each element in the array represents a given site value.
            var dataPerSiteArray = new double[tsmContext.SiteNumbers.Count];
            for (int i = 0; i < dataPerSiteArray.Length; i++)
            {
                dataPerSiteArray[i] = measurementA.GetValue(tsmContext.SiteNumbers.ElementAt(i));
            }
            tsmContext.SetSiteData("MeasurementsFromCodeModuleA", dataPerSiteArray);
        }

        internal static void GetSiteDataFromAnotherCodeModule(ISemiconductorModuleContext tsmContext)
        {
            var measurments = new SiteData<double>(tsmContext.GetSiteData<double>("MeasurementsFromCodeModuleA"));
        }

        internal static void StorePinSiteDataForLaterRetrival(ISemiconductorModuleContext tsmContext)
        {
            PinSiteData<double> measurementsAB = PinSiteDataExamples.Measure();

            // The Semiconductor Module Context does not currently support being passed PinSiteData objects.
            // Therefore, PinSiteData must first be converted into a 1D array of per-site values.
            // Where, each element in the array represents a given site value, and is a dictionary of per-
            // pin values.
            var dataPerSiteArray = new IDictionary<string, double>[tsmContext.SiteNumbers.Count];
            for (int i = 0; i < dataPerSiteArray.Length; i++)
            {
                dataPerSiteArray[i] = measurementsAB.ExtractSite(tsmContext.SiteNumbers.ElementAt(i));
            }
            tsmContext.SetSiteData("MeasurementsAB", dataPerSiteArray);
        }

        internal static void GetPinSiteDataFromAnotherCodeModule(ISemiconductorModuleContext tsmContext)
        {
            // The Semiconductor Module Context does not currently support being passed PinSiteData objects.
            // Therefore, PinSiteData stored via the TSM context, it be converted from an array of
            // dictionaries. Where, each element in the array represents a given site values as a dictionary,
            // where each item in the dictionary represents a specific pin's value for the given site.
            var perSitePinDict = tsmContext.GetSiteData<IDictionary<string, double>>("MeasurementsAB");
            var pinSiteDictionary = new Dictionary<string, IDictionary<int, double>>();
            for (int i = 0; i < tsmContext.SiteNumbers.Count; i++)
            {
                var siteNumber = tsmContext.SiteNumbers.ElementAt(i);
                foreach (var pin in perSitePinDict[i].Keys)
                {
                    var singlePinSiteValue = perSitePinDict[i][pin];
                    if (!pinSiteDictionary.TryGetValue(pin, out IDictionary<int, double> perSitePinValues))
                    {
                        perSitePinValues = new Dictionary<int, double>() { [siteNumber] = singlePinSiteValue };
                        pinSiteDictionary.Add(pin, perSitePinValues);
                        continue;
                    }
                    if (!perSitePinValues.ContainsKey(siteNumber))
                    {
                        perSitePinValues.Add(siteNumber, singlePinSiteValue);
                        continue;
                    }
                    perSitePinValues[siteNumber] = singlePinSiteValue;
                }
            }
            var measurmentsAB = new PinSiteData<double>(pinSiteDictionary);
        }
    }
}
