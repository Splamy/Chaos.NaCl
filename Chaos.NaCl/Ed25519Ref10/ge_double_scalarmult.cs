using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	public static partial class GroupOperations
	{
		private static void slide(Span<sbyte> r, ReadOnlySpan<byte> a)
		{
			int i;
			int b;
			int k;

			for (i = 0; i < 256; ++i)
				r[i] = (sbyte)(1 & (a[i >> 3] >> (i & 7)));

			for (i = 0; i < 256; ++i)
				if (r[i] != 0)
				{
					for (b = 1; b <= 6 && i + b < 256; ++b)
					{
						if (r[i + b] != 0)
						{
							if (r[i] + (r[i + b] << b) <= 15)
							{
								r[i] += (sbyte)(r[i + b] << b); r[i + b] = 0;
							}
							else if (r[i] - (r[i + b] << b) >= -15)
							{
								r[i] -= (sbyte)(r[i + b] << b);
								for (k = i + b; k < 256; ++k)
								{
									if (r[k] == 0)
									{
										r[k] = 1;
										break;
									}
									r[k] = 0;
								}
							}
							else
								break;
						}
					}
				}

		}

		/// <summary>
		/// r = a * A + b * B
		/// <para>B is the Ed25519 base point(x,4/5) with x positive.</para>
		/// </summary>
		/// <param name="r">result</param>
		/// <param name="a">a = a[0] + 256 * a[1] +...+ 256 ^ 31 a[31]</param>
		/// <param name="A"></param>
		/// <param name="b">b = b[0] + 256 * b[1] +...+ 256 ^ 31 b[31]</param>
		public static void ge_double_scalarmult_vartime(out GroupElementP2 r, ReadOnlySpan<byte> a, in GroupElementP3 A, ReadOnlySpan<byte> b)
		{
			GroupElementPreComp[] Bi = LookupTables.Base2;
			Span<sbyte> aslide = stackalloc sbyte[256];
			Span<sbyte> bslide = stackalloc sbyte[256];
			Span<GroupElementCached> Ai = stackalloc GroupElementCached[8]; /* A,3A,5A,7A,9A,11A,13A,15A */
			GroupElementP1P1 t;
			GroupElementP3 u;
			GroupElementP3 A2;
			int i;

			slide(aslide, a);
			slide(bslide, b);

			ge_p3_to_cached(out Ai[0], in A);
			ge_p3_dbl(out t, in A); ge_p1p1_to_p3(out A2, in t);
			ge_add(out t, in A2, in Ai[0]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[1], in u);
			ge_add(out t, in A2, in Ai[1]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[2], in u);
			ge_add(out t, in A2, in Ai[2]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[3], in u);
			ge_add(out t, in A2, in Ai[3]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[4], in u);
			ge_add(out t, in A2, in Ai[4]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[5], in u);
			ge_add(out t, in A2, in Ai[5]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[6], in u);
			ge_add(out t, in A2, in Ai[6]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[7], in u);

			ge_p2_0(out r);

			for (i = 255; i >= 0; --i)
			{
				if ((aslide[i] != 0) || (bslide[i] != 0)) break;
			}

			for (; i >= 0; --i)
			{
				ge_p2_dbl(out t, in r);

				if (aslide[i] > 0)
				{
					ge_p1p1_to_p3(out u, in t);
					ge_add(out t, in u, in Ai[aslide[i] / 2]);
				}
				else if (aslide[i] < 0)
				{
					ge_p1p1_to_p3(out u, in t);
					ge_sub(out t, in u, in Ai[(-aslide[i]) / 2]);
				}

				if (bslide[i] > 0)
				{
					ge_p1p1_to_p3(out u, in t);
					ge_madd(out t, in u, in Bi[bslide[i] / 2]);
				}
				else if (bslide[i] < 0)
				{
					ge_p1p1_to_p3(out u, in t);
					ge_msub(out t, in u, in Bi[(-bslide[i]) / 2]);
				}

				ge_p1p1_to_p2(out r, in t);
			}
		}

		/// <summary>
		/// r = a * A
		/// <para>B is the Ed25519 base point(x,4/5) with x positive.</para>
		/// </summary>
		/// <param name="t">result</param>
		/// <param name="a">a = a[0] + 256 * a[1] +...+ 256 ^ 31 a[31]</param>
		/// <param name="A"></param>
		public static void ge_scalarmult_vartime(out GroupElementP1P1 t, ReadOnlySpan<byte> a, in GroupElementP3 A)
		{
			GroupElementPreComp[] Bi = LookupTables.Base2;
			Span<sbyte> aslide = stackalloc sbyte[256];
			Span<GroupElementCached> Ai = stackalloc GroupElementCached[8]; /* A,3A,5A,7A,9A,11A,13A,15A */
			GroupElementP3 u;
			GroupElementP3 A2;
			GroupElementP2 r;
			int i;

			slide(aslide, a);

			ge_p3_to_cached(out Ai[0], in A);
			ge_p3_dbl(out t, in A); ge_p1p1_to_p3(out A2, in t);
			ge_add(out t, in A2, in Ai[0]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[1], in u);
			ge_add(out t, in A2, in Ai[1]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[2], in u);
			ge_add(out t, in A2, in Ai[2]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[3], in u);
			ge_add(out t, in A2, in Ai[3]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[4], in u);
			ge_add(out t, in A2, in Ai[4]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[5], in u);
			ge_add(out t, in A2, in Ai[5]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[6], in u);
			ge_add(out t, in A2, in Ai[6]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[7], in u);

			ge_p2_0(out r);

			for (i = 255; i >= 0; --i)
			{
				if ((aslide[i] != 0)) break;
			}

			for (; i >= 0; --i)
			{
				ge_p2_dbl(out t, in r);

				if (aslide[i] > 0)
				{
					ge_p1p1_to_p3(out u, in t);
					ge_add(out t, in u, in Ai[aslide[i] / 2]);
				}
				else if (aslide[i] < 0)
				{
					ge_p1p1_to_p3(out u, in t);
					ge_sub(out t, in u, in Ai[(-aslide[i]) / 2]);
				}

				ge_p1p1_to_p2(out r, in t);
			}
		}

		/// <summary>
		/// r = a * A
		/// <para>B is the Ed25519 base point(x,4/5) with x positive.</para>
		/// </summary>
		/// <param name="r">result</param>
		/// <param name="a">a = a[0] + 256 * a[1] +...+ 256 ^ 31 a[31]</param>
		/// <param name="A"></param>
		public static void ge_scalarmult_vartime(out GroupElementP2 r, ReadOnlySpan<byte> a, in GroupElementP3 A)
		{
			GroupElementPreComp[] Bi = LookupTables.Base2;
			Span<sbyte> aslide = stackalloc sbyte[256];
			Span<GroupElementCached> Ai = stackalloc GroupElementCached[8]; /* A,3A,5A,7A,9A,11A,13A,15A */
			GroupElementP3 u;
			GroupElementP3 A2;
			GroupElementP1P1 t;
			int i;

			slide(aslide, a);

			ge_p3_to_cached(out Ai[0], in A);
			ge_p3_dbl(out t, in A); ge_p1p1_to_p3(out A2, in t);
			ge_add(out t, in A2, in Ai[0]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[1], in u);
			ge_add(out t, in A2, in Ai[1]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[2], in u);
			ge_add(out t, in A2, in Ai[2]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[3], in u);
			ge_add(out t, in A2, in Ai[3]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[4], in u);
			ge_add(out t, in A2, in Ai[4]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[5], in u);
			ge_add(out t, in A2, in Ai[5]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[6], in u);
			ge_add(out t, in A2, in Ai[6]); ge_p1p1_to_p3(out u, in t); ge_p3_to_cached(out Ai[7], in u);

			ge_p2_0(out r);

			for (i = 255; i >= 0; --i)
			{
				if ((aslide[i] != 0)) break;
			}

			for (; i >= 0; --i)
			{
				ge_p2_dbl(out t, in r);

				if (aslide[i] > 0)
				{
					ge_p1p1_to_p3(out u, in t);
					ge_add(out t, in u, in Ai[aslide[i] / 2]);
				}
				else if (aslide[i] < 0)
				{
					ge_p1p1_to_p3(out u, in t);
					ge_sub(out t, in u, in Ai[(-aslide[i]) / 2]);
				}

				ge_p1p1_to_p2(out r, in t);
			}
		}
	}
}
