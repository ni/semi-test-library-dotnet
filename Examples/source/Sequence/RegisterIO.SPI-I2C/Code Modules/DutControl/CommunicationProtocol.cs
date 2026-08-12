namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl
{
    /// <summary>
    /// Identifies the digital communication protocol used to read and write DUT registers.
    /// The <c>TestStep</c> methods use this value to select which <see cref="IDigitalProtocol"/>
    /// implementation to instantiate at runtime.
    /// </summary>
    public enum CommunicationProtocol
    {
        /// <summary>
        /// Selects the <see cref="DutControl.SPI"/> implementation.
        /// </summary>
        SPI
    }
}
