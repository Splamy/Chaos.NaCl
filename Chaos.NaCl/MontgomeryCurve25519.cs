using System;
using Chaos.NaCl.Ed25519Ref10;

namespace Chaos.NaCl
{
	// This class is mainly for compatibility with NaCl's Curve25519 implementation
	// If you don't need that compatibility, use Ed25519.KeyExchange
	public static class MontgomeryCurve25519
	{
		public static readonly int PublicKeySizeInBytes = 32;
		public static readonly int PrivateKeySizeInBytes = 32;
		public static readonly int SharedKeySizeInBytes = 32;

		public static byte[] GetPublicKey(byte[] privateKey)
		{
			if (privateKey == null)
				throw new ArgumentNullException(nameof(privateKey));
			if (privateKey.Length != PrivateKeySizeInBytes)
				throw new ArgumentException("privateKey.Length must be 32");
			var publicKey = new byte[32];
			GetPublicKey(publicKey, privateKey);
			return publicKey;
		}

		static readonly byte[] _basePoint = new byte[32]
		{
			9, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0 ,0, 0, 0, 0, 0,
			0, 0, 0 ,0, 0, 0, 0, 0,
			0, 0, 0 ,0, 0, 0, 0, 0
		};

		public static void GetPublicKey(Span<byte> publicKey, ReadOnlySpan<byte> privateKey)
		{
			if (publicKey.Length != PublicKeySizeInBytes)
				throw new ArgumentException("privateKey.Count must be 32");
			if (privateKey.Length != PrivateKeySizeInBytes)
				throw new ArgumentException("privateKey.Count must be 32");

			// hack: abusing publicKey as temporary storage
			// todo: remove hack
			for (int i = 0; i < 32; i++)
			{
				publicKey[i] = privateKey[i];
			}
			ScalarOperations.sc_clamp(publicKey);

			GroupElementP3 A;
			GroupOperations.ge_scalarmult_base(out A, publicKey);
			EdwardsToMontgomeryX(out var publicKeyFE, ref A.Y, ref A.Z);
			FieldOperations.fe_tobytes(publicKey, in publicKeyFE);
		}

		public static byte[] KeyExchange(byte[] publicKey, byte[] privateKey)
		{
			var sharedKey = new byte[SharedKeySizeInBytes];
			KeyExchange(sharedKey, publicKey, privateKey);
			return sharedKey;
		}

		public static void KeyExchange(Span<byte> sharedKey, ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
		{
			if (sharedKey.Length != 32)
				throw new ArgumentException("sharedKey.Count != 32");
			if (publicKey.Length != 32)
				throw new ArgumentException("publicKey.Count != 32");
			if (privateKey.Length != 32)
				throw new ArgumentException("privateKey.Count != 32");
			MontgomeryOperations.scalarmult(sharedKey, privateKey, publicKey);
			//KeyExchangeOutputHashNaCl(sharedKey.Array, sharedKey.Offset); XXX
		}

		internal static void EdwardsToMontgomeryX(out FieldElement montgomeryX, ref FieldElement edwardsY, ref FieldElement edwardsZ)
		{
			FieldElement tempX, tempZ;
			FieldOperations.fe_add(out tempX, in edwardsZ, in edwardsY);
			FieldOperations.fe_sub(out tempZ, in edwardsZ, in edwardsY);
			FieldOperations.fe_invert(out tempZ, in tempZ);
			FieldOperations.fe_mul(out montgomeryX, in tempX, in tempZ);
		}
	}
}
