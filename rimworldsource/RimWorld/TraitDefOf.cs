using System;
namespace RimWorld
{
	public static class TraitDefOf
	{
		public static TraitDef Nudist;
		public static TraitDef Brawler;
		public static TraitDef GreenThumb;
		public static TraitDef Abrasive;
		public static TraitDef DrugDesire;
		public static TraitDef Cannibal;
		public static TraitDef Ascetic;
		public static TraitDef Psychopath;
		public static TraitDef Gay;
		public static TraitDef Industriousness;
		public static TraitDef Prettiness;
		public static TraitDef DislikesMen;
		public static TraitDef DislikesWomen;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<TraitDef>(typeof(TraitDefOf));
		}
	}
}
