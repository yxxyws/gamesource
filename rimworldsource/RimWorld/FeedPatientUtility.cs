using System;
using Verse;
namespace RimWorld
{
	public static class FeedPatientUtility
	{
		public static bool ShouldBeFed(Pawn p)
		{
			if (p.GetPosture() == PawnPosture.Standing)
			{
				return false;
			}
			if (p.RaceProps.Humanlike)
			{
				if (p.Faction != Faction.OfColony && p.HostFaction != Faction.OfColony)
				{
					return false;
				}
			}
			else
			{
				Building_Bed building_Bed = p.CurrentBed();
				if (building_Bed == null || building_Bed.Faction != Faction.OfColony)
				{
					return false;
				}
			}
			if (!p.health.PrefersMedicalRest)
			{
				return false;
			}
			if (!p.RaceProps.EatsFood)
			{
				return false;
			}
			if (p.HostFaction != null)
			{
				if (p.HostFaction != Faction.OfColony)
				{
					return false;
				}
				if (p.guest != null && !p.guest.ShouldBeBroughtFood)
				{
					return false;
				}
			}
			return true;
		}
	}
}
