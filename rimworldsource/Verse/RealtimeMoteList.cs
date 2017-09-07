using System;
using System.Collections.Generic;
namespace Verse
{
	public class RealtimeMoteList
	{
		private List<Mote> allMotes = new List<Mote>();
		public void MoteSpawned(Mote newMote)
		{
			this.allMotes.Add(newMote);
		}
		public void MoteDespawned(Mote oldMote)
		{
			this.allMotes.Remove(oldMote);
		}
		public void MoteListUpdate()
		{
			for (int i = this.allMotes.Count - 1; i >= 0; i--)
			{
				this.allMotes[i].RealtimeUpdate();
			}
		}
	}
}
