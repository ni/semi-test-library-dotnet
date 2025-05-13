# Semiconductor Test Library Examples

This directory contains examples for how to use the Semiconductor Test Library.
There are different styles of examples available, as described below.

- [Code Snippet Style](#code-snippet-style)
- [Standalone Style](#standalone-style)
- [Sequence Style](#sequence-style)
- [Test Program Style](#test-program-style)

## Code Snippet Style

Code Snippet style examples demonstrate how to use specific API methods or implement coding concepts using an assumed context.

- Different use cases, scenarios, and concepts are broken down into individual static methods within separate static class files organized by high-level categories, such a instrument abstraction and data abstraction.
- The context required to run the example is implied and not directly provided by the example itself. Rather, it is documented as an assumption within the example method summary and/or via comments in-line with the code statements.
- The example method code is not expected to run standalone outside of the implied context.

## Standalone Style

Standalone style examples demonstrate how to program instrumentation or implement code-level concepts.

- Any assumed context required to run or understand the example is documented within an accompanying README.md file and via comments in-line with the code statements and method summaries.
- It can be executed standalone, outside of the TestStand Sequence Editor, if the criteria defined by the provided context is met.
- Runs in offline mode with simulated hardware or with actual hardware depending on the criteria defined by the provided context.

## Sequence Style

Sequence style examples demonstrate how to program instrumentation or implement code-level or sequence-level concepts within the context of an STS test sequence.

- There is a single sequence file for each high-level example.
- Within a given example sequence there may be multiple sub-sequences, steps with code-modules to cover the various use cases, scenarios, or high-level concepts.
- Any assumed context required to run or understand the example is documented within an accompanying README.md file and via comments within the sequence file and in-line with the code statements.
- They are intended to be run from within the TestStand Sequence Editor (assuming the criteria defined by the provided context is met).
- Runs both with actual hardware and in offline mode with simulated hardware.
- Code modules are not expected to run standalone or be executable outside of TestStand.

## Test Program Style

Test Program style examples demonstrate a mock test program for testing a certain DUT type.

- Accompanied by a README.md file that documents the DUT type, assumptions, and any requirements.
- Contains fully documented code implementations (every test method has code-level summary documentation).
- Runs in offline mode with simulated hardware.
