using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument
{
    /// <summary>
    /// A fake implementation of <see cref="ICustomInstrument"/> for testing purposes.
    /// </summary>
    public class FakeCustomInstrument : ICustomInstrument
    {
        public string InstrumentName { get; }

        public string ChannelGroupId { get; }

        public NIDCPower Session { get; }

        public FakeCustomInstrument(string instrumentName, string channelGroupId)
        {
            InstrumentName = instrumentName;
            ChannelGroupId = channelGroupId;
            Session = new NIDCPower(instrumentName, resetDevice: false, optionString: string.Empty);
        }
        public void Close()
        {
            Session.Close();
        }

        public void Reset()
        {
            Session.Utility.Reset();
        }
    }
}