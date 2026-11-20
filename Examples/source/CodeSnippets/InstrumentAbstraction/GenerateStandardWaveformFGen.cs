using System.Linq;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to configure and initiate NI-FGEN standard waveforms using different ConfigureStandardWaveform overloads.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class GenerateStandardWaveformFGen
    {
        /// <summary>
        /// This example demonstrates how to configure and initiate the same waveform across all specified pins and sites.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="fgenPinNames">The FGEN pins to configure.</param>
        internal static void SameWaveformAcrossAllPinsAndSites(
            ISemiconductorModuleContext tsmContext,
            string[] fgenPinNames)
        {
            // Create the session manager and get the FGEN sessions for the specified pins.
            var sessionManager = new TSMSessionManager(tsmContext);
            var fgenPins = sessionManager.Fgen(fgenPinNames);

            // Define a standard waveform configuration to apply a sinewave.
            var sineWaveSettings = new StandardWaveformSettings(
                waveformFunctionType: StandardWaveform.Sine,
                frequency: 1000.0,
                amplitude: 2.0,
                dcOffset: 0.0,
                startPhase: 0.0);

            // Configure the Function output mode to enable standard waveform generation.
            fgenPins.ConfigureOutputMode(OutputMode.Function);

            // Apply the same waveform settings to all pins and sites.
            fgenPins.ConfigureStandardWaveform(sineWaveSettings);

            // Enable the output.
            fgenPins.ConfigureOutputEnabled(true);

            // Start waveform generation.
            fgenPins.Initiate();
        }

        /// <summary>
        /// This example demonstrates how to configure and initiate site-unique waveform settings across all specified pins.
        /// Waveform types used here are valid standard waveforms (Square and Triangle), excluding User.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="fgenPinNames">The FGEN pins to configure.</param>
        internal static void DifferentWaveformsPerSite(
            ISemiconductorModuleContext tsmContext,
            string[] fgenPinNames)
        {
            // Create the session manager, get the active sites, and get the FGEN sessions for the pins.
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();
            var fgenPins = sessionManager.Fgen(fgenPinNames);

            // Build a SiteData object to store unique waveform settings per site.
            var perSiteSettings = new SiteData<StandardWaveformSettings>();

            foreach (var site in activeSites)
            {
                // Even sites get a square wave; odd sites get a triangle wave.
                var settings = site % 2 == 0
                    ? new StandardWaveformSettings(
                        StandardWaveform.Square,
                        frequency: 5000.0,
                        amplitude: 1.5,
                        dcOffset: 0.0,
                        startPhase: 0.0)
                    : new StandardWaveformSettings(
                        StandardWaveform.Triangle,
                        frequency: 8000.0,
                        amplitude: 1.0,
                        dcOffset: 0.1,
                        startPhase: 45.0);

                perSiteSettings.SetValue(value: settings, siteNumbers: site);
            }

            // Configure function output mode, apply per-site settings, enable output, and start generation.
            fgenPins.ConfigureOutputMode(OutputMode.Function);
            fgenPins.ConfigureStandardWaveform(perSiteSettings);
            fgenPins.ConfigureOutputEnabled(true);
            fgenPins.Initiate();
        }

        /// <summary>
        /// This example demonstrates how to configure and initiate pin- and site-unique waveform settings.
        /// Waveform types used here include RampUp, RampDown, and Noise (excluding User).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="fgenPinNames">The FGEN pins to configure.</param>
        internal static void DifferentWaveformsPerPinPerSite(
            ISemiconductorModuleContext tsmContext,
            string[] fgenPinNames)
        {
            // Create the session manager, get the active sites, and get the FGEN sessions for the pins.
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();
            var fgenPins = sessionManager.Fgen(fgenPinNames);

            // Build a PinSiteData object to store unique waveform settings per pin and site.
            var perPinPerSiteSettings = new PinSiteData<StandardWaveformSettings>(fgenPinNames, activeSites);
            for (int pinIndex = 0; pinIndex < fgenPinNames.Length; pinIndex++)
            {
                foreach (var site in activeSites)
                {
                    // Cycle through RampUp, RampDown, and Noise based on pin index and site number.
                    var waveformType = (pinIndex + site) % 3 == 0
                        ? StandardWaveform.RampUp
                        : (pinIndex + site) % 3 == 1
                            ? StandardWaveform.RampDown
                            : StandardWaveform.Noise;

                    // Frequency scales with site number; amplitude scales with pin index.
                    var settings = new StandardWaveformSettings(
                        waveformFunctionType: waveformType,
                        frequency: 2000.0 + (site * 500.0),
                        amplitude: 1.0 + (pinIndex * 0.2),
                        dcOffset: 0.0,
                        startPhase: 0.0);

                    perPinPerSiteSettings.SetValue(
                        value: settings,
                        pinName: fgenPinNames[pinIndex],
                        siteNumbers: site);
                }
            }

            // Configure function output mode, apply per-pin-per-site settings, enable output, and start generation.
            fgenPins.ConfigureOutputMode(OutputMode.Function);
            fgenPins.ConfigureStandardWaveform(perPinPerSiteSettings);
            fgenPins.ConfigureOutputEnabled(true);
            fgenPins.Initiate();
        }
    }
}
