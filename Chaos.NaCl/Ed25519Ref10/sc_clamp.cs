using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	public static partial class ScalarOperations
	{
		public static void sc_clamp(Span<byte> s)
		{
			s[00] &= 0xF8;
			s[31] &= 0x7F;
			s[31] |= 0x40;
		}
	}
}
