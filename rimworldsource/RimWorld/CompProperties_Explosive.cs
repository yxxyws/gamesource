using System;
using Verse;
namespace RimWorld
{
	public class CompProperties_Explosive : CompProperties
	{
		public float explosiveRadius = 1.9f;
		public DamageDef explosiveDamageType = DamageDefOf.Bomb;
		public ThingDef postExplosionSpawnThingDef;
		public float postExplosionSpawnChance;
		public bool applyDamageToExplosionCellsNeighbors;
		public ThingDef preExplosionSpawnThingDef;
		public float preExplosionSpawnChance;
		public float explosiveExpandPerStackcount;
		public EffecterDef explosionEffect;
		public DamageDef startWickOnDamageTaken;
		public float startWickHitPointsPercent = 0.2f;
		public IntRange wickTicks = new IntRange(140, 150);
		public float wickScale = 1f;
		public CompProperties_Explosive()
		{
			this.compClass = typeof(CompExplosive);
		}
	}
}
