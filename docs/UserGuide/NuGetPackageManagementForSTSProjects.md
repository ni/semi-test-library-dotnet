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

NuGet packages can be added to an STS project via the **NuGet package manager** in Visual Studio. This action will add a `PackageReference` entry to the project’s `.csproj` file and expect the package to be installed on the target system at build time.

Packages can now be consumed in one of the following ways:

- **From NuGet.org** (recommended for internet-connected environments)

  Packages are published on [NuGet.org](https://www.nuget.org/packages/NationalInstruments.SemiconductorTestLibrary) for direct consumption.
  - Open **Visual Studio > Tools > NuGet Package Manager > Manage NuGet Packages for Solution**.
  - Ensure `nuget.org` is listed as a package source. If not, add it in **NuGet Package Manager Settings > Package Sources**.
  - Search for the target package (example, `NationalInstruments.SemiconductorTestLibrary`) and select **Install**.
  - This will add a `PackageReference` in your `.csproj` and fetch the package from NuGet.org.

- **From the GitHub Releases page**

  Packages are also attached to the [GitHub Releases](https://github.com/ni/semi-test-library-dotnet/releases) page of the repository. You may download the `.nupkg` files manually from the latest release.

- **From a local `Code Modules\packages` directory** (recommended for disconnected/offline environments)

  Configure `NuGet.config` to point to your project’s `Code Modules\packages` directory. Copy all required `.nupkg` files for your project into this folder and commit them to source control.  

  > [!NOTE]  
  > Ensure the `.nupkg` files for all dependent NuGet packages are copied into the `packages` directory and source controlled, so your project can build reliably on any system.
  >
  > To verify the integrity of a NuGet package, use the [`nuget verify --all <nupkg>`](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-nuget-verify) command. This checks the package against its signature and reports an error if tampering is detected: `NU3008: The package integrity check failed`.
  >
  > The traditional method of comparing of file checksum hashes is not an adequate means of verifying NuGet packages. The checksum of a NuGet package downloaded from NuGet.org will differ from the package uploaded as part of a GitHub release notes or the one included in the NI Default C#/.NET template project when using the STS Project Creation Tool. This is because NuGet.org adds an additional NuGet.org-specific signature to the package when uploaded, in additional to the ni-signature already included in the package, and thereby changing its overall checksum hash. Instead, you should use the `nuget verify` command as mentioned above.

## Upgrading NuGet Packages

> [!WARNING]
> Upgrading package dependencies during development can introduce unnecessary risks. After a NuGet package dependency is added and development starts, you should not upgrade the package in the development cycle unless there is a valid reason requiring an upgrade.

You may upgrade a NuGet package using one of the following methods, depending on your environment:  

### Upgrade from NuGet.org (Internet-connected systems)

1. Open the **NuGet Package Manager**.  
2. Select the **Updates** tab.  
3. Choose the target package and click **Update**.  

   > [!NOTE]  
   > The NuGet Package Manager automatically detects newer versions published on NuGet.org. When you update the `.csproj`, `PackageReference` is updated accordingly.  

### Upgrade from GitHub Releases

1. Download the newer `.nupkg` from the GitHub Releases page.  
2. Replace the older version in your `Code Modules\packages` folder (if maintaining offline copies).  
3. Open the **NuGet Package Manager** and select **Update**.  

### Upgrade from a local `Code Modules\packages` directory

1. Download the newer `.nupkg` for the package and copy it into your project’s `Code Modules\packages` directory.
   - The newer version will temporarily coexist with the existing version.  
2. Open the **NuGet Package Manager**, select the package from the Installed packages list, and then select **Upgrade**.  

   > [!NOTE]  
   > After the upgrade, the package reference in `.csproj` is updated.  

3. Build your project against the newer version to confirm compatibility.  
4. Remove the older `.nupkg` file from your source and commit the change to source control.

## Using STL Dependent 3rd-Party NuGet Packages

If there is a 3rd-party NuGet package you want to use that is dependent on the Semiconductor Test Library (STL), then your STS project must use the same version of STL that the 3rd party package was built with. Otherwise, your STS project may encounter a runtime error.

>[!NOTE]
> In all other instances, the STS project is free to target any version of the STL NuGet package that is compatible with the STS Software version being used. Refer to the [STS Software Version Compatibility](Overview.md#sts-software-version-compatibility) table for more details.
