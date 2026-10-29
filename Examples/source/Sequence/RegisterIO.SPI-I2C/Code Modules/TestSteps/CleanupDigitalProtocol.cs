using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C
{
    /// <summary>
    /// Set of protocol-agnostic examples to demonstrate using the <see cref="SPI"/> class
    /// through the shared <see cref="IDigitalProtocol"/> interface.
    /// The <see cref="CommunicationProtocol"/> parameter selects which protocol is used
    /// at runtime.
    /// </summary>
    public static partial class TestStep
    {
        /// <summary>
        /// Frees underlying memory references used by the Digital Protocol instance.
        /// </summary>
        /// <remarks>
        /// Call this method once per test program after all protocol instances are used and the test program is ending.
        /// Call it from the ProcessCleanup callback, before the instrument sessions are closed.
        /// Without this step, TestStand reports unreleased object references when it shuts down.
        /// </remarks>
        /// <param name="communicationProtocol">The protocol whose instance to clean up.</param>
        public static void CleanupDigitalProtocol(CommunicationProtocol communicationProtocol = CommunicationProtocol.SPI)
        {
            IDigitalProtocol protocol = SemiconductorModuleContextExtensions.GetProtocol(communicationProtocol);
            protocol.ClearBundle();
        }
    }
}
