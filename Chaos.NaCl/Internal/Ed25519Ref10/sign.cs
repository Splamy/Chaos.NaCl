using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	internal static partial class Ed25519Operations
	{
		public static void crypto_sign2(Span<byte> sig, ReadOnlySpan<byte> m, ReadOnlySpan<byte> sk)
		{
			var hasher = new Sha512();
			{
				hasher.Update(sk.Slice(0, 32));
				Span<byte> az = stackalloc byte[64];
				hasher.Finish(az);
				ScalarOperations.sc_clamp(az);

				hasher.Init();
				hasher.Update(az.Slice(32, 32));
				hasher.Update(m);
				Span<byte> r = stackalloc byte[64];
				hasher.Finish(r);

				ScalarOperations.sc_reduce(r);
				GroupOperations.ge_scalarmult_base(out var R, r);
				GroupOperations.ge_p3_tobytes(sig, in R);

				hasher.Init();
				hasher.Update(sig.Slice(0, 32));
				hasher.Update(sk.Slice(32, 32));
				hasher.Update(m);
				Span<byte> hram = stackalloc byte[64];
				hasher.Finish(hram);

				ScalarOperations.sc_reduce(hram);
				ScalarOperations.sc_muladd(sig.Slice(32, 32), hram, az, r);
			}
		}
	}
}
