using System;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class Genstep_ScatterMineableBlotches : Genstep_ScatterThings
	{
		protected override ThingDef RandomThingDefToScatter
		{
			get
			{
				return DefDatabase<ThingDef>.AllDefs.RandomElementByWeight(delegate(ThingDef def)
				{
					if (def.building == null)
					{
						return 0f;
					}
					return def.building.mineableScatterCommonality;
				});
			}
		}
		public override void Generate()
		{
			this.minSpacing = 5f;
			this.warnOnFail = false;
			base.Generate();
		}
		protected override bool CanScatterAt(IntVec3 c)
		{
			foreach (IntVec3 current in this.usedSpots)
			{
				if ((current - c).LengthHorizontal < this.minSpacing)
				{
					return false;
				}
			}
			return Find.ThingGrid.ThingsListAt(c).Any((Thing t) => t.def.building != null && t.def.building.isNaturalRock);
		}
		protected override void ScatterAt(IntVec3 c)
		{
			ThingDef randomThingDefToScatter = this.RandomThingDefToScatter;
			int randomInRange = randomThingDefToScatter.building.mineableScatterBlotchSizeRange.RandomInRange;
			foreach (IntVec3 current in GridShapeMaker.IrregularLump(c, randomInRange))
			{
				GenSpawn.Spawn(randomThingDefToScatter, current);
			}
		}
	}
}
