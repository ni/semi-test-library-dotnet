# Semiconductor Test Library

The Semiconductor Test Library simplifies programming on the NI [Semiconductor Test System (STS)](https://www.ni.com/sts) and enables users to develop test programs efficiently using C#/.NET. Refer to [Overview](UserGuide/Overview.md) for more details.

## Latest Release

For the latest release, visit the [GitHub Releases](https://github.com/ni/semi-test-library-dotnet/releases) page.

## Examples and Source Code

For examples and source code, visit the [semi-test-library-dotnet](https://github.com/ni/semi-test-library-dotnet) repository in GitHub.

## Software Requirements

You must have the following software to use the Semiconductor Test Library:

- STS Software 24.5 or later
- .NET Framework 4.8 or later

Visual Studio 2022 is highly recommended.

Refer to the [STS Software Version Compatibility](UserGuide/Overview.md#sts-software-version-compatibility) table for more details.

## Getting Started

To get started, use the STS Project Creation Tool included with STS Software 24.5 (or later) to create a new test program using the NI Default - C#/.NET template. The template program includes and references a copy of the NationalInstruments.SemiconductorTestLibrary NuGet package. NI recommends using this template test program as a starting point to use the Semiconductor Test Library in new projects.

To add or upgrade to the latest NationalInstruments.SemiconductorTestLibrary NuGet package for an existing test program, download the latest NuGet package file from [GitHub Releases page](https://github.com/ni/semi-test-library-dotnet/releases), Refer to [Adding Nuget Packages](UserGuide/NuGetPackageManagementForSTSProjects.md#adding-nuget-packages) or [Upgrading NuGet Packages](UserGuide/NuGetPackageManagementForSTSProjects.md#upgrading-nuget-packages) for more details.

Refer to the [Overview](UserGuide/Overview.md) for more information.

**Related Information:**

- [Creating an STS Project](https://ni.com/docs/en-US/bundle/sts-t4-m2/page/create-sts-project.html)
- [STS Project Creation Tool](https://ni.com/docs/en-US/bundle/sts-ms-auxiliary-tools/page/project-creation-tool.html)
