using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class InteractionWorker_Breakup : InteractionWorker
	{
		private const float BaseChance = 0.02f;
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			if (!LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
			{
				return 0f;
			}
			float num = Mathf.InverseLerp(100f, -100f, (float)initiator.relations.OpinionOf(recipient));
			return 0.02f * num;
		}
		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks)
		{
			if (initiator.relations.DirectRelationExists(PawnRelationDefOf.Spouse, recipient))
			{
				initiator.relations.RemoveDirectRelation(PawnRelationDefOf.Spouse, recipient);
				initiator.relations.AddDirectRelation(PawnRelationDefOf.ExSpouse, recipient);
				Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)ThoughtMaker.MakeThought(ThoughtDefOf.DivorcedMe);
				thought_SocialMemory.SetOtherPawn(initiator);
				recipient.needs.mood.thoughts.TryGainThought(thought_SocialMemory);
				initiator.needs.mood.thoughts.RemoveThoughtsOfDef(ThoughtDefOf.GotMarried);
				recipient.needs.mood.thoughts.RemoveThoughtsOfDef(ThoughtDefOf.GotMarried);
				initiator.needs.mood.thoughts.RemoveSocialThoughts(ThoughtDefOf.HoneymoonPhase, recipient);
				recipient.needs.mood.thoughts.RemoveSocialThoughts(ThoughtDefOf.HoneymoonPhase, initiator);
			}
			else
			{
				initiator.relations.TryRemoveDirectRelation(PawnRelationDefOf.Lover, recipient);
				initiator.relations.TryRemoveDirectRelation(PawnRelationDefOf.Fiance, recipient);
				initiator.relations.AddDirectRelation(PawnRelationDefOf.ExLover, recipient);
				Thought_SocialMemory thought_SocialMemory2 = (Thought_SocialMemory)ThoughtMaker.MakeThought(ThoughtDefOf.BrokeUpWithMe);
				thought_SocialMemory2.SetOtherPawn(initiator);
				recipient.needs.mood.thoughts.TryGainThought(thought_SocialMemory2);
			}
			if (initiator.ownership.OwnedBed != null && initiator.ownership.OwnedBed == recipient.ownership.OwnedBed)
			{
				Pawn pawn = (Rand.Value >= 0.5f) ? recipient : initiator;
				pawn.ownership.UnclaimBed();
			}
			TaleRecorder.RecordTale(TaleDefOf.Breakup, new object[]
			{
				initiator,
				recipient
			});
		}
	}
}
