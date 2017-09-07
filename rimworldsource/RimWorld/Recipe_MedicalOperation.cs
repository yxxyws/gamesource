using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public abstract class Recipe_MedicalOperation : RecipeWorker
	{
		private const float CatastrophicFailChance = 0.5f;
		protected bool CheckSurgeryFail(Pawn surgeon, Pawn patient)
		{
			if (surgeon == null)
			{
				Log.Error("surgeon is null");
				return false;
			}
			if (patient == null)
			{
				Log.Error("patient is null");
				return false;
			}
			float num = 1f;
			float num2 = surgeon.GetStatValue(StatDefOf.SurgerySuccessChance, true);
			if (num2 < 1f)
			{
				num2 = Mathf.Pow(num2, this.recipe.surgeonSurgerySuccessChanceExponent);
			}
			num *= num2;
			Room room = surgeon.GetRoom();
			if (room != null)
			{
				float num3 = room.GetStat(RoomStatDefOf.SurgerySuccessChanceFactor);
				if (num3 < 1f)
				{
					num3 = Mathf.Pow(num3, this.recipe.roomSurgerySuccessChanceFactorExponent);
				}
				num *= num3;
			}
			Thing thing = surgeon.CurJob.targetB.Thing as Medicine;
			if (thing != null)
			{
				num *= thing.GetStatValue(StatDefOf.MedicalPotency, true);
			}
			num *= this.recipe.surgerySuccessChanceFactor;
			if (Rand.Value > num)
			{
				if (Rand.Value < this.recipe.deathOnFailedSurgeryChance)
				{
					int num4 = 0;
					while (!patient.Dead)
					{
						HealthUtility.GiveInjuriesOperationFailureCatastrophic(patient);
						num4++;
						if (num4 > 300)
						{
							Log.Error("Could not kill patient.");
							break;
						}
					}
				}
				else
				{
					if (Rand.Value < 0.5f)
					{
						Messages.Message("MessageMedicalOperationFailureCatastrophic".Translate(new object[]
						{
							surgeon.LabelBaseShort,
							patient.LabelBaseShort
						}), patient, MessageSound.SeriousAlert);
						HealthUtility.GiveInjuriesOperationFailureCatastrophic(patient);
					}
					else
					{
						Messages.Message("MessageMedicalOperationFailureMinor".Translate(new object[]
						{
							surgeon.LabelBaseShort,
							patient.LabelBaseShort
						}), patient, MessageSound.Negative);
						HealthUtility.GiveInjuriesOperationFailureMinor(patient);
					}
				}
				if (!patient.Dead)
				{
					this.TryGainBotchedSurgeryThought(patient, surgeon);
				}
				return true;
			}
			return false;
		}
		private void TryGainBotchedSurgeryThought(Pawn patient, Pawn surgeon)
		{
			if (!patient.RaceProps.Humanlike)
			{
				return;
			}
			Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)ThoughtMaker.MakeThought(ThoughtDefOf.BotchedMySurgery);
			thought_SocialMemory.SetOtherPawn(surgeon);
			patient.needs.mood.thoughts.TryGainThought(thought_SocialMemory);
		}
	}
}
