using System;
using System.Collections.Generic;
using System.Linq;

namespace NationalInstruments.SemiconductorTestLibrary.Common
{
    internal static class HelperMethods
    {
        internal static T GetDistinctValue<T>(IEnumerable<T> values, string errorMessage)
        {
            try
            {
                return values.Distinct().Single();
            }
            catch (InvalidOperationException)
            {
                throw new NISemiconductorTestException(errorMessage);
            }
        }

        internal static void DummyCode()
        {
        }
    }
}
