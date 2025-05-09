using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to measure current on pins that could be mapped to a DCPower instrument, Digital Pattern Instrument, or Digital Multi Meter.
    /// Note that DCPower Instruments include both Source Measurement Units (SMUs) and Programmable Power Supplies (PPS) devices.
    /// This class, and it's methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have been already initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class MeasureVoltageMixed
    {
        internal static void SimpleMeasureAndPublishCurrentSmuOrPpmuOrDmm(ISemiconductorModuleContext tsmContext, string[] pinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var publishedDataId = "Measurement";
            var filteredPinNamesSmu = tsmContext.FilterPinsByInstrumentType(pinNames, InstrumentTypeIdConstants.NIDCPower);
            var filteredPinNamesPpmu = tsmContext.FilterPinsByInstrumentType(pinNames, InstrumentTypeIdConstants.NIDigitalPattern);
            var filteredPinNamesDmm = tsmContext.FilterPinsByInstrumentType(pinNames, InstrumentTypeIdConstants.NIDmm);
            var smuPins = sessionManager.DCPower(filteredPinNamesSmu);
            var ppmuPins = sessionManager.Digital(filteredPinNamesPpmu);
            var dmmPins = sessionManager.DMM(filteredPinNamesPpmu);

            // Just an example of the InvokeInParallel method, assumes that the instrumentation is already configured.
            Utilities.InvokeInParallel(
                () => ppmuPins.MeasureAndPublishCurrent(publishedDataId),
                () => smuPins.MeasureAndPublishCurrent(publishedDataId),
                () => dmmPins.ReadAndPublish(maximumTimeInMilliseconds: 2000, publishedDataId));
        }

        internal static void SimplerMeasureAndPublishCurrentSmuOrPpmuOrDmm(ISemiconductorModuleContext tsmContext, string[] pinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var publishedDataId = "Measurement";
            // Note the filterPins parameter was introduced in the 24.5.1 release.
            var smuPins = sessionManager.DCPower(pinNames, filterPins: true);
            var ppmuPins = sessionManager.Digital(pinNames, filterPins: true);
            var dmmPins = sessionManager.DMM(pinNames, filterPins: true);

            // Just an example of the InvokeInParallel method, assumes that the instrumentation is already configured.
            // Also, note that the null-conditional operator (?.) allows for the operation to be skipped when the SessionBundle object is null,
            // which is the case when no pins of that instrument type are passed into the function when using the filterPins parameter.
            Utilities.InvokeInParallel(
                () => ppmuPins?.MeasureAndPublishCurrent(publishedDataId),
                () => smuPins?.MeasureAndPublishCurrent(publishedDataId),
                () => dmmPins?.ReadAndPublish(maximumTimeInMilliseconds: 2000, publishedDataId));
        }
    }
}
