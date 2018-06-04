using System;
using System.Buffers.Binary;

namespace Chaos.NaCl.Internal
{
	// Loops? Arrays? Never heard of that stuff
	// Library avoids unnecessary heap allocations and unsafe code
	// so this ugly code becomes necessary :(
	internal static class ByteIntegerConverter
	{
		public static void Array16LoadBigEndian64(out Array16<UInt64> output, ReadOnlySpan<byte> input)
		{
			output = new Array16<ulong>(
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(0)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(8)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(16)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(24)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(32)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(40)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(48)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(56)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(64)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(72)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(80)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(88)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(96)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(104)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(112)),
				BinaryPrimitives.ReadUInt64BigEndian(input.Slice(120)));
		}
	}
}
