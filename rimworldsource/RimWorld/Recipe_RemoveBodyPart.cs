using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	internal class Recipe_RemoveBodyPart : Recipe_MedicalOperation
	{
		private const float ViolationGoodwillImpact = 20f;
		[DebuggerHidden]
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			Recipe_RemoveBodyPart.<GetPartsToApplyOn>c__Iterator8F <GetPartsToApplyOn>c__Iterator8F = new Recipe_RemoveBodyPart.<GetPartsToApplyOn>c__Iterator8F();
			<GetPartsToApplyOn>c__Iterator8F.pawn = pawn;
			<GetPartsToApplyOn>c__Iterator8F.<$>pawn = pawn;
			Recipe_RemoveBodyPart.<GetPartsToApplyOn>c__Iterator8F expr_15 = <GetPartsToApplyOn>c__Iterator8F;
			expr_15.$PC = -2;
			return expr_15;
		}
		public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
		{
			return pawn.Faction != billDoerFaction && HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest;
		}
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer)
		{
			bool flag = MedicalRecipesUtility.IsClean(pawn, part);
			bool flag2 = this.IsViolationOnPawn(pawn, part, Faction.OfColony);
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
				MedicalRecipesUtility.SpawnNaturalPartIfClean(pawn, part, billDoer.Position);
				MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, part, billDoer.Position);
			}
			BodyPartDamageInfo value = new BodyPartDamageInfo(part, false, null);
			DamageInfo dinfo = new DamageInfo(DamageDefOf.SurgicalCut, 99999, null, new BodyPartDamageInfo?(value), null);
			pawn.TakeDamage(dinfo);
			if (flag)
			{
				if (pawn.Dead)
				{
					ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, PawnExecutionKind.OrganHarvesting);
				}
				else
				{
					ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn);
				}
			}
			if (flag2)
			{
				pawn.Faction.AffectGoodwillWith(billDoer.Faction, -20f);
			}
		}
	}
}
