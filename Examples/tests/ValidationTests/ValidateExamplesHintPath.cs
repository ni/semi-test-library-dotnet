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
            string sourceFolderPath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "..",
                "..",
                "..",
                "source"));
            string[] projectPaths = Directory.GetFiles(sourceFolderPath, "*.csproj", SearchOption.AllDirectories);
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
            List<string> absoluteHintPaths = GetAbsoluteHintPaths(hintPaths, projectPath);
            List<string> invalidHintPaths = new List<string>();

            for (int index = 0; index < absoluteHintPaths.Count; index++)
            {
                string absoluteHintPath = absoluteHintPaths[index];
                if (!File.Exists(absoluteHintPath))
                {
                    invalidHintPaths.Add(projectPath + ": " + hintPaths[index]);
                }
            }

            Assert.False(invalidHintPaths.Any(), $"Invalid hint paths found:{Environment.NewLine}{string.Join(Environment.NewLine, invalidHintPaths)}");
        }

        private static List<string> GetHintPaths(string projectPath)
        {
            XDocument projectXml = XDocument.Load(projectPath);
            List<string> hintPaths = new List<string>();
            List<XElement> referenceNodes = projectXml.Descendants()
                .Where(node => node.Name.LocalName == "Reference")
                .ToList();

            foreach (XElement referenceNode in referenceNodes)
            {
                List<XElement> hintNodes = referenceNode.Elements()
                    .Where(node => node.Name.LocalName == "HintPath")
                    .ToList();

                foreach (XElement hintNode in hintNodes)
                {
                    string rawHintPath = hintNode.Value.Trim();
                    if (!string.IsNullOrEmpty(rawHintPath))
                    {
                        hintPaths.Add(rawHintPath);
                    }
                }
            }
            return hintPaths;
        }

        private static List<string> GetAbsoluteHintPaths(List<string> hintPaths, string projectPath)
        {
            string projectDirectory = Path.GetDirectoryName(projectPath);
            List<string> absoluteHintPaths = new List<string>();
            foreach (string rawHintPath in hintPaths)
            {
                string resolvedPath = Path.IsPathRooted(rawHintPath)
                    ? rawHintPath
                    : Path.Combine(projectDirectory, rawHintPath);

                absoluteHintPaths.Add(Path.GetFullPath(resolvedPath));
            }
            return absoluteHintPaths;
        }
    }
}