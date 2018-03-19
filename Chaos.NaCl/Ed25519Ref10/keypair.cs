using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	public static partial class Ed25519Operations
	{
		public static void crypto_sign_keypair(Span<byte> pk, Span<byte> sk, ReadOnlySpan<byte> seed)
		{
			Sha512.Hash(seed, sk);

			ScalarOperations.sc_clamp(sk);

			GroupOperations.ge_scalarmult_base(out var A, sk.Slice(0, 32));
			GroupOperations.ge_p3_tobytes(pk, in A);

			seed.CopyTo(sk);
			pk.CopyTo(sk.Slice(32, 32));
		}
	}
}
