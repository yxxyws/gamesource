using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.AI
{
	public class JobDriver_DoBill : JobDriver
	{
		public const TargetIndex BillGiverInd = TargetIndex.A;
		public const TargetIndex IngredientInd = TargetIndex.B;
		public const TargetIndex IngredientPlaceCellInd = TargetIndex.C;
		public float workLeft;
		public int billStartTick;
		public IBillGiver BillGiver
		{
			get
			{
				IBillGiver billGiver = this.pawn.jobs.curJob.GetTarget(TargetIndex.A).Thing as IBillGiver;
				if (billGiver == null)
				{
					throw new InvalidOperationException("DoBill on non-Billgiver.");
				}
				return billGiver;
			}
		}
		public override string GetReport()
		{
			if (this.pawn.jobs.curJob.RecipeDef != null)
			{
				return base.ReportStringProcessed(this.pawn.jobs.curJob.RecipeDef.jobString);
			}
			return base.GetReport();
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.workLeft, "workLeft", 0f, false);
			Scribe_Values.LookValue<int>(ref this.billStartTick, "billStartTick", 0, false);
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_DoBill.<MakeNewToils>c__Iterator12C <MakeNewToils>c__Iterator12C = new JobDriver_DoBill.<MakeNewToils>c__Iterator12C();
			<MakeNewToils>c__Iterator12C.<>f__this = this;
			JobDriver_DoBill.<MakeNewToils>c__Iterator12C expr_0E = <MakeNewToils>c__Iterator12C;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private static Toil JumpToCollectNextIntoHandsForBill(Toil gotoGetTargetToil, TargetIndex ind)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				List<TargetInfo> targetQueue = curJob.GetTargetQueue(ind);
				if (targetQueue.NullOrEmpty<TargetInfo>())
				{
					return;
				}
				if (actor.carrier.CarriedThing == null)
				{
					Log.Error("JumpToAlsoCollectTargetInQueue run on " + actor + " who is not carrying something.");
					return;
				}
				for (int i = 0; i < targetQueue.Count; i++)
				{
					if (GenAI.CanUseItemForWork(actor, targetQueue[i].Thing))
					{
						if (targetQueue[i].Thing.CanStackWith(actor.carrier.CarriedThing))
						{
							if ((actor.Position - targetQueue[i].Thing.Position).LengthHorizontalSquared <= 64f)
							{
								int num = (actor.carrier.CarriedThing != null) ? actor.carrier.CarriedThing.stackCount : 0;
								int num2 = curJob.numToBringList[i];
								if (num2 + num > targetQueue[i].Thing.def.stackLimit)
								{
									num2 = targetQueue[i].Thing.def.stackLimit - num;
								}
								if (num2 != 0)
								{
									List<int> numToBringList;
									List<int> expr_172 = numToBringList = curJob.numToBringList;
									int num3;
									int expr_177 = num3 = i;
									num3 = numToBringList[num3];
									expr_172[expr_177] = num3 - num2;
									curJob.maxNumToCarry = num + num2;
									curJob.SetTarget(ind, targetQueue[i].Thing);
									if (curJob.numToBringList[i] == 0)
									{
										curJob.numToBringList.RemoveAt(i);
										targetQueue.RemoveAt(i);
									}
									actor.jobs.curDriver.JumpToToil(gotoGetTargetToil);
									return;
								}
							}
						}
					}
				}
			};
			return toil;
		}
	}
}
