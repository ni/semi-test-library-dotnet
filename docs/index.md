# Semiconductor Test Library

The Semiconductor Test Library simplifies programming on the NI [Semiconductor Test System (STS)](https://www.ni.com/sts) and enables users to develop test programs efficiently using C#/.NET.

The Semiconductor Test Library includes the following high-level features:

- Interfaces and classes—Abstract instrument sessions and encapsulate the necessary pin and site awareness.
- Pin- and site-aware data types— Simplify instrument configuration and measurement results processing.
- Extension methods—Abstract common, high-level instrument operations.
- Parallelization methods—Abstract parallel for loops required to iterate over multiple instrument sessions regardless of how sessions map to pins or sites.
- Publishing methods—Simplify results publishing and add support for the SiteData and PinSiteData types.
- Utilities methods—Provide utility methods commonly required for writing test code.
- TestStand step types—Perform common operations, such as setting up and closing instruments, powering up a DUT, or executing common tests.

For the latest release, and examples, and source code, visit the [GitHub Releases page](https://github.com/ni/semi-test-library-dotnet/releases).

## Software Requirements

You must have the following software to use the Semiconductor Test Library:

- STS Software 24.5 or later
- .NET Framework 4.8 or later

Visual Studio 2022 is highly recommended.

## Installation

With the above software requirements met, the latest 