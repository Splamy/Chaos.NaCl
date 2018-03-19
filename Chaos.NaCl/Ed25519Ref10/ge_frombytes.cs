using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	public static partial class GroupOperations
	{
		public static int ge_frombytes_negate_vartime(out GroupElementP3 h, ReadOnlySpan<byte> data)
		{
			FieldElement u;
			FieldElement v;
			FieldElement v3;
			FieldElement vxx;
			FieldElement check;

			FieldOperations.fe_frombytes(out h.Y, data);
			FieldOperations.fe_1(out h.Z);
			FieldOperations.fe_sq(out u, in h.Y);
			FieldOperations.fe_mul(out v, in u, in LookupTables.d);
			FieldOperations.fe_sub(out u, in u, in h.Z);       /* u = y^2-1 */
			FieldOperations.fe_add(out v, in v, in h.Z);       /* v = dy^2+1 */

			FieldOperations.fe_sq(out v3, in v);
			FieldOperations.fe_mul(out v3, in v3, in v);        /* v3 = v^3 */
			FieldOperations.fe_sq(out h.X, in v3);
			FieldOperations.fe_mul(out h.X, in h.X, in v);
			FieldOperations.fe_mul(out h.X, in h.X, in u);    /* x = uv^7 */

			FieldOperations.fe_pow22523(out h.X, in h.X); /* x = (uv^7)^((q-5)/8) */
			FieldOperations.fe_mul(out h.X, in h.X, in v3);
			FieldOperations.fe_mul(out h.X, in h.X, in u);    /* x = uv^3(uv^7)^((q-5)/8) */

			FieldOperations.fe_sq(out vxx, in h.X);
			FieldOperations.fe_mul(out vxx, in vxx, in v);
			FieldOperations.fe_sub(out check, in vxx, in u);    /* vx^2-u */
			if (FieldOperations.fe_isnonzero(in check) != 0)
			{
				FieldOperations.fe_add(out check, in vxx, in u);  /* vx^2+u */
				if (FieldOperations.fe_isnonzero(in check) != 0)
				{
					h = default(GroupElementP3);
					return -1;
				}
				FieldOperations.fe_mul(out h.X, in h.X, in LookupTables.sqrtm1);
			}

			if (FieldOperations.fe_isnegative(in h.X) == (data[31] >> 7))
				FieldOperations.fe_neg(out h.X, in h.X);

			FieldOperations.fe_mul(out h.T, in h.X, in h.Y);
			return 0;
		}

	}
}
