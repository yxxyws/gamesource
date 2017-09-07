using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Need_Mood : Need_Seeker
	{
		public ThoughtHandler thoughts;
		public PawnObserver observer;
		public PawnRecentMemory recentMemory;
		public override float CurInstantLevel
		{
			get
			{
				float num = this.thoughts.TotalMood();
				if (this.pawn.IsColonist || this.pawn.IsPrisonerOfColony)
				{
					num += Find.Storyteller.difficulty.colonistMoodOffset;
				}
				return Mathf.Clamp01(this.def.baseLevel + num / 100f);
			}
		}
		public string MoodString
		{
			get
			{
				if (this.pawn.MentalStateDef != null)
				{
					return "Mood_MentalState".Translate();
				}
				float statValue = this.pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true);
				if (this.CurLevel < statValue)
				{
					return "Mood_AboutToBreak".Translate();
				}
				if (this.CurLevel < statValue + 0.05f)
				{
					return "Mood_OnEdge".Translate();
				}
				if (this.CurLevel < 0.35f)
				{
					return "Mood_Stressed".Translate();
				}
				if (this.CurLevel < 0.65f)
				{
					return "Mood_Neutral".Translate();
				}
				if (this.CurLevel < 0.9f)
				{
					return "Mood_Content".Translate();
				}
				return "Mood_Happy".Translate();
			}
		}
		public Need_Mood(Pawn pawn) : base(pawn)
		{
			this.thoughts = new ThoughtHandler(pawn);
			this.observer = new PawnObserver(pawn);
			this.recentMemory = new PawnRecentMemory(pawn);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.LookDeep<ThoughtHandler>(ref this.thoughts, "thoughts", new object[]
			{
				this.pawn
			});
			Scribe_Deep.LookDeep<PawnRecentMemory>(ref this.recentMemory, "recentMemory", new object[]
			{
				this.pawn
			});
		}
		public override void NeedInterval()
		{
			base.NeedInterval();
			if (!this.pawn.Spawned)
			{
				return;
			}
			this.recentMemory.RecentMemoryInterval();
			this.thoughts.ThoughtInterval();
			this.observer.ObserverInterval();
		}
		public override string GetTipString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetTipString());
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("MentalBreakThresholdHard".Translate() + ": " + this.pawn.mindState.mentalStateStarter.StartHardMentalStateThreshold.ToStringPercent());
			stringBuilder.AppendLine("MentalBreakThresholdSoft".Translate() + ": " + this.pawn.mindState.mentalStateStarter.StartSoftMentalStateThreshold.ToStringPercent());
			return stringBuilder.ToString();
		}
		public override void DrawOnGUI(Rect rect)
		{
			if (this.threshPercents == null)
			{
				this.threshPercents = new List<float>();
			}
			this.threshPercents.Clear();
			this.threshPercents.Add(this.pawn.mindState.mentalStateStarter.StartHardMentalStateThreshold);
			this.threshPercents.Add(this.pawn.mindState.mentalStateStarter.StartSoftMentalStateThreshold);
			base.DrawOnGUI(rect);
		}
	}
}
