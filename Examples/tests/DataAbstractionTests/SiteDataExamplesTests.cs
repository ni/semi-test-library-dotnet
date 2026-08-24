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

        [Fact]
        public void BuildWithArray_Succeeds()
        {
            BuildWithArray();
        }

        [Fact]
        public void BuildWithPerSiteDataDictionary_Succeeds()
        {
            BuildWithPerSiteDataDictionary();
        }

        [Fact]
        public void BuildWithDictionaryWithSystemData_Succeeds()
        {
            BuildWithDictionaryWithSystemData();
        }

        [Fact]
        public void BuildWithSingleValue_Succeeds()
        {
            BuildWithSingleValue();
        }

        [Fact]
        public void BuildWithSiteUniqueDataArray_Succeeds()
        {
            BuildWithSiteUniqueDataArray();
        }
    }
}
