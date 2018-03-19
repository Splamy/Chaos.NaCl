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
			output.x0 = BinaryPrimitives.ReadUInt64BigEndian(input.Slice(0));
			output.x1 =  BinaryPrimitives.ReadUInt64BigEndian(input.Slice(8));
			output.x2 =  BinaryPrimitives.ReadUInt64BigEndian(input.Slice(16));
			output.x3 =  BinaryPrimitives.ReadUInt64BigEndian(input.Slice(24));
			output.x4 =  BinaryPrimitives.ReadUInt64BigEndian(input.Slice(32));
			output.x5 =  BinaryPrimitives.ReadUInt64BigEndian(input.Slice(40));
			output.x6 =  BinaryPrimitives.ReadUInt64BigEndian(input.Slice(48));
			output.x7 =  BinaryPrimitives.ReadUInt64BigEndian(input.Slice(56));
			output.x8 =  BinaryPrimitives.ReadUInt64BigEndian(input.Slice(64));
			output.x9 =  BinaryPrimitives.ReadUInt64BigEndian(input.Slice(72));
			output.x10 = BinaryPrimitives.ReadUInt64BigEndian(input.Slice(80));
			output.x11 = BinaryPrimitives.ReadUInt64BigEndian(input.Slice(88));
			output.x12 = BinaryPrimitives.ReadUInt64BigEndian(input.Slice(96));
			output.x13 = BinaryPrimitives.ReadUInt64BigEndian(input.Slice(104));
			output.x14 = BinaryPrimitives.ReadUInt64BigEndian(input.Slice(112));
			output.x15 = BinaryPrimitives.ReadUInt64BigEndian(input.Slice(120));
		}
	}
}
