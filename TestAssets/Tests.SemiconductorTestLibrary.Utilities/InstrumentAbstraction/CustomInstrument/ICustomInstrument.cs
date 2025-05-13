namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument
{
    /// <summary>
    /// Custom instrument interface needs to be implemented by customers.
    /// </summary>
    public interface ICustomInstrument
    {
        /// <summary>
        /// Resource Name - "instrumentName/channelGroupId"
        /// </summary>
        public string ResourceName { get; }

        /// <summary>
        /// Initializes the custom instrument.
        /// </summary>
        /// <param name="instrumentName">Instrument Name</param>
        /// <param name="channelGroupId">Channel Group Id</param>
        public void Initialize(string instrumentName, string channelGroupId);

        /// <summary>
        /// Closes the custom instrument.
        /// </summary>
        public void Close();

        /// <summary>
        /// Resets the custom instrument.
        /// </summary>
        public void Reset();
    }
}
