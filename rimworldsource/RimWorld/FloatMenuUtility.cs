using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class FloatMenuUtility
	{
		public static Action GetRangedAttackAction(Pawn pawn, TargetInfo target, out string failStr)
		{
			failStr = string.Empty;
			if (pawn.equipment.Primary == null)
			{
				return null;
			}
			Verb primaryVerb = pawn.equipment.PrimaryEq.PrimaryVerb;
			if (primaryVerb.verbProps.MeleeRange)
			{
				return null;
			}
			if (!pawn.Drafted)
			{
				failStr = "IsNotDraftedLower".Translate(new object[]
				{
					pawn.NameStringShort
				});
			}
			else
			{
				if (!pawn.IsColonistPlayerControlled)
				{
					failStr = "CannotOrderNonControlledLower".Translate();
				}
				else
				{
					if (target.IsValid && !pawn.equipment.PrimaryEq.PrimaryVerb.CanHitTarget(target))
					{
						if (!pawn.Position.InHorDistOf(target.Cell, primaryVerb.verbProps.range))
						{
							failStr = "OutOfRange".Translate();
						}
						else
						{
							failStr = "CannotHitTarget".Translate();
						}
					}
					else
					{
						if (!pawn.story.WorkTagIsDisabled(WorkTags.Violent))
						{
							return delegate
							{
								Job job = new Job(JobDefOf.AttackStatic, target);
								job.playerForced = true;
								pawn.drafter.TakeOrderedJob(job);
							};
						}
						failStr = "IsIncapableOfViolenceLower".Translate(new object[]
						{
							pawn.NameStringShort
						});
					}
				}
			}
			return null;
		}
		public static Action GetMeleeAttackAction(Pawn pawn, TargetInfo target, out string failStr)
		{
			failStr = string.Empty;
			if (!pawn.Drafted)
			{
				failStr = "IsNotDraftedLower".Translate(new object[]
				{
					pawn.NameStringShort
				});
			}
			else
			{
				if (!pawn.IsColonistPlayerControlled)
				{
					failStr = "CannotOrderNonControlledLower".Translate();
				}
				else
				{
					if (target.IsValid && !pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
					{
						failStr = "NoPath".Translate();
					}
					else
					{
						if (pawn.story.WorkTagIsDisabled(WorkTags.Violent))
						{
							failStr = "IsIncapableOfViolenceLower".Translate(new object[]
							{
								pawn.NameStringShort
							});
						}
						else
						{
							if (pawn.meleeVerbs.TryGetMeleeVerb() != null)
							{
								return delegate
								{
									Job job = new Job(JobDefOf.AttackMelee, target);
									job.playerForced = true;
									Pawn pawn2 = target.Thing as Pawn;
									if (pawn2 != null)
									{
										job.killIncappedTarget = pawn2.Downed;
									}
									pawn.drafter.TakeOrderedJob(job);
								};
							}
							failStr = "Incapable".Translate();
						}
					}
				}
			}
			return null;
		}
		public static Action GetAttackAction(Pawn pawn, TargetInfo target, out string failStr)
		{
			if (pawn.equipment.Primary != null && !pawn.equipment.PrimaryEq.PrimaryVerb.verbProps.MeleeRange)
			{
				return FloatMenuUtility.GetRangedAttackAction(pawn, target, out failStr);
			}
			return FloatMenuUtility.GetMeleeAttackAction(pawn, target, out failStr);
		}
	}
}
