using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public abstract class IncidentMaker
	{
		public const int IntervalsPerDay = 8;
		public const int QueueInterval = 7500;
		protected Storyteller Teller
		{
			get
			{
				return Find.Storyteller;
			}
		}
		protected StorytellerDef Def
		{
			get
			{
				return this.Teller.def;
			}
		}
		protected IncidentQueue IncQueue
		{
			get
			{
				return Find.Storyteller.incidentQueue;
			}
		}
		protected StoryState StoryState
		{
			get
			{
				return Find.StoryWatcher.storyState;
			}
		}
		protected int IntervalsPassed
		{
			get
			{
				return Find.TickManager.TicksGame / 7500;
			}
		}
		protected int DaysPassed
		{
			get
			{
				return this.IntervalsPassed / 8;
			}
		}
		protected virtual float MinIncidentChancePopFactor
		{
			get
			{
				return 0.05f;
			}
		}
		public abstract IEnumerable<QueuedIncident> NewIncidentSet();
		public virtual IncidentParms ParmsNow(IncidentCategory incCat)
		{
			return IncidentMakerUtility.DefaultParmsNow(this.Def, incCat);
		}
		protected IEnumerable<IncidentDef> UsableIncidentsOfCategory(IncidentCategory cat)
		{
			return 
				from x in DefDatabase<IncidentDef>.AllDefsListForReading
				where x.category == cat && x.Worker.StorytellerCanUseNow()
				select x;
		}
		public void IncidentMakerTick()
		{
			if (Find.TickManager.TicksGame % 7500 == 5)
			{
				if (!DebugSettings.enableStoryteller)
				{
					return;
				}
				foreach (QueuedIncident current in this.NewIncidentSet())
				{
					if (current.def.minThreatPoints >= 0f && current.parms.points < current.def.minThreatPoints)
					{
						Log.Warning(string.Concat(new object[]
						{
							"Queued ",
							current.def,
							" (from ",
							base.GetType(),
							") with ",
							current.parms.points,
							" threat points when the minimum is ",
							current.def.minThreatPoints
						}));
					}
					this.IncQueue.Add(current);
				}
			}
		}
		protected virtual float IncidentChanceAdjustedForPopulation(IncidentDef def)
		{
			float num = 1f;
			if (def.populationEffect >= IncidentPopulationEffect.Increase)
			{
				num = Find.Storyteller.intenderPopulation.PopulationIntent;
			}
			else
			{
				if (def.populationEffect <= IncidentPopulationEffect.Decrease)
				{
					num = -Find.Storyteller.intenderPopulation.PopulationIntent;
				}
			}
			num = Mathf.Max(num, this.MinIncidentChancePopFactor);
			return Mathf.Max(0f, def.Worker.AdjustedChance * num);
		}
		public virtual void DebugLogIncidentChances(IncidentCategory cat)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetType() + ":");
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"PopulationIntent: ",
				this.Teller.intenderPopulation.PopulationIntent,
				", AdjustedPopulation: ",
				this.Teller.intenderPopulation.AdjustedPopulation
			}));
			foreach (IncidentDef current in 
				from d in this.UsableIncidentsOfCategory(cat)
				orderby this.IncidentChanceAdjustedForPopulation(d) descending
				select d)
			{
				stringBuilder.AppendLine(current.defName.PadRight(25) + this.IncidentChanceAdjustedForPopulation(current).ToString());
			}
			Log.Message(stringBuilder.ToString());
		}
	}
}
