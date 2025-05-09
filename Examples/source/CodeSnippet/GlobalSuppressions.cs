// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "VSSpell001:Spell Check", Justification = "Custom dictionary is not working, so suppress for now")]
[assembly: SuppressMessage("Naming", "VSSpell002:Ignore Word", Justification = "Custom dictionary is not working, so suppress for now", Scope = "type", Target = "~T:NationalInstruments.SemiconductorTestLibrary.Examples.WorkFlowExamples")]
[assembly: SuppressMessage("Style", "NI1704:Identifiers should be spelled correctly", Justification = "Not sure why custom dictionary is not working, so suppress for now")]
[assembly: SuppressMessage("Style", "IDE0008:Use explicit type", Justification = "Use var to simplify code")]
[assembly: SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "To simplify code")]
[assembly: SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "This being done for example purposes.")]
