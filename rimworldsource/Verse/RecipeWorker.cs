using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public class RecipeWorker
	{
		public RecipeDef recipe;
		[DebuggerHidden]
		public virtual IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			RecipeWorker.<GetPartsToApplyOn>c__Iterator8A <GetPartsToApplyOn>c__Iterator8A = new RecipeWorker.<GetPartsToApplyOn>c__Iterator8A();
			RecipeWorker.<GetPartsToApplyOn>c__Iterator8A expr_07 = <GetPartsToApplyOn>c__Iterator8A;
			expr_07.$PC = -2;
			return expr_07;
		}
		public virtual void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer)
		{
		}
		public virtual bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
		{
			return pawn.Faction != billDoerFaction && this.recipe.isViolation;
		}
	}
}
