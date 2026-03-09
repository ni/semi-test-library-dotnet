using System;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.Common
{
    internal class Utilities
    {
        /// <summary>
        /// Updates the bit value within a byte.
        /// </summary>
        /// <param name="currentByteValue">The current byte value.</param>
        /// <param name="bitValue">The bit value.</param>
        /// <param name="bitIndex">The index of the bit within the byte.</param>
        /// <returns>The new value of the byte.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte UpdateBitInByte(byte currentByteValue, bool bitValue, int bitIndex)
        {
            if ((uint)bitIndex > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(bitIndex));
            }

            int mask = 1 << bitIndex;

            return (byte)(bitValue
                // set bit
                ? currentByteValue | mask
                // clear bit
                : currentByteValue & (byte)~mask);
        }
    }
}
