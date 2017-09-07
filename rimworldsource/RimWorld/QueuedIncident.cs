using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class QueuedIncident : IExposable
	{
		public IncidentDef def;
		public int occurTick;
		public IncidentParms parms = new IncidentParms();
		public QueuedIncident()
		{
		}
		public QueuedIncident(IncidentDef def, IncidentParms parms = null)
		{
			this.def = def;
			int num = Mathf.RoundToInt(Rand.Range(0.05f, 0.95f) * 7500f);
			this.occurTick = Find.TickManager.TicksGame + num;
			if (parms != null)
			{
				this.parms = parms;
			}
		}
		public void ExposeData()
		{
			Scribe_Defs.LookDef<IncidentDef>(ref this.def, "def");
			Scribe_Values.LookValue<int>(ref this.occurTick, "occurTick", 0, false);
			Scribe_Deep.LookDeep<IncidentParms>(ref this.parms, "parms", new object[0]);
		}
		public override string ToString()
		{
			string str = this.occurTick.TicksToDays().ToString("#####0.0") + "d";
			string text = this.def.ToString();
			text = text.PadRight(17);
			string text2 = str + ": " + text;
			if (this.parms != null)
			{
				text2 = text2 + " " + this.parms.ToString();
			}
			return text2;
		}
	}
}
