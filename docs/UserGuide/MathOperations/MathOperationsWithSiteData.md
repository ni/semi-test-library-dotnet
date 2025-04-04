# Math Operations with SiteData

The `SiteData` type supports various math operations which include both binary and unary math operations.

---

## Binary Math Operations with SiteData

The following table outlines the binary operator-based mathematical operations available for `SiteData<T>` and specifies the permitted data types for `T` for each operation.

**Table of Binary Math Operations for SiteData:**

|Methods |Operator|Description|Supported Data Types|
| :- | :- | :- | :- |
|[Add](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Add.html)|[+](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_Addition.html)|Performs an add operation between every element in current `SiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[BitwiseAnd](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.BitwiseAnd.html)|[&](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_BitwiseAnd.html)|Performs a bitwise AND operation with another `SiteData` object, for each element across each site.|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[BitwiseOr](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.BitwiseOr.html)|[\|](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_BitwiseOr.html)|Performs a bitwise OR operation with another `SiteData` object, for each element across each site.|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[BitwiseXor](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.BitwiseXor.html)|[^](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_ExclusiveOr.html)|Performs a bitwise XOR operation with another `SiteData` object|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Compare](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Compare.html)||Performs a compare operation between every element in current `SiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Divide](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Divide.html)|[/](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_Division.html)|Performs a divide operation between every element in current `SiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Maximum](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Maximum.html)||Returns the larger of the element in current `SiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Minimum](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Minimum.html)||Returns the smaller of the element in current `SiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Multiply](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Multiply.html)|[\*](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_Multiply.html)|Performs a multiply operation between every element in current `SiteData` object and the given value.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Power](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Power.html)||Raises every element in current `SiteData` to the power of the given value.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Subtract](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Subtract.html)|[-](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_Subtraction.html)|Subtracts the given value from every element in current `SiteData` object.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|

### Usage Considerations for Binary Math Operations with SiteData

When performing binary math operations on `SiteData` objects you should consider the following:

1. The `SiteData` object on which the method operates, and the input argument provided to the method serve as the two operands in the binary math operation being performed.
2. All methods can accept either a scalar, an array, or `SiteData` object as an input argument.
3. Operators can only accept a scalar or `SiteData` object as the second operand.
4. When the input is a `SiteData` object it must match the same underlying type, `T`, as the `SiteData` object being operated on.
5. When both operands are scaler, they must have identical data types, and that data type must be supported by the desired method.
6. When one of the operand is `SiteData<T>` and other operand is scaler, then both operand must be of identical data type, `T`, and that data type must be supported by the desired method.
7. When the underlying data, `T`, of the `SiteData<T>` object is an array type, if the second operand is:
   1. Also an array type, the second operand must have the same length, dimensions, and underlying type as the array contained within the `SiteData<T>` object.
   2. Scalar type, the scalar type must match the array element type of the underlining data within the `SiteData<T>` object.
   3. `SiteData<T>` object, both operand objects must be of identical data types, `T`.
8. The Bitwise methods are only supported when the underlying data type of the `SiteData` object, `T`, is an `integer` type, either a scalar integer, array of integers, or another `SiteData` object of the same integer type.
9. When the input argument is an array or a `SiteData` object of an array type, the array element data type must match the underlying type of the `SiteData<T>` object, `T`, and be of equal or lesser dimensions (i.e. `TOther` cannot be 2D when `T` is 1D).
10. The `Divide` method returns a scalar double value per site by default. When the underlying data type `T` of the `SiteData<T>` object is an array, the `TResult` type must be explicitly specified as a `double` array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*. Refer to the [`Divide<TOther, TResult>(TOther)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData_1_Divide__2___0_) and [`Divide<TOther, TResult>(SiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData_1_Divide__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData___0__) method signatures in the API Reference documentation.
11. The `Compare` method returns a `boolean` value per site by default. When the underlying data type `T` of the `SiteData<T>` object is an array, the TResult type must be explicitly specified as a `boolean` array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*. Refer to the [`Compare<TOther, TResult>(ComparisonType, TOther)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType___0_) and [`Compare<TOther, TResult>(ComparisonType, SiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData___0__) method signatures in the API Reference documentation.

>[!NOTE]
> For more information on specific exception conditions when preforming math operations refer to [Exception Conditions For Math Operations](ExceptionConditionsForMathOperations.md).

### Examples for Binary Math Operations with SiteData

Example of binary math operations :heavy_check_mark: **ALLOWED**  with `SiteData`:

```csharp
var siteData1 = new SiteData<int>(new int[] { 1, 2, 3 });
var siteData2 = new SiteData<int>(new int[] { 4, 5, 6 });

var operatorOverloadResult = siteData1 + siteData2; 
// The operatorOverloadResult will be a SiteData<int> object containing three sites worth of scalar data equivalent to:
// { [0] = 5, [1] = 7, [2] = 9} }

var siteData3 = new SiteData<long[]>(new long[][] { new long [] { 1, 2, 3 }, new long [] { 4, 5, 6 } });
var siteData4 = new SiteData<long>(new long [] { 4, -5 });

var result = siteData3.Add(siteData4);
// The result will be a SiteData<long[]> object containing two sites worth of array data equivalent to:
// { [0] = {5, 6, 7}, [1] = {-1, 0, 1} }
```

Example of a binary math operation :x: **NOT ALLOWED** with `SiteData`:

```csharp
var siteData1 = new SiteData<int>(new int[] { 1, 2, 3 });
var siteData2 = new SiteData<long>(new long[] { 4, -5, 6 });

var result = siteData1.Add(siteData2);
// The above operation will throw an NISemiconductorTestException with the following message:
// "For Add operation, the inner data type of the first operand (System.Double) and that of the second operand (System.Int64) must match."
```

---

## Unary Math Operations with SiteData

The following table outlines the unary operator-based mathematical operations available for `SiteData<T>` and specifies the permitted data types for `T` for each operation.

**Table of Unary Math Operations for SiteData:**

|Methods|Operator|Description|Supported Data Types|
| :- | :- | :- | :- |
|[Abs](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Abs.html)||Performs `Math.Abs` operation on every element in current `SiteData` object.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[BitwiseComplement](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.BitwiseComplement.html)|[~](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_OnesComplement.html)|Gets the bitwise complement of the original `SiteData` object.|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Invert](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Invert.html)||Performs invert operation on every element in current `SiteData` object.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Log10](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Log10.html)||Performs `Math.Log10` operation on every element in current `SiteData` object.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Max](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Max.html)||Calculates the maximum value across sites.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Mean](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Mean.html)||Calculates the mean value across sites.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Min](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Min.html)||Calculates the minimum value across sites.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Negate](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Negate.html)||Returns the negative value of every element in current `SiteData` object.|`int`, `long`, `sbyte`, `short`, `double`, `decimnal`, `float`|
|[ShiftLeft](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.ShiftLeft.html)|[<<](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_LeftShift.html)|Shifts the value to the left by the specified bit count, for each element, per site. |`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[ShiftRight](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.ShiftRight.html)|[>>](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.op_RightShift.html)|Shifts the value to the right by the specified bit count, for each element, per site.|`int`, `unint` , `long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[SquareRoot](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.SquareRoot.html)||Returns the square root of every element in current `SiteData` object.|`double`, `decimal`, `float`, `int`, `unint`,`long`, `ulong`, `byte`, `sbyte`, `short`, `ushort`|
|[Truncate](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Truncate.html)||Returns integer portion of every element in current `SiteData` object.|`double`, `decimnal`, `float`|

### Usage Considerations for Unary Math Operations with SiteData

When performing unary math operations on `SiteData` objects you should consider the following:

1. The `Invert`, `Log10`, and `SquareRoot` methods return a scalar double value per site by default. When the underlying data type `T` of the `SiteData<T>` object is an array, the `TResult` type must be explicitly specified as a `double` array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*.
1. The `count` input value passed to the `ShiftLeft` and `ShiftRight` operators must be positive, otherwise an exception `NISemiconductorTestException` is thrown with exception message matching the exception scenarios of *Shift Count Must Be Positive*.

>[!NOTE]
> For more information on specific exception conditions when preforming math operations refer to [Exception Conditions For Math Operations](ExceptionConditionsForMathOperations.md).

### Examples for Unary Math Operations with SiteData

Example of a unary math operation :heavy_check_mark: **ALLOWED** with `SiteData`:

```csharp
var siteData = new SiteData<double>(new double[] { -1, 2, -3 });

var result = siteData.Abs();
// The result will be { [0] =1, [1] = 2, [2] = 3 }
```

Example of a unary math operation :x: **NOT ALLOWED** with `SiteData`:

```csharp
var siteData = new SiteData<string>(new string[] { "A", "B", "C" });

var result= siteData.Abs();
// The above operation will throw an NISemiconductorTestException with the following message:
// "Math operations not supported on the System.String type data".
```
