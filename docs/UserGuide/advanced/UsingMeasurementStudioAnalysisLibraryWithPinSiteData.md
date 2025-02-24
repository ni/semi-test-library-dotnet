# Using Measurement Studio Analysis Library with PinSiteData

Measurement Studio Analysis Library provides powerful and easy-to-use mathematical analysis tools for measurement applications written for the Microsoft .NET platform. For more information, refer to *Measurement Studio Analysis Help*.

> **Note:** Measurement Studio Analysis Library does not support PinSiteData objects directly.

Follow the steps below to use Measurement Studio Analysis Library with `PinSiteData`.

 1. Reference the following assemblies (or packages) if not already
	 - `NationalInstruments.Analysis.Standard/Professional/Enterprise` (contains various analysis tools)
	 - `NationalInstruments.Common` (contains `ComplexDouble` data type)
	 - `NationalInstruments.SemiconductorTestLibrary.Abstractions` or `NationalInstruments.SemiconductorTestLibrary.25.0.0.nupkg` (contains `PinSiteData` data type)
2. Enumerate each element of `PinSiteData` object using the `Select` method.
3. Make a copy of the `PinSiteData` object element.
4. Pass the copy of the `PinSiteData` object element to the Analysis Library methods.

> **Note:** Check the Measurement Studio Analysis Library license you have to decide whether to reference the standard, professional or enterprise version of the library.

> **Note:** Need to make a copy of each `PinSiteData` object element because a lot of Analysis Library methods operate the input data in place.

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
					double[] xData = (double[])siteDataX.GetValue(siteNumber).Clone();
					double[] yData = (double[])siteDataY.GetValue(siteNumber).Clone();
					result[pinName].Add(siteNumber, SignalProcessing.Convolve(xData, yData));
				}
			}
			return new PinSiteData<double[]>(result);
		}
    }
}
```