namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannel.Common.CustomInstrumentB
{

    public class CustomInstrumentB
    {
        public readonly string ResourceName;
        public readonly string ChannelId;

        public CustomInstrumentB(string resourceName, string channelId)
        {
            ResourceName = resourceName;
            ChannelId = channelId;
        }

        public void Configure(double range) { }

        public double Measure() => 99;

        public void Close() { }
    }
}
