using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// This class exposes native methods for the RSeries7822Driver's C API.
    /// </summary>
    internal static class RSeries7822RDriverAPI
    {
        private const string NativeDLLName = "RSeries7822RDriverAPI.dll";
        public const string BitFileName = "RSeries7822R_ReadWriteDigital.lvbitx";

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenFPGA(string resource, string bitFilePath, out ulong referenceID);

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int EnableLoopBack(ulong referenceID, ulong enable);

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteData(ulong referenceID, string portName, byte portData);

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadData(ulong referenceID, string portName, out byte portData);

        [DllImport(NativeDLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseFPGA(ulong referenceID);

        public static string BitFilePath()
        {
            // Removed Assembly.CodeBase because it is obsolete in .NET 8 : https://learn.microsoft.com/en-us/dotnet/fundamentals/syslib-diagnostics/syslib0012
            // Verify the actual on-disk location of the current assembly and use the appropriate path resolution mechanism.
            var assembly = typeof(RSeries7822RDriverAPI).Assembly;
            string assemblyPath = assembly.Location;
            string assemblyDir = Path.GetDirectoryName(assemblyPath);
            return Path.Combine(assemblyDir, BitFileName);
        }
    }
}
