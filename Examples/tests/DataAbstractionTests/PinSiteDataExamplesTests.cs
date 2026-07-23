using Xunit;
using static NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.DataAbstraction.PinSiteDataExamples;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.DataAbstractionTests
{
    
public class PinSiteDataExamplesTests
    {
        [Fact]
        public void ConstructWithDefaultConstructor_Succeeds()
        {
            ConstructWithDefaultConstructor();
        }

        [Fact]
        public void ConstructWithSinglePinNameAndSiteNumbers_Succeeds()
        {
            ConstructWithSinglePinNameAndSiteNumbers();
        }

        [Fact]
        public void ConstructWithPinNamesAndSiteNumbers_Succeeds()
        {
            ConstructWithPinNamesAndSiteNumbers();
        }
    }
}
