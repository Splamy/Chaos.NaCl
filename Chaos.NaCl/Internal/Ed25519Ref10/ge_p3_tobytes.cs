using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		public static void ge_p3_tobytes(Span<byte> s, in GroupElementP3 h)
		{
			FieldElement recip;
			FieldElement x;
			FieldElement y;

			FieldOperations.fe_invert(out recip, in h.Z);
			FieldOperations.fe_mul(out x, in h.X, in recip);
			FieldOperations.fe_mul(out y, in h.Y, in recip);
			FieldOperations.fe_tobytes(s, in y);
			s[31] ^= (byte)(FieldOperations.fe_isnegative(in x) << 7);
		}
	}
}
