using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class InteractionWorker_RecruitAttempt : InteractionWorker
	{
		private const float MinMoodToRecruit = 0.35f;
		private const float MinRecruitChance = 0.005f;
		private const float BondRelationChanceFactor = 1.5f;
		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks)
		{
			float num = (!recipient.RaceProps.Humanlike) ? initiator.GetStatValue(StatDefOf.TameAnimalChance, true) : initiator.GetStatValue(StatDefOf.RecruitPrisonerChance, true);
			if (!DebugSettings.instantRecruit)
			{
				if (recipient.needs.mood != null && recipient.needs.mood.CurLevel < 0.35f)
				{
					if (recipient.RaceProps.Humanlike)
					{
						Messages.Message("MessageRecruitFailBecauseMood".Translate(new object[]
						{
							initiator.NameStringShort,
							recipient,
							0.35f.ToStringPercent()
						}), recipient, MessageSound.Silent);
					}
					extraSentencePacks.Add(RulePackDefOf.Sentence_RecruitAttemptRejected);
					return;
				}
				if (recipient.RaceProps.Humanlike)
				{
					num *= 1f - recipient.guest.RecruitDifficulty / 100f;
				}
				else
				{
					num *= (1f - recipient.RaceProps.wildness) * 2f;
				}
				num = Mathf.Max(num, 0.005f);
				if (initiator.relations.DirectRelationExists(PawnRelationDefOf.Bond, recipient))
				{
					num *= 1.5f;
				}
				num = Mathf.Clamp01(num);
				if (Rand.Value > num)
				{
					if (recipient.RaceProps.Humanlike)
					{
						Messages.Message("MessageRecruitFail".Translate(new object[]
						{
							initiator,
							recipient,
							num.ToStringPercent()
						}), recipient, MessageSound.Negative);
					}
					else
					{
						MoteThrower.ThrowText((initiator.DrawPos + recipient.DrawPos) / 2f, "TextMote_TameFail".Translate(new object[]
						{
							num.ToStringPercent()
						}), 500);
					}
					extraSentencePacks.Add(RulePackDefOf.Sentence_RecruitAttemptRejected);
					return;
				}
			}
			InteractionWorker_RecruitAttempt.DoRecruit(initiator, recipient, num);
			extraSentencePacks.Add(RulePackDefOf.Sentence_RecruitAttemptAccepted);
		}
		public static void DoRecruit(Pawn recruiter, Pawn recruitee, float recruitChance)
		{
			if (recruitee.guest != null)
			{
				recruitee.guest.SetGuestStatus(null, false);
			}
			bool flag = recruitee.Name != null;
			if (recruitee.Faction != recruiter.Faction)
			{
				recruitee.SetFaction(recruiter.Faction, recruiter);
			}
			if (recruitee.RaceProps.Humanlike)
			{
				Find.LetterStack.ReceiveLetter("LetterLabelMessageRecruitSuccess".Translate(), "MessageRecruitSuccess".Translate(new object[]
				{
					recruiter,
					recruitee,
					recruitChance.ToStringPercent()
				}), LetterType.Good, recruitee, null);
				TaleRecorder.RecordTale(TaleDef.Named("Recruited"), new object[]
				{
					recruiter,
					recruitee
				});
				recruiter.records.Increment(RecordDefOf.PrisonersRecruited);
				Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)ThoughtMaker.MakeThought(ThoughtDefOf.RecruitedMe);
				thought_SocialMemory.SetOtherPawn(recruiter);
				recruitee.needs.mood.thoughts.TryGainThought(thought_SocialMemory);
			}
			else
			{
				if (!flag)
				{
					Messages.Message("MessageTameAndNameSuccess".Translate(new object[]
					{
						recruiter.LabelBaseShort,
						recruitee.LabelIndefinite(),
						recruitChance.ToStringPercent(),
						recruitee.Name.ToStringFull
					}), recruitee, MessageSound.Benefit);
				}
				else
				{
					Messages.Message("MessageTameSuccess".Translate(new object[]
					{
						recruiter.LabelBaseShort,
						recruitee.LabelIndefinite(),
						recruitChance.ToStringPercent()
					}), recruitee, MessageSound.Benefit);
				}
				MoteThrower.ThrowText((recruiter.DrawPos + recruitee.DrawPos) / 2f, "TextMote_TameSuccess".Translate(new object[]
				{
					recruitChance.ToStringPercent()
				}), 500);
				recruiter.records.Increment(RecordDefOf.AnimalsTamed);
				RelationsUtility.TryDevelopBondRelation(recruiter, recruitee, 0.05f);
				float num = Mathf.Lerp(0.02f, 1f, recruitee.RaceProps.wildness);
				if (Rand.Value < num)
				{
					TaleRecorder.RecordTale(TaleDefOf.TamedAnimal, new object[]
					{
						recruiter,
						recruitee
					});
				}
			}
			InteractionWorker_RecruitAttempt.PostJoinFaction(recruitee);
		}
		public static void PostJoinFaction(Pawn joiner)
		{
			joiner.mindState.exitMapAfterTick = -99999;
			if (joiner.RaceProps.Humanlike)
			{
				joiner.playerSettings.medCare = MedicalCareCategory.Best;
			}
			else
			{
				joiner.playerSettings.medCare = MedicalCareCategory.NoMeds;
			}
			if (joiner.jobQueue != null)
			{
				joiner.jobQueue.Clear();
			}
			joiner.jobs.EndCurrentJob(JobCondition.InterruptForced);
			if (joiner.caller != null)
			{
				joiner.caller.DoCall();
			}
		}
	}
}
