namespace SemiconductorTestLibrary.Examples.CustomInstrument.Common
{
    public class MyCustomInstrument
    {
        public static string InstrumentTypeId => "MyCustomInstrument";

        public readonly string ResourceName;
        public readonly string ChannelGroupId;
        public readonly string ChannelList;

        public MyCustomInstrument(string resourceName, string channelGroupId, string channelList)
        {
            ResourceName = resourceName;
            ChannelGroupId = channelGroupId;
            ChannelList = channelList;
        }

        public void Configure(double range) { }

        public double Measure()
        {
            return 99;
        }

        public void Close() { }
    }
}
