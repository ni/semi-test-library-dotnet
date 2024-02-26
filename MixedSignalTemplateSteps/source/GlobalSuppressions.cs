// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("NationalInstruments", "LRT001:There is only one restricted namespace", Justification = "Template Step instances are owned by customers not NI")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "The method could do things synchronously based on input.")]
[assembly: SuppressMessage("Style", "IDE0008:Use explicit type", Justification = "Use var to simplify code")]
[assembly: SuppressMessage("Style", "NI1704:Identifiers should be spelled correctly", Justification = "Not sure why custom dictionary is not working, so suppress for now")]
[assembly: SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "To simplify code")]
[assembly: SuppressMessage("Style", "IDE0010:Add missing cases", Justification = "To simplify code, we might remove DCPower instrument model enum later")]
