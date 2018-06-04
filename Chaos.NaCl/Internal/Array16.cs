namespace Chaos.NaCl.Internal
{
	// Array16<UInt32> Salsa20 state
	// Array16<UInt64> SHA-512 block
	internal readonly struct Array16<T>
	{
		public readonly T x0;
		public readonly T x1;
		public readonly T x2;
		public readonly T x3;
		public readonly T x4;
		public readonly T x5;
		public readonly T x6;
		public readonly T x7;
		public readonly T x8;
		public readonly T x9;
		public readonly T x10;
		public readonly T x11;
		public readonly T x12;
		public readonly T x13;
		public readonly T x14;
		public readonly T x15;

		public Array16(T x0, T x1, T x2, T x3, T x4, T x5, T x6, T x7, T x8, T x9, T x10, T x11, T x12, T x13, T x14, T x15)
		{
			this.x0 = x0;
			this.x1 = x1;
			this.x2 = x2;
			this.x3 = x3;
			this.x4 = x4;
			this.x5 = x5;
			this.x6 = x6;
			this.x7 = x7;
			this.x8 = x8;
			this.x9 = x9;
			this.x10 = x10;
			this.x11 = x11;
			this.x12 = x12;
			this.x13 = x13;
			this.x14 = x14;
			this.x15 = x15;
		}
	}
}
