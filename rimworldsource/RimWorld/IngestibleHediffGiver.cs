using System;
using Verse;
namespace RimWorld
{
	public class IngestibleHediffGiver
	{
		public HediffDef hediffDef;
		public float chance = 1f;
		public virtual void CheckGiveHediff(Pawn pawn, Thing ingested, int numTaken)
		{
			if (Rand.Value < this.chance)
			{
				for (int i = 0; i < numTaken; i++)
				{
					pawn.health.AddHediff(this.hediffDef, null, null);
				}
			}
		}
	}
}
