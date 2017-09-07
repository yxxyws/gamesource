using RimWorld;
using System;
namespace Verse.AI
{
	public static class Toils_Goto
	{
		public static Toil GotoThing(TargetIndex ind, PathEndMode peMode)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				actor.pather.StartPath(actor.jobs.curJob.GetTarget(ind), peMode);
			};
			toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			toil.FailOnDespawnedOrNull(ind);
			return toil;
		}
		public static Toil GotoThing(TargetIndex ind, IntVec3 exactCell)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				actor.pather.StartPath(exactCell, PathEndMode.OnCell);
			};
			toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			toil.FailOnDespawnedOrNull(ind);
			return toil;
		}
		public static Toil GotoCell(TargetIndex ind, PathEndMode peMode)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				actor.pather.StartPath(actor.jobs.curJob.GetTarget(ind), peMode);
			};
			toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			return toil;
		}
		public static Toil GotoCell(IntVec3 cell, PathEndMode peMode)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				actor.pather.StartPath(cell, peMode);
			};
			toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			return toil;
		}
		public static Toil MoveOffTargetBlueprint(TargetIndex targetInd)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Thing thing = actor.jobs.curJob.GetTarget(targetInd).Thing as Blueprint;
				if (thing == null || !actor.Position.IsInside(thing))
				{
					actor.jobs.curDriver.ReadyForNextToil();
					return;
				}
				foreach (IntVec3 current in GenAdj.CellsAdjacent8Way(thing).InRandomOrder(null))
				{
					if (current.Standable())
					{
						actor.pather.StartPath(current, PathEndMode.OnCell);
						return;
					}
				}
				foreach (IntVec3 current2 in GenAdj.CellsAdjacent8Way(thing).InRandomOrder(null))
				{
					if (current2.Walkable())
					{
						actor.pather.StartPath(current2, PathEndMode.OnCell);
						return;
					}
				}
				actor.jobs.EndCurrentJob(JobCondition.Incompletable);
			};
			toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			return toil;
		}
	}
}
