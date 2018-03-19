namespace Chaos.NaCl.Ed25519Ref10
{
	public static partial class GroupOperations
	{
		/*
		r = p
		*/
		public static void ge_p1p1_to_p2(out GroupElementP2 r, in GroupElementP1P1 p)
		{
			FieldOperations.fe_mul(out r.X, in p.X, in p.T);
			FieldOperations.fe_mul(out r.Y, in p.Y, in p.Z);
			FieldOperations.fe_mul(out r.Z, in p.Z, in p.T);
		}

	}
}
