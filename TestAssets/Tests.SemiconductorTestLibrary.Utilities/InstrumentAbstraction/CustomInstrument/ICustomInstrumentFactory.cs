namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument
{
    /// <summary>
    /// Defines a factory for creating <see cref="ICustomInstrument"/> instances.
    /// </summary>
    public interface ICustomInstrumentFactory
    {
        /// <summary>
        /// Gets the instrument type ID.
        /// </summary>
        string InstrumentTypeId { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ICustomInstrument"/> based on the instrument type ID.
        /// </summary>
        /// <returns>A new instance of <see cref="ICustomInstrument"/>.</returns>
        ICustomInstrument CreateInstrument();
    }
}