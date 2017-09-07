using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public sealed class Storyteller : IExposable
	{
		public StorytellerDef def;
		public DifficultyDef difficulty;
		public List<IncidentMaker> incidentMakers;
		public IncidentQueue incidentQueue = new IncidentQueue();
		public WeatherDecider weatherDecider = new WeatherDecider();
		public StoryIntender_Population intenderPopulation;
		public static readonly Vector2 PortraitSizeTiny = new Vector2(116f, 124f);
		public static readonly Vector2 PortraitSizeLarge = new Vector2(580f, 620f);
		public string DebugReadout
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Storyteller : " + this.def.label);
				stringBuilder.AppendLine(this.intenderPopulation.DebugReadout);
				stringBuilder.AppendLine("Event queue:");
				stringBuilder.AppendLine(this.incidentQueue.DebugQueueReadout);
				stringBuilder.AppendLine("Wealth: " + Find.StoryWatcher.watcherWealth.WealthTotal);
				stringBuilder.AppendLine("DaysSinceSeriousDamage: " + Find.StoryWatcher.watcherDamage.DaysSinceSeriousDamage);
				stringBuilder.AppendLine("numRaidsEnemy: " + Find.StoryWatcher.statsRecord.numRaidsEnemy);
				stringBuilder.AppendLine("LastThreatBigQueueTick: " + Find.StoryWatcher.storyState.LastThreatBigQueueTick.TicksToPeriodString(true));
				stringBuilder.AppendLine("TotalThreatFactor: " + Find.StoryWatcher.watcherRampUp.TotalThreatPointsFactor.ToString("F5"));
				stringBuilder.AppendLine("   ShortFactor: " + Find.StoryWatcher.watcherRampUp.ShortTermFactor.ToString("F5"));
				stringBuilder.AppendLine("   LongFactor: " + Find.StoryWatcher.watcherRampUp.LongTermFactor.ToString("F5"));
				for (int i = 0; i < this.incidentMakers.Count; i++)
				{
					IncidentParms incidentParms = this.incidentMakers[i].ParmsNow(IncidentCategory.ThreatBig);
					stringBuilder.AppendLine("Current default threat params (" + this.incidentMakers[i].GetType() + "):");
					stringBuilder.AppendLine("    ThreatBig points: " + incidentParms.points);
				}
				return stringBuilder.ToString();
			}
		}
		public Storyteller()
		{
		}
		public Storyteller(StorytellerDef def, DifficultyDef difficulty)
		{
			this.def = def;
			this.difficulty = difficulty;
			this.intenderPopulation = new StoryIntender_Population(this);
			this.InitializeIncidentMakers();
		}
		private void InitializeIncidentMakers()
		{
			this.incidentMakers = new List<IncidentMaker>();
			for (int i = 0; i < this.def.incidentMakers.Count; i++)
			{
				this.incidentMakers.Add((IncidentMaker)Activator.CreateInstance(this.def.incidentMakers[i]));
			}
		}
		public void ExposeData()
		{
			Scribe_Defs.LookDef<StorytellerDef>(ref this.def, "def");
			Scribe_Defs.LookDef<DifficultyDef>(ref this.difficulty, "difficulty");
			Scribe_Deep.LookDeep<IncidentQueue>(ref this.incidentQueue, "incidentQueue", new object[0]);
			Scribe_Deep.LookDeep<WeatherDecider>(ref this.weatherDecider, "weatherDecider", new object[0]);
			Scribe_Deep.LookDeep<StoryIntender_Population>(ref this.intenderPopulation, "intenderPopulation", new object[]
			{
				this
			});
			if (this.difficulty == null)
			{
				Log.Error("Loaded storyteller without difficulty");
				this.difficulty = DefDatabase<DifficultyDef>.AllDefsListForReading[3];
			}
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
			{
				this.InitializeIncidentMakers();
			}
		}
		public void StorytellerTick()
		{
			this.incidentQueue.IncidentQueueTick();
			this.weatherDecider.WeatherDeciderTick();
			for (int i = 0; i < this.incidentMakers.Count; i++)
			{
				this.incidentMakers[i].IncidentMakerTick();
			}
		}
		public void Notify_DefChanged()
		{
			this.InitializeIncidentMakers();
		}
	}
}
