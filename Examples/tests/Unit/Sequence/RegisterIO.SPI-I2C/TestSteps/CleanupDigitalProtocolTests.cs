using System;
using System.Reflection;
using System.Runtime.Serialization;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.Test.TestSteps
{
    [Collection("NonParallelizable")]
    public class CleanupDigitalProtocolTests
    {
        [Fact]
        public void CleanupDigitalProtocol_UnsupportedProtocol_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TestStep.CleanupDigitalProtocol((CommunicationProtocol)99));
        }

        [Theory]
        [InlineData(CommunicationProtocol.SPI)]
        public void CleanupDigitalProtocol_RetainedBundle_ClearsReferenceAndCanBeRepeated(CommunicationProtocol protocol)
        {
            IDigitalProtocol digitalProtocol = SemiconductorModuleContextExtensions.GetProtocol(protocol);
            FieldInfo bundleField = typeof(DigitalProtocol).GetField("_digitalSessionsBundle", BindingFlags.Instance | BindingFlags.NonPublic);
            var bundle = (DigitalSessionsBundle)FormatterServices.GetUninitializedObject(typeof(DigitalSessionsBundle));

            try
            {
                bundleField.SetValue(digitalProtocol, bundle);

                TestStep.CleanupDigitalProtocol(protocol);

                Assert.Null(bundleField.GetValue(digitalProtocol));

                TestStep.CleanupDigitalProtocol(protocol);

                Assert.Null(bundleField.GetValue(digitalProtocol));
            }
            finally
            {
                digitalProtocol.ClearBundle();
            }
        }

        [Fact]
        public void CleanupDigitalProtocol_DefaultProtocol_ClearsSpiBundle()
        {
            FieldInfo bundleField = typeof(DigitalProtocol).GetField("_digitalSessionsBundle", BindingFlags.Instance | BindingFlags.NonPublic);
            var bundle = (DigitalSessionsBundle)FormatterServices.GetUninitializedObject(typeof(DigitalSessionsBundle));

            try
            {
                bundleField.SetValue(SPI.Instance, bundle);

                TestStep.CleanupDigitalProtocol();

                Assert.Null(bundleField.GetValue(SPI.Instance));
            }
            finally
            {
                SPI.Instance.ClearBundle();
            }
        }
    }
}
