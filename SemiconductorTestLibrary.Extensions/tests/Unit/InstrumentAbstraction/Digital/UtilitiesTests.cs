using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    public class UtilitiesTests
    {
        [Fact]
        public void InitializeUintSiteData_UnpackParallelCaptureDataByPin_ReturnsCorrectValues()
        {
            var samples = new uint[][]
            {
                // Site0 Samples
                new uint[] { 0b01, 0b10, 0b11, 0b00 },
                // Site1 Samples
                new uint[] { 0b11, 0b00, 0b01, 0b10 }
            };
            var siteNumbers = new int[] { 0, 1 };
            var pinNames = new string[] { "PinA", "PinB" };
            SiteData<uint[]> siteData = new SiteData<uint[]>(siteNumbers, samples);

            PinSiteData<uint[]> reformattedData = siteData.UnpackParallelCaptureDataByPin(pinNames);

            // Site0 Pin Values
            Assert.Equal(new[] { 0u, 1u, 1u, 0u }, reformattedData.GetValue(0, "PinA"));
            Assert.Equal(new[] { 1u, 0u, 1u, 0u }, reformattedData.GetValue(0, "PinB"));

            // Site1 Pin Values
            Assert.Equal(new[] { 1u, 0u, 0u, 1u }, reformattedData.GetValue(1, "PinA"));
            Assert.Equal(new[] { 1u, 0u, 1u, 0u }, reformattedData.GetValue(1, "PinB"));
        }

        [Fact]
        public void InitializeUintSiteData_UnpackParallelCaptureDataByPinAsBoolArray_ReturnsCorrectValues()
        {
            var samples = new uint[][]
            {
                // Site0 Samples
                new uint[] { 0b01, 0b10, 0b11, 0b00 },
                // Site1 Samples
                new uint[] { 0b11, 0b00, 0b01, 0b10 }
            };
            var siteNumbers = new int[] { 0, 1 };
            var pinNames = new string[] { "PinA", "PinB" };
            SiteData<uint[]> siteData = new SiteData<uint[]>(siteNumbers, samples);

            PinSiteData<bool[]> reformattedData = siteData.UnpackParallelCaptureDataByPinAsBoolArray(pinNames);

            // Site0 Pin Values
            Assert.Equal(new[] { false, true, true, false }, reformattedData.GetValue(0, "PinA"));
            Assert.Equal(new[] { true, false, true, false }, reformattedData.GetValue(0, "PinB"));

            // Site1 Pin Values
            Assert.Equal(new[] { true, false, false, true }, reformattedData.GetValue(1, "PinA"));
            Assert.Equal(new[] { true, false, true, false }, reformattedData.GetValue(1, "PinB"));
        }
    }
}
