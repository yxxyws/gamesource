using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class Toils_Construct
	{
		public static Toil MakeSolidThingFromBlueprintIfNecessary(TargetIndex blueTarget)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				Blueprint blueprint = curJob.GetTarget(blueTarget).Thing as Blueprint;
				if (blueprint != null)
				{
					Thing thing;
					bool flag;
					if (blueprint.TryReplaceWithSolidThing(actor, out thing, out flag))
					{
						curJob.SetTarget(blueTarget, thing);
						if (thing is Frame)
						{
							actor.Reserve(thing, 1);
						}
					}
					if (flag)
					{
						return;
					}
				}
			};
			return toil;
		}
	}
}
