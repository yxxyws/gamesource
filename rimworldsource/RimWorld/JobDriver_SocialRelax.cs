using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_SocialRelax : JobDriver
	{
		private const TargetIndex GatherSpotParentInd = TargetIndex.A;
		private const TargetIndex ChairOrSpotInd = TargetIndex.B;
		private const TargetIndex OptionalDrinkInd = TargetIndex.C;
		private Thing GatherSpotParent
		{
			get
			{
				return base.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}
		private bool HasChair
		{
			get
			{
				return base.CurJob.GetTarget(TargetIndex.B).HasThing;
			}
		}
		private bool HasDrink
		{
			get
			{
				return base.CurJob.GetTarget(TargetIndex.C).HasThing;
			}
		}
		private IntVec3 ClosestGatherSpotParentCell
		{
			get
			{
				return this.GatherSpotParent.OccupiedRect().ClosestCellTo(this.pawn.Position);
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_SocialRelax.<MakeNewToils>c__Iterator24 <MakeNewToils>c__Iterator = new JobDriver_SocialRelax.<MakeNewToils>c__Iterator24();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_SocialRelax.<MakeNewToils>c__Iterator24 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool TryGetCarriedThingDrawPos(out Vector2 drawPos)
		{
			drawPos = Vector2.zero;
			Thing carriedThing = this.pawn.carrier.CarriedThing;
			if (carriedThing == null || !carriedThing.IngestibleNow)
			{
				return false;
			}
			IntVec3 closestGatherSpotParentCell = this.ClosestGatherSpotParentCell;
			if (!closestGatherSpotParentCell.AdjacentToCardinal(this.pawn.Position))
			{
				return false;
			}
			if (!closestGatherSpotParentCell.HasEatSurface())
			{
				return false;
			}
			drawPos = new Vector2((float)closestGatherSpotParentCell.x + 0.5f, (float)closestGatherSpotParentCell.z + 0.5f);
			return true;
		}
	}
}
