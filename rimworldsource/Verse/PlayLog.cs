using System;
using System.Collections.Generic;
namespace Verse
{
	public class PlayLog : IExposable
	{
		private const int Capacity = 150;
		private List<PlayLogEntry> entries = new List<PlayLogEntry>();
		public List<PlayLogEntry> AllEntries
		{
			get
			{
				return this.entries;
			}
		}
		public void Add(PlayLogEntry entry)
		{
			this.entries.Insert(0, entry);
			this.ReduceToCapacity();
		}
		private void ReduceToCapacity()
		{
			if (this.entries.Count > 150)
			{
				this.entries.RemoveRange(150, this.entries.Count - 150);
			}
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<PlayLogEntry>(ref this.entries, "entries", LookMode.Deep, new object[0]);
		}
		public void Notify_PawnDiscarded(Pawn p)
		{
			for (int i = this.entries.Count - 1; i >= 0; i--)
			{
				if (this.entries[i].Concerns(p))
				{
					this.entries.RemoveAt(i);
				}
			}
		}
	}
}
