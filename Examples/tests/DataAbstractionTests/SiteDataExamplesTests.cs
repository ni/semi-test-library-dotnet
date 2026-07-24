using Xunit;
using static NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.DataAbstraction.SiteDataExamples;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.DataAbstractionTests
{
    public class SiteDataExamplesTests
    {
        [Fact]
        public void ConstructWithDefaultConstructor_Succeeds()
        {
            ConstructWithDefaultConstructor();
        }

        [Fact]
        public void ConstructWithSingleSiteNumber_Succeeds()
        {
            ConstructWithSingleSiteNumber();
        }
    }
}
