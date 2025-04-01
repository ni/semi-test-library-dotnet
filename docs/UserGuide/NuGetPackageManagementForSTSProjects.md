# NuGet Package Management for STS Projects

NuGet is the package manager for .NET code development. It provides the tooling for packaging, versioning, and distributing reusable code libraries that aid in managing code dependencies between .NET projects.

NI recommends the following best practices for managing NuGet package dependencies in STS projects:

- Use a solution-level NuGet.config file to define the NuGet package manager behavior, such as where to find packages and where to install.

- Use the latest [SDK style](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/overview) formatting for your .csproj files, which defines NuGet package dependencies as PackageReference within the .csproj file. This simplifies referencing NuGet package dependencies within the .csproj file and visualizing them in Visual Studio, and improves source code control.
  > [!NOTE]
  > This directly impacts how the NuGet.config file is formatted.

- Distribute NuGet package dependencies within the project source under the `Code Modules\packages` directory. This can be configured in the solution-level `NuGet.config` file.
  > [!NOTE]
  > This ensures that the target system where the test program source is deployed always has access to the dependent NuGet packages to be rebuilt without a connection to an external server, such as nuget.org.
  >
  > The size of a NuGet package file (.nupkg) is expected to be small and therefore it is acceptable to include .nupkg files as part of the source code control.

- Configure the NuGet Package manager to unpack and install packages in the `global-packages` folder on the target system. You can define the `global-packages` location in the solution-level NuGet.config file.
  > [!NOTE]
  > This minimizes the disk space programs use. Packages will only be unpacked from the `Code Modules\packages` directory into the `global-packages` location if the target system does not have the version of the package already installed in the `global-packages` location on the target system.
  >
  > When the SDK style project format is used, the default `global-packages` location is set to `%userprofile%\.nuget\packages`.

**Related Information:**

- [NuGet Documentation](https://learn.microsoft.com/en-us/nuget/)
- [NuGet Configuration Files](https://learn.microsoft.com/en-us/nuget/reference/nuget-config-file)
- [.NET SDK Project Files](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/overview#project-files)
- [Managing the global packages, cache, and temp folders](https://learn.microsoft.com/en-us/nuget/consume-packages/managing-the-global-packages-and-cache-folders)

**Example STS Test Program Project Source With Local Package Reference:**

```Text
(\) Code Modules
 ├──(\) bin
 │   ├── <TestProgramName>.dll
 │   ├── NationalInstruments.SemiconductorTestLibrary.Abstractions.dll
 │   └── NationalInstruments.SemiconductorTestLibrary.Extensions.dll
 ├──(\) packages
 │   └── NationalInstruments.SemiconductorTestLibrary.24.5.0.nupkg
 ├──(+) DutControl
 ├──(+) LoadBoardControl
 ├──(+) TestSteps
 ├── NuGet.config
 ├── <TestProgramName>.csproj 
 └── <TestProgramName>.sln
```

> [!NOTE]
> At build-time, any dependent assembly files from referenced NuGet packages will be placed next to the final output assembly (`<TestProgramName>.dll`) within the project's output directory (`Code Modules\bin`).

## Adding NuGet Packages

Adding NuGet packages to an STS project can be done via the NuGet package manager, which will add a PackageReference to the .csproj file, and expect the package to be installed on the target system. The NuGet package manager can be configured to look at various sources and configured with a NuGet.config file, such as nuget.org or the project's `Code Modules\packages` directory,

> [!NOTE]
> Ensure the .nupkg files for all dependent NuGet packages added to your project have been copied into the packages directory of your project's source files and source code controlled.

## Upgrading NuGet Packages

> [!WARNING]
> Upgrading package dependencies during development can introduce unnecessary risks to your project. After a NuGet package dependency is added to your project and your project development starts, you should not upgrade the package in the development cycle unless there is a valid reason requiring an upgrade.

Use the following procedure to upgrade a dependent NuGet package that has already been distributed with an STS test program's source code in the `Code Modules\packages` directory.

1. Download the newer version of the .nupkg file for the target NuGet package and copy it to the `Code Modules\packages` directory within the project source. The newer version will temporarily coexist with the existing version of the target NuGet package file.
2. Open the NuGet package manager to view the Installed packages for your project. Select the target package from the Installed packages list and then select Upgrade.
   > [!NOTE]
   > The NuGet package manager automatically recognizes an update to the target package and provides the option to upgrade. After you select Upgrade, the NuGet package manager will update PackageReference in .csproj.

3. After you upgrade the package reference and successfully build your project against the newer version, remove the older version of the package (.nupkg) from your project's source files and commit the change to your source code control system.

## Using STL Dependent 3rd-Party NuGet Packages

If there is a 3rd-party NuGet package you want to use that is dependent on the Semiconductor Test Library (STL), then your STS project must use the same version of STL that the 3rd party package was built with. Otherwise, your STS project may encounter a runtime error.

>[!NOTE]
> In all other instances, the STS project is free to target any version of the STL NuGet package that is compatible with the STS Software version being used. Refer to the [STS Software Version Compatibility](Overview.md#sts-software-version-compatibility) table for more details.
