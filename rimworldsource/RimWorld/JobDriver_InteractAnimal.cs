using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class JobDriver_InteractAnimal : JobDriver
	{
		protected const TargetIndex AnimalInd = TargetIndex.A;
		private const TargetIndex FoodHandInd = TargetIndex.B;
		private const int FeedDuration = 270;
		private const int TalkDuration = 270;
		private const float NutritionPercentagePerFeed = 0.15f;
		private const float MaxMinNutritionPerFeed = 0.3f;
		public const int FeedCount = 2;
		public const FoodPreferability MaxFoodPreferability = FoodPreferability.Raw;
		protected Pawn Animal
		{
			get
			{
				return (Pawn)base.CurJob.targetA.Thing;
			}
		}
		protected abstract Toil FinalInteractToil();
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_InteractAnimal.<MakeNewToils>c__Iterator5 <MakeNewToils>c__Iterator = new JobDriver_InteractAnimal.<MakeNewToils>c__Iterator5();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_InteractAnimal.<MakeNewToils>c__Iterator5 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public static float RequiredNutritionPerFeed(Pawn animal)
		{
			return Mathf.Min(animal.needs.food.MaxLevel * 0.15f, 0.3f);
		}
		private static Toil TalkToAnimal(TargetIndex tameeInd)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.GetActor();
				Pawn recipient = (Pawn)((Thing)actor.CurJob.GetTarget(tameeInd));
				actor.interactions.TryInteractWith(recipient, InteractionDefOf.AnimalChat);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.defaultDuration = 270;
			return toil;
		}
		private static Toil StartFeedAnimal(TargetIndex tameeInd)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.GetActor();
				Pawn pawn = (Pawn)((Thing)actor.CurJob.GetTarget(tameeInd));
				float num = JobDriver_InteractAnimal.RequiredNutritionPerFeed(pawn);
				PawnUtility.ForceWait(pawn, 270, actor);
				Thing thing = FoodUtility.FoodInInventory(actor, pawn, FoodPreferability.NeverForNutrition, FoodPreferability.Raw, num);
				if (thing == null)
				{
					actor.jobs.EndCurrentJob(JobCondition.Incompletable);
					return;
				}
				actor.mindState.lastInventoryRawFoodUseTick = Find.TickManager.TicksGame;
				int count = FoodUtility.StackCountForNutrition(thing.def, num);
				Thing thing2 = actor.inventory.container.Get(thing, count);
				actor.carrier.TryStartCarry(thing2);
				actor.CurJob.targetB = thing2;
			};
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.defaultDuration = 270;
			return toil;
		}
	}
}
