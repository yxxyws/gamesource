using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Recipe_InstallImplant : Recipe_MedicalOperation
	{
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_InstallImplant.<GetPartsToApplyOn>c__Iterator8C <GetPartsToApplyOn>c__Iterator8C = new Recipe_InstallImplant.<GetPartsToApplyOn>c__Iterator8C();
			<GetPartsToApplyOn>c__Iterator8C.recipe = recipe;
			<GetPartsToApplyOn>c__Iterator8C.pawn = pawn;
			<GetPartsToApplyOn>c__Iterator8C.<$>recipe = recipe;
			<GetPartsToApplyOn>c__Iterator8C.<$>pawn = pawn;
			Recipe_InstallImplant.<GetPartsToApplyOn>c__Iterator8C expr_23 = <GetPartsToApplyOn>c__Iterator8C;
			expr_23.$PC = -2;
			return expr_23;
		}
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer)
		{
			if (billDoer != null)
			{
				if (base.CheckSurgeryFail(billDoer, pawn))
				{
					return;
				}
				TaleRecorder.RecordTale(TaleDef.Named("DidSurgery"), new object[]
				{
					billDoer,
					pawn
				});
			}
			pawn.health.AddHediff(this.recipe.addsHediff, part, null);
		}
	}
}
