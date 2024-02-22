using System.IO;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
using static NationalInstruments.Tests.Utilities.PathManagement;

namespace NationalInstruments.Tests.Utilities
{
    public static class TSMContext
    {
        public static ISemiconductorModuleContext CreateTSMContext(string pinMapFileName, string digitalPatternProjectFileName = null)
        {
            return CreateTSMContext(pinMapFileName, out _, digitalPatternProjectFileName);
        }

        public static ISemiconductorModuleContext CreateTSMContext(string pinMapFileName, out IPublishedDataReader publishedDataReader, string digitalPatternProjectFileName = null)
        {
            var publishedDataReaderFactory = new PublishedDataReaderFactory();
            string pinMapFilePath = Path.Combine(SupportingMaterialsDirectory, "Pin Maps", pinMapFileName);
            string digitalPatternProjectFilePath = string.IsNullOrEmpty(digitalPatternProjectFileName)
                ? null
                : Path.Combine(SupportingMaterialsDirectory, digitalPatternProjectFileName);
            var tsmContext = publishedDataReaderFactory.NewSemiconductorModuleContext(pinMapFilePath, digitalPatternProjectFilePath, specificationsFilePaths: null, out publishedDataReader);
            return SemiconductorModuleContextFactory.ConstructSemiconductorModuleContextForDotNet(tsmContext);
        }
    }
}
