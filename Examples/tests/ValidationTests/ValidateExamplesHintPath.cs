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
            string FindExamplesRoot()
            {
                var current = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (current != null)
                {
                    var hasSourceFolder = Directory.Exists(Path.Combine(current.FullName, "source"));
                    var hasTestsFolder = Directory.Exists(Path.Combine(current.FullName, "tests"));
                    if (hasSourceFolder && hasTestsFolder)
                    {
                        return current.FullName;
                    }

                    current = current.Parent;
                }

                throw new DirectoryNotFoundException("Could not locate Examples root from current working directory.");
            }

            var examplesRoot = FindExamplesRoot();
            var projectsRoot = Path.Combine(examplesRoot, "source");
            if (!Directory.Exists(projectsRoot))
            {
                throw new DirectoryNotFoundException(string.Format("Examples source root not found: {0}", projectsRoot));
            }

            foreach (var projectPath in Directory.GetFiles(projectsRoot, "*.csproj", SearchOption.AllDirectories))
            {
                yield return new object[] { projectPath };
            }
        }

        [Theory]
        [MemberData(nameof(GetExampleProjectPaths))]
        public void ValidateExamplesHintPaths_WhenPathsAreMissing_ShouldFailPerExample(string projectPath)
        {
            var issues = ValidateExampleHintPaths(projectPath);
            Assert.True(issues.Count == 0, BuildIssueReport(projectPath, issues));
        }

        private static List<HintPathIssue> ValidateExampleHintPaths(string projectPath)
        {
            var issues = new List<HintPathIssue>();
            var projectDirectory = Path.GetDirectoryName(projectPath);
            var projectXml = XDocument.Load(projectPath);

            void AddIssue(string include, string hintPath, string details)
            {
                issues.Add(new HintPathIssue
                {
                    Include = include,
                    HintPath = hintPath,
                    Details = details,
                });
            }

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
                        AddIssue(referenceName, rawHintPath, "HintPath target file does not exist.");
                    }
                }
            }

            return issues;
        }
        private static string BuildIssueReport(string projectPath, IReadOnlyCollection<HintPathIssue> issues)
        {
            string EscapeCsv(string value)
            {
                var text = value ?? string.Empty;
                if (text.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0)
                {
                    return "\"" + text.Replace("\"", "\"\"") + "\"";
                }

                return text;
            }

            var exampleName = Path.GetFileNameWithoutExtension(projectPath);

            var lines = new List<string>
            {
                "ExampleName,ReferenceName,HintPath,Issue",
            };

            foreach (var issue in issues)
            {
                lines.Add(string.Format(
                    "{0},{1},{2},{3}",
                    EscapeCsv(exampleName),
                    EscapeCsv(issue.Include),
                    EscapeCsv(issue.HintPath),
                    EscapeCsv(issue.Details)));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private sealed class HintPathIssue
        {
            public string Include { get; set; }

            public string HintPath { get; set; }

            public string Details { get; set; }
        }
    }
}