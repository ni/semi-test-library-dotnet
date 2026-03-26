using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.Common
{
    public class HelperMethodsTests
    {
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

        [Fact]
        public void CreateRampSequence_DescendingRampValuesCorrect()
        {
            var seq = HelperMethods.CreateRampSequence(5.0, 1.0, 5);

            Assert.Equal(new[] { 5.0, 4.0, 3.0, 2.0, 1.0 }, seq);
        }

        [Fact]
        public void CreateRampSequenceWithPerSiteArrays_ReturnsCorrectSequencesForEachSite()
        {
            var siteNumbers = new[] { 0, 1, 2 };
            var outputStart = new[] { 0.0, 1.0, 2.0 };
            var outputStop = new[] { 4.0, 5.0, 6.0 };
            var numberOfPoints = new[] { 5, 5, 5 };
            var siteData = HelperMethods.CreateRampSequence(siteNumbers, outputStart, outputStop, numberOfPoints);

            Assert.Equal(new[] { 0.0, 1.0, 2.0, 3.0, 4.0 }, siteData.GetValue(0));
            Assert.Equal(new[] { 1.0, 2.0, 3.0, 4.0, 5.0 }, siteData.GetValue(1));
            Assert.Equal(new[] { 2.0, 3.0, 4.0, 5.0, 6.0 }, siteData.GetValue(2));
        }

        [Fact]
        public void CreateRampSequenceWithPerPinSiteArrays_ReturnsCorrectSequencesForEachPinSite()
        {
            var pinNames = new[] { "P1", "P2" };
            var siteNumbers = new[] { 0, 1 };
            var outputStart = new[] { 0.0, 1.0 };
            var outputStop = new[] { 2.0, 3.0 };
            var numberOfPoints = new[] { 3, 3 };
            var pinSiteData = HelperMethods.CreateRampSequence(pinNames, siteNumbers, outputStart, outputStop, numberOfPoints);

            Assert.Equal(new[] { 0.0, 1.0, 2.0 }, pinSiteData.GetValue(0, "P1"));
            Assert.Equal(new[] { 1.0, 2.0, 3.0 }, pinSiteData.GetValue(1, "P2"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void CreateRampSequenceWithZeroOrNegativeNumberOfPoints_ThrowsException(int numberOfPoints)
        {
            var exception = Assert.Throws<NISemiconductorTestException>(() =>
                HelperMethods.CreateRampSequence(0.0, 1.0, numberOfPoints));

            Assert.Contains("Number of points must be greater than one", exception.Message);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void CreateRampSequenceWithInvalidOutputStart_ThrowsException(double outputStart)
        {
            var exception = Assert.Throws<NISemiconductorTestException>(() =>
                HelperMethods.CreateRampSequence(outputStart, 1.0, 10));

            Assert.Contains("Output Start value must be a finite number", exception.Message);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void CreateRampSequenceWithInvalidOutputStop_ThrowsException(double outputStop)
        {
            var exception = Assert.Throws<NISemiconductorTestException>(() =>
                HelperMethods.CreateRampSequence(0.0, outputStop, 10));

            Assert.Contains("Output Stop value must be a finite number", exception.Message);
        }
    }
}