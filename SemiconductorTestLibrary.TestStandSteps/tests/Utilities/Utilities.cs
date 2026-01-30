using System;
using System.Linq;
using System.Reflection;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Utilities
{
    internal static class Utilities
    {
        #region Integration Tests

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

        internal static void AssertPublishedDataValue(double expectedValue, IPublishedData[] publishedData)
        {
            foreach (var data in publishedData)
            {
                Assert.Equal(expectedValue, data.DoubleValue);
            }
        }

        internal static void AssertPublishedDataValue(bool expectedValue, IPublishedData[] publishedData)
        {
            foreach (var data in publishedData)
            {
                Assert.Equal(expectedValue, data.BooleanValue);
            }
        }

        internal static void AssertPublishedDataId(string expectedId, IPublishedData[] publishedData)
        {
            foreach (var data in publishedData)
            {
                Assert.Equal(expectedId, data.PublishedDataId);
            }
        }

        #endregion

        #region Signature Check

        internal static void AssertEnumParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional, int expectedDefaultValue)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
            Assert.Equal(expectedDefaultValue, (int)parameterInfo.DefaultValue);
        }

        internal static void AssertBoolParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional, bool expectedDefaultValue)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
            Assert.Equal(expectedDefaultValue, parameterInfo.DefaultValue);
        }

        internal static void AssertDoubleParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional, double expectedDefaultValue)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
            Assert.Equal(expectedDefaultValue, parameterInfo.DefaultValue);
        }

        internal static void AssertIntParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional, int expectedDefaultValue)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
            Assert.Equal(expectedDefaultValue, parameterInfo.DefaultValue);
        }

        internal static void AssertStringParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional, string expectedDefaultValue)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
            Assert.Equal(expectedDefaultValue, parameterInfo.DefaultValue);
        }

        internal static void AssertStructParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional, object expectedDefaultValue)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
            Assert.Equal(expectedDefaultValue, parameterInfo.DefaultValue);
        }

        internal static void AssertParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
        }

        internal static MethodInfo GetMethod(Type classType, string methodName, Type[] parameterTypes)
        {
            return classType.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);
        }

        #endregion
    }
}
