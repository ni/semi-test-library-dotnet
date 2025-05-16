namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannelGroup.Common.CustomInstrumentC
{
    public class CustomInstrumentC
    {
        public readonly string ResourceName;
        public readonly string ChannelGroupId;
        public readonly string ChannelList;
        public readonly int ChannelCount;

        public CustomInstrumentC(string resourceName, string channelGroupId, string channelList)
        {
            ResourceName = resourceName;
            ChannelGroupId = channelGroupId;
            ChannelList = channelList;
            ChannelCount = ChannelList.Split(',').Length;
        }

        public void Configure(double range) { }

        public double[] Measure()
        {
            var measurements = new double[ChannelCount];
            for (int i = 0; i < ChannelCount; i++)
            {
                measurements[i] = 999 + i;
            }
            return measurements;
        }

        public void Close() { }
    }
}

