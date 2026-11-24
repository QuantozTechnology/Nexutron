using System;
using System.Numerics;

namespace Nexutron
{
    /// <summary>
    /// Exact conversions between human readable token amounts and on-chain integer amounts.
    /// </summary>
    public static class TokenAmount
    {
        /// <summary>Converts e.g. 1.5 with 6 decimals into 1500000. Throws if the value has more precision than the token supports.</summary>
        public static BigInteger ToRaw(decimal amount, int decimals)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must not be negative.");
            if (decimals < 0) throw new ArgumentOutOfRangeException(nameof(decimals));

            var integerPart = decimal.Truncate(amount);
            var fraction = amount - integerPart;

            var result = new BigInteger(integerPart) * BigInteger.Pow(10, decimals);

            for (var i = 0; i < decimals && fraction != 0; i++)
            {
                fraction *= 10;
                var digit = decimal.Truncate(fraction);
                fraction -= digit;
                result += new BigInteger(digit) * BigInteger.Pow(10, decimals - i - 1);
            }

            if (fraction != 0)
            {
                throw new ArgumentException($"Amount {amount} has more than {decimals} decimal places.", nameof(amount));
            }

            return result;
        }

        /// <summary>Converts e.g. 1500000 with 6 decimals into 1.5.</summary>
        public static decimal FromRaw(BigInteger raw, int decimals)
        {
            if (decimals < 0) throw new ArgumentOutOfRangeException(nameof(decimals));
            if (raw.Sign < 0) return -FromRaw(-raw, decimals);

            var divisor = BigInteger.Pow(10, decimals);
            var integerPart = BigInteger.DivRem(raw, divisor, out var remainder);

            var result = (decimal)integerPart;
            if (remainder.IsZero) return result;

            // decimal holds at most 28 fractional digits; drop the least significant ones beyond that.
            var scale = decimals;
            while (scale > 28)
            {
                remainder /= 10;
                scale--;
            }

            return result + new decimal(LowBits(remainder, 0), LowBits(remainder, 32), LowBits(remainder, 64), false, (byte)scale);
        }

        private static int LowBits(BigInteger value, int shift)
        {
            return unchecked((int)(uint)((value >> shift) & uint.MaxValue));
        }
    }
}
