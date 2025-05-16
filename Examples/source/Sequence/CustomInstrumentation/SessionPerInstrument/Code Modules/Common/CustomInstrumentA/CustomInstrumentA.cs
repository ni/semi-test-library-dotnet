namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerInstrument.Common.CustomInstrumentA
{
    public class CustomInstrumentA
    {
        public readonly string ResourceName;
        public readonly string ChannelList;
        public readonly int ChannelCount;

        public CustomInstrumentA(string resourceName, string channelList)
        {
            ResourceName = resourceName;
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
