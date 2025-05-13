This directory contains Sequence style examples.

**What is a Sequence style example?**
- Sequence style examples demonstrate how to program instrumentation or implement code-level or sequence-level concepts within the context of an STS test sequence.
- There is a single sequence file for each high-level example.
- Within a given example sequence there may be multiple sub-sequences, steps with code-modules to cover the various use cases, scenarios, or high-level concepts.
- Any assumed context required to run or understand the example is documented within an accompanying README.md file and via comments within the sequence file and in-line with the code statements.
- They are intended to be run from within the TestStand Sequence Editor (assuming the criteria defined by the provided context is met). 
- Runs both with actual hardware and in offline mode with simulated hardware.
- Code modules are not expected to run standalone or be executable outside of TestStand.
