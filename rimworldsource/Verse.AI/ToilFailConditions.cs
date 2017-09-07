using RimWorld;
using System;
namespace Verse.AI
{
	public static class ToilFailConditions
	{
		public static Toil FailOn(this Toil toil, Func<Toil, bool> condition)
		{
			toil.AddEndCondition(delegate
			{
				if (condition(toil))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return toil;
		}
		public static T FailOn<T>(this T f, Func<bool> condition) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				if (condition())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnDestroyedOrNull<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				if (f.GetActor().jobs.curJob.GetTarget(ind).Thing.DestroyedOrNull())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnDespawnedOrNull<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (thing == null || !thing.Spawned)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T EndOnDespawnedOrNull<T>(this T f, TargetIndex ind, JobCondition endCondition = JobCondition.Incompletable) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (thing == null || !thing.Spawned)
				{
					return endCondition;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnDowned<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (((Pawn)thing).Downed)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnNotAwake<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (!((Pawn)thing).Awake())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnNotCasualInterruptible<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (!((Pawn)thing).CasualInterruptibleNow())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnMentalState<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn pawn = f.GetActor().jobs.curJob.GetTarget(ind).Thing as Pawn;
				if (pawn != null && pawn.InMentalState)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnAggroMentalState<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn pawn = f.GetActor().jobs.curJob.GetTarget(ind).Thing as Pawn;
				if (pawn != null && pawn.InAggroMentalState)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnForbidden<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn actor = f.GetActor();
				if (actor.Faction != Faction.OfColony)
				{
					return JobCondition.Ongoing;
				}
				if (actor.jobs.curJob.ignoreForbidden)
				{
					return JobCondition.Ongoing;
				}
				if (actor.jobs.curJob.GetTarget(ind).Thing.IsForbidden(actor))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnDespawnedNullOrForbidden<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.FailOnDespawnedOrNull(ind);
			f.FailOnForbidden(ind);
			return f;
		}
		public static T FailOnDestroyedNullOrForbidden<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.FailOnDestroyedOrNull(ind);
			f.FailOnForbidden(ind);
			return f;
		}
		public static T FailOnThingMissingDesignation<T>(this T f, TargetIndex ind, DesignationDef desDef) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Job curJob = f.GetActor().jobs.curJob;
				if (curJob.ignoreDesignations)
				{
					return JobCondition.Ongoing;
				}
				if (Find.DesignationManager.DesignationOn(curJob.GetTarget(ind).Thing, desDef) == null)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnCellMissingDesignation<T>(this T f, TargetIndex ind, DesignationDef desDef) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Job curJob = f.GetActor().jobs.curJob;
				if (curJob.ignoreDesignations)
				{
					return JobCondition.Ongoing;
				}
				if (Find.DesignationManager.DesignationAt(curJob.GetTarget(ind).Cell, desDef) == null)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static T FailOnBurningImmobile<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				if (f.GetActor().jobs.curJob.GetTarget(ind).IsBurning())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}
		public static Toil FailOnDespawnedOrForbiddenPlacedTargets(this Toil toil)
		{
			toil.AddFailCondition(delegate
			{
				if (toil.actor.jobs.curJob.placedTargets == null)
				{
					return false;
				}
				foreach (TargetInfo current in toil.actor.jobs.curJob.placedTargets)
				{
					if (!current.Thing.Spawned || (!toil.actor.CurJob.ignoreForbidden && current.Thing.IsForbidden(toil.actor)))
					{
						return true;
					}
				}
				return false;
			});
			return toil;
		}
	}
}
