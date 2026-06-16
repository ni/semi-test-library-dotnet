using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class WriteSiteUniqueValueToRegisterAndCompareReadbackValuesTests
    {
        private const string PinMapFileName = "STLExample.RegisterIO.SpiAndI2c.pinmap";

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.SPI)]
        [InlineData(CommunicationProtocol.I2C)]
        public void WriteSiteUniqueValueToRegister_ComparesReadbackSuccessfully(CommunicationProtocol protocol)
        {
            var tsmContext = CreateTSMContext(PinMapFileName, out _);

            TestStep.WriteSiteUniqueValueToRegisterAndCompareReadbackValues(tsmContext, protocol);
        }
    }
}
