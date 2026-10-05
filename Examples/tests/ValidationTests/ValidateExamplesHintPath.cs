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
        // when this test is run as part of test mode, it tests installed Examples present in this location
        private const string InstalledExamplesRoot = @"C:\Users\Public\Documents\National Instruments\NI_SemiconductorTestLibrary\Examples";

        public static TheoryData<string> GetExampleProjectPaths()
        {
            string[] projectPaths = Directory.GetFiles(InstalledExamplesRoot, "*.csproj", SearchOption.AllDirectories);
            TheoryData<string> data = new TheoryData<string>();
            foreach (string projectPath in projectPaths)
            {
                data.Add(projectPath);
            }
            return data;
        }

        [Theory]
        [MemberData(nameof(GetExampleProjectPaths))]
        public void ValidateExamplesHintPaths_WhenPathsAreMissing_ShouldReportInvalidHintPaths(string projectPath)
        {
            List<string> hintPaths = GetHintPaths(projectPath);
            List<string> invalidHintPaths = hintPaths
                .Select(hintPath => new
                {
                    RawHintPath = hintPath,
                    AbsoluteHintPath = GetAbsoluteHintPaths(hintPath, projectPath)
                })
                .Where(x => !File.Exists(x.AbsoluteHintPath))
                .Select(x => $"{projectPath}: {x.RawHintPath}")
                .ToList();

            Assert.False(invalidHintPaths.Any(), $"Invalid hint paths found:{Environment.NewLine}{string.Join(Environment.NewLine, invalidHintPaths)}");
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