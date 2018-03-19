using System;
using System.Runtime.CompilerServices;

namespace Chaos.NaCl
{
    public static class CryptoBytes
    {
        public static bool ConstantTimeEquals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            if (x.Length != y.Length)
                throw new ArgumentException("x.Length must equal y.Length");
            return InternalConstantTimeEquals(x, y, x.Length) != 0;
        }

        public static bool ConstantTimeEquals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "length < 0");
            if (x.Length < length)
                throw new ArgumentException("xOffset + length > x.Length");
            if (y.Length < length)
                throw new ArgumentException("yOffset + length > y.Length");
            return InternalConstantTimeEquals(x, y, length) != 0;
        }

        public static bool ConstantTimeEquals(ReadOnlySpan<byte> x, int xoffset, ReadOnlySpan<byte> y, int yoffset, int length)
        {
            return ConstantTimeEquals(x.Slice(xoffset), y.Slice(yoffset), length);
        }

        private static uint InternalConstantTimeEquals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y, int length)
        {
            int differentbits = 0;
            for (int i = 0; i < length; i++)
                differentbits |= x[i] ^ y[i];
            return (1 & (unchecked((uint)differentbits - 1) >> 8));
        }

        public static void Wipe(Span<byte> data)
        {
            InternalWipe(data);
        }

        public static void Wipe(Span<byte> data, int offset, int len)
        {
            InternalWipe(data.Slice(offset, len));
        }

        // Secure wiping is hard
        // * the GC can move around and copy memory
        //   Perhaps this can be avoided by using unmanaged memory or by fixing the position of the array in memory
        // * Swap files and error dumps can contain secret information
        //   It seems possible to lock memory in RAM, no idea about error dumps
        // * Compiler could optimize out the wiping if it knows that data won't be read back
        //   I hope this is enough, suppressing inlining
        //   but perhaps `RtlSecureZeroMemory` is needed
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void InternalWipe(Span<byte> data)
        {
            data.Clear();
        }

        // constant time hex conversion
        // see http://stackoverflow.com/a/14333437/445517
        //
        // An explanation of the weird bit fiddling:
        //
        // 1. `bytes[i] >> 4` extracts the high nibble of a byte  
        //   `bytes[i] & 0xF` extracts the low nibble of a byte
        // 2. `b - 10`  
        //    is `< 0` for values `b < 10`, which will become a decimal digit  
        //    is `>= 0` for values `b > 10`, which will become a letter from `A` to `F`.
        // 3. Using `i >> 31` on a signed 32 bit integer extracts the sign, thanks to sign extension.
        //    It will be `-1` for `i < 0` and `0` for `i >= 0`.
        // 4. Combining 2) and 3), shows that `(b-10)>>31` will be `0` for letters and `-1` for digits.
        // 5. Looking at the case for letters, the last summand becomes `0`, and `b` is in the range 10 to 15. We want to map it to `A`(65) to `F`(70), which implies adding 55 (`'A'-10`).
        // 6. Looking at the case for digits, we want to adapt the last summand so it maps `b` from the range 0 to 9 to the range `0`(48) to `9`(57). This means it needs to become -7 (`'0' - 55`).  
        // Now we could just multiply with 7. But since -1 is represented by all bits being 1, we can instead use `& -7` since `(0 & -7) == 0` and `(-1 & -7) == -7`.
        //
        // Some further considerations:
        //
        // * I didn't use a second loop variable to index into `c`, since measurement shows that calculating it from `i` is cheaper. 
        // * Using exactly `i < bytes.Length` as upper bound of the loop allows the JITter to eliminate bounds checks on `bytes[i]`, so I chose that variant.
        // * Making `b` an int avoids unnecessary conversions from and to byte.
        public static string ToHexStringUpper(byte[] data)
        {
            if (data == null)
                return null;
            char[] c = new char[data.Length * 2];
            int b;
            for (int i = 0; i < data.Length; i++)
            {
                b = data[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = data[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }

        // Explanation is similar to ToHexStringUpper
        // constant 55 -> 87 and -7 -> -39 to compensate for the offset 32 between lowercase and uppercase letters
        public static string ToHexStringLower(byte[] data)
        {
            if (data == null)
                return null;
            char[] c = new char[data.Length * 2];
            int b;
            for (int i = 0; i < data.Length; i++)
            {
                b = data[i] >> 4;
                c[i * 2] = (char)(87 + b + (((b - 10) >> 31) & -39));
                b = data[i] & 0xF;
                c[i * 2 + 1] = (char)(87 + b + (((b - 10) >> 31) & -39));
            }
            return new string(c);
        }

        public static byte[] FromHexString(string hexString)
        {
            if (hexString == null)
                return null;
            if (hexString.Length % 2 != 0)
                throw new FormatException("The hex string is invalid because it has an odd length");
            var result = new byte[hexString.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return result;
        }

        public static string ToBase64String(byte[] data)
        {
            if (data == null)
                return null;
            return Convert.ToBase64String(data);
        }

        public static byte[] FromBase64String(string s)
        {
            if (s == null)
                return null;
            return Convert.FromBase64String(s);
        }
    }
}
