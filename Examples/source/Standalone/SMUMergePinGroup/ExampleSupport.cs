using System;
using System.IO;
using System.Reflection;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.NIDCPower
{
    /// <summary>
    /// Provides support methods for standalone examples in the Semiconductor Test Library.
    /// </summary>
    public static class ExampleSupport
    {
        /// <summary>
        /// Gets the directory path to the executable for the example.
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
        /// Creates a standalone instance of the ISemiconductorModuleContext,
        /// for use outside of a TestStand sequence file context.
        /// </summary>
        /// <param name="pinMapFileName">The pin map file name.</param>
        /// <returns>The <see cref="ISemiconductorModuleContext"/> object.</returns>
        public static ISemiconductorModuleContext CreateStandaloneTSMContext(string pinMapFileName)
        {
            return CreateTSMContext(pinMapFileName, out _);
        }

        /// <summary>
        /// Creates a standalone instance of the ISemiconductorModuleContext,
        /// for use outside of a TestStand sequence file context.
        /// </summary>
        /// <param name="pinMapFileName">The pin map file name.</param>
        /// <param name="publishedDataReader">The <see cref="IPublishedDataReader"/> object used to query published data.</param>
        /// <returns>The <see cref="ISemiconductorModuleContext"/> object.</returns>
        public static ISemiconductorModuleContext CreateTSMContext(string pinMapFileName, out IPublishedDataReader publishedDataReader)
        {
            PublishedDataReaderFactory publishedDataReaderFactory = new PublishedDataReaderFactory();
            string pinMapFilePath = Path.Combine(Path.GetDirectoryName(ExecutableDirectory), pinMapFileName);
            var tsmContext = publishedDataReaderFactory.NewSemiconductorModuleContext(pinMapFilePath, out publishedDataReader);
            return SemiconductorModuleContextFactory.ConstructSemiconductorModuleContextForDotNet(tsmContext);
        }
    }
}