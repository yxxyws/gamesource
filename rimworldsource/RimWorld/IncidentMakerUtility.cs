using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public static class IncidentMakerUtility
	{
		private const float WealthBase = 2000f;
		private const float PointsPer1000Wealth = 11.5f;
		private const float PointsPerColonist = 40f;
		private const float MinMaxSquadCost = 50f;
		private const float BuildingWealthFactor = 0.5f;
		private const float HalveLimitLo = 1000f;
		private const float HalveLimitHi = 2000f;
		public static IncidentParms DefaultParmsNow(StorytellerDef tellerDef, IncidentCategory incCat)
		{
			IncidentParms incidentParms = new IncidentParms();
			if (incCat == IncidentCategory.ThreatSmall || incCat == IncidentCategory.ThreatBig)
			{
				float num = Find.StoryWatcher.watcherWealth.WealthItems + Find.StoryWatcher.watcherWealth.WealthBuildings * 0.5f;
				num -= 2000f;
				if (num < 0f)
				{
					num = 0f;
				}
				float num2 = num / 1000f * 11.5f;
				float num3 = (float)Find.MapPawns.FreeColonistsCount * 40f;
				incidentParms.points = num2 + num3;
				incidentParms.points *= Find.StoryWatcher.watcherRampUp.TotalThreatPointsFactor;
				incidentParms.points *= Find.Storyteller.difficulty.threatScale;
				switch (Find.StoryWatcher.statsRecord.numThreatBigs)
				{
				case 0:
					incidentParms.points = 35f;
					incidentParms.raidForceOneIncap = true;
					incidentParms.raidNeverFleeIndividual = true;
					break;
				case 1:
					incidentParms.points *= 0.5f;
					break;
				case 2:
					incidentParms.points *= 0.7f;
					break;
				case 3:
					incidentParms.points *= 0.8f;
					break;
				case 4:
					incidentParms.points *= 0.9f;
					break;
				default:
					incidentParms.points *= 1f;
					break;
				}
				if (incidentParms.points < 0f)
				{
					incidentParms.points = 0f;
				}
				if (incidentParms.points > 1000f)
				{
					if (incidentParms.points > 2000f)
					{
						incidentParms.points = 2000f + (incidentParms.points - 2000f) * 0.5f;
					}
					incidentParms.points = 1000f + (incidentParms.points - 1000f) * 0.5f;
				}
			}
			return incidentParms;
		}
		public static void DebugLogTestFutureIncidents()
		{
			StoryState storyState = Find.StoryWatcher.storyState;
			int ticksGame = Find.TickManager.TicksGame;
			Find.StoryWatcher.storyState = new StoryState();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Test future incidents:");
			List<IncidentMaker> incidentMakers = Find.Storyteller.incidentMakers;
			int[] array = new int[incidentMakers.Count];
			for (int i = 0; i < 240; i++)
			{
				for (int j = 0; j < incidentMakers.Count; j++)
				{
					foreach (QueuedIncident current in incidentMakers[j].NewIncidentSet())
					{
						stringBuilder.Append("M" + j + "  ");
						array[j]++;
						if (!Find.StoryWatcher.storyState.AllowNewQueuedIncident(current))
						{
							stringBuilder.AppendLine("    DISALLOWED " + current);
						}
						else
						{
							Find.StoryWatcher.storyState.RecordDataFromNewQueuedIncident(current);
							stringBuilder.AppendLine("    " + current);
						}
					}
				}
				Find.TickManager.DebugSetTicksGame(Find.TickManager.TicksGame + 7500);
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Incident totals:");
			for (int k = 0; k < array.Length; k++)
			{
				float f = (float)array[k] / (float)array.Sum();
				float num = (float)array[k] / 30f;
				float num2 = 1f / num;
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"   M",
					k,
					": ",
					array[k],
					"  (",
					f.ToStringPercent("F2"),
					" of total, avg ",
					num.ToString("F2"),
					" per day, avg interval ",
					num2,
					")"
				}));
			}
			Log.Message(stringBuilder.ToString());
			Find.TickManager.DebugSetTicksGame(ticksGame);
			Find.StoryWatcher.storyState = storyState;
		}
	}
}
