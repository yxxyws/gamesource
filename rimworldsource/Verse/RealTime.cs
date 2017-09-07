using System;
using UnityEngine;
namespace Verse
{
	public class RealTime : IExposable
	{
		private float lastRealTime;
		public float realDeltaTime;
		public RealtimeMoteList moteList = new RealtimeMoteList();
		public float timeUnpaused;
		public void ExposeData()
		{
			Scribe_Values.LookValue<float>(ref this.timeUnpaused, "timeUnpaused", 0f, false);
		}
		public void Update()
		{
			this.realDeltaTime = Time.realtimeSinceStartup - this.lastRealTime;
			this.lastRealTime = Time.realtimeSinceStartup;
			this.moteList.MoteListUpdate();
			if (Game.Mode == GameMode.MapPlaying && !Find.TickManager.Paused)
			{
				this.timeUnpaused += Time.deltaTime;
			}
		}
	}
}
