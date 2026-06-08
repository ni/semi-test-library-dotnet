using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    /// <summary>
    /// Integration tests for the unified Register I/O <see cref="TestStep"/> methods,
    /// parameterized over <see cref="CommunicationProtocol"/>.
    /// </summary>
    /// <remarks>
    /// These tests are scaffolds. They require the shared digital project
    /// (pin map, levels, timing, and SPI/I2C pattern sets) to be present and an
    /// NI Digital Pattern instrument (or offline simulation) to be available before
    /// they can run. Remove the <c>Skip</c> argument once those assets are added.
    /// </remarks>
    [Collection("NonParallelizable")]
    public class TestStepsTests
    {
        private const string PinMapFileName = "STLExample.RegisterIO.SpiAndI2c.pinmap";

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.Spi)]
        [InlineData(CommunicationProtocol.I2c)]
        public void WriteValueToRegister_ComparesReadbackSuccessfully(CommunicationProtocol protocol)
        {
            var tsmContext = CreateTSMContext(PinMapFileName, out _);

            TestStep.WriteValueToRegisterAndCompareReadbackValue(tsmContext, protocol);
        }

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.Spi)]
        [InlineData(CommunicationProtocol.I2c)]
        public void WriteSiteUniqueValueToRegister_ComparesReadbackSuccessfully(CommunicationProtocol protocol)
        {
            var tsmContext = CreateTSMContext(PinMapFileName, out _);

            TestStep.WriteSiteUniqueValueToRegisterAndCompareReadbackValues(tsmContext, protocol);
        }

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.Spi)]
        [InlineData(CommunicationProtocol.I2c)]
        public void WriteUniqueValuesToMultipleRegisters_ComparesReadbackSuccessfully(CommunicationProtocol protocol)
        {
            var tsmContext = CreateTSMContext(PinMapFileName, out _);

            TestStep.WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(tsmContext, protocol);
        }
    }
}
