using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class GenHostility
	{
		public static bool HostileTo(this Thing a, Thing b)
		{
			if (a.Destroyed || b.Destroyed || a == b)
			{
				return false;
			}
			Pawn pawn = a as Pawn;
			Pawn pawn2 = b as Pawn;
			return (pawn != null && pawn.MentalState != null && pawn.MentalState.ForceHostileTo(b)) || (pawn2 != null && pawn2.MentalState != null && pawn2.MentalState.ForceHostileTo(a)) || (pawn != null && pawn2 != null && (GenHostility.IsPredatorHostileTo(pawn, pawn2) || GenHostility.IsPredatorHostileTo(pawn2, pawn))) || ((a.Faction != null && pawn2 != null && pawn2.HostFaction == a.Faction && PrisonBreakUtility.IsEscaping(pawn2)) || (b.Faction != null && pawn != null && pawn.HostFaction == b.Faction && PrisonBreakUtility.IsEscaping(pawn))) || ((a.Faction == null || pawn2 == null || pawn2.HostFaction != a.Faction) && (b.Faction == null || pawn == null || pawn.HostFaction != b.Faction) && (pawn == null || !pawn.IsPrisoner) && (pawn2 == null || !pawn2.IsPrisoner) && a.Faction != null && b.Faction != null && a.Faction.HostileTo(b.Faction));
		}
		public static bool HostileTo(this Thing t, Faction fac)
		{
			if (t.Destroyed)
			{
				return false;
			}
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				MentalState mentalState = pawn.MentalState;
				if (mentalState != null && mentalState.ForceHostileTo(fac))
				{
					return true;
				}
				if (GenHostility.IsPredatorHostileTo(pawn, fac))
				{
					return true;
				}
				if (pawn.HostFaction == fac && PrisonBreakUtility.IsEscaping(pawn))
				{
					return true;
				}
				if (pawn.HostFaction == fac)
				{
					return false;
				}
				if (pawn.IsPrisoner)
				{
					return false;
				}
			}
			return t.Faction != null && t.Faction.HostileTo(fac);
		}
		private static bool IsPredatorHostileTo(Pawn predator, Pawn toPawn)
		{
			if (toPawn.Faction == null)
			{
				return false;
			}
			if (toPawn.Faction.HasPredatorRecentlyAttackedAnyone(predator))
			{
				return true;
			}
			Pawn preyOfMyFaction = GenHostility.GetPreyOfMyFaction(predator, toPawn.Faction);
			return preyOfMyFaction != null && predator.Position.InHorDistOf(preyOfMyFaction.Position, 12f);
		}
		private static bool IsPredatorHostileTo(Pawn predator, Faction toFaction)
		{
			return toFaction.HasPredatorRecentlyAttackedAnyone(predator) || GenHostility.GetPreyOfMyFaction(predator, toFaction) != null;
		}
		private static Pawn GetPreyOfMyFaction(Pawn predator, Faction myFaction)
		{
			Job curJob = predator.CurJob;
			if (curJob != null && curJob.def == JobDefOf.PredatorHunt && !predator.jobs.curDriver.ended)
			{
				Pawn pawn = curJob.GetTarget(TargetIndex.A).Thing as Pawn;
				if (pawn != null && pawn.Faction == myFaction)
				{
					return pawn;
				}
			}
			return null;
		}
	}
}
