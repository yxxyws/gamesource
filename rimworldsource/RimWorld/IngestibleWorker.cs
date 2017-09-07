using System;
using Verse;
namespace RimWorld
{
	public class IngestibleWorker
	{
		public ThingDef def;
		public void IngestedBy(Pawn ingester, Thing ingested, int numTaken)
		{
			if (this.def.ingestible.hediffGivers != null)
			{
				for (int i = 0; i < this.def.ingestible.hediffGivers.Count; i++)
				{
					this.def.ingestible.hediffGivers[i].CheckGiveHediff(ingester, ingested, numTaken);
				}
			}
			if (this.def.ingestible.isPleasureDrug)
			{
				ingester.mindState.lastPleasureDrugUseTick = Find.TickManager.TicksGame;
			}
		}
	}
}
