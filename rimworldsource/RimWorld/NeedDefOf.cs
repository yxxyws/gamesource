using System;
namespace RimWorld
{
	public static class NeedDefOf
	{
		public static NeedDef Food;
		public static NeedDef Rest;
		public static NeedDef Joy;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<NeedDef>(typeof(NeedDefOf));
		}
	}
}
