using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Threading;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Utilities
{
    /// <summary>
    /// Resolves vendor (NI/TestStand/IVI) assemblies at runtime that are referenced as loose
    /// <c>&lt;Reference HintPath="..."&gt;</c> entries and are therefore not tracked transitively
    /// via NuGet/.deps.json. This avoids requiring every consuming test project to explicitly
    /// reference or copy-local every transitive dependency of those vendor assemblies.
    /// </summary>
    internal static class AssemblyResolution
    {
        private static readonly ConcurrentDictionary<string, string> ResolvedPathCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, string> ResolvedNativeLibraryPathCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static int _isInitialized;

        /// <summary>
        /// Describes a root directory to probe for vendor assemblies, along with how many levels
        /// of subdirectories under that root are allowed to be searched.
        /// A depth of 0 means only the exact root directory itself is checked (no subdirectories).
        /// A depth of 1 also checks each immediate subdirectory, and so on.
        /// </summary>
        private sealed class ProbingRoot
        {
            public ProbingRoot(Func<string> resolvePath, int maxSubdirectoryDepth = 0)
            {
                ResolvePath = resolvePath;
                MaxSubdirectoryDepth = maxSubdirectoryDepth;
            }

            public Func<string> ResolvePath { get; }

            public int MaxSubdirectoryDepth { get; }
        }

        // Ordered list of root directories to probe. Each path is resolved lazily (env vars may
        // not be set in all environments, e.g., CI agents without TestStand installed).
        // The second argument controls how many levels of subdirectories under that root are
        // searched for a matching assembly file name (0 = exact folder only).
        private static readonly ProbingRoot[] ProbingRoots =
        {
            new ProbingRoot(() => Environment.GetEnvironmentVariable("TestStand64") is string ts ? Path.Combine(ts, "Bin") : null),
            new ProbingRoot(() => Environment.GetEnvironmentVariable("TestStand64") is string ts ? Path.Combine(ts, "API", "DotNET", "Assemblies", "CurrentVersion") : null),
            new ProbingRoot(() => @"C:\Program Files\IVI Foundation\IVI\Microsoft.NET\Framework64\v2.0.50727\IviFoundationSharedComponents 1.4.1"),
            new ProbingRoot(() => @"C:\Program Files\IVI Foundation\IVI\Bin"),
            new ProbingRoot(() => @"C:\Program Files (x86)\National Instruments\MeasurementStudioVS2010\DotNET\Assemblies\Current"),
            // Search up to 2 subdirectory levels deep under this root, since installed NI driver
            // component versions live in version-numbered subfolders .
            new ProbingRoot(() => @"C:\Program Files\IVI Foundation\IVI\Microsoft.NET\Framework64\v4.0.30319", maxSubdirectoryDepth: 2),
        };

        /// <summary>
        /// Registers the resolver exactly once per process, regardless of how many test
        /// assemblies/projects trigger this module initializer.
        /// </summary>
        /// <remarks>
        /// CA2255 is suppressed because this is a test-only utility assembly: the module
        /// initializer is used intentionally here to guarantee the vendor assembly resolver is
        /// registered as soon as this assembly is loaded by any test host, without requiring
        /// every consuming test project to explicitly call an initialization method.
        /// </remarks>
#pragma warning disable CA2255 // The 'ModuleInitializer' attribute is only intended to be used in application code or advanced source generator scenarios.
        [ModuleInitializer]
        public static void Initialize()
#pragma warning restore CA2255
        {
            // Guard against multiple [ModuleInitializer] firings if this assembly is loaded
            // more than once in different contexts.
            if (Interlocked.Exchange(ref _isInitialized, 1) == 1)
            {
                return;
            }

            AssemblyLoadContext.Default.Resolving += ResolveVendorAssembly;
            AssemblyLoadContext.Default.ResolvingUnmanagedDll += ResolveNativeLibrary;
        }

        /// <summary>
        /// Explicitly ensures the resolver is registered. Safe to call multiple times from
        /// multiple call sites; only the first call has any effect. Intended as a defensive
        /// fallback for hosts/call paths that may reach vendor assembly resolution before the
        /// <see cref="ModuleInitializerAttribute"/>-driven <see cref="Initialize"/> has run
        /// (e.g., running a single test directly from Test Explorer).
        /// </summary>
        public static void EnsureInitialized() => Initialize();

        private static Assembly ResolveVendorAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            var name = assemblyName.Name;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (ResolvedPathCache.TryGetValue(name, out var cachedPath))
            {
                return context.LoadFromAssemblyPath(cachedPath);
            }

            foreach (var probingRoot in ProbingRoots)
            {
                var root = SafeResolveRoot(probingRoot.ResolvePath);
                if (root is null)
                {
                    continue;
                }

                var candidatePath = FindAssemblyUnderRoot(root, name + ".dll", probingRoot.MaxSubdirectoryDepth);
                if (candidatePath != null)
                {
                    ResolvedPathCache[name] = candidatePath;
                    return context.LoadFromAssemblyPath(candidatePath);
                }
            }

            return null;
        }

        /// <summary>
        /// Resolves native (unmanaged) libraries loaded via P/Invoke, such as <c>lua54.dll</c>,
        /// which is not a managed assembly and therefore cannot be resolved by
        /// <see cref="AssemblyLoadContext.Resolving"/>. This handler is the native-library
        /// equivalent, using the same set of <see cref="ProbingRoots"/>.
        /// </summary>
        private static IntPtr ResolveNativeLibrary(Assembly assembly, string libraryName)
        {
            if (string.IsNullOrEmpty(libraryName))
            {
                return IntPtr.Zero;
            }

            if (ResolvedNativeLibraryPathCache.TryGetValue(libraryName, out var cachedPath))
            {
                return NativeLibrary.Load(cachedPath);
            }

            // P/Invoke declarations commonly omit the file extension (e.g., [DllImport("lua54")]),
            // so probe both the bare name and the ".dll"-suffixed name.
            var candidateFileNames = libraryName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                ? new[] { libraryName }
                : new[] { libraryName + ".dll", libraryName };

            foreach (var probingRoot in ProbingRoots)
            {
                var root = SafeResolveRoot(probingRoot.ResolvePath);
                if (root is null)
                {
                    continue;
                }

                foreach (var candidateFileName in candidateFileNames)
                {
                    var candidatePath = FindAssemblyUnderRoot(root, candidateFileName, probingRoot.MaxSubdirectoryDepth);
                    if (candidatePath != null && NativeLibrary.TryLoad(candidatePath, out var handle))
                    {
                        ResolvedNativeLibraryPathCache[libraryName] = candidatePath;
                        return handle;
                    }
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Looks for <paramref name="fileName"/> directly under <paramref name="root"/>, and
        /// optionally within subdirectories up to <paramref name="maxSubdirectoryDepth"/> levels
        /// deep. A depth of 0 only checks the root folder itself.
        /// </summary>
        private static string FindAssemblyUnderRoot(string root, string fileName, int maxSubdirectoryDepth)
        {
            var directPath = Path.Combine(root, fileName);
            if (File.Exists(directPath))
            {
                return directPath;
            }

            if (maxSubdirectoryDepth <= 0)
            {
                return null;
            }

            return SearchSubdirectories(root, fileName, maxSubdirectoryDepth);
        }

        private static string SearchSubdirectories(string directory, string fileName, int remainingDepth)
        {
            IEnumerable<string> subdirectories;
            try
            {
                subdirectories = Directory.EnumerateDirectories(directory);
            }
            catch
            {
                // Defensive: directory may disappear or be inaccessible (permissions) between checks.
                return null;
            }

            foreach (var subdirectory in subdirectories)
            {
                var candidatePath = Path.Combine(subdirectory, fileName);
                if (File.Exists(candidatePath))
                {
                    return candidatePath;
                }

                if (remainingDepth > 1)
                {
                    var nestedMatch = SearchSubdirectories(subdirectory, fileName, remainingDepth - 1);
                    if (nestedMatch != null)
                    {
                        return nestedMatch;
                    }
                }
            }

            return null;
        }

        private static string SafeResolveRoot(Func<string> resolver)
        {
            try
            {
                var root = resolver();
                return !string.IsNullOrEmpty(root) && Directory.Exists(root) ? root : null;
            }
            catch
            {
                // Defensive: a resolver might throw (e.g., restricted env var access in some sandboxes).
                return null;
            }
        }
    }
}
