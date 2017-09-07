using System;
using Verse;
namespace RimWorld
{
	public static class BodyDefOf
	{
		public static BodyDef Human;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<BodyDef>(typeof(BodyDefOf));
		}
	}
}
