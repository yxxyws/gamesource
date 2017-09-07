using System;
using System.Linq;
using Verse;
namespace RimWorld
{
	public static class FactionUtility
	{
		public static bool HostileTo(this Faction fac, Faction other)
		{
			return fac != null && other != null && other != fac && fac.RelationWith(other, false).hostile;
		}
		public static Faction DefaultFactionFrom(FactionDef ft)
		{
			if (ft == null)
			{
				return null;
			}
			if (ft == FactionDefOf.Colony)
			{
				return Faction.OfColony;
			}
			return (
				from fac in Find.FactionManager.AllFactions
				where fac.def == ft
				select fac).RandomElement<Faction>();
		}
	}
}
