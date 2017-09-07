using System;
using Verse;
namespace RimWorld
{
	public static class EffecterDefOf
	{
		public static EffecterDef ConstructMetal;
		public static EffecterDef ConstructWood;
		public static EffecterDef ConstructDirt;
		public static EffecterDef EatMeat;
		public static EffecterDef ProgressBar;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<EffecterDef>(typeof(EffecterDefOf));
		}
	}
}
