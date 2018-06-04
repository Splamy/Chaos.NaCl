namespace Chaos.NaCl.Internal
{
	// Array8<UInt32> Poly1305 key
	// Array8<UInt64> SHA-512 state/output
	internal readonly struct Array8<T>
	{
		public readonly T x0;
		public readonly T x1;
		public readonly T x2;
		public readonly T x3;
		public readonly T x4;
		public readonly T x5;
		public readonly T x6;
		public readonly T x7;

		public Array8(T x0, T x1, T x2, T x3, T x4, T x5, T x6, T x7)
		{
			this.x0 = x0;
			this.x1 = x1;
			this.x2 = x2;
			this.x3 = x3;
			this.x4 = x4;
			this.x5 = x5;
			this.x6 = x6;
			this.x7 = x7;
		}
	}
}