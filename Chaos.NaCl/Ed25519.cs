using System;
using Chaos.NaCl.Ed25519Ref10;

namespace Chaos.NaCl
{
	public static class Ed25519
	{
		public const int PublicKeySizeInBytes = 32;
		public const int SignatureSizeInBytes = 64;
		public const int ExpandedPrivateKeySizeInBytes = 32 * 2;
		public const int PrivateKeySeedSizeInBytes = 32;
		public const int SharedKeySizeInBytes = 32;

		public static bool Verify(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> message, ReadOnlySpan<byte> publicKey)
		{
			if (signature.Length != SignatureSizeInBytes)
				throw new ArgumentException($"Signature size must be {SignatureSizeInBytes}", nameof(signature));
			if (publicKey.Length != PublicKeySizeInBytes)
				throw new ArgumentException($"Public key size must be {PublicKeySizeInBytes}", nameof(signature));
			return Ed25519Operations.crypto_sign_verify(signature, message, publicKey);
		}

		public static void Sign(Span<byte> signature, ReadOnlySpan<byte> message, ReadOnlySpan<byte> expandedPrivateKey)
		{
			if (signature.Length != SignatureSizeInBytes)
				throw new ArgumentException($"Signature size must be {SignatureSizeInBytes}", nameof(signature));
			if (expandedPrivateKey.Length != ExpandedPrivateKeySizeInBytes)
				throw new ArgumentException($"Private key size must be {ExpandedPrivateKeySizeInBytes}", nameof(expandedPrivateKey));
			Ed25519Operations.crypto_sign2(signature, message, expandedPrivateKey);
		}

		public static byte[] Sign(ReadOnlySpan<byte> message, ReadOnlySpan<byte> expandedPrivateKey)
		{
			var signature = new byte[SignatureSizeInBytes];
			Sign(signature, message, expandedPrivateKey);
			return signature;
		}

		public static byte[] PublicKeyFromSeed(ReadOnlySpan<byte> privateKeySeed)
		{
			var publicKey = new byte[PublicKeySizeInBytes];
			Span<byte> privateKey = stackalloc byte[ExpandedPrivateKeySizeInBytes];
			KeyPairFromSeed(publicKey, privateKey, privateKeySeed);
			CryptoBytes.Wipe(privateKey);
			return publicKey;
		}

		public static byte[] ExpandedPrivateKeyFromSeed(ReadOnlySpan<byte> privateKeySeed)
		{
			Span<byte> publicKey = stackalloc byte[PublicKeySizeInBytes];
			var privateKey = new byte[ExpandedPrivateKeySizeInBytes];
			KeyPairFromSeed(publicKey, privateKey, privateKeySeed);
			CryptoBytes.Wipe(publicKey);
			return privateKey;
		}

		public static void KeyPairFromSeed(out byte[] publicKey, out byte[] expandedPrivateKey, ReadOnlySpan<byte> privateKeySeed)
		{
			publicKey = new byte[PublicKeySizeInBytes];
			expandedPrivateKey = new byte[ExpandedPrivateKeySizeInBytes];
			KeyPairFromSeed(publicKey, expandedPrivateKey, privateKeySeed);
		}

		public static void KeyPairFromSeed(Span<byte> publicKey, Span<byte> expandedPrivateKey, ReadOnlySpan<byte> privateKeySeed)
		{
			if (publicKey.Length != PublicKeySizeInBytes)
				throw new ArgumentException("publicKey.Count");
			if (expandedPrivateKey.Length != ExpandedPrivateKeySizeInBytes)
				throw new ArgumentException("expandedPrivateKey.Count");
			if (privateKeySeed.Length != PrivateKeySeedSizeInBytes)
				throw new ArgumentException("privateKeySeed.Count");
			Ed25519Operations.crypto_sign_keypair(publicKey, expandedPrivateKey, privateKeySeed);
		}

		[Obsolete("Needs more testing")]
		public static byte[] KeyExchange(byte[] publicKey, byte[] privateKey)
		{
			var sharedKey = new byte[SharedKeySizeInBytes];
			KeyExchange(sharedKey, publicKey, privateKey);
			return sharedKey;
		}

		[Obsolete("Needs more testing")]
		public static void KeyExchange(Span<byte> sharedKey, ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
		{
			if (sharedKey.Length != 32)
				throw new ArgumentException("sharedKey.Count != 32");
			if (publicKey.Length != 32)
				throw new ArgumentException("publicKey.Count != 32");
			if (privateKey.Length != 64)
				throw new ArgumentException("privateKey.Count != 64");

			FieldOperations.fe_frombytes(out var edwardsY, publicKey);
			FieldOperations.fe_1(out var edwardsZ);
			MontgomeryCurve25519.EdwardsToMontgomeryX(out var montgomeryX, ref edwardsY, ref edwardsZ);
			Span<byte> h = stackalloc byte[64];
			Sha512.Hash(privateKey.Slice(0, 32), h);
			ScalarOperations.sc_clamp(h);
			MontgomeryOperations.scalarmult(out var sharedMontgomeryX, h, in montgomeryX);
			CryptoBytes.Wipe(h);
			FieldOperations.fe_tobytes(sharedKey, in sharedMontgomeryX);
		}
	}
}
