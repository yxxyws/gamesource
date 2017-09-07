using System;
using Verse;
namespace RimWorld
{
	public static class PawnKindDefOf
	{
		public static PawnKindDef Colonist;
		public static PawnKindDef Slave;
		public static PawnKindDef Villager;
		public static PawnKindDef Drifter;
		public static PawnKindDef SpaceRefugee;
		public static PawnKindDef SpaceSoldier;
		public static PawnKindDef Thrumbo;
		public static PawnKindDef Megascarab;
		public static PawnKindDef Spelopede;
		public static PawnKindDef Megaspider;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<PawnKindDef>(typeof(PawnKindDefOf));
		}
	}
}
