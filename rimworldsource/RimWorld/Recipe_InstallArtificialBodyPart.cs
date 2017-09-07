using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Recipe_InstallArtificialBodyPart : Recipe_MedicalOperation
	{
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_InstallArtificialBodyPart.<GetPartsToApplyOn>c__Iterator8B <GetPartsToApplyOn>c__Iterator8B = new Recipe_InstallArtificialBodyPart.<GetPartsToApplyOn>c__Iterator8B();
			<GetPartsToApplyOn>c__Iterator8B.recipe = recipe;
			<GetPartsToApplyOn>c__Iterator8B.pawn = pawn;
			<GetPartsToApplyOn>c__Iterator8B.<$>recipe = recipe;
			<GetPartsToApplyOn>c__Iterator8B.<$>pawn = pawn;
			Recipe_InstallArtificialBodyPart.<GetPartsToApplyOn>c__Iterator8B expr_23 = <GetPartsToApplyOn>c__Iterator8B;
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
				MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, billDoer.Position);
			}
			pawn.health.AddHediff(this.recipe.addsHediff, part, null);
		}
	}
}
