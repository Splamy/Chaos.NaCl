using System;

namespace Chaos.NaCl.Ed25519Ref10
{
	internal static partial class Ed25519Operations
	{
		public static bool crypto_sign_verify(ReadOnlySpan<byte> sig, ReadOnlySpan<byte> m, ReadOnlySpan<byte> pk)
		{

			if ((sig[63] & 224) != 0) return false;
			if (GroupOperations.ge_frombytes_negate_vartime(out var A, pk) != 0)
				return false;

			var hasher = new Sha512();
			hasher.Update(sig.Slice(0, 32));
			hasher.Update(pk.Slice(0, 32));
			hasher.Update(m);
			Span<byte> h = stackalloc byte[64];
			hasher.Finish(h);

			ScalarOperations.sc_reduce(h);

			GroupOperations.ge_double_scalarmult_vartime(out var R, h, in A, sig.Slice(32, 32));
			Span<byte> checkr = stackalloc byte[32];
			GroupOperations.ge_tobytes(checkr, in R);
			var result = CryptoBytes.ConstantTimeEquals(checkr, sig, 32);
			CryptoBytes.Wipe(h);
			CryptoBytes.Wipe(checkr);
			return result;
		}
	}
}
