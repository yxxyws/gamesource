using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_ShipChunkDrop : IncidentWorker
	{
		private static readonly Pair<int, float>[] CountChance = new Pair<int, float>[]
		{
			new Pair<int, float>(1, 1f),
			new Pair<int, float>(2, 0.95f),
			new Pair<int, float>(3, 0.7f),
			new Pair<int, float>(4, 0.4f)
		};
		private int RandomCountToDrop
		{
			get
			{
				float x2 = (float)Find.TickManager.TicksGame / 3600000f;
				float timePassedFactor = Mathf.Clamp(GenMath.LerpDouble(0f, 1.2f, 1f, 0.1f, x2), 0.1f, 1f);
				return IncidentWorker_ShipChunkDrop.CountChance.RandomElementByWeight(delegate(Pair<int, float> x)
				{
					if (x.First == 1)
					{
						return x.Second;
					}
					return x.Second * timePassedFactor;
				}).First;
			}
		}
		public override bool TryExecute(IncidentParms parms)
		{
			IntVec3 intVec;
			if (!ShipChunkDropCellFinder.TryFindShipChunkDropCell(out intVec, Find.Map.Center, 999999))
			{
				return false;
			}
			this.SpawnShipChunks(intVec, this.RandomCountToDrop);
			Messages.Message("MessageShipChunkDrop".Translate(), intVec, MessageSound.Standard);
			return true;
		}
		private void SpawnShipChunks(IntVec3 firstChunkPos, int count)
		{
			GenSpawn.Spawn(ThingDefOf.ShipChunk, firstChunkPos);
			for (int i = 0; i < count - 1; i++)
			{
				IntVec3 loc;
				if (ShipChunkDropCellFinder.TryFindShipChunkDropCell(out loc, firstChunkPos, 5))
				{
					GenSpawn.Spawn(ThingDefOf.ShipChunk, loc);
				}
			}
		}
	}
}
