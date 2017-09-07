using System;
using Verse;
namespace RimWorld
{
	public static class BodyPartGroupDefOf
	{
		public static BodyPartGroupDef Torso;
		public static BodyPartGroupDef Legs;
		public static BodyPartGroupDef FullHead;
		public static BodyPartGroupDef UpperHead;
		public static BodyPartGroupDef Eyes;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<BodyPartGroupDef>(typeof(BodyPartGroupDefOf));
		}
	}
}
