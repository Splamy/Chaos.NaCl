using Chaos.NaCl.Internal;
using System;
using System.Buffers.Binary;

namespace Chaos.NaCl
{
	public class Sha512
	{
		private Array8<UInt64> _state;
		private readonly byte[] _buffer;
		private ulong _totalBytes;
		public const int BlockSize = 128;
		private static readonly byte[] _padding = { 0x80 };

		public Sha512()
		{
			_buffer = new byte[BlockSize];//todo: remove allocation
			Init();
		}

		public void Init()
		{
			Sha512Internal.Sha512Init(out _state);
			_totalBytes = 0;
		}

		[Obsolete]
		public void Update(byte[] data, int offset, int count)
		{
			Update(data.AsSpan().Slice(offset, count));
		}

		public void Update(ReadOnlySpan<byte> data)
		{
			Array16<ulong> block;
			int bytesInBuffer = (int)_totalBytes & (BlockSize - 1);
			_totalBytes += (uint)data.Length;
			var bufferSpan = _buffer.AsSpan();

			if (_totalBytes >= ulong.MaxValue / 8)
				throw new InvalidOperationException("Too much data");
			// Fill existing buffer
			if (bytesInBuffer != 0)
			{
				var toCopy = Math.Min(BlockSize - bytesInBuffer, data.Length);
				data.Slice(0, toCopy).CopyTo(bufferSpan.Slice(bytesInBuffer));
				data = data.Slice(toCopy);

				bytesInBuffer += toCopy;
				if (bytesInBuffer == BlockSize)
				{
					ByteIntegerConverter.Array16LoadBigEndian64(out block, _buffer);
					Sha512Internal.Core(out _state, in _state, in block);
					CryptoBytes.Wipe(_buffer, 0, _buffer.Length);
					bytesInBuffer = 0;
				}
			}
			// Hash complete blocks without copying
			while (data.Length >= BlockSize)
			{
				ByteIntegerConverter.Array16LoadBigEndian64(out block, data);
				Sha512Internal.Core(out _state, in _state, in block);
				data = data.Slice(BlockSize);
			}
			// Copy remainder into buffer
			if (data.Length > 0)
			{
				data.CopyTo(bufferSpan.Slice(bytesInBuffer));
			}
		}

		public void Finish(Span<byte> output)
		{
			if (output.Length != 64)
				throw new ArgumentException("output.Count must be 64");

			Update(_padding);
			Array16<ulong> block;
			ByteIntegerConverter.Array16LoadBigEndian64(out block, _buffer);
			CryptoBytes.Wipe(_buffer, 0, _buffer.Length);
			int bytesInBuffer = (int)_totalBytes & (BlockSize - 1);
			if (bytesInBuffer > BlockSize - 16)
			{
				Sha512Internal.Core(out _state, in _state, in block);
				block = default;
			}
			block = new Array16<ulong>(
				block.x0,
				block.x1,
				block.x2,
				block.x3,
				block.x4,
				block.x5,
				block.x6,
				block.x7,
				block.x8,
				block.x9,
				block.x10,
				block.x11,
				block.x12,
				block.x13,
				block.x14,
				(_totalBytes - 1) * 8);

			Sha512Internal.Core(out _state, in _state, in block);

			BinaryPrimitives.WriteUInt64BigEndian(output.Slice(0), _state.x0);
			BinaryPrimitives.WriteUInt64BigEndian(output.Slice(8), _state.x1);
			BinaryPrimitives.WriteUInt64BigEndian(output.Slice(16), _state.x2);
			BinaryPrimitives.WriteUInt64BigEndian(output.Slice(24), _state.x3);
			BinaryPrimitives.WriteUInt64BigEndian(output.Slice(32), _state.x4);
			BinaryPrimitives.WriteUInt64BigEndian(output.Slice(40), _state.x5);
			BinaryPrimitives.WriteUInt64BigEndian(output.Slice(48), _state.x6);
			BinaryPrimitives.WriteUInt64BigEndian(output.Slice(56), _state.x7);
			_state = default;
		}

		[Obsolete]
		public byte[] Finish()
		{
			var result = new byte[64];
			Finish(result);
			return result;
		}

		public static byte[] Hash(byte[] data, int offset, int count)
		{
			return Hash(data.AsSpan().Slice(offset, count));
		}

		public static byte[] Hash(ReadOnlySpan<byte> data)
		{
			var hasher = new Sha512();
			hasher.Update(data);
			return hasher.Finish();
		}

		public static void Hash(ReadOnlySpan<byte> data, Span<byte> output)
		{
			var hasher = new Sha512();
			hasher.Update(data);
			hasher.Finish(output);
		}
	}
}
