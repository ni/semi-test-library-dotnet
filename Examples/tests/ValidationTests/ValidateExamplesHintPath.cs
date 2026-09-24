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
        public static TheoryData<string> GetExampleProjectPaths()
        {
            var sourceFolderPath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "..",
                "..",
                "source"));
            var projectPaths = Directory.GetFiles(sourceFolderPath, "*.csproj", SearchOption.AllDirectories);
            var data = new TheoryData<string>();
            foreach (var projectPath in projectPaths)
            {
                data.Add(projectPath);
            }
            return data;
        }

        [Theory]
        [MemberData(nameof(GetExampleProjectPaths))]
        public void ValidateExamplesHintPaths_WhenPathsAreMissing_ShouldReportInvalidHintPaths(string projectPath)
        {
            var projectDirectory = Path.GetDirectoryName(projectPath);
            var projectXml = XDocument.Load(projectPath);
            var hintPaths = GetHintPaths(projectXml);
            var invalidHintPaths = GetInvalidHintPaths(hintPaths, projectDirectory, projectPath);

            Assert.False(invalidHintPaths.Any(), $"Invalid hint paths found:{Environment.NewLine}{string.Join(Environment.NewLine, invalidHintPaths)}");
        }

        private static List<string> GetHintPaths(XDocument projectXml)
        {
            var hintPaths = new List<string>();
            var referenceNodes = projectXml.Descendants().Where(node => node.Name.LocalName == "Reference");

            foreach (var referenceNode in referenceNodes)
            {
                var hintNodes = referenceNode.Elements().Where(node => node.Name.LocalName == "HintPath");

                foreach (var hintNode in hintNodes)
                {
                    var rawHintPath = hintNode.Value.Trim();
                    if (!string.IsNullOrEmpty(rawHintPath))
                    {
                        hintPaths.Add(rawHintPath);
                    }
                }
            }
            return hintPaths;
        }
        private static List<string> GetInvalidHintPaths(List<string> hintPaths, string projectDirectory, string projectPath)
        {
            var invalidHintPaths = new List<string>();
            foreach (var rawHintPath in hintPaths)
            {
                if (!string.IsNullOrEmpty(rawHintPath))
                {
                     var expandedHintPath = Environment.ExpandEnvironmentVariables(rawHintPath);
                     var resolvedPath = Path.IsPathRooted(expandedHintPath)
                    ? expandedHintPath
                    : Path.Combine(projectDirectory, expandedHintPath);

                    if (!File.Exists(Path.GetFullPath(resolvedPath)))
                    {
                        invalidHintPaths.Add(projectPath + ": " + rawHintPath);
                    }
                }
            }
            return invalidHintPaths;
        }
    }
}