using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class Genstep_RocksFromGrid : Genstep
	{
		private class RoofThreshold
		{
			public RoofDef roofDef;
			public float minGridVal;
		}
		public static ThingDef RockDefAt(IntVec3 c)
		{
			ThingDef thingDef = null;
			float num = -999999f;
			for (int i = 0; i < RockNoises.rockNoises.Count; i++)
			{
				float value = RockNoises.rockNoises[i].noise.GetValue(c);
				if (value > num)
				{
					thingDef = RockNoises.rockNoises[i].rockDef;
					num = value;
				}
			}
			if (thingDef == null)
			{
				Log.ErrorOnce("Did not get rock def to generate at " + c, 50812);
				thingDef = ThingDefOf.Sandstone;
			}
			return thingDef;
		}
		public override void Generate()
		{
			RegionAndRoomUpdater.Enabled = false;
			float num = 0.7f;
			List<Genstep_RocksFromGrid.RoofThreshold> list = new List<Genstep_RocksFromGrid.RoofThreshold>();
			list.Add(new Genstep_RocksFromGrid.RoofThreshold
			{
				roofDef = DefDatabase<RoofDef>.GetNamed("RoofRockThick", true),
				minGridVal = num * 1.14f
			});
			list.Add(new Genstep_RocksFromGrid.RoofThreshold
			{
				roofDef = DefDatabase<RoofDef>.GetNamed("RoofRockThin", true),
				minGridVal = num * 1.04f
			});
			MapGenFloatGrid elevation = MapGenerator.Elevation;
			foreach (IntVec3 current in Find.Map.AllCells)
			{
				float num2 = elevation[current];
				if (num2 > num)
				{
					ThingDef def = Genstep_RocksFromGrid.RockDefAt(current);
					GenSpawn.Spawn(def, current);
					for (int i = 0; i < list.Count; i++)
					{
						if (num2 > list[i].minGridVal)
						{
							Find.RoofGrid.SetRoof(current, list[i].roofDef);
							break;
						}
					}
				}
			}
			Genstep_ScatterMineableBlotches genstep_ScatterMineableBlotches = new Genstep_ScatterMineableBlotches();
			float num3 = 10f;
			switch (Find.World.grid.Get(MapInitData.landingCoords).hilliness)
			{
			case Hilliness.Flat:
				num3 = 4f;
				break;
			case Hilliness.SmallHills:
				num3 = 8f;
				break;
			case Hilliness.LargeHills:
				num3 = 11f;
				break;
			case Hilliness.Mountainous:
				num3 = 15f;
				break;
			}
			genstep_ScatterMineableBlotches.countPer10kCellsRange = new FloatRange(num3, num3);
			genstep_ScatterMineableBlotches.Generate();
			RegionAndRoomUpdater.Enabled = true;
		}
	}
}
