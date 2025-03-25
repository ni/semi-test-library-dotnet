# Math Operations with PinSiteData

The `PinSiteData` type supports various math operations which include both binary and unary math operations.

---

## Binary Math Operations with PinSiteData

The following table outlines the binary operator-based mathematical operations available for `PinSiteData<T>` and specifies the permitted data types for `T` for each operation.

**Table of Binary Math Operations for PinSiteData:**

|Methods |Operator|Description|Supported Data Types|
| :- | :- | :- | :- |
|[Add](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Add.html)|[+](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_Addition.html)|Performs an add operation between every element in current `PinSiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[BitwiseAnd](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.BitwiseAnd.html)|[&](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_BitwiseAnd.html)|Performs a bitwise `AND` operation with a scalar for each element across each pin and each site.|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[BitwiseOr](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.BitwiseOr.html)|[\|](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_BitwiseOr.html)|Performs a bitwise `OR` operation with a scalar for each element across each pin and each site.|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[BitwiseXor](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.BitwiseXor.html)|[^](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_ExclusiveOr.html)|Performs a bitwise `XOR` operation with a scalar for each element across each pin and each site.|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Compare](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Compare.html)||Performs a compare operation between every element in current `PinSiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Divide](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Divide.html)|[/](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_Division.html)|Performs a divide operation between every element in current `PinSiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Maximum](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Maximum.html)||Returns the larger one of the elements in current `PinSiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Minimum](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Minimum.html)||Returns the smaller one of the elements in current `PinSiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Multiply](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Multiply.html)|[\*](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_Multiply.html)|Performs a multiply operation on every element in current `PinSiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Power](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Power.html)||Raises every element in current `PinSiteData` object to the power of the given value.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Subtract](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Subtract.html)|[-](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_Subtraction.html)|Subtracts the given value from every element in current `PinSiteData` object.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|

### Usage Considerations for Binary Math Operations with PinSiteData

When performing binary math operations on `PinSiteData` objects you should consider the following:

1. The `PinSiteData` object on which the method operates, and the input argument provided to the method serve as the two operands in the binary math operation being performed.
2. All methods can accept either a scalar, an array, `SiteData` object or `PinSiteData` as an input argument.
3. Operators can only accept a scalar, `SiteData` or `PinSiteData`object as the second operand.
4. When the input is a `SiteData` or `PinSiteData` object it must match the same underlying type, `T`, as the `PinSiteData` object being operated on.
5. When both operands are scaler, they must have identical data types, and that data type must be supported by the desired method.
6. When one of the operand is `PinSiteData<T>` and other operand is scaler, then both operand must be of identical data type, `T`, and that data type must be supported by the desired method.
7. When the underlying data, `T`,  of the `PinSiteData<T>` object is an array type, if the second operand is:
   1. Also an array type, the second operand must have the same length, dimensions, and underlying type as the array contained within the `PinSiteData<T>` object.
   2. Scalar type, the scalar type must match the array element type of the underlining data within the `PinSiteData<T>` object.
   3. `SiteData<T>` object, both operand objects must be of identical data types, `T`.
   4. `PinSiteData<T>` object, both operand objects must be of identical data types, `T`.
8. The Bitwise methods are only supported when the underlying data type of the `PinSiteData` object, `T`, is an integer type, either a scalar integer, array of integers, or another `PinSiteData` object of the same integer type.
9. When the input argument is an array or a `PinSiteData` object of an array type, the array element data type must match the underlying type of the `PinSiteData<T>` object, `T`, and be of equal or lesser dimensions (i.e. TOther cannot be 2D when `T` is 1D).
10. The `Divide` method returns a scalar double value per site by default. When the underlying data type `T` of the `PinSiteData<T>` object is an array, the `TResult` type must be explicitly specified as a double array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*. Refer to the [`Divide<TOther, TResult>(TOther)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Divide__2___0_), [`Divide<TOther, TResult>(SiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Divide__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData___0__) and [`Divide<TOther, TResult>(PinSiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Divide__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData___0__)  method signatures in the API Reference documentation.
11. The `Compare` method returns a boolean value per site by default. When the underlying data type `T` of the `PinSiteData<T>` object is an array, the `TResult` type must be explicitly specified as a boolean array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*. Refer to the [`Compare<TOther, TResult>(ComparisonType, TOther)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType___0_), [`Compare<TOther, TResult>(ComparisonType, SiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData___0__) and [`Compare<TOther, TResult>(ComparisonType, PinSiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData___0__)  method signatures in the API Reference documentation.

>[!NOTE]
> For more information on specific exception conditions when preforming math operations refer to [Exception Conditions For Math Operations](ExceptionConditionsForMathOperations.md).

### Examples for Binary Math Operations with PinSiteData

Example of binary math operations :heavy_check_mark: **ALLOWED** with `PinSiteData`:

```csharp
var pinSiteData = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>
{ 
   ["VCC1"] = new Dictionary<int, double> { [0] = 3.5 } 
});
var siteData = new SiteData<double>(new Dictionary<int, double> { [0] = 1 });

var result = pinSiteData.Add(siteData);
// The result is a PinSiteData<double> object containing scalar data for one pin, one site equivalent to:
// { ["VCC1"] = { [0] = 4.5 } }

var operatorOverloadResult = pinSiteData + 2;
// The operatorOverloadResult is a PinSiteData<double> object containing scalar data for one pin, one site equivalent to:
// { ["VCC1"] = { [0] = 5.5 } }
```

Example of a binary math operation :x: **NOT ALLOWED** with `PinSiteData`:

```csharp
var pinSiteData = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>
{ 
   ["VCC1"] = new Dictionary<int, double> { [0] = 3.5 } 
});
var siteData = new SiteData<long>(new Dictionary<int, long> { { 0, 1 } });

var result = pinSiteData.Add(siteData);
// The above operation will throw an NISemiconductorTestException with the following message:
// "For Add operation, the inner data type of the first operand (System.Double) and that of the second operand (System.Int64) must match."
```

---

## Unary Math Operations with PinSiteData

The following table outlines the unary operator-based mathematical operations available for `PinSiteData<T>` and specifies the permitted data types for `T` for each operation.

**Table of Unary Math Operations for PinSiteData:**

|Methods|Operator|Description|Supported Data Types|
| :- | :- | :- | :- |
|[Abs](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Abs.html)||Performs `Math.Abs` operation on every element in current `PinSiteData` object.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[BitwiseComplement](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.BitwiseComplement.html)|[~](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_OnesComplement.html)|Gets the bitwise complement of the original `PinSiteData` object.|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Invert](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Invert.html)||Performs invert operation on every element in current `PinSiteData` object.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Log10](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Log10.html)||Performs `Math.Log10`operation on every element in current `PinSiteData` object.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Max](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Max.html)||Calculates the maximum value across sites.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[MaxByPin](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.MaxByPin.html)||Gets the maximum value across pins for each site and returns both the maximum value and each pin(s) where the maximum value was found for each site.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[MaxBySite](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.MaxBySite.html)||Gets the maximum value across sites for each pin and returns both the maximum value and each site(s) where the maximum value was found for each pin.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Mean](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Mean.html)||Calculates the mean value across pins for each site and returns the mean value for each site.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[MeanBySite](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.MeanBySite.html)||Calculates the mean value across sites for each pin and returns the site-to-site mean value for each pin.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Min](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Min.html)||Gets the minimum value across pins for each site.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[MinByPin](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.MinByPin.html)||Gets the minimum value across pins for each site and returns both the minimum value and each pin(s) where the minimum value was found for each site.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[MinBySite](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.MinBySite.html)||Gets the minimum value across sites for each pin and returns both the minimum value and each site(s) where the minimum value was found for each pin.|`double`, `decimal`, `float`, `int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Negate](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Negate.html)||Returns the negative value of every element in current `PinSiteData` object.|`int`, `long`, `sbyte`, `short`, `double`, `decimnal`, `float`|
|[ShiftLeft](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.ShiftLeft.html)|[<<](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_LeftShift.html)|Shifts the value to the left by the specified bit count for each element per site. |`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[ShiftRight](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.ShiftRight.html)|[>>](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.op_RightShift.html)|Shifts the value to the right by the specified bit count.|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[SquareRoot](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SquareRoot.html)||Returns the square root of every element in the current `PinSiteData` object.|`double`, `decimal`, `float`, `int`, `unint`, `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Truncate](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Truncate.html)||Returns integer portion of every element in current `PinSiteData` object.|`double`, `decimnal`, `float`|

### Usage Considerations for Unary Math Operations with PinSiteData

When performing unary math operations on `PinSiteData` objects you should consider the following:

1. The `Invert`, `Log10`, and `SquareRoot` methods return a scalar double value per site by default. When the underlying data type `T` of the `PinSiteData<T>` object is an array, the `TResult` type must be explicitly specified as a `double` array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*.
1. The `count` input value passed to the `ShiftLeft` and `ShiftRight` operators must be positive, otherwise an exception `NISemiconductorTestException` is thrown with exception message matching the exception scenarios of *Shift Count Must Be Positive*.

>[!NOTE]
> For more information on specific exception conditions when preforming math operations refer to [Exception Conditions For Math Operations](ExceptionConditionsForMathOperations.md).

### Examples for Unary Math Operations with PinSiteData

Example of a unary math operation :heavy_check_mark: **ALLOWED** with `PinSiteData`:

```csharp
var pinSiteData = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>
{ 
   ["VCC1"] = new Dictionary<int, double> { [0] = -3.5 }
}); 

var result = pinSiteData.Abs();
// The result is a PinSiteData<double> object containing scalar data for one pin, one site equivalent to: { ["VCC1"] = { [0] = 3.5 } }
```

Example of a unary math operation :x: **NOT ALLOWED** with `PinSiteData`:

```csharp
var pinSiteData = new PinSiteData<string>(new Dictionary<string, IDictionary<int, string>>
{ 
   ["VCC1"] = new Dictionary<int, string> { [0] = "Negative 3.5" } 
});

var result = pinSiteData.Abs();
// Above operation with throw an exception of NISemiconductorTestException with the following message:
// "Math operations not supported on the System.String type data."
```
