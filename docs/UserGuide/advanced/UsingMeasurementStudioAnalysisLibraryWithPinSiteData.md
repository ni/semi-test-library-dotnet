# Using Measurement Studio Analysis Library with PinSiteData

NI Measurement Studio Analysis Library provides powerful and easy-to-use mathematical analysis tools for measurement applications written for the Microsoft .NET Framework. For more information, refer to *Measurement Studio Analysis Help*.

> **Note:** Measurement Studio Analysis Library does not support PinSiteData objects directly.

Follow the steps below to use Measurement Studio Analysis Library with `PinSiteData`.

1. Check the NI Measurement Studio Analysis Library license you have.
2. In the CS project, reference one of the Analysis Library assemblies below according to the license you have.
  - `NationalInstruments.Analysis.Standard`
  - `NationalInstruments.Analysis.Professional`
  - `NationalInstruments.Analysis.Enterprise`
3. In the CS project, reference `NationalInstruments.Common` assembly which contains the `ComplexDouble` data type.
4. In the CS project, reference either `NationalInstruments.SemiconductorTestLibrary.Abstractions` assembly or `NationalInstruments.SemiconductorTestLibrary.25.0.0.nupkg` which contains the `PinSiteData` data type.
5. In the CS file, add the following using directives.
  - `using NationalInstruments;`
  - `using NationalInstruments.Analysis;`
  - `using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;`
6. Enumerate each element of `PinSiteData` object using the `Select` method.
7. Use a Lambda expression to select the inner data element.
8. Make a copy of the selected data element within the body of the expression when both of the following statements are true.
  - The inner data element is reference type (e.g. an array).
  - The Analysis Library method to be invoked operates data in place.
9. Then convert the data or data copy appropriately as necessary, or directly pass it as an input into the desired Analysis Library method.

## Example
The following example shows how to compute Fast Fourier Transform (FFT) of a PinSiteData object of real-valued arrays, and real, two-dimensional time-domain signals.
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

There are also Analysis Library methods that take more than one input data. In these cases, you need to explicitly traverse pin names and site numbers on all input PinSiteData objects to retrieve the right elements to pass to Analysis Library methods.

## Example
The following example shows how to calculate the convolution of the input PinSiteData objects of arrays.
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