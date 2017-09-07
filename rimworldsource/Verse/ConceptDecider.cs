using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Verse
{
	public static class ConceptDecider
	{
		private const float MapStartGracePeriod = 8f;
		private const float KnowledgeDecayRate = 0.015f;
		private const float LessonNeedDecayRate = 0.4f;
		private const float OpportunityMaxDesireAdd = 60f;
		public const float KnowledgeForAutoNoteVanish = 20f;
		private const int CheckInterval = 15;
		private const float MaxLessonInterval = 200f;
		private static Dictionary<ConceptDef, float> opportunities = new Dictionary<ConceptDef, float>();
		private static float lastLessonTime = -999f;
		private static List<ConceptDef> alertingConcepts = new List<ConceptDef>();
		private static float SecondsSinceLesson
		{
			get
			{
				return Time.realtimeSinceStartup - ConceptDecider.lastLessonTime;
			}
		}
		private static float RelaxDesire
		{
			get
			{
				return 100f - ConceptDecider.SecondsSinceLesson * 0.5f;
			}
		}
		private static TutorNote ActiveNote
		{
			get
			{
				return ActiveTutorNoteManager.activeNote;
			}
		}
		public static void Reset()
		{
			ConceptDecider.alertingConcepts.Clear();
		}
		public static bool AlertShouldDisplay(ConceptDef conc)
		{
			return (ConceptDecider.alertingConcepts.Contains(conc) || conc.alertStartActive) && !ConceptDatabase.IsComplete(conc);
		}
		public static void TeachOpportunity(ConceptDef conc, OpportunityType opp)
		{
			ConceptDecider.TeachOpportunity(conc, null, opp);
		}
		public static void TeachOpportunity(ConceptDef conc, Thing subject, OpportunityType opp)
		{
			float value = 999f;
			switch (opp)
			{
			case OpportunityType.GuiFrame:
				value = 0.2f;
				break;
			case OpportunityType.Low:
				value = 25f;
				break;
			case OpportunityType.GoodToKnow:
				value = 40f;
				break;
			case OpportunityType.Important:
				value = 75f;
				break;
			case OpportunityType.Critical:
				value = 100f;
				break;
			default:
				Log.Error("Unknown need");
				break;
			}
			ConceptDecider.opportunities[conc] = value;
		}
		public static void Notify_KnowledgeDemonstrated(ConceptDef conc)
		{
			if (ConceptDatabase.IsComplete(conc))
			{
				ConceptDecider.opportunities[conc] = 0f;
			}
			if (ConceptDecider.ActiveNote != null && ConceptDecider.ActiveNote.def == conc && ConceptDatabase.GetKnowledge(conc) > 20f)
			{
				ConceptDecider.ActiveNote.InitiateExpiry();
			}
		}
		public static void ConceptDeciderUpdate()
		{
			if (!Prefs.AdaptiveTrainingEnabled)
			{
				return;
			}
			if (Game.Mode == GameMode.MapPlaying)
			{
				if (Time.timeSinceLevelLoad < 8f)
				{
					return;
				}
				if (Find.WindowStack.SecondsSinceClosedGameStartDialog < 8f)
				{
					return;
				}
				if (Find.TickManager.NotPlaying)
				{
					return;
				}
			}
			for (int i = ConceptDecider.alertingConcepts.Count - 1; i >= 0; i--)
			{
				if (ConceptDatabase.IsComplete(ConceptDecider.alertingConcepts[i]))
				{
					ConceptDecider.alertingConcepts.RemoveAt(i);
				}
			}
			if (ActiveTutorNoteManager.activeNote != null)
			{
				ConceptDecider.lastLessonTime = Time.realtimeSinceStartup;
			}
			if (Time.frameCount % 15 == 0 && ActiveTutorNoteManager.activeNote == null)
			{
				for (int j = 0; j < DefDatabase<ConceptDef>.AllDefsListForReading.Count; j++)
				{
					ConceptDef conceptDef = DefDatabase<ConceptDef>.AllDefsListForReading[j];
					if (!ConceptDatabase.IsComplete(conceptDef))
					{
						float num = ConceptDatabase.GetKnowledge(conceptDef);
						num -= 0.015f * Time.deltaTime * 15f;
						if (num < 0f)
						{
							num = 0f;
						}
						ConceptDatabase.SetKnowledge(conceptDef, num);
						if (conceptDef.opportunityDecays)
						{
							float num2 = ConceptDecider.GetOpportunity(conceptDef);
							num2 -= 0.4f * Time.deltaTime * 15f;
							if (num2 < 0f)
							{
								num2 = 0f;
							}
							ConceptDecider.opportunities[conceptDef] = num2;
						}
					}
				}
				ConceptDef conceptDef2 = ConceptDecider.MostDesiredConcept();
				if (conceptDef2 != null)
				{
					float desire = ConceptDecider.GetDesire(conceptDef2);
					if (desire > 0.1f && ConceptDecider.RelaxDesire < desire)
					{
						ConceptDecider.InitiateLesson(conceptDef2);
					}
				}
			}
		}
		private static ConceptDef MostDesiredConcept()
		{
			float num = -9999f;
			ConceptDef result = null;
			foreach (ConceptDef current in DefDatabase<ConceptDef>.AllDefs)
			{
				float desire = ConceptDecider.GetDesire(current);
				if (desire > num)
				{
					if (!current.needsOpportunity || ConceptDecider.GetOpportunity(current) >= 0.1f)
					{
						if (ConceptDatabase.GetKnowledge(current) <= 15f)
						{
							num = desire;
							result = current;
						}
					}
				}
			}
			return result;
		}
		private static float GetDesire(ConceptDef conc)
		{
			if (ConceptDatabase.IsComplete(conc))
			{
				return 0f;
			}
			if (conc.alertStartActive)
			{
				return 0f;
			}
			if (conc.alertDisplay && ConceptDecider.alertingConcepts.Contains(conc))
			{
				return 0f;
			}
			if (Game.Mode != conc.gameMode)
			{
				return 0f;
			}
			if (conc.needsOpportunity && ConceptDecider.GetOpportunity(conc) < 0.1f)
			{
				return 0f;
			}
			if (ConceptDecider.SecondsSinceLesson < conc.minSecondsSinceLesson)
			{
				return 0f;
			}
			float num = 0f;
			num += (float)conc.priority;
			num += ConceptDecider.GetOpportunity(conc) / 100f * 60f;
			return num * (1f - ConceptDatabase.GetKnowledge(conc) / 100f);
		}
		private static float GetOpportunity(ConceptDef conc)
		{
			float result;
			if (ConceptDecider.opportunities.TryGetValue(conc, out result))
			{
				return result;
			}
			ConceptDecider.opportunities[conc] = 0f;
			return 0f;
		}
		private static void InitiateLesson(ConceptDef def)
		{
			if (def.alertDisplay)
			{
				ConceptDecider.alertingConcepts.Add(def);
			}
			else
			{
				ActiveTutorNoteManager.StartShowing(new TutorNote(def));
			}
		}
		public static string GetDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("RelaxDesire: " + ConceptDecider.RelaxDesire);
			foreach (ConceptDef current in 
				from co in DefDatabase<ConceptDef>.AllDefs
				orderby ConceptDecider.GetDesire(co) descending
				select co)
			{
				if (ConceptDatabase.IsComplete(current))
				{
					stringBuilder.AppendLine(current.defName + " complete");
				}
				else
				{
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						current.defName,
						"\n   know ",
						ConceptDatabase.GetKnowledge(current).ToString("######0.000"),
						"\n   need ",
						ConceptDecider.opportunities[current].ToString("######0.000"),
						"\n   des ",
						ConceptDecider.GetDesire(current).ToString("#####0.000")
					}));
				}
			}
			return stringBuilder.ToString();
		}
		public static void DebugForceInitiateBestLessonNow()
		{
			ConceptDecider.InitiateLesson((
				from def in DefDatabase<ConceptDef>.AllDefs
				orderby ConceptDecider.GetDesire(def) descending
				select def).First<ConceptDef>());
		}
	}
}
