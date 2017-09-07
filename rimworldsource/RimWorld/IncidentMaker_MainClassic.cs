using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class IncidentMaker_MainClassic : IncidentMaker
	{
		[DebuggerHidden]
		public override IEnumerable<QueuedIncident> NewIncidentSet()
		{
			IncidentMaker_MainClassic.<NewIncidentSet>c__IteratorAA <NewIncidentSet>c__IteratorAA = new IncidentMaker_MainClassic.<NewIncidentSet>c__IteratorAA();
			<NewIncidentSet>c__IteratorAA.<>f__this = this;
			IncidentMaker_MainClassic.<NewIncidentSet>c__IteratorAA expr_0E = <NewIncidentSet>c__IteratorAA;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private QueuedIncident QueuedThreatBig()
		{
			IncidentDef incidentDef;
			if (Find.StoryWatcher.statsRecord.numRaidsEnemy <= 4)
			{
				incidentDef = DefDatabase<IncidentDef>.GetNamed("RaidEnemy", true);
			}
			else
			{
				incidentDef = (
					from def in DefDatabase<IncidentDef>.AllDefs
					where def.category == IncidentCategory.ThreatBig
					select def).RandomElementByWeight(new Func<IncidentDef, float>(this.IncidentChanceAdjustedForPopulation));
			}
			return new QueuedIncident(incidentDef, null)
			{
				parms = this.ParmsNow(incidentDef.category)
			};
		}
		private QueuedIncident QueuedThreatSmall()
		{
			IncidentDef incidentDef = (
				from def in DefDatabase<IncidentDef>.AllDefs
				where def.category == IncidentCategory.ThreatSmall
				select def).RandomElementByWeight(new Func<IncidentDef, float>(this.IncidentChanceAdjustedForPopulation));
			return new QueuedIncident(incidentDef, null)
			{
				parms = this.ParmsNow(incidentDef.category)
			};
		}
		public void ForceQueueThreatBig()
		{
			base.IncQueue.Add(this.QueuedThreatBig());
		}
	}
}
