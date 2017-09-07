using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Ingest : JobDriver
	{
		public const float EatCorpseBodyPartsUntilFoodLevelPct = 0.9f;
		private const TargetIndex DispenserInd = TargetIndex.A;
		private const TargetIndex FoodInd = TargetIndex.A;
		private const TargetIndex TableCellInd = TargetIndex.B;
		private Thing Food
		{
			get
			{
				return base.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Ingest.<MakeNewToils>c__Iterator3F <MakeNewToils>c__Iterator3F = new JobDriver_Ingest.<MakeNewToils>c__Iterator3F();
			<MakeNewToils>c__Iterator3F.<>f__this = this;
			JobDriver_Ingest.<MakeNewToils>c__Iterator3F expr_0E = <MakeNewToils>c__Iterator3F;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private Toil ReserveFoodIfWillEatWholeStack()
		{
			return new Toil
			{
				initAction = delegate
				{
					if (this.pawn.Faction == null)
					{
						return;
					}
					Thing thing = this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
					int num = FoodUtility.WillEatStackCountOf(this.pawn, thing.def);
					if (num >= thing.stackCount)
					{
						if (!thing.Spawned || !Find.Reservations.CanReserve(this.pawn, thing, 1))
						{
							this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
							return;
						}
						Find.Reservations.Reserve(this.pawn, thing, 1);
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
		private IEnumerable<Toil> PrepareToEatToils()
		{
			if (base.CurJob.GetTarget(TargetIndex.A).Thing is Building_NutrientPasteDispenser)
			{
				return this.PrepareToEatToils_Dispenser();
			}
			if (this.pawn.RaceProps.ToolUser)
			{
				return this.PrepareToEatToils_ToolUser();
			}
			return this.PrepareToEatToils_NonToolUser();
		}
		[DebuggerHidden]
		private IEnumerable<Toil> PrepareToEatToils_Dispenser()
		{
			JobDriver_Ingest.<PrepareToEatToils_Dispenser>c__Iterator40 <PrepareToEatToils_Dispenser>c__Iterator = new JobDriver_Ingest.<PrepareToEatToils_Dispenser>c__Iterator40();
			<PrepareToEatToils_Dispenser>c__Iterator.<>f__this = this;
			JobDriver_Ingest.<PrepareToEatToils_Dispenser>c__Iterator40 expr_0E = <PrepareToEatToils_Dispenser>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		[DebuggerHidden]
		private IEnumerable<Toil> PrepareToEatToils_ToolUser()
		{
			JobDriver_Ingest.<PrepareToEatToils_ToolUser>c__Iterator41 <PrepareToEatToils_ToolUser>c__Iterator = new JobDriver_Ingest.<PrepareToEatToils_ToolUser>c__Iterator41();
			<PrepareToEatToils_ToolUser>c__Iterator.<>f__this = this;
			JobDriver_Ingest.<PrepareToEatToils_ToolUser>c__Iterator41 expr_0E = <PrepareToEatToils_ToolUser>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		[DebuggerHidden]
		private IEnumerable<Toil> PrepareToEatToils_NonToolUser()
		{
			JobDriver_Ingest.<PrepareToEatToils_NonToolUser>c__Iterator42 <PrepareToEatToils_NonToolUser>c__Iterator = new JobDriver_Ingest.<PrepareToEatToils_NonToolUser>c__Iterator42();
			<PrepareToEatToils_NonToolUser>c__Iterator.<>f__this = this;
			JobDriver_Ingest.<PrepareToEatToils_NonToolUser>c__Iterator42 expr_0E = <PrepareToEatToils_NonToolUser>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool TryGetCarriedThingDrawPos(out Vector2 drawPos)
		{
			Thing carriedThing = this.pawn.carrier.CarriedThing;
			if (carriedThing == null || !carriedThing.IngestibleNow)
			{
				drawPos = Vector2.zero;
				return false;
			}
			TargetInfo target = base.CurJob.GetTarget(TargetIndex.B);
			if (!target.IsValid)
			{
				drawPos = Vector2.zero;
				return false;
			}
			IntVec3 cell = target.Cell;
			drawPos = new Vector2((float)cell.x + 0.5f, (float)cell.z + 0.5f);
			return true;
		}
	}
}
