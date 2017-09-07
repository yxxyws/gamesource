using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class StoryState : IExposable
	{
		private int lastThreatBigQueueTick = -1;
		private Dictionary<IncidentDef, int> lastQueueTicks = new Dictionary<IncidentDef, int>();
		public int LastThreatBigQueueTick
		{
			get
			{
				if (this.lastThreatBigQueueTick > Find.TickManager.TicksGame + 7500)
				{
					Log.Error(string.Concat(new object[]
					{
						"Latest big threat queue time was ",
						this.lastThreatBigQueueTick,
						" at tick ",
						Find.TickManager.TicksGame,
						". This is too far in the future. Resetting."
					}));
					this.lastThreatBigQueueTick = Find.TickManager.TicksGame - 1;
				}
				return this.lastThreatBigQueueTick;
			}
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.lastThreatBigQueueTick, "lastThreatBigQueueTick", 0, true);
			Scribe_Collections.LookDictionary<IncidentDef, int>(ref this.lastQueueTicks, "lastQueueTicks", LookMode.DefReference, LookMode.Value);
		}
		public bool AllowNewQueuedIncident(QueuedIncident qi)
		{
			bool flag = true;
			int num;
			if (this.lastQueueTicks.TryGetValue(qi.def, out num))
			{
				float num2 = (float)(qi.occurTick - num) / 60000f;
				if (num2 < (float)qi.def.minRefireDays)
				{
					flag = false;
				}
			}
			if (flag)
			{
				List<IncidentDef> refireCheckIncidents = qi.def.RefireCheckIncidents;
				if (refireCheckIncidents != null)
				{
					for (int i = 0; i < refireCheckIncidents.Count; i++)
					{
						if (this.lastQueueTicks.TryGetValue(refireCheckIncidents[i], out num))
						{
							float num3 = (float)(qi.occurTick - num) / 60000f;
							if (num3 < (float)qi.def.minRefireDays)
							{
								flag = false;
								break;
							}
						}
					}
				}
			}
			return flag;
		}
		public void RecordDataFromNewQueuedIncident(QueuedIncident qi)
		{
			if (qi.parms.forced)
			{
				return;
			}
			if (qi.def.category == IncidentCategory.ThreatBig)
			{
				if (this.lastThreatBigQueueTick <= qi.occurTick)
				{
					this.lastThreatBigQueueTick = qi.occurTick;
				}
				else
				{
					Log.Error("Queueing threats backwards in time (" + qi + ")");
				}
				Find.StoryWatcher.statsRecord.numThreatBigs++;
			}
			if (this.lastQueueTicks.ContainsKey(qi.def))
			{
				this.lastQueueTicks[qi.def] = qi.occurTick;
			}
			else
			{
				this.lastQueueTicks.Add(qi.def, qi.occurTick);
			}
		}
	}
}
