# Data Abstraction

The Semiconductor Test Library provides Pin- and Site-Aware data types to simplify how you configure instrumentation and manage returned measurement results. The high-level extension methods both return and accept these types as input parameters. This abstracts your need to manage confusing array manipulations or translate between instrument- and channel-formatted data to pin- and site-formatted data. Basic [math functions](#math-operations) can also be operated on these types.

## Pin- and Site-Aware Data Types

### **SiteData**

Class: `SiteData<T>` \
Namespace: `NationalInstruments.SemiconductorTestLibrary.DataAbstraction`

Defines an object containing values for one or more sites, where `T` can be passed as any data type.

This type is returned from methods in the Semiconductor Test Library where there may be a unique per-site value. This type can also be passed as an input parameter to some Semiconductor Test Library methods when there is a unique per-site value to operate with.

The `SiteData` object exposes basic methods for extracting a single site value or subset of values. Additionally, frequently used mathematical operations can be performed on a `SiteData` object for the most commonly used data types.

>[!NOTE]
> Refer to the API Reference for more details regarding the properties and methods exposed by the `SiteData`.
>
> Find examples for using the `SiteData` object [here on GitHub](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/DataAbstraction).

A `SiteData` object is an immutable dictionary of key-value pairs, where each key corresponds to a unique site number, and each value represents site-specific data for that site. This is also the observable structure when debugging a `SiteData` object at runtime in Visual Studio, as shown in the example below.

**Example of `SiteData` objects in Visual Studio at runtime**

![ExampleOfDebuggingSiteDataInVisualStudio](../images/ExampleOfDebuggingSiteDataInVisualStudio.png)

---

### **PinSiteData**

Class: `PinSiteData<T>` \
Namespace: `NationalInstruments.SemiconductorTestLibrary.DataAbstraction`

Defines an object containing values for one or more sites that are associated with a particular pin or set of pins, where `T` can be passed as any data type.

This type is returned from methods in the Semiconductor Test Library, such as measurement methods, where there may be a unique value for each pin, regardless of whether the value is the same across all sites and/or pins. This type can also be passed as an input parameter to some Semiconductor Test Library methods when there is a unique per-site, per-pin value to operate with.

The `PinSiteData` object exposes basic methods for extracting a single site value or subset of values. Additionally, frequently used mathematical operations can be performed on a `PinSiteData` object for the most commonly used data types.

>[!NOTE]
> Refer to the API Reference for more details regarding the properties and methods exposed by the `PinSiteData`.
>
> Find examples for using the `PinSiteData` object [here on GitHub](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/DataAbstraction).

A `PinSiteData` object is an immutable dictionary of key-value pairs, where each key corresponds to a unique pin name, and each value is a `SiteData` object containing site-unique data for the given pin. This is also the observable structure when debugging a `SiteData` object at runtime in Visual Studio, as shown in the example below.

**Example of `PinSiteData` objects in Visual Studio at runtime:**

![ExampleOfDebuggingPinSiteDataInVisualStudio](../images/ExampleOfDebuggingPinSiteDataInVisualStudio.png)

---

## Math Operations

### **SiteData**

#### Binary Operations

The following table outlines the binary operator-based mathematical operations available for `SiteData<T>` and specifies the permitted data types for `T` for each operation.

**Table of Binary Operations:**

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

**Usage Considerations:**

1. The `SiteData` object on which the method operates, and the input argument provided to the method serve as the two operands in the binary math operation being performed.
1. All methods can accept either a scalar, an array, or `SiteData` object as an input argument.
1. Operators can only accept a scalar or `SiteData` object as the second operand.
1. When the input is a `SiteData` object it must match the same underlying type, `T`, as the `SiteData` object being operated on.
1. When both operands are scaler, they must have identical data types, and that data type must be supported by the desired method. 
1. When one of the operand is `SiteData<T>` and other operand is scaler, then both operand must be of identical data type, `T`, and that data type must be supported by the desired method.
1. When the underlying data, `T`, of the `SiteData<T>` object is an array type, if the second operand is:
   1. Also an array type, the second operand must have the same length, dimensions, and underlying type as the array contained within the `SiteData<T>` object.
   1. Scalar type, the scalar type must match the array element type of the underlining data within the `SiteData<T>` object.
   1. `SiteData<T>` object, both operand objects must be of identical data types, `T`.
1. The Bitwise methods are only supported when the underlying data type of the `SiteData` object, `T`, is an `integer` type, either a scalar integer, array of integers, or another `SiteData` object of the same integer type.
1. When the input argument is an array or a `SiteData` object of an array type, the array element data type must match the underlying type of the `SiteData<T>` object, `T`, and be of equal or lesser dimensions (i.e. `TOther` cannot be 2D when `T` is 1D).
1. The `Divide` method returns a scalar double value per site by default. When the underlying data type `T` of the `SiteData<T>` object is an array, the `TResult` type must be explicitly specified as a `double` array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*. Refer to the [`Divide<TOther, TResult>(TOther)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData_1_Divide__2___0_) and [`Divide<TOther, TResult>(SiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData_1_Divide__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData___0__) method signatures in the API Reference documentation.
1. The `Compare` method returns a `boolean` value per site by default. When the underlying data type `T` of the `SiteData<T>` object is an array, the TResult type must be explicitly specified as a `boolean` array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*. Refer to the [`Compare<TOther, TResult>(ComparisonType, TOther)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType___0_) and [`Compare<TOther, TResult>(ComparisonType, SiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData___0__) method signatures in the API Reference documentation.

**Example of binary operations :heavy_check_mark: ALLOWED  with `SiteData` objects:**

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

**Example of a binary operation :x: NOT ALLOWED with `SiteData`:**

```csharp
var siteData1 = new SiteData<int>(new int[] { 1, 2, 3 });
var siteData2 = new SiteData<long>(new long[] { 4, -5, 6 });

var result = siteData1.Add(siteData2);
// The above operation will throw an NISemiconductorTestException with the following message:
// "For Add operation, the inner data type of the first operand (System.Double) and that of the second operand (System.Int64) must match."
```

#### Unary Operations

The following table outlines the Unary operator-based mathematical operations available for `SiteData<T>` and specifies the permitted data types for `T` for each operation.

**Table of Unary Operations:**

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

**Usage Considerations:**

1. The `Invert`, `Log10`, and `SquareRoot` methods return a scalar double value per site by default. When the underlying data type `T` of the `SiteData<T>` object is an array, the `TResult` type must be explicitly specified as a `double` array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*.
1. The `count` input value passed to the `ShiftLeft` and `ShiftRight` operators must be positive, otherwise an exception `NISemiconductorTestException` is thrown with exception message matching the exception scenarios of *Shift Count Must Be Positive*.

**Example of unary operation :heavy_check_mark: ALLOWED with `SiteData` objects:**

```csharp
var siteData = new SiteData<double>(new double[] { -1, 2, -3 });

var result = siteData.Abs();
// The result will be { [0] =1, [1] = 2, [2] = 3 }
```

**Example of unary operation :x: NOT ALLOWED with `SiteData` objects:**

```csharp
var siteData = new SiteData<string>(new string[] { "A", "B", "C" });

var result= siteData.Abs();
// The above operation will throw an NISemiconductorTestException with the following message:
// "Math operations not supported on the System.String type data".
```

---

### **PinSiteData**

#### Binary Operations

The following table outlines the binary operator-based mathematical operations available for `PinSiteData<T>` and specifies the permitted data types for `T` for each operation.

**Table of Binary Operations:**

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

**Usage Considerations:**

1. The `PinSiteData` object on which the method operates, and the input argument provided to the method serve as the two operands in the binary math operation being performed.
1. All methods can accept either a scalar, an array, `SiteData` object or `PinSiteData` as an input argument.
1. Operators can only accept a scalar, `SiteData` or `PinSiteData`object as the second operand.
1. When the input is a `SiteData` or `PinSiteData` object it must match the same underlying type, `T`, as the `PinSiteData` object being operated on.
1. When both operands are scaler, they must have identical data types, and that data type must be supported by the desired method.
1. When one of the operand is `PinSiteData<T>` and other operand is scaler, then both operand must be of identical data type, `T`, and that data type must be supported by the desired method.
1. When the underlying data, `T`,  of the `PinSiteData<T>` object is an array type, if the second operand is:
   1. Also an array type, the second operand must have the same length, dimensions, and underlying type as the array contained within the `PinSiteData<T>` object.
   1. Scalar type, the scalar type must match the array element type of the underlining data within the `PinSiteData<T>` object.
   1. `SiteData<T>` object, both operand objects must be of identical data types, `T`.
   1. `PinSiteData<T>` object, both operand objects must be of identical data types, `T`.
1. The Bitwise methods are only supported when the underlying data type of the `PinSiteData` object, `T`, is an integer type, either a scalar integer, array of integers, or another `PinSiteData` object of the same integer type.
1. When the input argument is an array or a `PinSiteData` object of an array type, the array element data type must match the underlying type of the `PinSiteData<T>` object, `T`, and be of equal or lesser dimensions (i.e. TOther cannot be 2D when `T` is 1D).
1. The `Divide` method returns a scalar double value per site by default. When the underlying data type `T` of the `PinSiteData<T>` object is an array, the `TResult` type must be explicitly specified as a double array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*. Refer to the [`Divide<TOther, TResult>(TOther)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Divide__2___0_), [`Divide<TOther, TResult>(SiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Divide__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData___0__) and [`Divide<TOther, TResult>(PinSiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Divide.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Divide__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData___0__)  method signatures in the API Reference documentation.
1. The `Compare` method returns a boolean value per site by default. When the underlying data type `T` of the `PinSiteData<T>` object is an array, the `TResult` type must be explicitly specified as a boolean array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*. Refer to the [`Compare<TOther, TResult>(ComparisonType, TOther)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType___0_), [`Compare<TOther, TResult>(ComparisonType, SiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_SiteData___0__) and [`Compare<TOther, TResult>(ComparisonType, PinSiteData<TOther>)`](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Compare.html#NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData_1_Compare__2_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_ComparisonType_NationalInstruments_SemiconductorTestLibrary_DataAbstraction_PinSiteData___0__)  method signatures in the API Reference documentation.

**Example of binary operations ALLOWED with `PinSiteData` objects:**

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

**Example of binary operation :x: NOT ALLOWED with `PinSiteData` objects:**

```csharp
var pinSiteData = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>
{ 
   ["VCC1"] = new Dictionary<int, double> { [0] = 3.5 } 
});
var siteData = new SiteData<long>(new Dictionary<int, long> { { 0, 1 } });

var result = pinSiteData.Add(siteData);
// The above operation will throw an NISemiconductorTestException with the following message:
// "Add operation, the inner data type of the first operand (System.Double) and that of the second operand (System.Int64) must match."
```

#### Unary Operations

The following table outlines the Unary operator-based mathematical operations available for `PinSiteData<T>` and specifies the permitted data types for `T` for each operation.

**Table of Unary Operations:**

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

**Usage Considerations:**

1. The `Invert`, `Log10`, and `SquareRoot` methods return a scalar double value per site by default. When the underlying data type `T` of the `PinSiteData<T>` object is an array, the `TResult` type must be explicitly specified as a `double` array with the same dimensions as `T`. Otherwise, a `NISemiconductorTestException` exception is thrown with exception message matching the exception scenarios of *Mismatched Array Dimensions*.
1. The `count` input value passed to the `ShiftLeft` and `ShiftRight` operators must be positive, otherwise an exception `NISemiconductorTestException` is thrown with exception message matching the exception scenarios of *Shift Count Must Be Positive*.

**Example of unary operation :heavy_check_mark: ALLOWED with `PinSiteData` objects:**

```csharp
var pinSiteData = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>
{ 
   ["VCC1"] = new Dictionary<int, double> { [0] = -3.5 }
}); 

var result = pinSiteData.Abs();
// The result is a PinSiteData<double> object containing scalar data for one pin, one site equivalent to: { ["VCC1"] = { [0] = 3.5 } }
```

**Example of unary operation :x: NOT ALLOWED with `PinSiteData` objects:**

```csharp
var pinSiteData = new PinSiteData<string>(new Dictionary<string, IDictionary<int, string>>
{ 
   ["VCC1"] = new Dictionary<int, string> { [0] = "Negative 3.5" } 
});

var result = pinSiteData.Abs();
// Above operation with throw an exception of NISemiconductorTestException with the following message:
// "Math operations not supported on the System.String type data."
```

---

### **Exception Conditions**

The aforementioned operations will throw `NISemiconductorTestException` in certain conditions, where the exception message will vary depending on the specific scenario. The possible exception conditions and their corresponding exception messages are list in the table below.

**Table of Possible Exceptions:**

|Condition|Description|Exception Message|Applicable To|
| :- | :- | :- | :- |
|Mismatched Array Dimensions |An exception occurs when array dimensions of the result array and input array are not matching.|When the underlying type, `T`, of the `SiteData` or `PinSiteData` object being operated on is an array, the `TResult` must also be an array of equal dimensions as the underlying type, `T`.|All Math Operations|
|Mismatched Array Lengths |An exception occurs when data array length of the first operand and that of the second array operand do not match.|For `<math operation>` operation, the data array length of the first operand (`<array length of operand 1>`) and that of the second operand (`<array length of operand 2>`) must match.|Binary Math Operations|
|Mismatched Operand Types |An exception occurs when operand types do not match.|For `<math operation>` operation, the inner data type of the first operand (`<type of first operand>`) and that of the second operand (`<type of second operand>`) must match.|Binary Math Operations|
|Result Type Must Be Array |An exception occurs when the defined data type of result is not array in case of array – scalar operations.|The `<TResult>` must be an array.|Binary Math Operations|
|Shift Count Must Be Positive |An exception occurs when the shift `count` is given negative.|The number of bits to shift must be positive.|`ShiftLeft` and `ShiftRight`|
|Type Not Supported |An exception occurs when the data type is not supported or appropriate for any math operation.|The `<math operation>` operation on the `<data type>` data type is not supported.|All Math Operations|
|Type Not Supported by Operation|An exception occurs when the data type of either operand is not supported.|The `<math operation>` operation on the `<data type>` data type is not supported.|All Math Operations|
