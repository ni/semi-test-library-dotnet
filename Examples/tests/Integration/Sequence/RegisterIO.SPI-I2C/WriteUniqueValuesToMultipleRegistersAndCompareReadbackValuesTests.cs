using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class WriteUniqueValuesToMultipleRegistersAndCompareReadbackValuesTests
    {
        private const string PinMapFileName = "STLExample.RegisterIO.SPIAndI2C.pinmap";

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.SPI)]
        public void WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues_UniqueValues_ReadbackMatches(CommunicationProtocol protocol)
        {
            var tsmContext = CreateTSMContext(PinMapFileName, out _);

            TestStep.WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(tsmContext, protocol);
        }
    }
}
