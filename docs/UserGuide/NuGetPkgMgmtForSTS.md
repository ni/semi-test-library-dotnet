# NuGet Package Management for STS Projects

NuGet is the package manager for .NET code development. It provides the tooling for packaging, versioning, and distributing reusable code libraries that aid in managing code dependencies between .NET projects.

NI recommends the following best practices for managing NuGet package dependencies in STS projects:

- Use a solution-level NuGet.config file to define the NuGet package manager behavior, such as where to find packages and where to install.

- Use the latest [SDK style](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/overview) formatting for your .csproj files, which defines NuGet package dependencies as PackageReference within the .csproj file. This simplifies referencing NuGet package dependencies within the .csproj file, visualizing dependencies in Visual Studio, and source code control.
  > [!NOTE]
  > This directly impacts how the NuGet.config file is formatted.

- Distribute NuGet package dependencies within the project source under the `Code Modules\packages` directory. This can be configured in the solution-level `NuGet.config` file.
  > [!NOTE]
  > This ensures the target system where test program source is deployed will always have access to the dependent NuGet packages to be rebuilt without needing a connection to an external server, such as nuget.org.
  >
  > The file size of NuGet packages (.nupkg) are expected to be small & therefore acceptable to include as part of the source code control.

- Configure the NuGet Package manager to unpack and install packages to a `global-packages` folder on the target system. The `global-packages` location can be defined in the solution-level NuGet.config file.
  > [!NOTE]
  > This minimizes the amount of disk space used by programs. Packages will only be unpacked from `Code Modules\packages` directory into `global-packages` location if the target system does not have the version of the package already installed in the `global-packages` location on the target system.
  >
  > When using the SDK style project format, the default `global-packages` location is set to `%userprofile%\.nuget\packages`.

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
> Ensure the .nupkg files for all dependent nuget packages added to your project have been copied into the packages directory within the project source and source code controlled.

## Upgrading NuGet Packages

> [!WARNING]
> Upgrading package dependencies mid-development can introduce unnecessary risk to your project. Once a NuGet package dependency has been added to your project and your project development has started, you should not look to upgrade the package mid-development cycle unless there is a valid or critical reason requiring your project to accept the upgrade.

Use the following procedure to upgrade a dependent NuGet package that is already being distributed with an STS test program's source code within the `Code Modules\packages` directory.

1. Download the newer version of the .nupkg file for the target nuget package and copy it to the `Code Modules\packages` directory within the project source. It will temporarily coexist alongside the existing version of that  target nuget package.
2. Then, open the NuGet package manager to view the Installed packages for your project. Select the target package from the Installed packages list in the NuGet package manager and select Upgrade.
   > [!NOTE]
   > The NuGet package manager should automatically recognize that an update to target package and provides the option to upgrade. Once you select Upgrade, the NuGet package manager will  update the PackageReference in .csproj.

3. After you have upgraded the package reference and successfully built your project against the newer version, you should then remove the older version of the package (.nupkg) from your source file and commit the change to your source code control system.
