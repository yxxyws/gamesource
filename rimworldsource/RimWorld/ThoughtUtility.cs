using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public static class ThoughtUtility
	{
		public static void GiveThoughtsForPawnExecuted(Pawn victim, PawnExecutionKind kind)
		{
			if (!victim.RaceProps.Humanlike)
			{
				return;
			}
			int forcedStage = 0;
			switch (kind)
			{
			case PawnExecutionKind.GenericBrutal:
				forcedStage = 1;
				break;
			case PawnExecutionKind.GenericHumane:
				forcedStage = 0;
				break;
			case PawnExecutionKind.OrganHarvesting:
				forcedStage = 2;
				break;
			}
			ThoughtDef def;
			if (victim.IsColonist)
			{
				def = ThoughtDefOf.KnowColonistExecuted;
			}
			else
			{
				def = ThoughtDefOf.KnowGuestExecuted;
			}
			foreach (Pawn current in Find.MapPawns.FreeColonistsAndPrisoners)
			{
				current.needs.mood.thoughts.TryGainThought(ThoughtMaker.MakeThought(def, forcedStage));
			}
		}
		public static void GiveThoughtsForPawnOrganHarvested(Pawn victim)
		{
			if (!victim.RaceProps.Humanlike)
			{
				return;
			}
			ThoughtDef thoughtDef = null;
			if (victim.IsColonist)
			{
				thoughtDef = ThoughtDefOf.KnowColonistOrganHarvested;
			}
			else
			{
				if (victim.HostFaction == Faction.OfColony)
				{
					thoughtDef = ThoughtDefOf.KnowGuestOrganHarvested;
				}
			}
			foreach (Pawn current in Find.MapPawns.FreeColonistsAndPrisoners)
			{
				if (current == victim)
				{
					current.needs.mood.thoughts.TryGainThought(ThoughtDefOf.MyOrganHarvested);
				}
				else
				{
					if (thoughtDef != null)
					{
						current.needs.mood.thoughts.TryGainThought(thoughtDef);
					}
				}
			}
		}
		public static bool IsSituationalThoughtNullifiedByHediffs(ThoughtDef def, Pawn pawn)
		{
			if (def.IsMemory)
			{
				return false;
			}
			float num = 0f;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				HediffStage curStage = hediffs[i].CurStage;
				if (curStage != null && curStage.pctConditionalThoughtsNullified > num)
				{
					num = curStage.pctConditionalThoughtsNullified;
				}
			}
			if (num == 0f)
			{
				return false;
			}
			Rand.PushSeed();
			Rand.Seed = pawn.thingIDNumber * 31 + (int)(def.index * 139);
			bool result = Rand.Value < num;
			Rand.PopSeed();
			return result;
		}
		public static bool IsThoughtNullifiedByOwnTales(ThoughtDef def, Pawn pawn)
		{
			if (def.nullifyingOwnTales != null)
			{
				for (int i = 0; i < def.nullifyingOwnTales.Count; i++)
				{
					if (Find.TaleManager.GetLatestTale(def.nullifyingOwnTales[i], pawn) != null)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
