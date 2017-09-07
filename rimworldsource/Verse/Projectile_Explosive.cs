using System;
namespace Verse
{
	public class Projectile_Explosive : Projectile
	{
		private int ticksToDetonation;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
		}
		public override void Tick()
		{
			base.Tick();
			if (this.ticksToDetonation > 0)
			{
				this.ticksToDetonation--;
				if (this.ticksToDetonation <= 0)
				{
					this.Explode();
				}
			}
		}
		protected override void Impact(Thing hitThing)
		{
			if (this.def.projectile.explosionDelay == 0)
			{
				this.Explode();
				return;
			}
			this.landed = true;
			this.ticksToDetonation = this.def.projectile.explosionDelay;
		}
		protected virtual void Explode()
		{
			this.Destroy(DestroyMode.Vanish);
			ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
			GenExplosion.DoExplosion(base.Position, this.def.projectile.explosionRadius, this.def.projectile.damageDef, this.launcher, this.def.projectile.soundExplode, this.def, this.equipmentDef, this.def.projectile.postExplosionSpawnThingDef, this.def.projectile.explosionSpawnChance, false, preExplosionSpawnThingDef, this.def.projectile.explosionSpawnChance);
		}
	}
}
