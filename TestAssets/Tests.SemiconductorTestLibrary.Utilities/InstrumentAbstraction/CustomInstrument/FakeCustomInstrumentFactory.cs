using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument
{
    /// <summary>
    /// A fake implementation of <see cref="ICustomInstrumentFactory"/> for testing purposes.
    /// </summary>
    public class FakeCustomInstrumentFactory : ICustomInstrumentFactory
    {
        public string InstrumentTypeId { get; }

        public FakeCustomInstrumentFactory(string instrumentTypeId)
        {
            InstrumentTypeId = instrumentTypeId;
        }

        public ICustomInstrument CreateInstrument(string instrumentName, string channelGroupId)
        {
            return new FakeCustomInstrument(instrumentName, channelGroupId);
        }
    }
}