using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	internal class Recipe_Kill : RecipeWorker
	{
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_Kill.<GetPartsToApplyOn>c__Iterator8E <GetPartsToApplyOn>c__Iterator8E = new Recipe_Kill.<GetPartsToApplyOn>c__Iterator8E();
			<GetPartsToApplyOn>c__Iterator8E.pawn = pawn;
			<GetPartsToApplyOn>c__Iterator8E.<$>pawn = pawn;
			Recipe_Kill.<GetPartsToApplyOn>c__Iterator8E expr_15 = <GetPartsToApplyOn>c__Iterator8E;
			expr_15.$PC = -2;
			return expr_15;
		}
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer)
		{
			pawn.health.AddHediff(this.recipe.addsHediff, part, null);
			ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, PawnExecutionKind.GenericHumane);
		}
	}
}
