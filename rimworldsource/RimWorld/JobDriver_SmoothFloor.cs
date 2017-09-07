using System;
using Verse;
namespace RimWorld
{
	public class JobDriver_SmoothFloor : JobDriver_AffectFloor
	{
		protected override int BaseWorkAmount
		{
			get
			{
				return 1200;
			}
		}
		protected override DesignationDef DesDef
		{
			get
			{
				return DesignationDefOf.SmoothFloor;
			}
		}
		protected override StatDef SpeedStat
		{
			get
			{
				return StatDefOf.SmoothingSpeed;
			}
		}
		protected override void DoEffect(IntVec3 c)
		{
			TerrainDef smoothedTerrain = base.TargetLocA.GetTerrain().smoothedTerrain;
			Find.TerrainGrid.SetTerrain(base.TargetLocA, smoothedTerrain);
		}
	}
}
