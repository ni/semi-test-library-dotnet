# Contributing to semi-test-library-dotnet

Contributions to **semi-test-library-dotnet** are welcome from all!

**semi-test-library-dotnet** is managed via [git](https://git-scm.com), with the canonical upstream
repository hosted on [GitHub](https://github.com/ni/semi-test-library-dotnet).

**semi-test-library-dotnet** follows a pull-request model for development.
If you wish to contribute, you will need a GitHub account, fork this project,
push a branch with your changes to your project, and then submit a pull request.

Please remember to sign off your commits (e.g., by using `git commit -s` if you
are using the command line client). This amends your git commit message with a line
of the form `Signed-off-by: Name Lastname <name.lastmail@emailaddress.com>`. Please
include all authors of any given commit into the commit message with a
`Signed-off-by` line. This indicates that you have read and signed the Developer
Certificate of Origin (see below) and are able to legally submit your code to
this repository.

See [GitHub's official documentation](https://help.github.com/articles/using-pull-requests/) for more details.

## Getting Started

In order to build the code base in the **semi-test-library-dotnet** repository, you must ensure your build machine has met the following prerequisites:
  
### Prerequisites

- System with Windows 10 64-bit
  - Can be a PXIe embedded controller or a computer (physical or virtual).
  - The system must not have NI software installed, unless the NI software is installed using the STS Version Selector.
- STS Version Selector 24.5 (or later)
- STS Software 24.5 (or later)
- Visual Studio 2022
- .NET Framework 4.8
- Git for Windows

### Build Solution

1. Open the top-level solution file in Visual Studio 2022.

   > Note: The main solution file (`SemiconductorTestLibrary.sln`) is located at the root of this repo.

2. Restore packages by right clicking the solution from the solution explorer in Visual Studio 2022.
3. Build the solution using Visual Studio 2022 by selecting the **Build** menu and then **Build Solution**.

> You can also build the solution from the command line using the following command:'
>
> ```bash
> msbuild.exe SemiconductorTestLibrary.sln "/t:Restore;Build"
> ```

### Running Tests

Ensure that all tests pass on your system before and after making any changes. To run tests, open the main solution file in Visual Studio 2022, select the **Test** menu and then **Run All Tests**.

## Contributing

You must build the solution and run system tests before contributing to the [semi-test-library-dotnet](https://github.com/ni/semi-test-library-dotnet) project.

1. Fork this project on GitHub and then clone your repository locally.
2. Make your change.
3. Write applicable unit tests for the change.
4. Verify all tests, including the new ones, pass.
5. Commit changes.
6. On GitHub, create a new pull request to the main branch of the upstream repository ([semi-test-library-dotnet](https://github.com/ni/semi-test-library-dotnet)).

## Developer Certificate of Origin (DCO)

   Developer's Certificate of Origin 1.1

   By making a contribution to this project, I certify that:

   (a) The contribution was created in whole or in part by me and I
       have the right to submit it under the open source license
       indicated in the file; or

   (b) The contribution is based upon previous work that, to the best
       of my knowledge, is covered under an appropriate open source
       license and I have the right under that license to submit that
       work with modifications, whether created in whole or in part
       by me, under the same open source license (unless I am
       permitted to submit under a different license), as indicated
       in the file; or

   (c) The contribution was provided directly to me by some other
       person who certified (a), (b) or (c) and I have not modified
       it.

   (d) I understand and agree that this project and the contribution
       are public and that a record of the contribution (including all
       personal information I submit with it, including my sign-off) is
       maintained indefinitely and may be redistributed consistent with
       this project or the open source license(s) involved.

(taken from [developercertificate.org](https://developercertificate.org/))

See [LICENSE](https://github.com/ni/semi-test-library-dotnet/blob/main/LICENSE)
for details about how semi-test-library-dotnet is licensed.
