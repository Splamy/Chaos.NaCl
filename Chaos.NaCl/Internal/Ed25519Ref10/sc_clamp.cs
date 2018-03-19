using System;

namespace Chaos.NaCl.Ed25519Ref10
{
    internal static partial class ScalarOperations
    {
        public static void sc_clamp(Span<byte> s)
        {
            s[0] &= 248;
            s[31] &= 127;
            s[31] |= 64;
        }
    }
}
