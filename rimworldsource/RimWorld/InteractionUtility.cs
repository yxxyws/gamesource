using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorld
{
	public static class InteractionUtility
	{
		public static bool CanInitiateInteraction(Pawn pawn)
		{
			return pawn.interactions != null && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking) && pawn.Awake() && !pawn.IsBurning();
		}
		public static bool CanReceiveInteraction(Pawn pawn)
		{
			return pawn.Awake() && !pawn.IsBurning();
		}
		public static bool HasAnyVerbForSocialFight(Pawn p)
		{
			if (p.Dead)
			{
				return false;
			}
			List<Verb> allVerbs = p.verbTracker.AllVerbs;
			for (int i = 0; i < allVerbs.Count; i++)
			{
				if (allVerbs[i] is Verb_MeleeAttack && allVerbs[i].IsStillUsableBy(p))
				{
					return true;
				}
			}
			return false;
		}
		public static bool TryGetRandomVerbForSocialFight(Pawn p, out Verb verb)
		{
			if (p.Dead)
			{
				verb = null;
				return false;
			}
			List<Verb> allVerbs = p.verbTracker.AllVerbs;
			return (
				from x in allVerbs
				where x is Verb_MeleeAttack && x.IsStillUsableBy(p)
				select x).TryRandomElementByWeight((Verb x) => (float)x.verbProps.AdjustedMeleeDamageAmount(x, p, null), out verb);
		}
		public static bool HasAnySocialFightProvokingThought(Pawn pawn, Pawn otherPawn)
		{
			Thought thought;
			return InteractionUtility.TryGetRandomSocialFightProvokingThought(pawn, otherPawn, out thought);
		}
		public static bool TryGetRandomSocialFightProvokingThought(Pawn pawn, Pawn otherPawn, out Thought thought)
		{
			if (pawn.needs.mood == null)
			{
				thought = null;
				return false;
			}
			List<Thought> thoughts = pawn.needs.mood.thoughts.Thoughts;
			return thoughts.Where(delegate(Thought x)
			{
				if (x.def == ThoughtDefOf.HadAngeringFight || x.def == ThoughtDefOf.HadCatharticFight)
				{
					return false;
				}
				ISocialThought socialThought = x as ISocialThought;
				return socialThought != null && socialThought.OpinionOffset(otherPawn) < 0f;
			}).TryRandomElementByWeight((Thought x) => -((ISocialThought)x).OpinionOffset(otherPawn), out thought);
		}
	}
}
