# Semiconductor Test Library for DotNet

The [semi-test-library-dotnet](https://github.com/ni/semi-test-library-dotnet) repository contains the source code for the Semiconductor Test Library written in C# using the .NET Framework.

For the latest release, visit the [GitHub Releases](https://github.com/ni/semi-test-library-dotnet/releases) page.

- [About](#about)
- [Software Requirements](#software-requirements)
- [Documentation](#documentation)
- [Support](#support)
- [Bugs / Feature Requests](#bugs--feature-requests)
- [Contributing](#contributing)
- [License](#license)

## About

The Semiconductor Test Library simplifies programming on the NI [Semiconductor Test System (STS)](https://www.ni.com/sts) and enables users to develop test programs efficiently using C#/.NET.

The Semiconductor Test Library includes the following high-level features:

- Interfaces and classes—Abstract instrument sessions and encapsulate the necessary pin and site awareness.
- Pin- and site-aware data types— Simplify instrument configuration and measurement results processing.
- Extension methods—Abstract common, high-level instrument operations.
- Parallelization methods—Abstract parallel for loops required to iterate over multiple instrument sessions regardless of how sessions map to pins or sites.
- Publishing methods—Simplify results publishing and add support for the SiteData and PinSiteData types.
- Utilities methods—Provide utility methods commonly required for writing test code.
- TestStand step types—Perform common operations, such as setting up and closing instruments, powering up a DUT, or executing common tests.

## Software Requirements

You must have the following software to use the Semiconductor Test Library:

- STS Software 24.5 or later
- .NET Framework 4.8 or later

Visual Studio 2022 is highly recommended.

Refer to the [STS Software Version Compatibility](docs/UserGuide/Overview.md#sts-software-version-compatibility) table in user guide for more details.

## Documentation

A complete set of documentation for the Semiconductor Test Library can be found here: [https://ni.github.io/semi-test-library-dotnet](https://ni.github.io/semi-test-library-dotnet)

## Support

- To report bug or make feature request specific to the Semiconductor Test Library, follow the process in [Bugs / Feature Requests](#bugs--feature-requests) section below.
- For general support using the Semiconductor Test Library contact [NI Support](https://ni.com/ask).

## Bugs / Feature Requests

To report a bug or submit a feature request specific to the Semiconductor Test Library, use the [GitHub Issues](https://github.com/ni/semi-test-library-dotnet/issues) page.

Fill in the issue template as completely as possible and we will respond as soon as we can.

## Contributing

We welcome contributions! Follow these [instructions](CONTRIBUTING.md) to get started and contribute to the project.

## License

**semi-test-library-dotnet** is licensed under an MIT-style license (see [LICENSE.txt](LICENSE.txt)). Other incorporated projects may be licensed under different licenses.
