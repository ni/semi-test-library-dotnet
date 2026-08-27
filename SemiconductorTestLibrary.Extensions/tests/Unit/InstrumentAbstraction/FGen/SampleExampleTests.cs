using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen;

using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.FGen
{
    public class SampleExampleTests
    {
        [Fact]
        public void GenerateSineWaveform_ShouldRunWithoutExceptions()
        {
            // Arrange
            var tsmContext = Utilities.TSMContext.CreateTSMContext("FgenTests.pinmap");
            NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen.InitializeAndClose.Initialize(tsmContext);
            // Act & Assert
            SampleExample.GenerateSineWaveform(tsmContext);
        }
    }
}
