using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Recipe_RemoveHediff : Recipe_MedicalOperation
	{
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_RemoveHediff.<GetPartsToApplyOn>c__Iterator90 <GetPartsToApplyOn>c__Iterator = new Recipe_RemoveHediff.<GetPartsToApplyOn>c__Iterator90();
			<GetPartsToApplyOn>c__Iterator.pawn = pawn;
			<GetPartsToApplyOn>c__Iterator.recipe = recipe;
			<GetPartsToApplyOn>c__Iterator.<$>pawn = pawn;
			<GetPartsToApplyOn>c__Iterator.<$>recipe = recipe;
			Recipe_RemoveHediff.<GetPartsToApplyOn>c__Iterator90 expr_23 = <GetPartsToApplyOn>c__Iterator;
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
				if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(billDoer))
				{
					string text;
					if (!this.recipe.successfullyRemovedHediffMessage.NullOrEmpty())
					{
						text = string.Format(this.recipe.successfullyRemovedHediffMessage, billDoer.LabelBaseShort, pawn.LabelBaseShort);
					}
					else
					{
						text = "MessageSuccessfullyRemovedHediff".Translate(new object[]
						{
							billDoer.LabelBaseShort,
							pawn.LabelBaseShort,
							this.recipe.removesHediff.label
						});
					}
					Messages.Message(text, pawn, MessageSound.Benefit);
				}
			}
			Hediff hediff = pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == this.recipe.removesHediff && x.Part == part);
			pawn.health.RemoveHediff(hediff);
		}
	}
}
