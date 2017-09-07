using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse.Sound;
namespace Verse
{
	public class Explosion : Thing
	{
		public float radius;
		public DamageDef damType;
		public int damAmount;
		public Thing instigator;
		public ThingDef source;
		public bool applyDamageToExplosionCellsNeighbors;
		public ThingDef preExplosionSpawnThingDef;
		public float preExplosionSpawnChance;
		public ThingDef postExplosionSpawnThingDef;
		public float postExplosionSpawnChance;
		private int startTick;
		private List<IntVec3> cellsToAffect;
		private List<Thing> damagedThings;
		private HashSet<IntVec3> addedCellsAffectedOnlyByDamage;
		private static HashSet<IntVec3> tmpCells = new HashSet<IntVec3>();
		public virtual void ExplosionStart(SoundDef explosionSound)
		{
			if (!base.Spawned)
			{
				Log.Warning("Tried to start explosion on unspawned explosion.");
				return;
			}
			this.startTick = Find.TickManager.TicksGame;
			this.cellsToAffect = SimplePool<List<IntVec3>>.Get();
			this.cellsToAffect.Clear();
			this.damagedThings = SimplePool<List<Thing>>.Get();
			this.damagedThings.Clear();
			this.addedCellsAffectedOnlyByDamage = SimplePool<HashSet<IntVec3>>.Get();
			this.addedCellsAffectedOnlyByDamage.Clear();
			this.cellsToAffect.AddRange(this.damType.Worker.ExplosionCellsToHit(this));
			if (this.applyDamageToExplosionCellsNeighbors)
			{
				this.AddCellsNeighbors(this.cellsToAffect);
			}
			this.damType.Worker.ExplosionStart(this, this.cellsToAffect);
			this.PlayExplosionSound(explosionSound);
			this.cellsToAffect.Sort((IntVec3 a, IntVec3 b) => this.GetCellAffectTick(b).CompareTo(this.GetCellAffectTick(a)));
		}
		public override void Tick()
		{
			int ticksGame = Find.TickManager.TicksGame;
			int count = this.cellsToAffect.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				if (ticksGame < this.GetCellAffectTick(this.cellsToAffect[i]))
				{
					break;
				}
				this.AffectCell(this.cellsToAffect[i]);
				this.cellsToAffect.RemoveAt(i);
			}
			if (!this.cellsToAffect.Any<IntVec3>())
			{
				this.Destroy(DestroyMode.Vanish);
			}
		}
		public override void DeSpawn()
		{
			this.cellsToAffect.Clear();
			SimplePool<List<IntVec3>>.Return(this.cellsToAffect);
			this.cellsToAffect = null;
			this.damagedThings.Clear();
			SimplePool<List<Thing>>.Return(this.damagedThings);
			this.damagedThings = null;
			this.addedCellsAffectedOnlyByDamage.Clear();
			SimplePool<HashSet<IntVec3>>.Return(this.addedCellsAffectedOnlyByDamage);
			this.addedCellsAffectedOnlyByDamage = null;
			base.DeSpawn();
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.radius, "radius", 0f, false);
			Scribe_Defs.LookDef<DamageDef>(ref this.damType, "damType");
			Scribe_Values.LookValue<int>(ref this.damAmount, "damAmount", 0, false);
			Scribe_References.LookReference<Thing>(ref this.instigator, "instigator", false);
			Scribe_Defs.LookDef<ThingDef>(ref this.source, "source");
			Scribe_Values.LookValue<bool>(ref this.applyDamageToExplosionCellsNeighbors, "applyDamageToExplosionCellsNeighbors", false, false);
			Scribe_Defs.LookDef<ThingDef>(ref this.preExplosionSpawnThingDef, "preExplosionSpawnThingDef");
			Scribe_Values.LookValue<float>(ref this.preExplosionSpawnChance, "preExplosionSpawnChance", 0f, false);
			Scribe_Defs.LookDef<ThingDef>(ref this.postExplosionSpawnThingDef, "postExplosionSpawnThingDef");
			Scribe_Values.LookValue<float>(ref this.postExplosionSpawnChance, "postExplosionSpawnChance", 0f, false);
			Scribe_Values.LookValue<int>(ref this.startTick, "startTick", 0, false);
			Scribe_Collections.LookList<IntVec3>(ref this.cellsToAffect, "cellsToAffect", LookMode.Value, new object[0]);
			Scribe_Collections.LookList<Thing>(ref this.damagedThings, "damagedThings", LookMode.MapReference, new object[0]);
			Scribe_Collections.LookHashSet<IntVec3>(ref this.addedCellsAffectedOnlyByDamage, "addedCellsAffectedOnlyByDamage", LookMode.Value);
		}
		private int GetCellAffectTick(IntVec3 cell)
		{
			return this.startTick + (int)((cell - base.Position).LengthHorizontal * 1.5f);
		}
		private void AffectCell(IntVec3 c)
		{
			bool flag = this.ShouldCellBeAffectedOnlyByDamage(c);
			if (!flag && c.Walkable() && Rand.Value < this.preExplosionSpawnChance)
			{
				this.TrySpawnExplosionThing(this.preExplosionSpawnThingDef, c);
			}
			this.damType.Worker.ExplosionAffectCell(this, c, this.damagedThings, !flag);
			if (!flag && c.Walkable() && Rand.Value < this.postExplosionSpawnChance)
			{
				this.TrySpawnExplosionThing(this.postExplosionSpawnThingDef, c);
			}
		}
		private void TrySpawnExplosionThing(ThingDef thingDef, IntVec3 c)
		{
			if (thingDef == null)
			{
				return;
			}
			if (thingDef.thingClass == typeof(LiquidFuel))
			{
				LiquidFuel liquidFuel = (LiquidFuel)Find.ThingGrid.ThingAt(c, thingDef);
				if (liquidFuel != null)
				{
					liquidFuel.Refill();
					return;
				}
			}
			GenSpawn.Spawn(thingDef, c);
		}
		private void PlayExplosionSound(SoundDef explosionSound)
		{
			bool flag;
			if (Prefs.DevMode)
			{
				flag = (explosionSound != null);
			}
			else
			{
				flag = !explosionSound.NullOrUndefined();
			}
			if (flag)
			{
				explosionSound.PlayOneShot(base.Position);
			}
			else
			{
				this.damType.soundExplosion.PlayOneShot(base.Position);
			}
		}
		private void AddCellsNeighbors(List<IntVec3> cells)
		{
			Explosion.tmpCells.Clear();
			this.addedCellsAffectedOnlyByDamage.Clear();
			for (int i = 0; i < cells.Count; i++)
			{
				Explosion.tmpCells.Add(cells[i]);
			}
			for (int j = 0; j < cells.Count; j++)
			{
				if (cells[j].Walkable())
				{
					for (int k = 0; k < GenAdj.AdjacentCells.Length; k++)
					{
						IntVec3 intVec = cells[j] + GenAdj.AdjacentCells[k];
						if (intVec.InBounds())
						{
							bool flag = Explosion.tmpCells.Add(intVec);
							if (flag)
							{
								this.addedCellsAffectedOnlyByDamage.Add(intVec);
							}
						}
					}
				}
			}
			cells.Clear();
			foreach (IntVec3 current in Explosion.tmpCells)
			{
				cells.Add(current);
			}
			Explosion.tmpCells.Clear();
		}
		private bool ShouldCellBeAffectedOnlyByDamage(IntVec3 c)
		{
			return this.applyDamageToExplosionCellsNeighbors && this.addedCellsAffectedOnlyByDamage.Contains(c);
		}
	}
}
