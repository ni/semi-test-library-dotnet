﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Functionality.HintPathValidation
{
    public class ValidateExamplesHintPath
    {
        public static IEnumerable<object[]> GetExampleProjectPaths()
        {
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (current != null)
            {
                var hasSourceFolder = Directory.Exists(Path.Combine(current.FullName, "source"));
                var hasTestsFolder = Directory.Exists(Path.Combine(current.FullName, "tests"));
                if (hasSourceFolder && hasTestsFolder)
                {
                    break;
                }
                current = current.Parent;
            }
            var sourceFolderPath = Path.Combine(current.FullName, "source");
            var projectPaths = Directory.GetFiles(sourceFolderPath, "*.csproj", SearchOption.AllDirectories);

            foreach (var projectPath in projectPaths)
            {
                yield return new object[] { projectPath };
            }
        }

        [Theory]
        [MemberData(nameof(GetExampleProjectPaths))]
        public void ValidateExamplesHintPaths_WhenPathsAreMissing_ShouldReportInvalidHintPaths(string projectPath)
        {
            var invalidHintPaths = ValidateExampleHintPaths(projectPath);
            Assert.True(invalidHintPaths.Count == 0, string.Join(Environment.NewLine, invalidHintPaths));
        }

        private static List<string> ValidateExampleHintPaths(string projectPath)
        {
            var invalidHintPaths = new List<string>();
            var projectDirectory = Path.GetDirectoryName(projectPath);
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            var projectXml = XDocument.Load(projectPath);

            var referenceNodes = projectXml.Descendants().Where(node => node.Name.LocalName == "Reference");
            foreach (var referenceNode in referenceNodes)
            {
                var includeName = ((string)referenceNode.Attribute("Include") ?? string.Empty).Trim();
                var referenceName = string.IsNullOrEmpty(includeName) ? "<missing>" : includeName;

                var hintNodes = referenceNode.Elements().Where(node => node.Name.LocalName == "HintPath").ToList();
                if (hintNodes.Count == 0)
                {
                    continue;
                }

                foreach (var hintNode in hintNodes)
                {
                    var rawHintPath = hintNode.Value.Trim();
                    if (string.IsNullOrEmpty(rawHintPath))
                    {
                        continue;
                    }

                    var expandedHintPath = Environment.ExpandEnvironmentVariables(rawHintPath);
                    var resolvedPath = Path.IsPathRooted(expandedHintPath)
                        ? expandedHintPath
                        : Path.Combine(projectDirectory, expandedHintPath);

                    if (!File.Exists(Path.GetFullPath(resolvedPath)))
                    {
                        invalidHintPaths.Add(string.Format(
                            "Project: {0}; HintPath: {1}; Reason: Reference '{2}' points to a file that does not exist.",
                            projectName,
                            rawHintPath,
                            referenceName));
                    }
                }
            }
            return invalidHintPaths;
        }
    }
}