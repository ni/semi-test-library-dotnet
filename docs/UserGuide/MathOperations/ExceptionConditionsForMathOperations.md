# Exception Conditions for Math Operations

Math operations for `SiteData` and `PinSiteData` will throw `NISemiconductorTestException` in certain conditions, where the exception message will vary depending on the specific scenario. The possible exception conditions and their corresponding exception messages are list in the table below.

**Table of Possible Exceptions:**

|Condition|Description|Exception Message|Applicable To|
| :- | :- | :- | :- |
|Mismatched Array Dimensions |An exception occurs when array dimensions of the result array and input array are not matching.|When the underlying type, `T`, of the `SiteData` or `PinSiteData` object being operated on is an array, the `TResult` must also be an array of equal dimensions as the underlying type, `T`.|All Math Operations|
|Mismatched Array Lengths |An exception occurs when data array length of the first operand and that of the second array operand do not match.|For `<math operation>` operation, the data array length of the first operand (`<array length of operand 1>`) and that of the second operand (`<array length of operand 2>`) must match.|Binary Math Operations|
|Mismatched Operand Types |An exception occurs when operand types do not match.|For `<math operation>` operation, the inner data type of the first operand (`<type of first operand>`) and that of the second operand (`<type of second operand>`) must match.|Binary Math Operations|
|Result Type Must Be Array |An exception occurs when the defined data type of result is not array in case of array – scalar operations.|The `<TResult>` must be an array.|Binary Math Operations|
|Shift Count Must Be Positive |An exception occurs when the shift `count` is given negative.|The number of bits to shift must be positive.|`ShiftLeft` and `ShiftRight`|
|Type Not Supported |An exception occurs when the data type is not supported or appropriate for any math operation.|Math operations not supported on the `<data type>` type data.|All Math Operations|
|Type Not Supported by Operation|An exception occurs when the data type of either operand is not supported.|The `<math operation>` operation on the `<data type>` data type is not supported.|All Math Operations|