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

        [Fact]
        public void BuildWithWithArraysAndSetValueWithPerPinData_Succeeds()
        {
            BuildWithWithArraysAndSetValueWithPerPinData();
        }

        [Fact]
        public void BuildWithArraysAndSetValueWithPerPinPerSiteData_Succeeds()
        {
            BuildWithArraysAndSetValueWithPerPinPerSiteData();
        }

        [Fact]
        public void BuildWithArraysWithSystemPin_Succeeds()
        {
            BuildWithArraysWithSystemPin();
        }

        [Fact]
        public void BuildWithDictionaryWithSystemPin_Succeeds()
        {
            BuildWithDictionaryWithSystemPin();
        }

        [Fact]
        public void BuildWithPinDataDictionaryAndSiteNumbersArray_Succeeds()
        {
            BuildWithPinDataDictionaryAndSiteNumbersArray();
        }

        [Fact]
        public void BuildWithPinDataDictionaryAndSiteNumbersArrayWithSystemPin_Succeeds()
        {
            BuildWithPinDataDictionaryAndSiteNumbersArrayWithSystemPin();
        }

        [Fact]
        public void BuildWithArraysForCommonDataValue_Succeeds()
        {
            BuildWithArraysForCommonDataValue();
        }

        [Fact]
        public void BuildWithPinUniqueDataArray_Succeeds()
        {
            BuildWithPinUniqueDataArray();
        }

        [Fact]
        public void BuildWithSiteUniqueDataArray_Succeeds()
        {
            BuildWithSiteUniqueDataArray();
        }

        [Fact]
        public void BuildWithPinAndSiteUniqueDataArray_Succeeds()
        {
            BuildWithPinAndSiteUniqueDataArray();
        }

        [Fact]
        public void BuildWithSiteAndPinUniqueDataArray_Succeeds()
        {
            BuildWithSiteAndPinUniqueDataArray();
        }
    }
}
