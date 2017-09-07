using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Verse;
namespace RimWorld
{
	public class IncidentQueue : IExposable
	{
		private List<QueuedIncident> queuedIncidents = new List<QueuedIncident>();
		public int Count
		{
			get
			{
				return this.queuedIncidents.Count;
			}
		}
		public string DebugQueueReadout
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (QueuedIncident current in this.queuedIncidents)
				{
					stringBuilder.AppendLine(current.ToString() + " (in " + (current.occurTick - Find.TickManager.TicksGame).ToString() + " ticks)");
				}
				return stringBuilder.ToString();
			}
		}
		[DebuggerHidden]
		public IEnumerator GetEnumerator()
		{
			IncidentQueue.<GetEnumerator>c__IteratorAD <GetEnumerator>c__IteratorAD = new IncidentQueue.<GetEnumerator>c__IteratorAD();
			<GetEnumerator>c__IteratorAD.<>f__this = this;
			return <GetEnumerator>c__IteratorAD;
		}
		public void Clear()
		{
			this.queuedIncidents.Clear();
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<QueuedIncident>(ref this.queuedIncidents, "queuedIncidents", LookMode.Deep, new object[0]);
		}
		public void Add(QueuedIncident qi)
		{
			if (!qi.parms.forced && !Find.StoryWatcher.storyState.AllowNewQueuedIncident(qi))
			{
				return;
			}
			Find.StoryWatcher.storyState.RecordDataFromNewQueuedIncident(qi);
			this.queuedIncidents.Add(qi);
			this.queuedIncidents.Sort((QueuedIncident a, QueuedIncident b) => a.occurTick.CompareTo(b.occurTick));
		}
		public void IncidentQueueTick()
		{
			for (int i = this.queuedIncidents.Count - 1; i >= 0; i--)
			{
				QueuedIncident queuedIncident = this.queuedIncidents[i];
				if (queuedIncident.occurTick <= Find.TickManager.TicksGame)
				{
					if (queuedIncident.def.Worker.StorytellerCanUseNow())
					{
						queuedIncident.def.Worker.TryExecute(queuedIncident.parms);
					}
					this.queuedIncidents.Remove(queuedIncident);
				}
			}
		}
	}
}
