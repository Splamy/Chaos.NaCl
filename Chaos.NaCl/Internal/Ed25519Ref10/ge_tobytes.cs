using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		public static void ge_tobytes(Span<byte> s, in GroupElementP2 h)
		{
			FieldOperations.fe_invert(out var recip, in h.Z);
			FieldOperations.fe_mul(out var x, in h.X, in recip);
			FieldOperations.fe_mul(out var y, in h.Y, in recip);
			FieldOperations.fe_tobytes(s, in y);
			s[31] ^= (byte)(FieldOperations.fe_isnegative(in x) << 7);
		}
	}
}
