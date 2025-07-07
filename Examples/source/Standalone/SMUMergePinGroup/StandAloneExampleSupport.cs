using System;
using System.IO;
using System.Reflection;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.StandAlone
{
    public static class StandAloneExampleSupport
    {
        /// <summary>
        /// The directory path to the executable for the example.
        /// </summary>
        public static string ExecutableDirectory
        {
            get
            {
                Uri assemblyLocationURI = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                string assemblyPath = Uri.UnescapeDataString(assemblyLocationURI.AbsolutePath);
                return Path.GetDirectoryName(assemblyPath);
            }
        }
        /// <summary>
        /// The directory path for the supporting source materials, such as the pin map and digital project files.
        /// This currently assumes the path is one-up from the executable path.
        /// This may be different when copying/reusing this file for a different project.
        /// </summary>
        public static string SupportingMaterialsDirectory => Path.GetDirectoryName(ExecutableDirectory);

        /// <summary>
        /// Creates a Standalone instance of the ISemiconductorModuleContext,
        /// for use outside of a TestStand sequence file context.
        /// </summary>
        /// <param name="pinMapFileName">The pin map file name.</param>
        /// <param name="digitalPatternProjectFileName">The pin map file name.</param>
        /// <returns>The <see cref="ISemiconductorModuleContext"/> object,</returns>
        public static ISemiconductorModuleContext CreateStandAloneSemiconductorModuleContext(string pinMapFileName, string digitalPatternProjectFileName = null)
        {
            return CreateStandAloneSemiconductorModuleContext(pinMapFileName, out _, digitalPatternProjectFileName);
        }

        /// <inheritdoc cref="CreateTSMContext(string, string)"/>
        /// <param name="publishedDataReader">The <see cref="IPublishedDataReader"/> object used to query published data.</param>
        public static ISemiconductorModuleContext CreateStandAloneSemiconductorModuleContext(string pinMapFileName, out IPublishedDataReader publishedDataReader, string digitalPatternProjectFileName = null)
        {
            PublishedDataReaderFactory publishedDataReaderFactory = new PublishedDataReaderFactory();
            string pinMapFilePath = Path.Combine(SupportingMaterialsDirectory, pinMapFileName);
            string digitalPatternProjectFilePath = string.IsNullOrEmpty(digitalPatternProjectFileName)
                ? null
                : Path.Combine(SupportingMaterialsDirectory, digitalPatternProjectFileName);
            var tsmContext = publishedDataReaderFactory.NewSemiconductorModuleContext(pinMapFilePath, digitalPatternProjectFilePath, specificationsFilePaths: null, out publishedDataReader);
            return SemiconductorModuleContextFactory.ConstructSemiconductorModuleContextForDotNet(tsmContext);
        }
    }
}