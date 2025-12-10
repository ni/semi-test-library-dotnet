using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries7822R
{
    /// <summary>
    /// This class exposes native methods for the RSeries7822Driver's C API.
    /// </summary>
    internal static class RSeries7822RDriverAPI
    {
        private const string NativeDLLName = "RSeries7822RDriverAPI.dll";
        public const string BitFileName = "RSeries7822R_ReadWriteDigitalPorts.lvbitx";

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenFPGA(string resource, string bitFilePath, out ulong referenceID);

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int EnableLoopBack(ulong referenceID, ulong enable);

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteData(ulong referenceID, string channelName, byte channelData);

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadData(ulong referenceID, string channelName, out byte channelData);

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseFPGA(ulong referenceID);

        public static string BitFilePath()
        {
            // Get the assembly that defines this class and get its location.
            var assembly = Assembly.GetAssembly(typeof(RSeries7822RDriverAPI));
            string assemblyPath;
            if (!string.IsNullOrEmpty(assembly.CodeBase))
            {
                assemblyPath = new Uri(assembly.CodeBase).LocalPath;
            }
            else
            {
                assemblyPath = assembly.Location;
            }

            string assemblyDir = Path.GetDirectoryName(assemblyPath);
            return Path.Combine(assemblyDir, BitFileName);
        }
    }
}
