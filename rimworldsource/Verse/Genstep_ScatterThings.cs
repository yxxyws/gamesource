using RimWorld;
using System;
using System.Collections.Generic;
namespace Verse
{
	public class Genstep_ScatterThings : Genstep_Scatterer
	{
		public int clearSpaceSize;
		public List<ThingDef> thingDefs = new List<ThingDef>();
		public IntRange stackCountRange = IntRange.one;
		public TerrainAffordance neededSurfaceType;
		protected virtual ThingDef RandomThingDefToScatter
		{
			get
			{
				return this.thingDefs.RandomElement<ThingDef>();
			}
		}
		protected override void ScatterAt(IntVec3 loc)
		{
			if (this.clearSpaceSize > 0)
			{
				foreach (IntVec3 current in GridShapeMaker.IrregularLump(loc, this.clearSpaceSize))
				{
					Thing edifice = current.GetEdifice();
					if (edifice != null)
					{
						edifice.Destroy(DestroyMode.Vanish);
					}
				}
			}
			this.TrySpawnThingNear(loc);
		}
		protected Thing TrySpawnThingNear(IntVec3 loc)
		{
			ThingDef randomThingDefToScatter = this.RandomThingDefToScatter;
			if (randomThingDefToScatter == null)
			{
				Log.Error("No defs to scatter for " + base.GetType() + ".");
				return null;
			}
			Rot4 rotation = (!randomThingDefToScatter.rotatable) ? Rot4.North : Rot4.Random;
			if (this.neededSurfaceType != TerrainAffordance.Any)
			{
				foreach (IntVec3 current in GenAdj.CellsOccupiedBy(loc, rotation, randomThingDefToScatter.size))
				{
					if (!current.SupportsStructureType(this.neededSurfaceType))
					{
						return null;
					}
				}
			}
			Thing thing = ThingMaker.MakeThing(randomThingDefToScatter, null);
			thing.Rotation = rotation;
			thing.stackCount = this.stackCountRange.RandomInRange;
			GenPlace.TryPlaceThing(thing, loc, ThingPlaceMode.Near);
			return thing;
		}
	}
}
