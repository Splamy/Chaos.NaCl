namespace Chaos.NaCl.Ed25519Ref10
{
	public static partial class GroupOperations
	{
		/*
		r = 2 * p
		*/
		public static void ge_p3_dbl(out GroupElementP1P1 r, in GroupElementP3 p)
		{
			ge_p3_to_p2(out var q, in p);
			ge_p2_dbl(out r, in q);
		}
	}
}
