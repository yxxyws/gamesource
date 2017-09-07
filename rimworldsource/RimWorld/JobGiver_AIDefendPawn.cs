using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class JobGiver_AIDefendPawn : JobGiver_AIFightEnemy
	{
		protected abstract Pawn GetDefendee(Pawn pawn);
		protected override IntVec3 GetFlagPosition(Pawn pawn)
		{
			Pawn defendee = this.GetDefendee(pawn);
			if (defendee.Spawned)
			{
				return defendee.Position;
			}
			return IntVec3.Invalid;
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			Pawn defendee = this.GetDefendee(pawn);
			if (!GenAI.CanInteractPawn(pawn, defendee))
			{
				return null;
			}
			return base.TryGiveTerminalJob(pawn);
		}
		protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest)
		{
			Verb verb = pawn.TryGetAttackVerb(!pawn.IsColonist);
			if (verb == null)
			{
				dest = IntVec3.Invalid;
				return false;
			}
			return CastPositionFinder.TryFindCastPosition(new CastPositionRequest
			{
				caster = pawn,
				target = pawn.mindState.enemyTarget,
				verb = verb,
				maxRangeFromTarget = 9999f,
				locus = this.GetDefendee(pawn).Position,
				maxRangeFromLocus = this.GetFlagRadius(pawn),
				wantCoverFromTarget = verb.verbProps.range > 7f,
				maxRegionsRadius = 50
			}, out dest);
		}
	}
}
