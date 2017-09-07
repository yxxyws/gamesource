using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Hunt : JobDriver
	{
		private const TargetIndex VictimInd = TargetIndex.A;
		private const TargetIndex CorpseInd = TargetIndex.A;
		private const TargetIndex StoreCellInd = TargetIndex.B;
		private const int MaxHuntTicks = 5000;
		private int jobStartTick = -1;
		public Pawn Victim
		{
			get
			{
				Corpse corpse = this.Corpse;
				if (corpse != null)
				{
					return corpse.innerPawn;
				}
				return (Pawn)base.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}
		private Corpse Corpse
		{
			get
			{
				return base.CurJob.GetTarget(TargetIndex.A).Thing as Corpse;
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.jobStartTick, "jobStartTick", 0, false);
		}
		public override string GetReport()
		{
			return base.CurJob.def.reportString.Replace("TargetA", this.Victim.LabelBaseShort);
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Hunt.<MakeNewToils>c__Iterator16 <MakeNewToils>c__Iterator = new JobDriver_Hunt.<MakeNewToils>c__Iterator16();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_Hunt.<MakeNewToils>c__Iterator16 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private Toil StartCollectCorpseToil()
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				if (this.Victim == null)
				{
					toil.actor.jobs.EndCurrentJob(JobCondition.Incompletable);
					return;
				}
				TaleRecorder.RecordTale(TaleDef.Named("Hunted"), new object[]
				{
					this.pawn,
					this.Victim
				});
				Corpse corpse = HuntJobUtility.TryFindCorpse(this.Victim);
				if (corpse == null || !this.pawn.CanReserveAndReach(corpse, PathEndMode.ClosestTouch, Danger.Deadly, 1))
				{
					this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
					return;
				}
				corpse.SetForbidden(false, true);
				IntVec3 vec;
				if (StoreUtility.TryFindBestBetterStoreCellFor(corpse, this.pawn, StoragePriority.Unstored, this.pawn.Faction, out vec, true))
				{
					Find.Reservations.Reserve(this.pawn, corpse, 1);
					Find.Reservations.Reserve(this.pawn, vec, 1);
					this.pawn.CurJob.SetTarget(TargetIndex.B, vec);
					this.pawn.CurJob.SetTarget(TargetIndex.A, corpse);
					this.pawn.CurJob.maxNumToCarry = 1;
					this.pawn.CurJob.haulMode = HaulMode.ToCellStorage;
					return;
				}
				this.pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
			};
			return toil;
		}
	}
}
