using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	internal static partial class Ed25519Operations
	{
		public static void crypto_sign_keypair(Span<byte> pk, Span<byte> sk, ReadOnlySpan<byte> seed)
		{
			seed.CopyTo(sk);
			Span<byte> h = stackalloc byte[64];
			Sha512.Hash(sk.Slice(0, 32), h);

			ScalarOperations.sc_clamp(h);

			GroupOperations.ge_scalarmult_base(out var A, h);
			GroupOperations.ge_p3_tobytes(pk, in A);

			for (int i = 0; i < 32; ++i) sk[32 + i] = pk[i];
			CryptoBytes.Wipe(h);
		}
	}
}
