using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Verse.AI
{
	public static class GenAI
	{
		public static bool CanInteractPawn(Pawn assister, Pawn assistee)
		{
			return assistee.Spawned && assister.CanReach(assistee, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
		}
		public static bool MachinesLike(Faction machineFaction, Pawn p)
		{
			return p.Faction != null && (!p.IsPrisoner || p.HostFaction != machineFaction) && !p.Faction.HostileTo(machineFaction);
		}
		public static bool CanUseItemForWork(Pawn p, Thing item)
		{
			return !item.IsForbidden(p) && p.CanReserveAndReach(item, PathEndMode.ClosestTouch, p.NormalMaxDanger(), 1);
		}
		public static bool CanBeArrested(this Pawn pawn)
		{
			return pawn.RaceProps.Humanlike && !pawn.InAggroMentalState && !pawn.HostileTo(Faction.OfColony) && (!pawn.IsPrisonerOfColony || !pawn.Position.IsInPrisonCell());
		}
		public static IntVec3 RandomRaidDest()
		{
			List<ThingDef> allBedDefBestToWorst = RestUtility.AllBedDefBestToWorst;
			List<Building> list = new List<Building>(Find.MapPawns.FreeColonistsAndPrisonersSpawnedCount);
			for (int i = 0; i < allBedDefBestToWorst.Count; i++)
			{
				foreach (Building current in Find.ListerBuildings.AllBuildingsColonistOfDef(allBedDefBestToWorst[i]))
				{
					if (((Building_Bed)current).owners.Any<Pawn>())
					{
						list.Add(current);
					}
				}
			}
			Building building;
			if (list.TryRandomElement(out building))
			{
				return building.Position;
			}
			IEnumerable<Building> source = 
				from b in Find.ListerBuildings.allBuildingsColonist
				where !b.def.building.ai_combatDangerous && !b.def.building.isInert
				select b;
			if (source.Any<Building>())
			{
				for (int j = 0; j < 500; j++)
				{
					Building t = source.RandomElement<Building>();
					IntVec3 intVec = t.RandomAdjacentCell8Way();
					if (intVec.Walkable())
					{
						return intVec;
					}
				}
			}
			if (Find.MapPawns.FreeColonistsSpawnedCount > 0)
			{
				return Find.MapPawns.FreeColonistsSpawned.RandomElement<Pawn>().Position;
			}
			return Find.Map.Center;
		}
		public static bool EnemyIsNear(Pawn p, float radius)
		{
			foreach (IAttackTarget current in Find.AttackTargetsCache.GetPotentialTargetsFor(p))
			{
				if (!current.ThreatDisabled())
				{
					if (p.Position.InHorDistOf(((Thing)current).Position, radius))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
