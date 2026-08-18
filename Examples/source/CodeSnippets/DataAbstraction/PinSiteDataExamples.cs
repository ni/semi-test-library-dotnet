using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.DataAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Data Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to use the PinStieData objects.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// that has already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class PinSiteDataExamples
    {
        // Generate Random Values
        private static readonly Random RandomNumber = new Random();
        private static readonly int SiteCount = 4;
        private static readonly string[] PinNames = new string[] { "PinA", "PinB", "PinC" };

        internal static void PinSiteDataMath()
        {
            PinSiteData<double> measurementA = Measure();
            PinSiteData<double> measurementB = Measure();

            // Binary Operations: Add, Compare, Divide, Maximum, Minimum, Multiply, Power, Subtract.
            PinSiteData<double> measurementBAddOffset = measurementB.Add(5);
            PinSiteData<bool> measurementsInBGreaterThanFive = measurementB.Compare(ComparisonType.GreaterThan, 5);
            PinSiteData<double> ratio = measurementB.Divide(measurementA);
            PinSiteData<double> maximumAToB = measurementA.Maximum(measurementB);
            PinSiteData<double> minimumA = measurementA.Minimum(2);
            PinSiteData<double> measurementAMultipledByTwo = measurementA.Multiply(2);
            PinSiteData<double> measurementAValuesToPowerOfTwo = measurementA.Power(2);
            PinSiteData<double> difference = measurementB.Subtract(measurementA);

            // Binary Operations Using Supported Operators (added in 24.5.1): Add, Subtract, Multiply, Divide.
            PinSiteData<double> measurementBAddmeasurementA = measurementB + measurementA;
            PinSiteData<double> measurementASubtractOffset = measurementB - 5;
            PinSiteData<double> measurementBMultipledByTwo = measurementB * 2;
            PinSiteData<double> ratioAOverB = measurementA / measurementB;

            // Unary Operations: Abs,  Invert, Log10, Negate, SquareRoot, Truncate.
            PinSiteData<double> absOfDifference = difference.Abs();
            PinSiteData<double> inverseOfMeasurementA = measurementB.Invert();
            PinSiteData<double> log10OfMeasurementA = measurementA.Log10();
            PinSiteData<double> negationOfMeasurementB = measurementB.Negate();
            PinSiteData<double> sqrtOfMeasurementB = measurementB.SquareRoot();
            PinSiteData<double> measurementBTruncated = measurementB.Truncate();

            // Unary Operations (added in 24.5.1): Max, Min, Mean
            SiteData<double> maxAcrossPinsForEachSite = difference.Max();
            SiteData<double> minAcrossPinsForEachSite = difference.Min();
            SiteData<double> meanAcrossPinsForEachSite = difference.Mean();

            // Unary Operations with greater granularity (added in 24.5.1): MaxByPin, MaxBySite, MinByPin, MinBySite, MeanBySite
            Dictionary<int, (double, string[])> maxAcrossPinsWithWhichPinsForEachSite = difference.MaxByPin();
            Dictionary<string, (double, int[])> maxAcrossSitesWithWhichSitesForEachPin = difference.MaxBySite();
            Dictionary<int, (double, string[])> minAcrossPinsWithWhichPinsForEachSite = difference.MinByPin();
            Dictionary<string, (double, int[])> minAcrossSitesWithWhichSitesForEachPin = difference.MinBySite();
            Dictionary<string, double> meanAcrossSitesForEachPin = difference.MeanBySite();
        }

        // These operations were added in the 24.5.1 release.
        internal static void PinSiteDataBitwiseOperations()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // Setup example dual-site data values
            PinSiteData<byte> data1 = new PinSiteData<byte>(pinNames, siteNumbers, 0b_0001);
            PinSiteData<byte> data2 = new PinSiteData<byte>(pinNames, siteNumbers, 0b_0010);

            // Shift Left Operations
            PinSiteData<byte> data1ShiftedLeft = data1.ShiftLeft(2);           // new value for all pins and sites: 0b_0100
            PinSiteData<byte> data2ShiftedLeftWithOperator = data2 << 2;       // new value for all pins and sites: 0b_1000
            // Shift Right Operations
            PinSiteData<byte> data1ShiftedRight = data1.ShiftRight(2);         // new value for all pins and sites: 0b_0000
            PinSiteData<byte> data2ShiftedRightWithOperator = data2 >> 1;      // new value for all pins and sites: 0b_0001
            // Bitwise And Operations (using method and operator, as both scalar and PinSiteData values).
            PinSiteData<byte> data1AndWithData2 = data1.BitwiseAnd(data2);     // new value for all pins and sites: 0b_0000
            PinSiteData<byte> data2AmdWithOperator = data2 & 0b_0010;          // new value for all pins and sites: 0b_0010
            // Bitwise Or Operations (using method and operator, as both scalar and PinSiteData values).
            PinSiteData<byte> data1Or = data1.BitwiseOr(0b_0011);              // new value for all pins and sites: 0b_0011
            PinSiteData<byte> data1OrWithData2UsingOperator = data1 | data2;   // new value for all pins and sites: 0b_0011
            // Bitwise XOr Operations (using method and operator, as both scalar and PinSiteData values).
            PinSiteData<byte> data1XorWithData2 = data1.BitwiseXor(data2);     // new value for all pins and sites: 0b_0011
            PinSiteData<byte> data1XorWithOperator = data1 ^ 0b_0011;          // new value for all pins and sites: 0b_0010
            // Bitwise Compliment Operations (using method and operator).
            PinSiteData<byte> data1Complement = data1.BitwiseComplement();     // new value for all pins and sites: 0b_1111_1110
            PinSiteData<byte> data1ComplementWithOperator = ~data2;            // new value for all pins and sites: 0b_1111_1101
        }

        internal static void PinSiteDataConvertIntToDouble()
        {
            // Establish new PinSiteData object with integer values.
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1", "VCC2" };
            // Constructs a PinSiteData object with the same data value across all pins and sites.
            var integerPinSiteData = new PinSiteData<int>(pinNames, siteNumbers, 256);

            // Use select function to operate on each site value to cast it to a double.
            // Note the select function was introduced in the 24.5.1 release.
            PinSiteData<double> doublePinSiteData = integerPinSiteData.Select(value => (double)value);
        }

        internal static void PinSiteDataTransformUintArrayToHexString()
        {
            // Establish new PinSiteData object of signed integer arrays for two sties.
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1, };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1", "VCC2" };
            // Constructs a PinSiteData object with the same data value across all pins and sites.
            var pinSiteData = new PinSiteData<uint[]>(pinNames, siteNumbers, new uint[] { 128, 32, 64 });

            // Use select function to operate on each site value to cast it to a double.
            // Note the select function was introduced in the 24.5.1 release.
            // The expected resulting string value would be: e0
            PinSiteData<string> doubleData = pinSiteData.Select(SumSamplesAndRepresentAsHexString);

            // Local method to define the data transformation
            string SumSamplesAndRepresentAsHexString(uint[] arrayOfSamples)
            {
                var wholeSample = arrayOfSamples.Sum(x => x);
                return $"{wholeSample:X}";
            }
        }

        internal static void PinSiteDataMethods()
        {
            PinSiteData<double> measurementsForPinsABandC = Measure();

            int[] sitesMeasured = measurementsForPinsABandC.SiteNumbers;
            string[] pinsMeasured = measurementsForPinsABandC.PinNames;
            double site0ValueForPinA = measurementsForPinsABandC.GetValue(siteNumber: 0, pinName: "PinA");
            IDictionary<string, double> site0ValuesForAllPins = measurementsForPinsABandC.ExtractSite(siteNumber: 0);
            SiteData<double> justPinAMeasurments = measurementsForPinsABandC.ExtractPin(pinName: "PinA");
            string[] pinNamesToExtract = new string[] { "PinA", "PinB" };
            PinSiteData<double> subsetOfPinAandPinBMeasurments = measurementsForPinsABandC.ExtractPins(pinNamesToExtract);
        }

        internal static void ConstructWithArrays()
        {
            // Pin names to associate with the data.
            var pinNames = new string[] { "VCC1", "VCC2" };
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0 };
            // Per-pin SiteData objects.
            var perPinSiteData = new[]
            {
                new SiteData<double>(siteNumbers, 1.5),
                new SiteData<double>(siteNumbers, 2.5)
            };
            // Constructs a PinSiteData object with pin names and associated SiteData object array.
            var pinSiteData = new PinSiteData<double>(pinNames, perPinSiteData);
        }

        internal static void BuildWithWithArraysAndSetValueWithPerPinData()
        {
            // Pin names to associate with the data.
            var pinNames = new string[] { "VCC1", "VCC2" };
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0 };
            // Per-pin data values.
            var perPinData = new[] { 1.5, 2.5 };

            // Construct PinSiteData object.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers);
            // Use SetValue to assign the per-pin values to all sites within the PinSiteData object.
            for (int i = 0; i < pinNames.Length; i++)
            {
                pinSiteData.SetValue(perPinData[i], pinNames[i]);
            }
        }

        internal static void BuildWithArraysAndSetValueWithPerPinPerSiteData()
        {
            // Pin names to associate with the data.
            var pinNames = new string[] { "VCC1", "VCC2" };
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1, 2, 3 };
            // 2D jagged array of pin and site unique data,
            // where the first dimension represents pins (2) and the second dimension represents sites (4).
            var perPinAndSiteData = new double[][]
            {
                new[] { 1.5, 1.6, 1.7, 1.8 }, // VCC1 data for sites: 0, 1, 2, 3
                new[] { 3.3, 3.4, 3.5, 3.6 } // VCC2 data for sites: 0, 1, 2, 3
            };

            // Construct empty PinSiteData object, this will be dynamically filled with data using SetValue.
            var pinSiteData = new PinSiteData<double>();
            // Use SetValue to assign values to the PinSiteData object for each pin and site.
            for (int pinIndex = 0; pinIndex < pinNames.Length; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
                {
                    pinSiteData.SetValue(perPinAndSiteData[pinIndex][siteIndex], pinNames[pinIndex], siteNumbers[siteIndex]);
                }
            }
        }

        internal static void ConstructWithArraysWithSystemPin()
        {
            // Pin names to associate with the data.
            var pinNames = new string[] { "VCC1", "VCC2", "SystemSupply" };
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1 };
            // Per-pin SiteData objects.
            // Note that data associated with system pins is considered site-agnostic,
            // and site-agnostic data can represented with -1 as the site value.
            var perPinSiteData = new[]
            {
                new SiteData<double>(siteNumbers, 1.5),
                new SiteData<double>(siteNumbers, 2.5),
                new SiteData<double>(new[] { -1 }, -22.5)
            };
            // Constructs a PinSiteData object with pin names and associated SiteData object array,
            // inclusive of system pin data.
            var pinSiteData = new PinSiteData<double>(pinNames, perPinSiteData);
        }

        internal static void BuildWithArraysWithSystemPin()
        {
            // Pin names to associate with the data.
            var pinNames = new string[] { "VCC1", "VCC2", "SystemSupply" };
            // Site numbers to associate with the DUT pins.
            var siteNumbers = new int[] { 0, 1 };
            // Per-pin, per-site data array.
            // Each element in the perPinData array corresponds to a pin in the pinNames array.
            var perPinData = new double[] { 1.5, 2.5, -22.5 };

            // Note that data associated with system pins is considered site-agnostic,
            // and site-agnostic data can represented with -1 as the site value.
            // Typically, the site numbers array used within a code module will not explicitly contain a -1 value.
            // Therefore, -1 will need to be appended to the siteNumbers array when creating a PinSiteData object that contains system pin data.
            // This can be done using array operations or by converting the array into a List, but in most instances it is best not to modify the original siteNumbers array,
            // as it may be used elsewhere in the code module where the system pin data is not relevant and can result in unintended consequences.
            // The most efficient way to handle this is by first constructing the PinSiteData object with the known Pin Names and Site Numbers array,
            // and then adding the -1 site manually with the AddSite method.
            // Alternatively, the site and pins can simply be added dynamically when SetValue is called.
            var pinSiteData = new PinSiteData<double>(pinNames: new string[] { "VCC1", "VCC2" }, siteNumbers: siteNumbers);
            pinSiteData.AddSite("SystemSupply", -1);
            for (int i = 0; i < perPinData.Length; i++)
            {
                // Check if the pin is a system pin and set the value accordingly.
                // In this example, we are assuming that the system pin is named "SystemSupply".
                // However, in practice, you would want to have a more robust way to identify system pins,
                // such as utilizing the ISemiconductorModuleContext.GetPins method.
                if (pinNames[i] == "SystemSupply")
                {
                    // For system pins, set the value for site -1 to indicate site-agnostic data.
                    pinSiteData.SetValue(value: perPinData[i], pinName: pinNames[i], siteNumbers: -1);
                }
                else
                {
                    // Set the uniform value for each DUT pin across all sites.
                    // Note that care must be taken when calling SetValue with system pins present in the PinSiteData object,
                    // As the SetValue(T value) and SetValue(T value, param string) overload will apply the same value to all sites declared within the PinSiteData, including system sites.
                    // Therefore, it is best to avoid those overloads when working with system pins,
                    // and instead, explicitly specify which site number to set a value for, as shown below.
                    pinSiteData.SetValue(value: perPinData[i], pinName: pinNames[i], siteNumbers: siteNumbers);
                }
            }
        }

        internal static void ConstructWithDictionaryWithSystemPin()
        {
            // Dictionary containing pin- and site-unique data, including system pin data.
            // Note that data associated with system pins is considered site-agnostic,
            // and site-agnostic data can represented with -1 as the site value.
            var pinAndSiteUnqiueDataDictionary = new Dictionary<string, IDictionary<int, double>>
            {
                ["VCC1"] = new Dictionary<int, double> { [0] = 1.5, [1] = 11.5 },
                ["SystemSupply"] = new Dictionary<int, double> { [-1] = -22.5 }
            };
            // Constructs a PinSiteData object with pin and site unique data dictionary,
            // inclusive of system pin data.
            var pinSiteData = new PinSiteData<double>(pinAndSiteUnqiueDataDictionary);
        }

        internal static void BuildWithDictionaryWithSystemPin()
        {
            // Dictionary containing pin and site-unique data, including system pin data.
            // Note that data associated with system pins is considered site-agnostic,
            // and site-agnostic data can represented with -1 as the site value.
            var pinAndSiteUnqiueDataDictionary = new Dictionary<string, IDictionary<int, double>>
            {
                ["VCC1"] = new Dictionary<int, double> { [0] = 1.5, [1] = 11.5 },
                ["SystemSupply"] = new Dictionary<int, double> { [-1] = -22.5 }
            };
            // Extract the pin names from the dictionary keys to use for constructing the PinSiteData object.
            var pinNames = pinAndSiteUnqiueDataDictionary.Keys.ToArray();
            // Construct a PinSiteData object with the pin names.
            var pinSiteData = new PinSiteData<double>(pinNames);
            // Add the site numbers to the PinSiteData object.
            foreach (var pin in pinNames)
            {
                // Parse the site numbers and corresponding value for the current pin from the dictionary.
                var siteNumberToValueDictionary = pinAndSiteUnqiueDataDictionary[pin];
                foreach (var siteNumber in siteNumberToValueDictionary.Keys)
                {
                    // Set the value for the current pin and site number in the PinSiteData object.
                    pinSiteData.SetValue(siteNumberToValueDictionary[siteNumber], pin, siteNumber);
                }
            }
        }

        internal static void ConstructWithPinDataDictionaryAndSiteNumbersArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3, 1 };
            // Dictionary containing pin-unique data.
            var perPinData = new Dictionary<string, double> { ["VDET"] = 22, ["VCC1"] = 44, ["VCC2"] = 33 };
            // Constructs a PinSiteData object with a pin specific data dictionary and siteNumbers array.
            var pinSiteData = new PinSiteData<double>(siteNumbers, perPinData);
        }

        internal static void BuildWithPinDataDictionaryAndSiteNumbersArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3, 1 };
            // Dictionary containing pin-unique data.
            var perPinData = new Dictionary<string, double> { ["VDET"] = 22, ["VCC1"] = 44, ["VCC2"] = 33 };
            // Get the pin names from the dictionary keys.
            var pinNames = perPinData.Keys.ToArray();
            // Use the empty constructor to build the PinSiteData dynamically.
            // This is useful when pin names or site numbers are not all known upfront.
            // For this example, since the pin names and site numbers are known, it would also be possible to provide that information directly to the constructor.
            var pinSiteData = new PinSiteData<double>();
            // Add all pin names first. No sites yet so each pin gets an empty SiteData.
            pinSiteData.AddPin(pinNames);
            // Add site numbers across all existing pins, initializing each to the default value (0.0).
            pinSiteData.AddSite(siteNumbers);
            // Set the per-pin value for each pin, repeating across all its sites.
            foreach (var pinName in pinNames)
            {
                pinSiteData.SetValue(perPinData[pinName], pinName);
            }
        }

        internal static void ConstructWithPinDataDictionaryAndSiteNumbersArrayWithSystemPin()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3, 1 };
            // Dictionaries containing pin-unique data.
            // Two separate dictionaries are used to delineate between DUT pins from System pin data.
            var perDutPinData = new Dictionary<string, double> { ["VDET"] = 22, ["VCC1"] = 44, ["VCC2"] = 33 };
            var perSystemPinSiteData = new Dictionary<string, double> { ["SystemSupply"] = -15 };
            // First, construct a PinSiteData object with the DUT pin specific data dictionary and the siteNumbers array.
            // Then, combine it with a new PinSiteData object constructed for the system pin specific data dictionary,
            // where the siteNumbers input is an array containing a single element value of -1.
            // Note that data associated with system pins is considered site-agnostic,
            // and site-agnostic data can represented with -1 as the site value.
            var pinSiteData = new PinSiteData<double>(siteNumbers, perDutPinData)
                .Combine(new PinSiteData<double>(new[] { -1 }, perSystemPinSiteData));
        }

        internal static void BuildWithPinDataDictionaryAndSiteNumbersArrayWithSystemPin()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3, 1 };
            // Dictionaries containing pin-unique data.
            // Two separate dictionaries are used to delineate between DUT pins from System pin data.
            var perDutPinData = new Dictionary<string, double> { ["VDET"] = 22, ["VCC1"] = 44, ["VCC2"] = 33 };
            var perSystemPinSiteData = new Dictionary<string, double> { ["SystemSupply"] = -15 };
            // Create an empty PinSiteData object to build dynamically.
            var pinSiteData = new PinSiteData<double>();
            // Set the per-pin value for each DUT pin, repeated across all sites for that pin.
            // All the pins and sites are added dynamically to the PinSiteData object as they are encountered.
            foreach (var pinName in perDutPinData.Keys)
            {
                pinSiteData.SetValue(perDutPinData[pinName], pinName, siteNumbers);
            }
            // System pins are site-agnostic; -1 is used as the site number.
            // SetValue automatically adds the SystemSupply pin and site -1 since they do not yet exist.
            foreach (var pinName in perSystemPinSiteData.Keys)
            {
                pinSiteData.SetValue(perSystemPinSiteData[pinName], pinName, -1);
            }
        }

        internal static void ConstructWithArraysForCommonDataValue()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1", "VCC2" };
            // Constructs a PinSiteData object with the same data value across all pins and sites.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, 55);
        }

        internal static void BuildWithArraysForCommonDataValue()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1", "VCC2" };
            // Since both pin names and site numbers are known, providing this information to the constructor is most efficient.
            // Alternatively, you can create empty PinSiteData and then add pins and sites manually with the AddPin and AddSite methods,
            // or have them be added dynamically as specified by the SetValue method.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers);
            // Set the same value across all pins and all sites at once.
            pinSiteData.SetValue(55);
        }

        internal static void ConstructWithPinUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // Per-pin data values.
            var perPinData = new double[] { 42, 105 };
            // Constructs a PinSiteData object with pin unique data.
            // Where the specified pin unique data will be repeated across all sites.
            // The lengths of the pinNames and perPinData inputs must be equal,
            // and both the pinNames and siteNumbers arrays must each contain unique values,
            // otherwise an exception will be thrown.
            // Non-sequential site order is accepted.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, perPinData);
        }

        internal static void BuildWithPinUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // Per-pin data values, where each element is the value for the pin at the same index in pinNames.
            var perPinData = new double[] { 42, 105 };
            // Use the empty constructor to build the PinSiteData dynamically.
            // This is useful when pin names or site numbers are not all known upfront.
            // For this example, since the pin names and site numbers are known, it would also be possible to provide that information directly to the constructor.
            var pinSiteData = new PinSiteData<double>();
            // Add pins first, then sites — each site is initialized to the default value (0.0).
            pinSiteData.AddPin(pinNames);
            pinSiteData.AddSite(siteNumbers);
            // Set the per-pin value for each pin, repeating the value across all its sites.
            for (int i = 0; i < pinNames.Length; i++)
            {
                pinSiteData.SetValue(perPinData[i], pinNames[i]);
            }
        }

        internal static void ConstructWithSiteUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // Per-site data values.
            var perSiteData = new double[] { 42, 105, 55 };
            // Constructs a PinSiteData object with site unique data.
            // Where the specified site unique data will be repeated for all pins.
            // The lengths of the siteNumbers and perSiteData inputs must be equal,
            // and both the pinNames and siteNumbers arrays must each contain unique values,
            // otherwise an exception will be thrown.
            // Non-sequential site order is accepted.
            var pinSiteData = new PinSiteData<double>(siteNumbers, pinNames, perSiteData);
        }

        internal static void BuildWithSiteUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // Per-site data values, where each element is the value for the site at the same index in siteNumbers.
            var perSiteData = new double[] { 42, 105, 55 };
            // Since both pin names and site numbers are known, providing this information to the constructor is most efficient.
            // Alternatively, you can create empty PinSiteData and then add pins and sites manually with the AddPin and AddSite methods,
            // or have them be added dynamically as specified by the SetValue method.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers);
            // Set the per-site value across all pins for each site.
            for (int i = 0; i < siteNumbers.Length; i++)
            {
                pinSiteData.SetValue(perSiteData[i], siteNumbers[i]);
            }
        }

        internal static void ConstructWithPinAndSiteUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // 2D jagged array of pin and site unique data,
            // where the first dimension represents pins (2) and the second dimension represents sites (3).
            var perPinPerSiteData = new double[][]
            {
                new double[] { 42, 105, 206 },
                new double[] { 55, 2048, 0.5 }
            };
            // Constructs a PinSiteData object with pin and site unique data.
            // Where the specified data value is unique for each pin and each site.
            // The length of pinNames must be equal to the length of the first dimension of perPinPerSiteData.
            // Similarly, the length of siteNumbers must be equal to the length of the second dimension of perPinPerSiteData.
            // Additionally, both the pinNames and siteNumbers arrays must each contain unique values.
            // If any of the above conditions are not met, an exception will be thrown.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, perPinPerSiteData);
        }

        internal static void BuildWithPinAndSiteUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // 2D jagged array of pin and site unique data,
            // where the first dimension represents pins (2) and the second dimension represents sites (3).
            // Note that order of the elements in the perPinPerSiteData array must match the order of the pinNames and siteNumbers arrays.
            var perPinPerSiteData = new double[][]
            {
                new double[] { 42, 105, 206 }, // VDET data for sites: 2, 4, 3.
                new double[] { 55, 2048, 0.5 } // VCC1 data for sites: 2, 4, 3.
            };
            // Use the pin-only constructor, then add the sites.
            // This constructor allows you to declare pins upfront and add sites later on.
            var pinSiteData = new PinSiteData<double>(pinNames);
            // Add sites.
            pinSiteData.AddSite(siteNumbers);
            // Set a unique value for each pin and each site combination.
            for (int pinIndex = 0; pinIndex < pinSiteData.PinNames.Length; pinIndex++)
            {
                string pinName = pinSiteData.PinNames[pinIndex];
                for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
                {
                    int siteNumber = siteNumbers[siteIndex];
                    pinSiteData.SetValue(perPinPerSiteData[pinIndex][siteIndex], pinName, siteNumber);
                }
            }
        }

        internal static void ConstructWithSiteAndPinUniqueDataArray()
        {
            var siteNumbers = new int[] { 2, 4, 3 };
            var pinNames = new string[] { "VDET", "VCC1" };
            // 2D jagged array of pin and site unique data,
            // where the first dimension represents sites (3) and the second dimension represents pins (2).
            var perSitePerPinData = new double[][]
            {
                new double[] { 42,  55 },
                new double[] { 105, 2048 },
                new double[] { 206, 0.5 },
            };
            // Constructs a PinSiteData object with site and pin unique data.
            // Where the specified data value is unique for each site and each pin.
            // The length of siteNumbers must be equal to the length of the first dimension of perSitePerPinData.
            // Similarly, the length of pinNames must be equal to the length of the second dimension of perSitePerPinData.
            // Additionally, both the pinNames and siteNumbers arrays must each contain unique values.
            // If any of the above conditions are not met, an exception will be thrown.
            // Non-sequential site order is accepted
            var pinSiteData = new PinSiteData<double>(siteNumbers, pinNames, perSitePerPinData);
        }

        internal static void BuildWithSiteAndPinUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // 2D jagged array of pin and site unique data,
            // where the first dimension represents sites (3) and the second dimension represents pins (2).
            var perSitePerPinData = new double[][]
            {
                new double[] { 42,  55 },
                new double[] { 105, 2048 },
                new double[] { 206, 0.5 },
            };
            // Since both pin names and site numbers are known, providing this information to the constructor is most efficient.
            // Alternatively, you can create empty PinSiteData and then add pins and sites manually with the AddPin and AddSite methods,
            // or have them be added dynamically as specified by the SetValue method.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers);
            // Set a unique value for each site and each pin combination.
            // Outer loop iterates over sites, inner loop iterates over pins — matching perSitePerPinData layout.
            for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
            {
                for (int pinIndex = 0; pinIndex < pinNames.Length; pinIndex++)
                {
                    pinSiteData.SetValue(perSitePerPinData[siteIndex][pinIndex], pinNames[pinIndex], siteNumbers[siteIndex]);
                }
            }
        }

        internal static void ConstructWithDefaultConstructor()
        {
            // Constructs an empty PinSiteData object using the default (parameterless) constructor.
            // No pins or sites are associated with the object at this point.
            var pinSiteData = new PinSiteData<double>();
            // (empty)

            // Use AddPin to add one or more pin names to the empty PinSiteData object.
            // Each added pin is initialized with existing site definitions.
            // Since no sites exist yet, an empty SiteData object is assigned to each added pin.
            pinSiteData.AddPin("VDET", "VCC1", "VCC2");
            // Pin    | (no sites)
            // VDET   |
            // VCC1   |
            // VCC2   |

            // Use AddSite to add site numbers across all existing pins.
            // Since VDET, VCC1, and VCC2 were already added, sites 0, 1 and 2 are added to each of them.
            // Each new site is initialized with the default value of the data type (0.0 for double).
            // Note that the AddSite method can be invoked either by passing a preformed array, as is shown below,
            // or by listing out the sites as separate parameter inputs, for example, pinSiteData.AddSite(0, 1, 2).
            var siteNumbersToAdd = new int[] { 0, 1, 2 };
            pinSiteData.AddSite(siteNumbersToAdd);
            // Pin    | Site 0 | Site 1 | Site 2
            // VDET   |  0.0   |  0.0   |  0.0
            // VCC1   |  0.0   |  0.0   |  0.0
            // VCC2   |  0.0   |  0.0   |  0.0

            // Use SetValue to assign a specific value to all pins and all sites at once.
            // This sets the value 1.5 for all pin-site combinations (VDET/0, VDET/1, VCC1/0, VCC1/1, VCC2/0, VCC2/1).
            pinSiteData.SetValue(value: 1.5);
            // Pin    | Site 0 | Site 1 | Site 2
            // VDET   |  1.5   |  1.5   |  1.5
            // VCC1   |  1.5   |  1.5   |  1.5
            // VCC2   |  1.5   |  1.5   |  1.5

            // Use SetValue with specific site numbers to overwrite only certain sites across all pins.
            // This sets 3.3 for site 1 of all pins (VDET/1, VCC1/1, VCC2/1), leaving site 0 and 2 unchanged at 1.5.
            pinSiteData.SetValue(value: 3.3, siteNumbers: 1);
            // Pin    | Site 0 | Site 1 | Site 2
            // VDET   |  1.5   |  3.3   |  1.5
            // VCC1   |  1.5   |  3.3   |  1.5
            // VCC2   |  1.5   |  3.3   |  1.5

            // Use RemoveSite to remove a specific site from all pins.
            // This removes site 2 from VDET, VCC1, and VCC2, leaving sites 0 and 1 for each pin.
            pinSiteData.RemoveSite(2);
            // Pin    | Site 0 | Site 1
            // VDET   |  1.5   |  3.3
            // VCC1   |  1.5   |  3.3
            // VCC2   |  1.5   |  3.3

            // Use RemovePin to remove one or more pins from the PinSiteData object entirely.
            // This removes VCC2 and all its associated site data from the object.
            pinSiteData.RemovePin("VCC2");
            // Pin    | Site 0 | Site 1
            // VDET   |  1.5   |  3.3
            // VCC1   |  1.5   |  3.3

            // Use SetValue with a specific pin name and site numbers to overwrite a single pin-site combination.
            // This sets 5.0 only for VCC1 at site 0, leaving all other pin-site combinations unchanged.
            pinSiteData.SetValue(value: 5.0, pinName: "VCC1", siteNumbers: 0);
            // Pin    | Site 0 | Site 1
            // VDET   |  1.5   |  3.3
            // VCC1   |  5.0   |  3.3
        }

        internal static void ConstructWithSinglePinNameAndSiteNumbers()
        {
            // Constructs a PinSiteData object with a single pin name and associated site numbers.
            // The site numbers are passed as a params array, so any number of site numbers can be specified.
            // This constructor is useful for initially declaring data for only one pin spanning across multiple sites.
            var siteNumbers = new int[] { 0, 1, 2 };
            var pinSiteData = new PinSiteData<double>("VDET", siteNumbers);
            // Pin    | Site 0 | Site 1 | Site 2
            // VDET   |  0.0   |  0.0   |  0.0

            // Use AddPin to add additional pins to the existing PinSiteData object.
            // The newly added pins inherit the existing site definitions (sites 0, 1, and 2).
            // Each new pin is initialized with the default value (0.0 for double) for each site.
            pinSiteData.AddPin("VCC1", "VCC2");
            // Pin    | Site 0 | Site 1 | Site 2
            // VDET   |  0.0   |  0.0   |  0.0
            // VCC1   |  0.0   |  0.0   |  0.0
            // VCC2   |  0.0   |  0.0   |  0.0

            // Use AddSite to add an additional site to pins (VDET, VCC1).
            // Site 3 is added to pins VDET, VCC1 and initialized with the default value (0.0 for double).
            // Note that using the overload for the AddSite method shown below can result in a jagged PinSiteData object,
            // where different pins have different numbers of sites.
            // This can be useful for certain scenarios but should be leveraged with caution.
            // The invocation below results in VCC2 not having a Site 3 value since only pins VDET and VCC1 are included in the pinNames argument.
            pinSiteData.AddSite(pinNames: new string[] { "VDET", "VCC1" }, siteNumbers: 3);
            // Pin    | Site 0 | Site 1 | Site 2 | Site 3
            // VDET   |  0.0   |  0.0   |  0.0   |  0.0
            // VCC1   |  0.0   |  0.0   |  0.0   |  0.0
            // VCC2   |  0.0   |  0.0   |  0.0   |  ---

            // Use SetValue with pin names and site numbers to assign specific values to a subset of pins across a subset of sites.
            // Note that the invocation below results in Site 4 being added since it does not already exist,
            // and is added to VCC1 and VCC2 only and not to VDET.
            // This operation further exaggerates the jagged structure of the PinSiteData object.
            // Also, note that VCC2 remains without a Site 3 value as a result of the preceding operation.
            // This sets 1.8 for both VCC1 and VCC2 at sites 0, 1 and 4.
            pinSiteData.SetValue(value: 1.8, pinNames: new string[] { "VCC1", "VCC2" }, siteNumbers: new int[] { 0, 1, 4 });
            // Pin    | Site 0 | Site 1 | Site 2 | Site 3 | Site 4
            // VDET   |  0.0   |  0.0   |  0.0   |  0.0   |  ---
            // VCC1   |  1.8   |  1.8   |  0.0   |  0.0   |  1.8
            // VCC2   |  1.8   |  1.8   |  0.0   |  ---   |  1.8

            // Use RemoveSite with specific pin names to remove a site from only those pins.
            // This removes site 2 from VDET and VCC1 only, leaving VCC2's site definitions unchanged.
            // Note that the invocation below results in Site 2 being removed from both VDET and VCC1
            // but is retained for VCC2.
            // Additionally, since VDET still lacks Site 4 and VCC2 still lacks Site 3,
            // this operation again perpetuates the jagged structure of the PinSiteData object.
            pinSiteData.RemoveSite(pinNames: new string[] { "VDET", "VCC1" }, siteNumbers: 2);
            // Pin    | Site 0 | Site 1 | Site 2 | Site 3 | Site 4
            // VDET   |  0.0   |  0.0   |  ---   |  0.0   |  ---
            // VCC1   |  1.8   |  1.8   |  ---   |  0.0   |  1.8
            // VCC2   |  1.8   |  1.8   |  0.0   |  ---   |  1.8

            // Use SetValue with a pin name only (no site numbers) to set a value for all sites on that pin.
            // This sets 3.3 for VDET across all of its sites (0, 1, 3).
            pinSiteData.SetValue(value: 3.3, pinNames: "VDET");
            // Pin    | Site 0 | Site 1 | Site 2 | Site 3 | Site 4
            // VDET   |  3.3   |  3.3   |  ---   |  3.3   |  ---
            // VCC1   |  1.8   |  1.8   |  ---   |  0.0   |  1.8
            // VCC2   |  1.8   |  1.8   |  0.0   |  ---   |  1.8
        }

        internal static void ConstructWithPinNamesAndSiteNumbers()
        {
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1", "VCC2" };
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1 };
            // Constructs a PinSiteData object with pin names and an array of site numbers.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers);
            // Pin    | Site 0 | Site 1
            // VDET   |  0.0   |  0.0
            // VCC1   |  0.0   |  0.0
            // VCC2   |  0.0   |  0.0

            // Use AddSite to extend all existing pins with additional sites.
            // Sites 2 and 3 are added to VDET, VCC1, and VCC2, each initialized with the default value (0.0).
            pinSiteData.AddSite(2, 3);
            // Pin    | Site 0 | Site 1 | Site 2 | Site 3
            // VDET   |  0.0   |  0.0   |  0.0   |  0.0
            // VCC1   |  0.0   |  0.0   |  0.0   |  0.0
            // VCC2   |  0.0   |  0.0   |  0.0   |  0.0

            var perSiteData = new double[] { 2.4, 3.6, 5.7, 4.3 };
            // Use SetValue to assign per-site data values for both VDET and VCC1 pins at once.
            // Note that the site values for VCC2 are left untouched and remain at their default value (0.0).
            for (int i = 0; i < perSiteData.Length; i++)
            {
                pinSiteData.SetValue(value: perSiteData[i], pinNames: new string[] { "VDET", "VCC1" }, siteNumbers: i);
            }
            // Pin    | Site 0 | Site 1 | Site 2 | Site 3
            // VDET   |  2.4   |  3.6   |  5.7   |  4.3
            // VCC1   |  2.4   |  3.6   |  5.7   |  4.3
            // VCC2   |  0.0   |  0.0   |  0.0   |  0.0

            // Use RemoveSite to remove a specific site from a specific pin.
            // This removes site 2 from VCC2 only, while VDET and VCC1 retain site 2.
            // Note that using the overload for the AddSite method shown below can result in a jagged PinSiteData object,
            // where different pins have different numbers of sites.
            // This can be useful for certain scenarios but should be leveraged with caution.
            // The invocation below results in Site 2 being removed from VCC2 but is retained for other pins.
            pinSiteData.RemoveSite(pinName: "VCC2", siteNumbers: 2);
            // Pin    | Site 0 | Site 1 | Site 2 | Site 3
            // VDET   |  2.4   |  3.6   |  5.7   |  4.3
            // VCC1   |  2.4   |  3.6   |  5.7   |  4.3
            // VCC2   |  0.0   |  0.0   |  ---   |  0.0

            // Use RemoveSite to remove specific sites from all pins.
            // This removes sites 2 and 3 from VDET, VCC1, and VCC2, leaving sites 0 and 1 for each pin.
            pinSiteData.RemoveSite(2, 3);
            // Pin   | Site 0 | Site 1
            // VDET  |  2.4   |  3.6
            // VCC1  |  2.4   |  3.6
            // VCC2  |  0.0   |  0.0

            // Use RemovePin to remove multiple pins from the PinSiteData object at once.
            // This removes both VCC1 and VCC2 and all their associated site data, leaving only VDET.
            pinSiteData.RemovePin("VCC1", "VCC2");
            // Pin    | Site 0 | Site 1
            // VDET   |  2.4   |  3.6

            // Use SetValue with a specific pin name and site number to overwrite a single combination.
            // This sets 0.9 only for VDET at site 0, leaving all other values unchanged.
            pinSiteData.SetValue(value: 0.9, pinName: "VDET", siteNumbers: 0);
            // Pin    | Site 0 | Site 1
            // VDET   |  0.9   |  3.6
        }

        /// <summary>
        /// This method is just for example purposes to simulate a measurement result being collected.
        /// </summary>
        /// <returns>Simulated random measurement result.</returns>
        internal static PinSiteData<double> Measure()
        {
            var siteDataArray = new SiteData<double>[PinNames.Length];
            for (int i = 0; i < PinNames.Length; i++)
            {
                siteDataArray[i] = new SiteData<double>(GenerateRandomPerSiteData());
            }

            return new PinSiteData<double>(PinNames, siteDataArray);
        }
        private static double[] GenerateRandomPerSiteData()
        {
            return Enumerable.Range(0, SiteCount).Select(x => x * RandomNumber.NextDouble()).ToArray();
        }
    }
}
