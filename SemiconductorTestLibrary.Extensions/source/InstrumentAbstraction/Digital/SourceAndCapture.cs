using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods for sourcing and capturing waveforms.
    /// </summary>
    public static class SourceAndCapture
    {
        /// <summary>
        /// Writes source waveform. Use this method to write the same waveform data to all sites.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="waveformName">The name of the source waveform.</param>
        /// <param name="waveformData">The waveform data.</param>
        /// <param name="expandToMinimumSize">Whether to make the size of the waveform data at least a specified value.</param>
        /// <param name="minimumSize">The minimum size of the waveform data.</param>
        public static void WriteSourceWaveformBroadcast(this DigitalSessionsBundle sessionsBundle, string waveformName, uint[] waveformData, bool expandToMinimumSize = false, int minimumSize = 128)
        {
            ResizeWaveformDataArrayOnDemand(ref waveformData, expandToMinimumSize, minimumSize);
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.SourceWaveforms.WriteBroadcast(waveformName, waveformData);
            });
        }

        /// <summary>
        /// Writes source waveform. Use this method to write different waveform data to different sites.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="waveformName">The name of the source waveform.</param>
        /// <param name="perSiteWaveformData">The per-site waveform data.</param>
        /// <param name="expandToMinimumSize">Whether to make the size of the waveform data at least a specified value.</param>
        /// <param name="minimumSize">The minimum size of the waveform data.</param>
        /// <remarks>This method takes per-site source waveform.</remarks>
        public static void WriteSourceWaveformSiteUnique(this DigitalSessionsBundle sessionsBundle, string waveformName, IDictionary<int, uint[]> perSiteWaveformData, bool expandToMinimumSize = false, int minimumSize = 128)
        {
            for (int i = 0; i < perSiteWaveformData.Count; i++)
            {
                var waveformData = perSiteWaveformData[i];
                ResizeWaveformDataArrayOnDemand(ref waveformData, expandToMinimumSize, minimumSize);
            }
            sessionsBundle.Do(perSiteWaveformData, (sessionInfo, perInstrumentWaveformData) =>
            {
                sessionInfo.Session.SourceWaveforms.WriteSiteUnique(sessionInfo.SiteListString, waveformName, perInstrumentWaveformData);
            });
        }

        /// <summary>
        /// Creates source waveform settings required for serial sourcing. Settings apply across all sites if multiple sites are configured in the pin map. You cannot reconfigure settings after waveforms are created.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="waveformName">The name of the source waveform.</param>
        /// <param name="dataMapping">Specifies whether the waveform is broadcast to all sites or a unique waveform is sourced per site.</param>
        /// <param name="sampleWidth">The width in bits of each serial sample. The value must be between 1 and 32.</param>
        /// <param name="bitOrder">The bit order significance.</param>
        public static void CreateSerialSourceWaveform(this DigitalSessionsBundle sessionsBundle, string waveformName, SourceDataMapping dataMapping, uint sampleWidth, BitOrder bitOrder)
        {
            sessionsBundle.CreateSerialSourceWaveform(pin: null, waveformName, dataMapping, sampleWidth, bitOrder);
        }

        /// <summary>
        /// Creates source waveform settings required for serial sourcing. Settings apply across all sites if multiple sites are configured in the pin map. You cannot reconfigure settings after waveforms are created.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pin">The pin for which to create the source waveform.</param>
        /// <param name="waveformName">The name of the source waveform.</param>
        /// <param name="dataMapping">Specifies whether the waveform is broadcast to all sites or a unique waveform is sourced per site.</param>
        /// <param name="sampleWidth">The width in bits of each serial sample. The value must be between 1 and 32.</param>
        /// <param name="bitOrder">The bit order significance.</param>
        public static void CreateSerialSourceWaveform(this DigitalSessionsBundle sessionsBundle, string pin, string waveformName, SourceDataMapping dataMapping, uint sampleWidth, BitOrder bitOrder)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                var pinSetString = pin ?? GetPinSetStringWithoutSiteNumber(sessionInfo);
                sessionInfo.Session.SourceWaveforms.CreateSerial(pinSetString, waveformName, dataMapping, sampleWidth, bitOrder);
            });
        }

        /// <summary>
        /// Creates capture waveform settings for serial acquisition. Settings apply across all sites if multiple sites are configured in the pin map. You cannot reconfigure settings after waveforms are created.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="waveformName">The name of the capture waveform.</param>
        /// <param name="sampleWidth">The width in bits of each serial sample. The value must be between 1 and 32.</param>
        /// <param name="bitOrder">The bit order significance.</param>
        /// <remarks>The number of waveforms is limited to 512.</remarks>
        public static void CreateSerialCaptureWaveform(this DigitalSessionsBundle sessionsBundle, string waveformName, uint sampleWidth, BitOrder bitOrder = BitOrder.MostSignificantBitFirst)
        {
            sessionsBundle.CreateSerialCaptureWaveform(pin: null, waveformName, sampleWidth, bitOrder);
        }

        /// <summary>
        /// Creates capture waveform settings for serial acquisition. Settings apply across all sites if multiple sites are configured in the pin map. You cannot reconfigure settings after waveforms are created.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pin">The pin for which to create the capture waveform.</param>
        /// <param name="waveformName">The name of the capture waveform.</param>
        /// <param name="sampleWidth">The width in bits of each serial sample. The value must be between 1 and 32.</param>
        /// <param name="bitOrder">The bit order significance.</param>
        /// <remarks>The number of waveforms is limited to 512.</remarks>
        public static void CreateSerialCaptureWaveform(this DigitalSessionsBundle sessionsBundle, string pin, string waveformName, uint sampleWidth, BitOrder bitOrder = BitOrder.MostSignificantBitFirst)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                var pinSetString = pin ?? GetPinSetStringWithoutSiteNumber(sessionInfo);
                sessionInfo.Session.CaptureWaveforms.CreateSerial(pinSetString, waveformName, sampleWidth, bitOrder);
            });
        }

        /// <summary>
        /// Creates the capture waveform settings for parallel acquisition. Settings apply across all sites if multiple sites are configured in the pin map. You cannot reconfigure settings after waveforms are created.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="waveformName">The name of the capture waveform.</param>
        public static void CreateParallelCaptureWaveform(this DigitalSessionsBundle sessionsBundle, string waveformName)
        {
            sessionsBundle.CreateParallelCaptureWaveform(pins: null, waveformName);
        }

        /// <summary>
        /// Creates the capture waveform settings for parallel acquisition. Settings apply across all sites if multiple sites are configured in the pin map. You cannot reconfigure settings after waveforms are created.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pin">The pin for which to create the capture waveform.</param>
        /// <param name="waveformName">The name of the capture waveform.</param>
        public static void CreateParallelCaptureWaveform(this DigitalSessionsBundle sessionsBundle, string pin, string waveformName)
        {
            sessionsBundle.CreateParallelCaptureWaveform(new string[] { pin }, waveformName);
        }

        /// <summary>
        /// Creates the capture waveform settings for parallel acquisition. Settings apply across all sites if multiple sites are configured in the pin map. You cannot reconfigure settings after waveforms are created.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pins">The pins for which to create the capture waveform.</param>
        /// <param name="waveformName">The name of the capture waveform.</param>
        public static void CreateParallelCaptureWaveform(this DigitalSessionsBundle sessionsBundle, string[] pins, string waveformName)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.CaptureWaveforms.CreateParallel(pins.ToPinSetStringWithoutSiteNumber(sessionInfo), waveformName);
            });
        }

        /// <summary>
        /// Creates source waveform settings required for serial sourcing. Settings apply across all sites if multiple sites are configured in the pin map. You cannot reconfigure settings after waveforms are created.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pins">The pins for which to source the source waveform.</param>
        /// <param name="waveformName">The name of the source waveform.</param>
        /// <param name="sourceDataMapping">The source data mapping: <see cref="SourceDataMapping.Broadcast" /> or <see cref="SourceDataMapping.SiteUnique" /> </param>
        /// <exception cref="IviCDriverException">The NI-Digital Pattern Driver returned an error.</exception>
        /// <exception cref="SelectorNameException">The pinSet contains a pin or pin group name not loaded in the pin map.</exception>
        /// <exception cref="InvalidOperationException">The pinSet contains a system pin</exception>
        /// <exception cref="ArgumentException">The value for waveformName is an empty string or contains an invalid character.</exception>
        /// <exception cref="OutOfRangeException">The number of waveforms in capture memory exceeds the maximum number of waveforms allowed.</exception>
        public static void CreateParallelSourceWaveform(this DigitalSessionsBundle sessionsBundle, string[] pins, string waveformName, SourceDataMapping sourceDataMapping)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.SourceWaveforms.CreateParallel(pins.ToPinSetStringWithoutSiteNumber(sessionInfo), waveformName, sourceDataMapping);
            });
        }

        /// <summary>
        /// Fetches the capture waveform and returns a pin- and site-aware object of uint values.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="waveformName">The name of the capture waveform.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <param name="timeoutInSeconds">The maximum time to read for waveform samples.</param>
        /// <returns>The captured data.</returns>
        public static SiteData<uint[]> FetchCaptureWaveform(this DigitalSessionsBundle sessionsBundle, string waveformName, int samplesToRead, double timeoutInSeconds = 5.0)
        {
            return sessionsBundle.DoAndReturnPerSiteResults(sessionInfo =>
            {
                uint[][] result = null;
                sessionInfo.Session.CaptureWaveforms.Fetch(sessionInfo.SiteListString, waveformName, samplesToRead, TimeSpan.FromSeconds(timeoutInSeconds), ref result);
                return result;
            });
        }

        private static void ResizeWaveformDataArrayOnDemand(ref uint[] waveformData, bool resize, int size)
        {
            if (resize && waveformData.Length < size)
            {
                Array.Resize(ref waveformData, size);
            }
        }

        private static string GetPinSetStringWithoutSiteNumber(DigitalSessionInformation sessionInfo)
        {
            return ToPinSetStringWithoutSiteNumber(pins: null, sessionInfo);
        }

        private static string ToPinSetStringWithoutSiteNumber(this string[] pins, DigitalSessionInformation sessionInfo)
        {
            // Waveform driver APIs don't accept sites in pin set string parameter.
            return string.Join(",", pins ?? sessionInfo.AssociatedSitePinList.Select(i => i.PinName).Distinct());
        }
    }
}
