using System;
using Verse;
namespace RimWorld
{
	public static class RoofDefOf
	{
		public static RoofDef RoofConstructed;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<RoofDef>(typeof(RoofDefOf));
		}
	}
}
