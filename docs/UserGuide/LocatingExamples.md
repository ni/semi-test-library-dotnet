# Locating Examples

Examples for how to use the Semiconductor Test Library (STL) are available on GitHub and are also included as part of the STS Software install.

- GitHub Location: [semi-test-library-dotnet/Examples](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source)
- Installed Location: `C:\Users\Public\Documents\National Instruments\NI_SemiconductorTestLibrary\Examples`

Reference the GitHub location for the latest version of the examples or to browse the example code source online.
When on a system with STS Software installed, reference the examples from their installed location.
The version of the examples included in the STS Software installer will match their state at the time of the specific STL release used by the STS Software version.
Refer to the [STS Software Version Compatibility](https://ni.github.io/semi-test-library-dotnet/UserGuide/Overview.html#sts-software-version-compatibility) table for more details.

> [!NOTE]
> You must build the code solutions for examples downloaded from GitHub before you can run them.
> The installed examples are pre-compiled and ready to run.

There are different styles of examples, each organized within their own sub-directory.
Refer to the sections below to learn more about each style of example.

- [Code Snippet Style](#code-snippet-examples)
- [Sequence Style](#sequence-examples)
- [Test Program Style](#test-program-examples)
- [Standalone Style](#standalone-style)

> [!Note]
> Each example includes documentation in the form of a README.md markdown file. Markdown files are human-readable plain-text documents you can view using any text editor, including Notepad.

## Code Snippet Examples

Code snippet examples demonstrate how to use specific API methods or implement coding concepts.

- Example use cases are represented by individual static methods within separate static class files that are organized by high-level categories, such as instrument abstraction and data abstraction.
- The example methods are not designed to run independently. Example methods have contextual requirements that are described within the example method summary or code comments.

> [!NOTE]
> Code snippet style examples were the only example style available for v24.5.0, v24.5.1, and v25.0.0 releases of the Semiconductor Test Library.

## Sequence Examples

Sequence examples demonstrate how to program instrumentation or implement code-level or sequence-level concepts within an STS test sequence.

- Each example provides a single TestStand sequence file that runs within the TestStand Sequence Editor. The sequence file may include sub-sequences or steps with code modules for specific use cases, scenarios, or high-level concepts. Code modules are not designed to run independently outside of the relevant example sequence.
- Any requirements for understanding or running the example are documented in an accompanying README.md. Context is also provided in sequence comments or inline code statements.
- Example sequences run in offline mode with simulated hardware or with actual hardware, depending on the documented requirements.

> [!NOTE]
> Sequence style examples are available as of the v25.5 release of the Semiconductor Test Library (STL). Unless documented otherwise, they can be leveraged with previous STL versions. Refer to the documentation accompanying the specific example for details.

## Test Program Examples

Test Program examples demonstrate mock test programs for testing specific DUT types.

- The supported DUT type, requirements, and assumptions are documented in an accompanying README.md.
- Each test program contains fully-documented code implementations. Each test method has code-level summary documentation.
- Test programs run in offline mode with simulated hardware.

> [!NOTE]
> Test program style examples are available as of the v25.5 release of the Semiconductor Test Library (STL). Unless documented otherwise, they can be leveraged with previous STL versions. Refer to the documentation accompanying the specific example for details.


## Standalone Style

Standalone style examples demonstrate how to program instrumentation or implement code-level concepts.

- Any assumed context required to run or understand the example is documented within an accompanying README.md file and via comments in-line with the code statements and method summaries.
- It can be executed standalone, outside of the TestStand Sequence Editor, if the criteria defined by the provided context is met.
Runs in offline mode with simulated hardware or with actual hardware depending on the criteria defined by the provided context.

> [!NOTE]
> There are no standalone style examples that have been released to date. This style of example is being considered for future releases.

