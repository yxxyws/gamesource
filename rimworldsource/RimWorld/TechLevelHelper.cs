using System;
using Verse;
namespace RimWorld
{
	public static class TechLevelHelper
	{
		public static bool CanSpawnWithEquipmentFrom(this TechLevel pawnLevel, TechLevel gearLevel)
		{
			if (gearLevel == TechLevel.Undefined)
			{
				return false;
			}
			switch (pawnLevel)
			{
			case TechLevel.Undefined:
				return false;
			case TechLevel.Neolithic:
				return gearLevel <= TechLevel.Neolithic;
			case TechLevel.Medieval:
				return gearLevel <= TechLevel.Medieval;
			case TechLevel.Midworld:
				return gearLevel == TechLevel.Midworld;
			case TechLevel.Spacer:
				return gearLevel == TechLevel.Spacer || gearLevel == TechLevel.Midworld;
			case TechLevel.Ultra:
				return gearLevel == TechLevel.Ultra || gearLevel == TechLevel.Spacer;
			case TechLevel.Transcendent:
				return gearLevel == TechLevel.Transcendent;
			}
			Log.Error(string.Concat(new object[]
			{
				"Unknown tech levels ",
				pawnLevel,
				", ",
				gearLevel
			}));
			return true;
		}
		public static bool IsNeolithicOrWorse(this TechLevel techLevel)
		{
			return techLevel != TechLevel.Undefined && techLevel <= TechLevel.Neolithic;
		}
	}
}
