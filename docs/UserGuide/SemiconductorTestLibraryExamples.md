# Locating Semiconductor Test Library Examples

Semiconductor Test Library examples are located here: [Semi-test-library-dotnet/Examples](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source). 
There are different styles of examples available, as described below.

1. [Code Snippet Style](#code-snippet-examples)
1. [Sequence Style](#sequence-examples)
1. [Test Program Style](#test-program-examples)
1. [Standalone Style](#standalone-style)

## Code Snippet Examples

Code snippet examples demonstrate how to use specific API methods or implement coding concepts.

Example use cases are represented by individual static methods within separate static class files that are organized by high-level categories, such as instrument abstraction and data abstraction.
The example methods are not designed to run independently. Example methods have contextual requirements that are described within the example method summary or code comments.

[Semi-test-library-dotnet/Examples/CodeSnippets](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/CodeSnippets)

## Sequence Examples

Sequence examples demonstrate how to program instrumentation or implement code-level or sequence-level concepts within an STS test sequence.

Each example provides a single TestStand sequence file that runs within the TestStand Sequence Editor. The sequence file may include sub-sequences or steps with code modules for specific use cases, scenarios, or high-level concepts. Code modules are not designed to run independently outside of the relevant example sequence.
Any requirements for understanding or running the example are documented in an accompanying README.md. Context is also provided in sequence comments or inline code statements.
Example sequences run in offline mode with simulated hardware or with actual hardware, depending on the documented requirements.

[Semi-test-library-dotnet/Examples/Sequence](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/Sequence)

## Test Program Examples

Test Program examples demonstrate mock test programs for testing specific DUT types.

The supported DUT type, requirements, and assumptions are documented in an accompanying README.md.
Each test program contains fully-documented code implementations. Each test method has code-level summary documentation.
Test programs run in offline mode with simulated hardware.

[Semi-test-library-dotnet/Examples/TestPrograms](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/TestPrograms)


## Standalone Style

Standalone style examples demonstrate how to program instrumentation or implement code-level concepts.

Any assumed context required to run or understand the example is documented within an accompanying README.md file and via comments in-line with the code statements and method summaries.
It can be executed standalone, outside of the TestStand Sequence Editor, if the criteria defined by the provided context is met.
Runs in offline mode with simulated hardware or with actual hardware depending on the criteria defined by the provided context.

[Semi-test-library-dotnet/Examples/Standalone](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/Standalone)

> [!Note]
> Each example includes documentation in the form of a README.md.
