using System;
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
                CreateBaseInputData(),
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
                CreateBaseInputData(),
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
                CreateBaseInputData(),
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
                CreateBaseInputData(),
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
            var siteData = new SiteData<uint[]>(CloneInputData(inputData));

            PinSiteData<uint[]> actual = siteData.UnpackParallelCaptureDataByPinAsUintArray(PinNames, pinBitOrder, sampleBitOrder);

            AssertPerSitePerPinUint(actual, expectedPerSitePerPin);
        }

        [Theory]
        [MemberData(nameof(UintMatrixData))]
        public void UnpackParallelCaptureDataByPinAsBoolArray_BitOrderMatrix_ReturnsExpectedPerPin(
            BitOrder pinBitOrder,
            BitOrder sampleBitOrder,
            Dictionary<int, uint[]> inputData,
            uint[][][] expectedPerSitePerPinAsUint)
        {
            var siteData = new SiteData<uint[]>(CloneInputData(inputData));

            PinSiteData<bool[]> actual = siteData.UnpackParallelCaptureDataByPinAsBoolArray(PinNames, pinBitOrder, sampleBitOrder);

            AssertPerSitePerPinBool(actual, expectedPerSitePerPinAsUint);
        }

        [Fact]
        public void UnpackParallelCaptureDataByPinAsUintArray_DoesNotMutateInputSamples()
        {
            uint[] originalSite0 = new uint[] { 5, 3, 6, 0 };
            uint[] originalSite1 = new uint[] { 150, 32, 26, 10 };

            var siteData = new SiteData<uint[]>(CreateBaseInputData());

            siteData.UnpackParallelCaptureDataByPinAsUintArray(PinNames, BitOrder.MSBFirst, BitOrder.LSBFirst);

            Assert.Equal(originalSite0, siteData.GetValue(0));
            Assert.Equal(originalSite1, siteData.GetValue(1));
        }

        public static IEnumerable<object[]> LongMatrixData()
        {
            foreach (object[] row in UintMatrixData())
            {
                BitOrder pinBitOrder = (BitOrder)row[0];
                BitOrder sampleBitOrder = (BitOrder)row[1];
                var inputData = (Dictionary<int, uint[]>)row[2];
                var expectedPerSitePerPinBits = (uint[][][])row[3];

                yield return new object[]
                {
                    pinBitOrder,
                    sampleBitOrder,
                    inputData,
                    PackExpectedBitsPerSitePerPinIntoLong(expectedPerSitePerPinBits)
                };
            }
        }

        [Theory]
        [MemberData(nameof(LongMatrixData))]
        public void PackParallelCaptureDataIntoLong_BitOrderMatrix_ReturnsExpectedPerPinPerSite(
            BitOrder pinBitOrder,
            BitOrder sampleBitOrder,
            Dictionary<int, uint[]> inputData,
            long[][] expectedPerSitePerPinLong)
        {
            var siteData = new SiteData<uint[]>(CloneInputData(inputData));

            PinSiteData<long> actual = siteData.PackParallelCaptureDataIntoLong(PinNames, pinBitOrder, sampleBitOrder);

            AssertPerSitePerPinLong(actual, expectedPerSitePerPinLong);
        }

        [Fact]
        public void PackParallelCaptureDataIntoLong_MoreThan64Samples_ThrowsArgumentOutOfRangeException()
        {
            var inputData = new Dictionary<int, uint[]>
            {
                [0] = CreateAlternatingSamples(65, 0b100u, 0b001u),
                [1] = CreateAlternatingSamples(65, 0b001u, 0b100u)
            };
            var siteData = new SiteData<uint[]>(inputData);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => siteData.PackParallelCaptureDataIntoLong(PinNames, BitOrder.MSBFirst, BitOrder.LSBFirst));
        }

        public static IEnumerable<object[]> LongArrayMatrixData()
        {
            // 70 samples => 2 packed longs per pin.
            yield return new object[]
            {
                BitOrder.MSBFirst,
                BitOrder.LSBFirst,
                new Dictionary<int, uint[]>
                {
                    [0] = CreateAlternatingSamples(70, 0b100u, 0b001u),
                    [1] = CreateAlternatingSamples(70, 0b001u, 0b100u)
                },
                BuildExpectedLongArraysForAlternatingPattern(BitOrder.MSBFirst, BitOrder.LSBFirst, 70)
            };

            yield return new object[]
            {
                BitOrder.MSBFirst,
                BitOrder.MSBFirst,
                new Dictionary<int, uint[]>
                {
                    [0] = CreateAlternatingSamples(70, 0b100u, 0b001u),
                    [1] = CreateAlternatingSamples(70, 0b001u, 0b100u)
                },
                BuildExpectedLongArraysForAlternatingPattern(BitOrder.MSBFirst, BitOrder.MSBFirst, 70)
            };

            yield return new object[]
            {
                BitOrder.LSBFirst,
                BitOrder.LSBFirst,
                new Dictionary<int, uint[]>
                {
                    [0] = CreateAlternatingSamples(70, 0b100u, 0b001u),
                    [1] = CreateAlternatingSamples(70, 0b001u, 0b100u)
                },
                BuildExpectedLongArraysForAlternatingPattern(BitOrder.LSBFirst, BitOrder.LSBFirst, 70)
            };

            yield return new object[]
            {
                BitOrder.LSBFirst,
                BitOrder.MSBFirst,
                new Dictionary<int, uint[]>
                {
                    [0] = CreateAlternatingSamples(70, 0b100u, 0b001u),
                    [1] = CreateAlternatingSamples(70, 0b001u, 0b100u)
                },
                BuildExpectedLongArraysForAlternatingPattern(BitOrder.LSBFirst, BitOrder.MSBFirst, 70)
            };
        }

        [Theory]
        [MemberData(nameof(LongArrayMatrixData))]
        public void PackParallelCaptureDataIntoLongArray_BitOrderMatrix_ReturnsExpectedPerPinPerSite(
            BitOrder pinBitOrder,
            BitOrder sampleBitOrder,
            Dictionary<int, uint[]> inputData,
            long[][][] expectedPerSitePerPinLongArray)
        {
            var siteData = new SiteData<uint[]>(CloneInputData(inputData));

            PinSiteData<long[]> actual = siteData.PackParallelCaptureDataIntoLongArray(PinNames, pinBitOrder, sampleBitOrder);

            AssertPerSitePerPinLongArray(actual, expectedPerSitePerPinLongArray);
        }

        [Fact]
        public void PackParallelCaptureDataIntoLongArray_With64Samples_MatchesPackParallelCaptureDataIntoLong()
        {
            var inputData = new Dictionary<int, uint[]>
            {
                [0] = CreateAlternatingSamples(64, 0b101u, 0b010u),
                [1] = CreateAlternatingSamples(64, 0b001u, 0b110u)
            };
            var siteData = new SiteData<uint[]>(inputData);

            PinSiteData<long> asLong = siteData.PackParallelCaptureDataIntoLong(PinNames, BitOrder.MSBFirst, BitOrder.LSBFirst);
            PinSiteData<long[]> asLongArray = siteData.PackParallelCaptureDataIntoLongArray(PinNames, BitOrder.MSBFirst, BitOrder.LSBFirst);

            for (int siteIndex = 0; siteIndex < SiteNumbers.Length; siteIndex++)
            {
                for (int pinIndex = 0; pinIndex < PinNames.Length; pinIndex++)
                {
                    long[] packed = asLongArray.GetValue(SiteNumbers[siteIndex], PinNames[pinIndex]);
                    Assert.Single(packed);
                    Assert.Equal(asLong.GetValue(SiteNumbers[siteIndex], PinNames[pinIndex]), packed[0]);
                }
            }
        }

        /// <summary>
        /// Creates a deep clone of the matrix test input so each test invocation receives isolated data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// xUnit <c>MemberData</c> passes object references, and this test input is a mutable
        /// <see cref="Dictionary{TKey, TValue}"/> containing mutable <c>uint[]</c> arrays.
        /// Reusing shared instances across test cases can allow accidental mutation to leak between tests.
        /// </para>
        /// <para>
        /// Cloning here prevents cross-test contamination and keeps failures deterministic, especially for
        /// tests that validate input immutability.
        /// </para>
        /// </remarks>
        /// <param name="inputData">Source per-site sample data to clone.</param>
        /// <returns>
        /// A new dictionary instance with cloned <c>uint[]</c> arrays for each site key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="inputData"/> is <see langword="null"/>.</exception>
        private static Dictionary<int, uint[]> CloneInputData(Dictionary<int, uint[]> inputData)
        {
            if (inputData == null)
            {
                throw new ArgumentNullException(nameof(inputData));
            }

            var clone = new Dictionary<int, uint[]>(inputData.Count);
            foreach (KeyValuePair<int, uint[]> kvp in inputData)
            {
                clone[kvp.Key] = (uint[])kvp.Value.Clone();
            }

            return clone;
        }

        private static Dictionary<int, uint[]> CreateBaseInputData()
        {
            return new Dictionary<int, uint[]>
            {
                [0] = new uint[] { 5, 3, 6, 0 },
                [1] = new uint[] { 150, 32, 26, 10 }
            };
        }

        private static void AssertPerSitePerPinUint(PinSiteData<uint[]> actual, uint[][][] expectedPerSitePerPin)
        {
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

        private static void AssertPerSitePerPinBool(PinSiteData<bool[]> actual, uint[][][] expectedPerSitePerPinAsUint)
        {
            for (int siteIndex = 0; siteIndex < SiteNumbers.Length; siteIndex++)
            {
                for (int pinIndex = 0; pinIndex < PinNames.Length; pinIndex++)
                {
                    uint[] expectedUint = expectedPerSitePerPinAsUint[siteIndex][pinIndex];
                    bool[] expectedBool = ConvertBitsToBool(expectedUint);

                    Assert.Equal(
                        expectedBool,
                        actual.GetValue(siteNumber: SiteNumbers[siteIndex], pinName: PinNames[pinIndex]));
                }
            }
        }

        private static void AssertPerSitePerPinLong(PinSiteData<long> actual, long[][] expectedPerSitePerPinLong)
        {
            for (int siteIndex = 0; siteIndex < SiteNumbers.Length; siteIndex++)
            {
                for (int pinIndex = 0; pinIndex < PinNames.Length; pinIndex++)
                {
                    Assert.Equal(
                        expectedPerSitePerPinLong[siteIndex][pinIndex],
                        actual.GetValue(SiteNumbers[siteIndex], PinNames[pinIndex]));
                }
            }
        }

        private static void AssertPerSitePerPinLongArray(PinSiteData<long[]> actual, long[][][] expectedPerSitePerPinLongArray)
        {
            for (int siteIndex = 0; siteIndex < SiteNumbers.Length; siteIndex++)
            {
                for (int pinIndex = 0; pinIndex < PinNames.Length; pinIndex++)
                {
                    Assert.Equal(
                        expectedPerSitePerPinLongArray[siteIndex][pinIndex],
                        actual.GetValue(SiteNumbers[siteIndex], PinNames[pinIndex]));
                }
            }
        }

        private static bool[] ConvertBitsToBool(uint[] bits)
        {
            var expectedBool = new bool[bits.Length];
            for (int i = 0; i < bits.Length; i++)
            {
                expectedBool[i] = bits[i] == 1u;
            }

            return expectedBool;
        }

        private static long[][] PackExpectedBitsPerSitePerPinIntoLong(uint[][][] expectedPerSitePerPinBits)
        {
            int siteCount = expectedPerSitePerPinBits.Length;
            long[][] expected = new long[siteCount][];

            for (int siteIndex = 0; siteIndex < siteCount; siteIndex++)
            {
                int pinCount = expectedPerSitePerPinBits[siteIndex].Length;
                expected[siteIndex] = new long[pinCount];

                for (int pinIndex = 0; pinIndex < pinCount; pinIndex++)
                {
                    expected[siteIndex][pinIndex] = PackBitsIntoLong(expectedPerSitePerPinBits[siteIndex][pinIndex]);
                }
            }

            return expected;
        }

        private static long PackBitsIntoLong(uint[] bits)
        {
            long value = 0;
            for (int i = 0; i < bits.Length; i++)
            {
                value |= ((long)bits[i] & 1L) << i;
            }

            return value;
        }

        private static long[] PackBitsIntoLongArray(uint[] bits)
        {
            int longCount = (bits.Length + 63) / 64;
            long[] values = new long[longCount];

            for (int i = 0; i < bits.Length; i++)
            {
                int longIndex = i / 64;
                int bitPosition = i % 64;
                values[longIndex] |= ((long)bits[i] & 1L) << bitPosition;
            }

            return values;
        }

        private static uint[] CreateAlternatingSamples(int length, uint evenSample, uint oddSample)
        {
            var samples = new uint[length];
            for (int i = 0; i < length; i++)
            {
                samples[i] = (i % 2 == 0) ? evenSample : oddSample;
            }

            return samples;
        }

        private static long[][][] BuildExpectedLongArraysForAlternatingPattern(BitOrder pinBitOrder, BitOrder sampleBitOrder, int sampleCount)
        {
            // Site0 sample pattern: 100,001,100,001...
            // Site1 sample pattern: 001,100,001,100...
            // PinA/PinC swap when pinBitOrder changes. PinB always 0.

            uint[] site0PinA = BuildAlternatingBits(sampleCount, startsWithOne: pinBitOrder == BitOrder.MSBFirst);
            uint[] site0PinB = BuildConstantBits(sampleCount, 0u);
            uint[] site0PinC = BuildAlternatingBits(sampleCount, startsWithOne: pinBitOrder == BitOrder.LSBFirst);

            uint[] site1PinA = BuildAlternatingBits(sampleCount, startsWithOne: pinBitOrder == BitOrder.LSBFirst);
            uint[] site1PinB = BuildConstantBits(sampleCount, 0u);
            uint[] site1PinC = BuildAlternatingBits(sampleCount, startsWithOne: pinBitOrder == BitOrder.MSBFirst);

            if (sampleBitOrder == BitOrder.MSBFirst)
            {
                site0PinA = ReverseCopy(site0PinA);
                site0PinB = ReverseCopy(site0PinB);
                site0PinC = ReverseCopy(site0PinC);

                site1PinA = ReverseCopy(site1PinA);
                site1PinB = ReverseCopy(site1PinB);
                site1PinC = ReverseCopy(site1PinC);
            }

            return new[]
            {
                new[]
                {
                    // PinA
                    PackBitsIntoLongArray(site0PinA),
                    // PinB
                    PackBitsIntoLongArray(site0PinB),
                    // PinC
                    PackBitsIntoLongArray(site0PinC)
                },
                new[]
                {
                    // PinA
                    PackBitsIntoLongArray(site1PinA),
                    // PinB
                    PackBitsIntoLongArray(site1PinB),
                    // PinC
                    PackBitsIntoLongArray(site1PinC)
                }
            };
        }

        private static uint[] BuildAlternatingBits(int length, bool startsWithOne)
        {
            var bits = new uint[length];
            for (int i = 0; i < length; i++)
            {
                bool isEven = (i % 2 == 0);
                bits[i] = (startsWithOne == isEven) ? 1u : 0u;
            }

            return bits;
        }

        private static uint[] BuildConstantBits(int length, uint value)
        {
            var bits = new uint[length];
            for (int i = 0; i < length; i++)
            {
                bits[i] = value;
            }

            return bits;
        }

        private static uint[] ReverseCopy(uint[] input)
        {
            var copy = new uint[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                copy[i] = input[input.Length - 1 - i];
            }

            return copy;
        }
    }
}