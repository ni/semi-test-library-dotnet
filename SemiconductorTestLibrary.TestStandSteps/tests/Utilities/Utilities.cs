using System.Linq;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Utilities
{
    internal static class Utilities
    {
        internal static void AssertPublishedDataCountPerPins(int expectedCount, string[] pins, IPublishedData[] publishedData)
        {
            foreach (var pinName in pins)
            {
                Assert.Equal(expectedCount, publishedData.Where(d => d.Pin == pinName).Count());
            }
        }
        internal static void AssertPublishedDataValueInRange(IPublishedData[] publishedData, double low, double high)
        {
            foreach (var data in publishedData)
            {
                Assert.InRange(data.DoubleValue, low, high);
            }
        }

        internal static void AssertPublishedDataValue(double value, IPublishedData[] publishedData)
        {
            foreach (var data in publishedData)
            {
                Assert.Equal(value, data.DoubleValue);
            }
        }

        internal static void AssertPublishedDataId(string expectedId, IPublishedData[] publishedData)
        {
            foreach (var data in publishedData)
            {
                Assert.Equal(expectedId, data.PublishedDataId);
            }
        }
    }
}
