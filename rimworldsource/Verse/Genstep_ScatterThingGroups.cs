using RimWorld;
using System;
namespace Verse
{
	public class Genstep_ScatterThingGroups : Genstep_ScatterThings
	{
		public IntRange groupSizeRange = IntRange.one;
		public int squareRadius = 6;
		protected override void ScatterAt(IntVec3 groupCenter)
		{
			int randomInRange = this.groupSizeRange.RandomInRange;
			for (int i = 0; i < randomInRange; i++)
			{
				IntVec3 loc = CellFinder.RandomClosewalkCellNear(groupCenter, this.squareRadius);
				if (!loc.IsValid)
				{
					Log.Error(string.Concat(new object[]
					{
						"Scattering a group near ",
						groupCenter,
						" but failed to find a place to put the ",
						this.RandomThingDefToScatter,
						"."
					}));
					return;
				}
				Thing thing = base.TrySpawnThingNear(loc);
				if (thing != null)
				{
					thing.SetForbidden(true, false);
				}
			}
		}
	}
}
