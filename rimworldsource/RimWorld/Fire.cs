using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	public class Fire : AttachableThing, ISizeReporter
	{
		public const float MinFireSize = 0.1f;
		private const float MinSizeForSpark = 1f;
		private const float TicksBetweenSparksBase = 150f;
		private const float TicksBetweenSparksReductionPerFireSize = 40f;
		private const float MinTicksBetweenSparks = 75f;
		private const float MinFireSizeToEmitSpark = 1f;
		private const float MaxFireSize = 1.75f;
		private const int ComplexCalcsInterval = 150;
		private const float CellIgniteChancePerTickPerSize = 0.01f;
		private const float MinSizeForIgniteMovables = 0.4f;
		private const float FireBaseGrowthPerTick = 0.00055f;
		private const int SmokeIntervalRandomAddon = 10;
		private const float BaseSkyExtinguishChance = 0.04f;
		private const int BaseSkyExtinguishDamage = 10;
		private const float HeatPerFireSizePerInterval = 200f;
		private const float HeatFactorWhenDoorPresent = 0.15f;
		private const float SnowClearRadiusPerFireSize = 3f;
		private const float SnowClearDepthFactor = 0.1f;
		private const int FireCountParticlesOff = 15;
		public float fireSize = 0.1f;
		private int ticksSinceSpread;
		private float flammabilityMax = 0.5f;
		private int ticksUntilSmoke;
		private Sustainer sustainer;
		private static List<Thing> flammableList = new List<Thing>();
		private static int fireCount;
		private static int lastFireCountUpdateTick;
		private static readonly IntRange SmokeIntervalRange = new IntRange(130, 200);
		public override string Label
		{
			get
			{
				if (this.parent != null)
				{
					return "FireOn".Translate(new object[]
					{
						this.parent.LabelCap
					});
				}
				return "Fire".Translate();
			}
		}
		public override string InspectStringAddon
		{
			get
			{
				return "Burning".Translate() + " (" + "FireSizeLower".Translate(new object[]
				{
					(this.fireSize * 100f).ToString("F0")
				}) + ")";
			}
		}
		private float SpreadInterval
		{
			get
			{
				float num = 150f - (this.fireSize - 1f) * 40f;
				if (num < 75f)
				{
					num = 75f;
				}
				return num;
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.fireSize, "fireSize", 0f, false);
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.RecalcPathsOnAndAroundMe();
			ConceptDecider.TeachOpportunity(ConceptDefOf.HomeArea, this, OpportunityType.Important);
			this.ticksSinceSpread = (int)(this.SpreadInterval * Rand.Value);
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				SoundDef def = SoundDef.Named("FireBurning");
				SoundInfo info = SoundInfo.InWorld(this, MaintenanceType.PerTick);
				this.sustainer = SustainerAggregatorUtility.AggregateOrSpawnSustainerFor(this, def, info);
			});
		}
		public float CurrentSize()
		{
			return this.fireSize;
		}
		public override void DeSpawn()
		{
			if (this.sustainer != null)
			{
				if (this.sustainer.externalParams.sizeAggregator == null)
				{
					this.sustainer.externalParams.sizeAggregator = new SoundSizeAggregator();
				}
				this.sustainer.externalParams.sizeAggregator.RemoveReporter(this);
			}
			else
			{
				Log.Error("Fire's sustainer is null in DeSpawn.");
			}
			base.DeSpawn();
			this.RecalcPathsOnAndAroundMe();
		}
		private void RecalcPathsOnAndAroundMe()
		{
			for (int i = 0; i < GenAdj.AdjacentCellsAndInside.Length; i++)
			{
				IntVec3 c = base.Position + GenAdj.AdjacentCellsAndInside[i];
				if (c.InBounds())
				{
					Find.PathGrid.RecalculatePerceivedPathCostAt(c);
				}
			}
		}
		public override void AttachTo(Thing parent)
		{
			base.AttachTo(parent);
			Pawn pawn = parent as Pawn;
			if (pawn != null)
			{
				TaleRecorder.RecordTale(TaleDefOf.WasOnFire, new object[]
				{
					pawn
				});
			}
		}
		public override void Tick()
		{
			if (Fire.lastFireCountUpdateTick != Find.TickManager.TicksGame)
			{
				Fire.fireCount = Find.ListerThings.ThingsOfDef(this.def).Count;
				Fire.lastFireCountUpdateTick = Find.TickManager.TicksGame;
			}
			if (this.sustainer != null)
			{
				this.sustainer.Maintain();
			}
			else
			{
				Log.ErrorOnce(string.Concat(new object[]
				{
					"Fire's sustainer is null, which should never happen. Did we try to tick Fire during loading? AnyEventNowOrWaiting: ",
					LongEventHandler.AnyEventNowOrWaiting,
					" Actions waiting to be executed: ",
					LongEventHandler.Debug_GetToExecuteWhenFinishedActionsCount(),
					" Fire is spawned: ",
					base.Spawned,
					" Fire pos: ",
					base.Position
				}), 1837466591);
			}
			this.ticksUntilSmoke--;
			if (this.ticksUntilSmoke <= 0)
			{
				if (Fire.fireCount < 15)
				{
					MoteThrower.ThrowSmoke(this.DrawPos, this.fireSize);
				}
				if (this.fireSize > 0.5f && this.parent == null)
				{
					MoteThrower.ThrowFireGlow(base.Position, this.fireSize);
				}
				float num = this.fireSize / 2f;
				if (num > 1f)
				{
					num = 1f;
				}
				num = 1f - num;
				this.ticksUntilSmoke = Fire.SmokeIntervalRange.Lerped(num) + (int)(10f * Rand.Value);
			}
			if (Fire.fireCount < 15 && this.fireSize > 0.7f && Rand.Value < this.fireSize * 0.01f)
			{
				MoteThrower.ThrowMicroSparks(this.DrawPos);
			}
			if (this.fireSize > 1f)
			{
				this.ticksSinceSpread++;
				if ((float)this.ticksSinceSpread >= this.SpreadInterval)
				{
					this.TrySpread();
					this.ticksSinceSpread = 0;
				}
			}
			if (this.IsHashIntervalTick(150))
			{
				bool flag = false;
				Fire.flammableList.Clear();
				this.flammabilityMax = 0f;
				if (this.parent == null)
				{
					List<Thing> list = Find.ThingGrid.ThingsListAt(base.Position);
					for (int i = 0; i < list.Count; i++)
					{
						Thing thing = list[i];
						if (thing is Building_Door)
						{
							flag = true;
						}
						float statValue = thing.GetStatValue(StatDefOf.Flammability, true);
						if (statValue >= 0.01f)
						{
							Fire.flammableList.Add(list[i]);
							if (statValue > this.flammabilityMax)
							{
								this.flammabilityMax = statValue;
							}
							if (this.parent == null && this.fireSize > 0.4f && list[i].def.category == ThingCategory.Pawn)
							{
								list[i].TryAttachFire(this.fireSize * 0.2f);
							}
						}
					}
				}
				else
				{
					Fire.flammableList.Add(this.parent);
					this.flammabilityMax = this.parent.GetStatValue(StatDefOf.Flammability, true);
				}
				if (this.flammabilityMax < 0.01f)
				{
					this.Destroy(DestroyMode.Vanish);
					return;
				}
				Thing thing2;
				if (this.parent != null)
				{
					thing2 = this.parent;
				}
				else
				{
					if (Fire.flammableList.Count > 0)
					{
						thing2 = Fire.flammableList.RandomElement<Thing>();
					}
					else
					{
						thing2 = null;
					}
				}
				if (thing2 != null && (this.fireSize >= 0.4f || thing2 == this.parent || thing2.def.category != ThingCategory.Pawn))
				{
					this.DoFireDamage(thing2);
				}
				float num2 = this.fireSize * 200f;
				if (flag)
				{
					num2 *= 0.15f;
				}
				GenTemperature.PushHeat(base.Position, num2);
				if (Rand.Value < 0.4f)
				{
					float radius = this.fireSize * 3f;
					SnowUtility.AddSnowRadial(base.Position, radius, -(this.fireSize * 0.1f));
				}
				this.fireSize += 0.00055f * this.flammabilityMax * 150f;
				if (this.fireSize > 1.75f)
				{
					this.fireSize = 1.75f;
				}
				if (Find.WeatherManager.RainRate > 0.01f && this.VulnerableToRain() && Rand.Value < 6f)
				{
					base.TakeDamage(new DamageInfo(DamageDefOf.Extinguish, 10, null, null, null));
				}
			}
		}
		private bool VulnerableToRain()
		{
			RoofDef roofDef = Find.RoofGrid.RoofAt(base.Position);
			if (roofDef == null)
			{
				return true;
			}
			if (roofDef.isThickRoof)
			{
				return false;
			}
			Thing edifice = base.Position.GetEdifice();
			return edifice != null && edifice.def.holdsRoof;
		}
		private void DoFireDamage(Thing targ)
		{
			float num = 0.0125f + 0.0036f * this.fireSize;
			num = Mathf.Clamp(num, 0.0125f, 0.05f);
			int num2 = GenMath.RoundRandom(num * 150f);
			if (num2 < 1)
			{
				num2 = 1;
			}
			Pawn pawn = targ as Pawn;
			if (pawn != null)
			{
				BodyPartDamageInfo value = new BodyPartDamageInfo(null, new BodyPartDepth?(BodyPartDepth.Outside));
				targ.TakeDamage(new DamageInfo(DamageDefOf.Flame, num2, this, new BodyPartDamageInfo?(value), null));
				Apparel apparel;
				if (pawn.apparel != null && pawn.apparel.WornApparel.TryRandomElement(out apparel))
				{
					apparel.TakeDamage(new DamageInfo(DamageDefOf.Flame, num2, this, null, null));
				}
			}
			else
			{
				targ.TakeDamage(new DamageInfo(DamageDefOf.Flame, num2, this, null, null));
			}
		}
		public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			if (!base.Destroyed && dinfo.Def == DamageDefOf.Extinguish)
			{
				this.fireSize -= (float)dinfo.Amount / 100f;
				if (this.fireSize <= 0.1f)
				{
					this.Destroy(DestroyMode.Vanish);
					return;
				}
			}
		}
		protected void TrySpread()
		{
			IntVec3 intVec = base.Position;
			bool flag;
			if (Rand.Value < 0.8f)
			{
				intVec = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(1, 8)];
				flag = true;
			}
			else
			{
				intVec = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(10, 20)];
				flag = false;
			}
			if (!intVec.InBounds())
			{
				return;
			}
			if (!flag)
			{
				List<Thing> thingList = intVec.GetThingList();
				bool flag2 = false;
				for (int i = 0; i < thingList.Count; i++)
				{
					if (thingList[i] is Fire)
					{
						return;
					}
					if (thingList[i].FlammableNow)
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					return;
				}
				if (!FireUtility.FireCanExistIn(intVec))
				{
					return;
				}
				CellRect startRect = CellRect.SingleCell(base.Position);
				CellRect endRect = CellRect.SingleCell(intVec);
				if (!GenSight.LineOfSight(base.Position, intVec, startRect, endRect))
				{
					return;
				}
				Spark spark = (Spark)GenSpawn.Spawn(ThingDefOf.Spark, base.Position);
				spark.Launch(this, intVec, null);
			}
			else
			{
				FireUtility.TryStartFireIn(intVec, 0.1f);
			}
		}
	}
}
