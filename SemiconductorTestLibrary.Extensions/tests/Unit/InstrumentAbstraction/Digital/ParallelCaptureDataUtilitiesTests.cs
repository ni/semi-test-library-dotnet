using System.Collections.Generic;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    public sealed class ParallelCaptureDataUtilitiesTests
    {
        private static readonly string[] PinNames = new[] { "PinA", "PinB", "PinC" };
        private static readonly int[] SiteNumbers = new[] { 0, 1 };

        public static IEnumerable<object[]> UintMatrixData()
        {
            // Site 0 input: [5,3,6,0] => [101,011,110,000]
            // Site 1 input: [150,32,26,10] => use bit[2:0] of each sample

            yield return new object[]
            {
                BitOrder.MSBFirst, BitOrder.LSBFirst,
                new Dictionary<int, uint[]>
                {
                    [0] = new uint[] { 5, 3, 6, 0 },
                    [1] = new uint[] { 150, 32, 26, 10 }
                },
                new uint[][][]
                {
                    // Site 0
                    new uint[][]
                    {
                        // PinA
                        new uint[] { 1, 0, 1, 0 },
                        // PinB
                        new uint[] { 0, 1, 1, 0 },
                        // PinC
                        new uint[] { 1, 1, 0, 0 }
                    },
                    // Site 1
                    new uint[][]
                    {
                        // PinA
                        new uint[] { 1, 0, 0, 0 },
                        // PinB
                        new uint[] { 1, 0, 1, 1 },
                        // PinC
                        new uint[] { 0, 0, 0, 0 }
                    }
                }
            };

            yield return new object[]
            {
                BitOrder.MSBFirst, BitOrder.MSBFirst,
                new Dictionary<int, uint[]>
                {
                    [0] = new uint[] { 5, 3, 6, 0 },
                    [1] = new uint[] { 150, 32, 26, 10 }
                },
                new uint[][][]
                {
                    // Site 0
                    new uint[][]
                    {
                        // PinA
                        new uint[] { 0, 1, 0, 1 },
                        // PinB
                        new uint[] { 0, 1, 1, 0 },
                        // PinC
                        new uint[] { 0, 0, 1, 1 }
                    },
                    // Site 1
                    new uint[][]
                    {
                        // PinA
                        new uint[] { 0, 0, 0, 1 },
                        // PinB
                        new uint[] { 1, 1, 0, 1 },
                        // PinC
                        new uint[] { 0, 0, 0, 0 }
                    }
                }
            };

            yield return new object[]
            {
                BitOrder.LSBFirst, BitOrder.LSBFirst,
                new Dictionary<int, uint[]>
                {
                    [0] = new uint[] { 5, 3, 6, 0 },
                    [1] = new uint[] { 150, 32, 26, 10 }
                },
                new uint[][][]
                {
                    // Site 0
                    new uint[][]
                    {
                        // PinA
                        new uint[] { 1, 1, 0, 0 },
                        // PinB
                        new uint[] { 0, 1, 1, 0 },
                        // PinC
                        new uint[] { 1, 0, 1, 0 }
                    },
                    // Site 1
                    new uint[][]
                    {
                        // PinA
                        new uint[] { 0, 0, 0, 0 },
                        // PinB
                        new uint[] { 1, 0, 1, 1 },
                        // PinC
                        new uint[] { 1, 0, 0, 0 }
                    }
                }
            };

            yield return new object[]
            {
                BitOrder.LSBFirst, BitOrder.MSBFirst,
                new Dictionary<int, uint[]>
                {
                    [0] = new uint[] { 5, 3, 6, 0 },
                    [1] = new uint[] { 150, 32, 26, 10 }
                },
                new uint[][][]
                {
                    // Site 0
                    new uint[][]
                    {
                        // PinA
                        new uint[] { 0, 0, 1, 1 },
                        // PinB
                        new uint[] { 0, 1, 1, 0 },
                        // PinC
                        new uint[] { 0, 1, 0, 1 }
                    },
                    // Site 1
                    new uint[][]
                    {
                        // PinA
                        new uint[] { 0, 0, 0, 0 },
                        // PinB
                        new uint[] { 1, 1, 0, 1 },
                        // PinC
                        new uint[] { 0, 0, 0, 1 }
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(UintMatrixData))]
        public void UnpackParallelCaptureDataByPinAsUintArray_BitOrderMatrix_ReturnsExpectedPerPin(
            BitOrder pinBitOrder,
            BitOrder sampleBitOrder,
            Dictionary<int, uint[]> inputData,
            uint[][][] expectedPerSitePerPin)
        {
            var siteData = new SiteData<uint[]>(inputData);

            var actual = siteData.UnpackParallelCaptureDataByPinAsUintArray(PinNames, pinBitOrder, sampleBitOrder);

            for (int siteIndex = 0; siteIndex < SiteNumbers.Length; siteIndex++)
            {
                for (int pinIndex = 0; pinIndex < PinNames.Length; pinIndex++)
                {
                    Assert.Equal(
                        expectedPerSitePerPin[siteIndex][pinIndex],
                        actual.GetValue(siteNumber: SiteNumbers[siteIndex], pinName: PinNames[pinIndex]));
                }
            }
        }

        [Theory]
        [MemberData(nameof(UintMatrixData))]
        public void UnpackParallelCaptureDataByPinAsBoolArray_BitOrderMatrix_ReturnsExpectedPerPin(
            BitOrder pinBitOrder,
            BitOrder sampleBitOrder,
            Dictionary<int, uint[]> inputData,
            uint[][][] expectedPerSitePerPinAsUint)
        {
            var siteData = new SiteData<uint[]>(inputData);

            var actual = siteData.UnpackParallelCaptureDataByPinAsBoolArray(PinNames, pinBitOrder, sampleBitOrder);

            for (int siteIndex = 0; siteIndex < SiteNumbers.Length; siteIndex++)
            {
                for (int pinIndex = 0; pinIndex < PinNames.Length; pinIndex++)
                {
                    var expectedUint = expectedPerSitePerPinAsUint[siteIndex][pinIndex];
                    var expectedBool = new bool[expectedUint.Length];
                    for (int i = 0; i < expectedUint.Length; i++)
                    {
                        expectedBool[i] = expectedUint[i] == 1u;
                    }

                    Assert.Equal(
                        expectedBool,
                        actual.GetValue(siteNumber: SiteNumbers[siteIndex], pinName: PinNames[pinIndex]));
                }
            }
        }

        [Fact]
        public void UnpackParallelCaptureDataByPinAsUintArray_DoesNotMutateInputSamples()
        {
            var originalSite0 = new uint[] { 5, 3, 6, 0 };
            var originalSite1 = new uint[] { 150, 32, 26, 10 };

            var siteData = new SiteData<uint[]>(new Dictionary<int, uint[]>
            {
                [0] = new uint[] { 5, 3, 6, 0 },
                [1] = new uint[] { 150, 32, 26, 10 }
            });

            siteData.UnpackParallelCaptureDataByPinAsUintArray(PinNames, BitOrder.MSBFirst, BitOrder.LSBFirst);

            Assert.Equal(originalSite0, siteData.GetValue(0));
            Assert.Equal(originalSite1, siteData.GetValue(1));
        }

        [Fact]
        public void InitializeUintSiteData_UnpackParallelCaptureDataByPin_ReturnsCorrectValues()
        {
            var samples = new uint[][]
            {
                // Site0 Samples
                new uint[] { 0b01, 0b10, 0b11, 0b00 },
                // Site1 Samples
                new uint[] { 0b11, 0b00, 0b01, 0b10 }
            };
            var siteNumbers = new int[] { 0, 1 };
            var pinNames = new string[] { "PinA", "PinB" };
            SiteData<uint[]> siteData = new SiteData<uint[]>(siteNumbers, samples);

            PinSiteData<uint[]> reformattedData = siteData.UnpackParallelCaptureDataByPinAsUintArray(pinNames, BitOrder.MSBFirst, BitOrder.LSBFirst);

            // Site0 Pin Values
            Assert.Equal(new[] { 0u, 1u, 1u, 0u }, reformattedData.GetValue(0, "PinA"));
            Assert.Equal(new[] { 1u, 0u, 1u, 0u }, reformattedData.GetValue(0, "PinB"));

            // Site1 Pin Values
            Assert.Equal(new[] { 1u, 0u, 0u, 1u }, reformattedData.GetValue(1, "PinA"));
            Assert.Equal(new[] { 1u, 0u, 1u, 0u }, reformattedData.GetValue(1, "PinB"));
        }

        [Fact]
        public void InitializeUintSiteData_UnpackParallelCaptureDataByPinAsBoolArray_ReturnsCorrectValues()
        {
            var samples = new uint[][]
            {
                // Site0 Samples
                new uint[] { 0b01, 0b10, 0b11, 0b00 },
                // Site1 Samples
                new uint[] { 0b11, 0b00, 0b01, 0b10 }
            };
            var siteNumbers = new int[] { 0, 1 };
            var pinNames = new string[] { "PinA", "PinB" };
            SiteData<uint[]> siteData = new SiteData<uint[]>(siteNumbers, samples);

            PinSiteData<bool[]> reformattedData = siteData.UnpackParallelCaptureDataByPinAsBoolArray(pinNames, BitOrder.MSBFirst, BitOrder.LSBFirst);

            // Site0 Pin Values
            Assert.Equal(new[] { false, true, true, false }, reformattedData.GetValue(0, "PinA"));
            Assert.Equal(new[] { true, false, true, false }, reformattedData.GetValue(0, "PinB"));

            // Site1 Pin Values
            Assert.Equal(new[] { true, false, false, true }, reformattedData.GetValue(1, "PinA"));
            Assert.Equal(new[] { true, false, true, false }, reformattedData.GetValue(1, "PinB"));
        }
    }
}