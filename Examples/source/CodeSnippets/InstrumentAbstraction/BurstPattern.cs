using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to burst patterns using a Digital Pattern instrument.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have been already initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class BurstPattern
    {
        internal static void BurstPatternAndPublishResults(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            patternPins.BurstPatternAndPublishResults(patternName);

            var failCount = patternPins.GetFailCount();
            tsmContext.PublishResults(failCount, publishedDataId: "FailCount");
        }

        internal static void BurstPatternWithDynamicSourceCapture(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName, uint[] sourceWaveformData)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            patternPins.WriteSourceWaveformBroadcast(sourceWaveformName, sourceWaveformData);

            patternPins.BurstPattern(patternName);
            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, samplesToRead: -1);
        }

        internal static void BurstPatternWithDynamicSourceCaptureSiteUnique(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            // Site-unique data hard-coded for 4 sites for example purposes.
            var siteUniqueSrcWfmData = new SiteData<uint[]>(new uint[][]
            {
                new uint[] { 255, 88, 01 }, // Site 0 Samples
                new uint[] { 255, 88, 11 }, // Site 1 Samples
                new uint[] { 255, 88, 21 }, // Site 2 Samples
                new uint[] { 255, 77, 31 }, // Site 3 Samples
            });

            patternPins.WriteSourceWaveformSiteUnique(sourceWaveformName, siteUniqueSrcWfmData);
            patternPins.BurstPattern(patternName);

            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, samplesToRead: -1);
        }

        internal static void BurstPatternWithDynamicSourceCaptureSiteUniqueSeperateContexts(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            // Site-unique data hard-coded for 4 sites for example purposes.
            var siteUniqueSrcWfmData = new SiteData<uint[]>(new uint[][]
            {
                new uint[] { 255, 88, 01 }, // Site 0 Samples
                new uint[] { 255, 88, 11 }, // Site 1 Samples
                new uint[] { 255, 88, 21 }, // Site 2 Samples
                new uint[] { 255, 77, 31 }, // Site 3 Samples
            });

            foreach (var siteContext in tsmContext.GetSiteSemiconductorModuleContexts())
            {
                var currentSite = siteContext.SiteNumbers.First();
                var singleSiteSessionManager = new TSMSessionManager(siteContext);
                var singleSitePatternPins = singleSiteSessionManager.Digital(patternPinNames);

                singleSitePatternPins.WriteSourceWaveformSiteUnique(sourceWaveformName, siteUniqueSrcWfmData);
                singleSitePatternPins.BurstPattern(patternName);
            }

            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, samplesToRead: -1);
        }

        internal static void BurstPatternWithParallelCaptureAsLongValue(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName, uint[] sourceWaveformData, string[] capturePinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            patternPins.BurstPattern(patternName);
            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, samplesToRead: -1);

            // Parallel capture data is returned as a uint array where each element represents the captured data for all pins at a given capture vector of pattern execution.
            // The captured data can be reformatted into a PinSiteData<long> object where each pin's value can be separated into its own sample array and then packed into a single long value.
            // The following helper method unpacks each pin's value from the individual bits of each uint sample,
            // where a bit value indicates the state of a particular pin (true for high, false for low),
            // and then packs the individual pin values into a single long value for each sample.
            // Additionally, the method assumes that the order of the pin names passed in corresponds to bit position in a unit sample,
            // and that it also matches the order of the pins in the pattern. Where, the first pin corresponds to the most significant bit (MSB) of the uint sample,
            // and the last pin corresponds to the least significant bit (LSB).
            PinSiteData<long> captureDataByPin = captureData.PackParallelCaptureDataIntoLong(capturePinNames);
        }

        internal static void BurstPatternWithParallelCaptureAsMultipleLongValues(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName, uint[] sourceWaveformData, string[] capturePinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            patternPins.BurstPattern(patternName);
            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, samplesToRead: -1);

            // Parallel capture data is returned as a uint array where each element represents the captured data for all pins at a given capture vector of pattern execution.
            // The captured data can be reformatted into a PinSiteData<long[]> object where each pin's value can be separated into its own sample array and then packed into multiple long values.
            // The following helper method unpacks each pin's value from the individual bits of each uint sample,
            // where a bit value indicates the state of a particular pin (true for high, false for low),
            // and then packs the individual pin values into multiple long values for each sample.
            // This is useful when the number of samples exceeds the maximum value that can be represented by a single long value (>64).
            // Additionally, the method assumes that the order of the pin names passed in corresponds to bit position in a unit sample,
            // and that it also matches the order of the pins in the pattern. Where, the first pin corresponds to the most significant bit (MSB) of the uint sample,
            // and the last pin corresponds to the least significant bit (LSB).
            PinSiteData<long[]> captureDataByPin = captureData.PackParallelCaptureDataIntoLongArray(capturePinNames);
        }

        internal static void BurstPatternWithParallelCaptureFormatAsUintArray(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName, uint[] sourceWaveformData, string[] capturePinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            patternPins.BurstPattern(patternName);
            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, samplesToRead: -1);

            // Parallel capture data is returned as a uint array where each element represents the captured data for all pins at a given capture vector of pattern execution.
            // The captured data can be reformatted into a PinSiteData<uint[]> object where each pin's value can be separated into its own sample array for easier analysis.
            // The following helper method unpacks each pin's value from the individual bits of each uint sample,
            // where a bit value indicates the state of a particular pin (true for high, false for low).
            // Additionally, the method assumes that the order of the pin names passed in corresponds to bit position in a unit sample,
            // and that it also matches the order of the pins in the pattern. Where, the first pin corresponds to the most significant bit (MSB) of the uint sample,
            // and the last pin corresponds to the least significant bit (LSB).
            PinSiteData<uint[]> captureDataByPin = captureData.UnpackParallelCaptureDataByPinAsUintArray(capturePinNames);
        }

        internal static void BurstPatternWithParallelCaptureFormatAsBoolArray(ISemiconductorModuleContext tsmContext, string[] patternPinNames, string patternName, string captureWaveformName, string sourceWaveformName, uint[] sourceWaveformData, string[] capturePinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var patternPins = sessionManager.Digital(patternPinNames);

            patternPins.BurstPattern(patternName);
            SiteData<uint[]> captureData = patternPins.FetchCaptureWaveform(captureWaveformName, samplesToRead: -1);

            // Parallel capture data is returned as a uint array where each element represents the captured data for all pins at a given capture vector of pattern execution.
            // The captured data can be reformatted into a PinSiteData<bool[]> object where each pin's value can be separated into its own sample array for easier analysis.
            // The following helper method unpacks each pin's value from the individual bits of each uint sample,
            // where a bit value indicates the state of a particular pin (1 for high, 0 for low).
            // Additionally, the method assumes that the order of the pin names passed in corresponds to bit position in a unit sample,
            // and that it also matches the order of the pins in the pattern. Where, the first pin corresponds to the most significant bit (MSB) of the uint sample,
            // and the last pin corresponds to the least significant bit (LSB).
            PinSiteData<bool[]> captureDataByPin = captureData.UnpackParallelCaptureDataByPinAsBoolArray(capturePinNames);
        }
    }
}
