# Using Measurement Studio Analysis Library with PinSiteData

NI Measurement Studio Analysis Library provides powerful and easy-to-use mathematical analysis tools for measurement applications written for the Microsoft .NET Framework. For more information, refer to *Measurement Studio Analysis Help*.

> [!NOTE]
> Measurement Studio Analysis Library does not support `PinSiteData` objects directly.

Follow the steps below to use Measurement Studio Analysis Library with `PinSiteData`.

1. Open NI License Manager and verify that your system has a valid license for the Measurement Studio Development System, and check the specific license level: Standard, Professional, or Enterprise.
2. In the CS project, ensure there is an assembly reference to one of the Analysis Library assemblies below according to the license you have.
   - `NationalInstruments.Analysis.Standard.dll`
   - `NationalInstruments.Analysis.Professional.dll`
   - `NationalInstruments.Analysis.Enterprise.dll`
3. In the CS project, ensure there is an assembly reference to the `NationalInstruments.Common.dll` assembly which contains the `ComplexDouble` data type.
4. In the CS project, ensure there is either a package reference to `NationalInstruments.SemiconductorTestLibrary` or an assembly reference to the `NationalInstruments.SemiconductorTestLibrary.Abstractions.dll` assembly which contains the `PinSiteData` data type.
5. In the CS file, add the following `using` directives.
   - `using NationalInstruments;`
   - `using NationalInstruments.Analysis;`
   - `using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;`
6. Enumerate each element of `PinSiteData` object using the `Select` method.
7. Use a Lambda expression to select the inner data element.
8. Make a copy of the selected data element within the body of the expression if both of the following statements are true:
   - The inner data element is reference type (e.g. an array).
   - The Analysis Library method to be invoked operates data in place.
9. Convert the data copy as necessary, then pass it as an input into the desired Analysis Library method..

> [!NOTE]
> The standard licensing for STS Software activates the Professional license level for Measurement Studio by default.
>
> The necessary references mentioned in step 3 & 4 should already be defined within the CSProject file if it was created by the STS Project Creation Tool using the NI Default - C#/.NET template.
>
> When the body of your Lambda expression spans multiple lines, you must explicitly return the appropriate result to the `Select` method. Depending on the number of lines, consider implementing the code as its own standalone method.

## Example: FFT Transformations
The following example shows how to compute Fast Fourier Transform (FFT) of a `PinSiteData` object of real-valued arrays, and real, two-dimensional time-domain signals.
```
using NationalInstruments;
using NationalInstruments.Analysis.Dsp;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace UsingAnalysisLibraryWithPinSiteData
{
    public static class SingleInputDataExample
    {
        public static PinSiteData<ComplexDouble[]> RealFft(PinSiteData<double[]> realData)
        {
            return realData.Select(data => Transforms.RealFft((double[])data.Clone()));
        }

        public static PinSiteData<ComplexDouble[,]> RealFft2D(PinSiteData<double[,]> realData, bool shiftDC)
        {
            return realData.Select(data => Transforms.RealFft2D((double[,])data.Clone(), shiftDC));
        }
    }
}
```

## Example: Handling Methods With No Return Value (in-place transforms)
There are Analysis Library methods that do not return values. In these cases, you have different ways to implement your methods to either return a new `PinSiteData` object that contains the calculated values, or do the calculation on the input `PinSiteData` object.

The following example shows how to calculate the power spectrum of a `PinSiteData` object of arrays. The `PowerSpectrum` method returns the calculated power spectrum as a new `PinSiteData` object. The `InPlacePowerSpectrum` method does the calculation in place.
```
using NationalInstruments.Analysis.Dsp;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace UsingAnalysisLibraryWithPinSiteData
{
    public class NoReturnValueAnalysisLibraryMethodExample
    {
        public static PinSiteData<double[]> PowerSpectrum(PinSiteData<double[]> inputData)
        {
            return inputData.Select(data =>
            {
                double[] dataCopy = (double[])data.Clone();
                Transforms.PowerSpectrum(dataCopy);
                return dataCopy;
            });
        }

        public static void InPlacePowerSpectrum(PinSiteData<double[]> data)
        {
            foreach (string pinName in data.PinNames)
            {
                SiteData<double[]> perPinData = data.ExtractPin(pinName);
                foreach (int siteNumber in perPinData.SiteNumbers)
                {
                    Transforms.PowerSpectrum(perPinData.GetValue(siteNumber));
                }
            }
        }
    }
}
```

## Example: Handling Methods With Multiple Inputs
There are also Analysis Library methods that take more than one input data. In these cases, you must explicitly traverse pin names and site numbers on all input `PinSiteData` objects to retrieve the right elements to pass to Analysis Library methods.

The following example shows how to calculate the convolution of the input `PinSiteData` objects of arrays.
```
using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.Analysis.Dsp;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace UsingAnalysisLibraryWithPinSiteData
{
    public static class MultipleInputDataExample
    {
        public static PinSiteData<double[]> Convolve(PinSiteData<double[]> inputXData, PinSiteData<double[]> inputYData)
        {
            if (!inputXData.PinNames.SequenceEqual(inputYData.PinNames))
            {
                throw new Exception("Pins contained in inputXData and inputYData don't match exactly.");
            }

            Dictionary<string, IDictionary<int, double[]>> result = new Dictionary<string, IDictionary<int, double[]>>();
            foreach (string pinName in inputXData.PinNames)
            {
                SiteData<double[]> siteDataX = inputXData.ExtractPin(pinName);
                SiteData<double[]> siteDataY = inputYData.ExtractPin(pinName);
                if (!siteDataX.SiteNumbers.SequenceEqual(siteDataY.SiteNumbers))
                {
                    throw new Exception($"Sites contained in inputXData and inputYData for the {pinName} pin don't match exactly.");
                }

                result.Add(pinName, new Dictionary<int, double[]>());
                foreach (int siteNumber in siteDataX.SiteNumbers)
                {
                    double[] xData = siteDataX.GetValue(siteNumber);
                    double[] yData = siteDataY.GetValue(siteNumber);
                    result[pinName].Add(siteNumber, SignalProcessing.Convolve(xData, yData));
                }
            }
            return new PinSiteData<double[]>(result);
        }
    }
}
```