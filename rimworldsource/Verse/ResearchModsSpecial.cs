using System;
using System.Linq;
namespace Verse
{
	public static class ResearchModsSpecial
	{
		public static void GunTurretCooling()
		{
			(
				from x in DefDatabase<ThingDef>.GetNamed("Gun_TurretImprovised", true).Verbs
				where x.isPrimary
				select x).First<VerbProperties>().burstShotCount = 4;
		}
	}
}
