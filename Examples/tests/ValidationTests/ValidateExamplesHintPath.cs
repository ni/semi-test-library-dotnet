﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Functionality.Example
{
    public class ValidateExamplesHintPath
    {
        // TestMode ATS runs require evaluating examples from the installed examples directory.
        private const string InstalledExamplesRoot = @"C:\Users\Public\Documents\National Instruments\NI_SemiconductorTestLibrary\Examples";

        public static TheoryData<string> GetExampleProjectPaths()
        {
            var data = new TheoryData<string>();

            foreach (var file in Directory.GetFiles(
                InstalledExamplesRoot,
                "*.csproj",
                SearchOption.AllDirectories))
            {
                data.Add(file);
            }
            
            return data;
        }

        [Theory]
        [MemberData(nameof(GetExampleProjectPaths))]
        public void ValidateExamplesHintPaths_WhenPathsAreMissing_ShouldReportInvalidHintPaths(string projectPath)
        {
            List<string> hintPaths = GetHintPaths(projectPath);

            List<string> invalidHintPaths = hintPaths
                .Where(hintPath => !File.Exists(GetAbsoluteHintPaths(hintPath, projectPath)))
                .Select(hintPath => $"{projectPath}: {hintPath}")
                .ToList();

            Assert.True(invalidHintPaths.Count == 0, $"Invalid hint paths found:{Environment.NewLine}{string.Join(Environment.NewLine, invalidHintPaths)}");
        }

        private static List<string> GetHintPaths(string projectPath)
        {
            XDocument projectXml = XDocument.Load(projectPath);

            return projectXml.Descendants()
               .Where(node => node.Name.LocalName == "Reference")
               .Elements()
               .Where(node => node.Name.LocalName == "HintPath")
               .Select(node => node.Value.Trim())
               .Where(path => !string.IsNullOrEmpty(path))
               .ToList();
        }

        private static string GetAbsoluteHintPaths(string hintPath, string projectPath)
        {
            string projectDirectory = Path.GetDirectoryName(projectPath);
            return Path.GetFullPath(
                    Path.IsPathRooted(hintPath)
                    ? hintPath
                    : Path.Combine(projectDirectory, hintPath));
        }
    }
}
