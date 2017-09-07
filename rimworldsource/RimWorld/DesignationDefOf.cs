using System;
using Verse;
namespace RimWorld
{
	public static class DesignationDefOf
	{
		public static DesignationDef Haul;
		public static DesignationDef Mine;
		public static DesignationDef Deconstruct;
		public static DesignationDef Uninstall;
		public static DesignationDef CutPlant;
		public static DesignationDef HarvestPlant;
		public static DesignationDef Hunt;
		public static DesignationDef SmoothFloor;
		public static DesignationDef RemoveFloor;
		public static DesignationDef Flick;
		public static DesignationDef Plan;
		public static DesignationDef Strip;
		public static DesignationDef RearmTrap;
		public static DesignationDef Slaughter;
		public static DesignationDef Tame;
		public static DesignationDef Open;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<DesignationDef>(typeof(DesignationDefOf));
		}
	}
}
