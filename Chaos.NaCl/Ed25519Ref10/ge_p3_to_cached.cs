namespace Chaos.NaCl.Ed25519Ref10
{
	public static partial class GroupOperations
	{
		/*
		r = p
		*/
		public static void ge_p3_to_cached(out GroupElementCached r, in GroupElementP3 p)
		{
			FieldOperations.fe_add(out r.YplusX, in p.Y, in p.X);
			FieldOperations.fe_sub(out r.YminusX, in p.Y, in p.X);
			r.Z = p.Z;
			FieldOperations.fe_mul(out r.T2d, in p.T, in LookupTables.d2);
		}
	}
}
