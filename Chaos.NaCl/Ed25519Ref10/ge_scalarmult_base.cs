using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	public static partial class GroupOperations
	{
		static byte equal(byte b, byte c)
		{

			byte ub = b;
			byte uc = c;
			byte x = (byte)(ub ^ uc); /* 0: yes; 1..255: no */
			UInt32 y = x; /* 0: yes; 1..255: no */
			unchecked { y -= 1; } /* 4294967295: yes; 0..254: no */
			y >>= 31; /* 1: yes; 0: no */
			return (byte)y;
		}

		static byte negative(sbyte b)
		{
			ulong x = unchecked((ulong)(long)b); /* 18446744073709551361..18446744073709551615: yes; 0..255: no */
			x >>= 63; /* 1: yes; 0: no */
			return (byte)x;
		}

		static void cmov(ref GroupElementPreComp t, in GroupElementPreComp u, byte b)
		{
			FieldOperations.fe_cmov(ref t.yplusx, in u.yplusx, b);
			FieldOperations.fe_cmov(ref t.yminusx, in u.yminusx, b);
			FieldOperations.fe_cmov(ref t.xy2d, in u.xy2d, b);
		}

		static void select(out GroupElementPreComp t, int pos, sbyte b)
		{
			GroupElementPreComp minust;
			byte bnegative = negative(b);
			byte babs = (byte)(b - (((-bnegative) & b) << 1));

			ge_precomp_0(out t);
			var table = LookupTables.Base[pos];
			cmov(ref t, in table[0], equal(babs, 1));
			cmov(ref t, in table[1], equal(babs, 2));
			cmov(ref t, in table[2], equal(babs, 3));
			cmov(ref t, in table[3], equal(babs, 4));
			cmov(ref t, in table[4], equal(babs, 5));
			cmov(ref t, in table[5], equal(babs, 6));
			cmov(ref t, in table[6], equal(babs, 7));
			cmov(ref t, in table[7], equal(babs, 8));
			minust.yplusx = t.yminusx;
			minust.yminusx = t.yplusx;
			FieldOperations.fe_neg(out minust.xy2d, in t.xy2d);
			cmov(ref t, in minust, bnegative);
		}

		/*
		h = a * B
		where a = a[0]+256*a[1]+...+256^31 a[31]
		B is the Ed25519 base point (x,4/5) with x positive.
		
		Preconditions:
		  a[31] <= 127
		*/

		public static void ge_scalarmult_base(out GroupElementP3 h, ReadOnlySpan<byte> a)
		{
			// todo: Perhaps remove this allocation
			sbyte[] e = new sbyte[64];
			sbyte carry;
			GroupElementP1P1 r;
			GroupElementP2 s;
			GroupElementPreComp t;
			int i;

			for (i = 0; i < 32; ++i)
			{
				e[2 * i + 0] = (sbyte)((a[i] >> 0) & 15);
				e[2 * i + 1] = (sbyte)((a[i] >> 4) & 15);
			}
			/* each e[i] is between 0 and 15 */
			/* e[63] is between 0 and 7 */

			carry = 0;
			for (i = 0; i < 63; ++i)
			{
				e[i] += carry;
				carry = (sbyte)(e[i] + 8);
				carry >>= 4;
				e[i] -= (sbyte)(carry << 4);
			}
			e[63] += carry;
			/* each e[i] is between -8 and 8 */

			ge_p3_0(out h);
			for (i = 1; i < 64; i += 2)
			{
				select(out t, i / 2, e[i]);
				ge_madd(out r, in h, in t); ge_p1p1_to_p3(out h, in r);
			}

			ge_p3_dbl(out r, in h); ge_p1p1_to_p2(out s, in r);
			ge_p2_dbl(out r, in s); ge_p1p1_to_p2(out s, in r);
			ge_p2_dbl(out r, in s); ge_p1p1_to_p2(out s, in r);
			ge_p2_dbl(out r, in s); ge_p1p1_to_p3(out h, in r);

			for (i = 0; i < 64; i += 2)
			{
				select(out t, i / 2, e[i]);
				ge_madd(out r, in h, in t); ge_p1p1_to_p3(out h, in r);
			}
		}

	}
}
