using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.Common
{
    public class HelperMethodsTests
    {
        [Theory]
        [InlineData(0.0, 1.0, 5)]
        [InlineData(-1.0, 1.0, 3)]
        [InlineData(2.5, 2.5, 1)]
        public void CreateRampSequence_RampValuesAreCorrect(double start, double stop, int points)
        {
            var seq = HelperMethods.CreateRampSequence(start, stop, points);

            Assert.NotNull(seq);
            Assert.Equal(points, seq.Length);
            Assert.Equal(start, seq.First());
            Assert.Equal(stop, seq.Last());
            if (points == 1)
            {
                Assert.Equal(start, seq[0]);
            }
            double expectedStep = 0.0;
            if (points > 1)
            {
                expectedStep = (stop - start) / (points - 1);
            }
            for (int i = 0; i < points; i++)
            {
                Assert.Equal(start + ((double)i * expectedStep), seq[i]);
            }
        }

        [Fact]
        public void CreateRampSequence_RampWithTwoPointsProducesEndsOnly()
        {
            var seq = HelperMethods.CreateRampSequence(5.0, 9.0, 2);

            Assert.Equal(new[] { 5.0, 9.0 }, seq);
        }

        [Fact]
        public void CreateRampSequenceForSiteData_AllSitesContainSameRampReference()
        {
            var sites = new[] { 0, 1, 2 };
            var siteData = HelperMethods.CreateRampSequence(sites, 0.0, 1.0, 4);

            var firstSiteSequence = siteData.GetValue(0);
            Assert.NotNull(firstSiteSequence);
            Assert.Equal(4, firstSiteSequence.Length);
            foreach (var site in sites)
            {
                var seq = siteData.GetValue(site);
                Assert.Same(firstSiteSequence, seq);
            }
        }

        [Fact]
        public void CreateRampSequenceForPinSiteData_AllPinsAllSitesContainSameRampReference()
        {
            var pinNames = new[] { "P1", "P2" };
            var sites = new[] { 0, 1 };
            var pinSiteData = HelperMethods.CreateRampSequence(pinNames, sites, -2.0, 2.0, 5);

            var seqP1Site0 = pinSiteData.GetValue(0, "P1");
            Assert.Equal(5, seqP1Site0.Length);
            Assert.Equal(-2.0, seqP1Site0.First());
            Assert.Equal(2.0, seqP1Site0.Last());
            foreach (var pin in pinNames)
            {
                foreach (var site in sites)
                {
                    Assert.Same(seqP1Site0, pinSiteData.GetValue(site, pin));
                }
            }
        }

        [Theory]
        [InlineData(10.0, 10.0)]
        [InlineData(-5.0, -5.0)]
        public void CreateRampSequence_SinglePointRampProducesConstant(double start, double stop)
        {
            var seq = HelperMethods.CreateRampSequence(start, stop, 1);

            Assert.Single(seq);
            Assert.Equal(start, seq[0]);
        }

        [Fact]
        public void CreateRampSequence_DescendingRampValuesCorrect()
        {
            var seq = HelperMethods.CreateRampSequence(5.0, 1.0, 5);

            Assert.Equal(new[] { 5.0, 4.0, 3.0, 2.0, 1.0 }, seq);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void CreateRampSequence_ZeroOrNegativeNumberOfPoints_ThrowsArgumentException(int numberOfPoints)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HelperMethods.CreateRampSequence(0.0, 1.0, numberOfPoints));

            Assert.Contains("Number of points must be greater than zero", exception.Message);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void CreateRampSequence_InvalidOutputStart_ThrowsArgumentException(double outputStart)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HelperMethods.CreateRampSequence(outputStart, 1.0, 10));

            Assert.Contains("Output Start value must be a finite number", exception.Message);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void CreateRampSequence_InvalidOutputStop_ThrowsArgumentException(double outputStop)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HelperMethods.CreateRampSequence(0.0, outputStop, 10));

            Assert.Contains("Output Stop value must be a finite number", exception.Message);
        }
    }
}