using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Recipe_InstallNaturalBodyPart : Recipe_MedicalOperation
	{
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_InstallNaturalBodyPart.<GetPartsToApplyOn>c__Iterator8D <GetPartsToApplyOn>c__Iterator8D = new Recipe_InstallNaturalBodyPart.<GetPartsToApplyOn>c__Iterator8D();
			<GetPartsToApplyOn>c__Iterator8D.recipe = recipe;
			<GetPartsToApplyOn>c__Iterator8D.pawn = pawn;
			<GetPartsToApplyOn>c__Iterator8D.<$>recipe = recipe;
			<GetPartsToApplyOn>c__Iterator8D.<$>pawn = pawn;
			Recipe_InstallNaturalBodyPart.<GetPartsToApplyOn>c__Iterator8D expr_23 = <GetPartsToApplyOn>c__Iterator8D;
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
		}
	}
}
