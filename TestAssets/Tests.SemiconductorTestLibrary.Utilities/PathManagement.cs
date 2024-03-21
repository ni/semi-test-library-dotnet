using System;
using System.IO;
using System.Reflection;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Utilities
{
    public static class PathManagement
    {
        private const string SupportingMaterialsDirectoryName = @"TestAssets\Supporting Materials";
        private static readonly string _waveformsDirectoryName = Path.Combine("RF", "Waveforms");

        public static string SupportingMaterialsDirectory
        {
            get
            {
                var path = TestsBaseDirectory;
                return string.IsNullOrEmpty(path) ? string.Empty : Path.Combine(path, SupportingMaterialsDirectoryName);
            }
        }

        public static string GetWaveformFilePath(string waveformName)
        {
            return Path.Combine(WaveformsDirectory, Path.ChangeExtension(waveformName, "tdms"));
        }

        public static string WaveformsDirectory => Path.Combine(SupportingMaterialsDirectory, _waveformsDirectoryName);

        public static string TestsBaseDirectory => GetParentPathOfTheSpecifiedDirectory(TestAssemblyDirectory, SupportingMaterialsDirectoryName);

        private static string TestAssemblyDirectory
        {
            get
            {
                var assemblyLocationURI = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                var assemblyPath = Uri.UnescapeDataString(assemblyLocationURI.AbsolutePath);
                return Path.GetDirectoryName(assemblyPath);
            }
        }

        private static string GetParentPathOfTheSpecifiedDirectory(string path, string directoryName)
        {
            while (!string.IsNullOrEmpty(path) && !Directory.Exists(Path.Combine(path, directoryName)))
            {
                path = Path.GetDirectoryName(path);
            }

            return path;
        }
    }
}
