using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NationalInstruments.Example.CustomInstrument.RSeries7822DriverAPI
{
    /// <summary>
    /// This class exposes methods from Rseries CAPI.
    /// </summary>
    internal static class RSeriesCAPI
    {
        private const string NativeDLLName = "RSeries7822R_ReadWriteDigitalPorts_CAPI.dll";
        public const string BitFileName = "FPGARSeriesExample.lvbitx";

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
            var assembly = Assembly.GetAssembly(typeof(RSeriesCAPI));
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
