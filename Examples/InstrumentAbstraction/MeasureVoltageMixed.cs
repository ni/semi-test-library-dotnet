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
    /// Specifically, how to measure current on pins that could be mapped to a DCPower instrument, Digital Pattern Instrument, or both.
    /// Note that DCPower Instruments include both Source Measurement Units (SMUs) and Programmable Power Supplies (PPS) devices.
    /// This class, and it's methods are intended for example purposes only,
    /// and are therefore intentionally marked as internal to prevent them from be directly invoked.
    /// </summary>
    internal static class MeasureVoltageMixed
    {
        internal static void SimpleMeasureAndPublishCurrentSmuOrPpmu(ISemiconductorModuleContext tsmContext, string[] pinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var publishDataID = "Measurement";
            var filteredPinNamesDmm = tsmContext.FilterPinsByInstrumentType(pinNames, InstrumentTypeIdConstants.NIDmm);
            var filteredPinNamesPpmu = tsmContext.FilterPinsByInstrumentType(pinNames, InstrumentTypeIdConstants.NIDigitalPattern);
            var filteredPinNamesSmu = tsmContext.FilterPinsByInstrumentType(pinNames, InstrumentTypeIdConstants.NIDCPower);
            var ppmuPins = sessionManager.DCPower(filteredPinNamesSmu);
            var smuPins = sessionManager.Digital(filteredPinNamesPpmu);
            var dmmPins = sessionManager.DMM(filteredPinNamesPpmu);

            // Just an example of the InvokeInParallel method, assumes that the instrumentation is already configured.
            Utilities.InvokeInParallel(
                () => ppmuPins.MeasureAndPublishCurrent(publishDataID),
                () => smuPins.MeasureAndPublishCurrent(publishDataID),
                () =>
                {
                    var measurements = dmmPins.Read(maximumTimeInMilliseconds: 2000);
                    tsmContext.PublishResults(measurements, publishDataID);
                });
        }
    }
}
