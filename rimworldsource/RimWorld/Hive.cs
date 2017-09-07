using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class Hive : ThingWithComps
	{
		public const int MaxHivesPerMap = 30;
		private const float HiveSpawnPreferredMinDist = 3.5f;
		private const float HiveSpawnRadius = 10f;
		private const int PawnSpawnRadius = 5;
		private const float MaxSpawnedPawnsPoints = 500f;
		private const int InitialPawnsPoints = 260;
		private const int InitialFilthCount = 10;
		private const float FilthSpawnMtbHours = 4f;
		private const float FilthSpawnRadius = 5f;
		private const int NoLongerAngryAfterTicks = 500;
		public bool active = true;
		private int nextHiveSpawnTick = -1;
		public int nextPawnSpawnTick = -1;
		private List<Pawn> spawnedPawns = new List<Pawn>();
		private int lastHarmedTick = -9999;
		private int ticksToSpawnInitialPawns = -1;
		private static readonly FloatRange HiveSpawnIntervalDays = new FloatRange(1.6f, 2.1f);
		private static readonly FloatRange PawnSpawnIntervalDays = new FloatRange(0.85f, 1.1f);
		public bool Dangerous
		{
			get
			{
				bool arg_3C_0;
				if (this.spawnedPawns.Count > 0)
				{
					arg_3C_0 = this.spawnedPawns.Any((Pawn p) => !p.Downed);
				}
				else
				{
					arg_3C_0 = false;
				}
				return arg_3C_0;
			}
		}
		public bool AngryNow
		{
			get
			{
				return Find.TickManager.TicksGame < this.lastHarmedTick + 500;
			}
		}
		private float SpawnedPawnsPoints
		{
			get
			{
				this.FilterOutUnspawnedPawns();
				float num = 0f;
				for (int i = 0; i < this.spawnedPawns.Count; i++)
				{
					num += this.spawnedPawns[i].kindDef.combatPower;
				}
				return num;
			}
		}
		private bool CanSpawnChildHive
		{
			get
			{
				return HivesUtility.TotalSpawnedHivesCount < 30;
			}
		}
		public void StartInitialPawnSpawnCountdown()
		{
			this.ticksToSpawnInitialPawns = 720;
		}
		private void SpawnInitialPawnsNow()
		{
			this.ticksToSpawnInitialPawns = -1;
			while (this.SpawnedPawnsPoints < 260f)
			{
				Pawn pawn;
				if (!this.TrySpawnPawn(out pawn))
				{
					return;
				}
			}
		}
		public void SpawnInitialFilth()
		{
			for (int i = 0; i < 10; i++)
			{
				this.TrySpawnSlimeFilth();
			}
		}
		public override void TickRare()
		{
			base.TickRare();
			this.FilterOutUnspawnedPawns();
			if (!this.active && !base.Position.Fogged())
			{
				this.Activate();
			}
			if (this.active)
			{
				if (this.ticksToSpawnInitialPawns > 0)
				{
					this.ticksToSpawnInitialPawns -= 250;
					if (this.ticksToSpawnInitialPawns <= 0)
					{
						this.SpawnInitialPawnsNow();
					}
				}
				if (this.nextHiveSpawnTick < 0)
				{
					this.CalculateNextHiveSpawnTick();
				}
				Hive hive;
				if (Find.TickManager.TicksGame >= this.nextHiveSpawnTick && this.TrySpawnChildHive(false, out hive))
				{
					hive.nextPawnSpawnTick = Find.TickManager.TicksGame + Rand.Range(150, 350);
					Messages.Message("MessageHiveReproduced".Translate(), hive, MessageSound.Negative);
				}
				if (this.nextPawnSpawnTick < 0)
				{
					this.CalculateNextPawnSpawnTick();
				}
				if (Find.TickManager.TicksGame >= this.nextPawnSpawnTick && this.SpawnedPawnsPoints < 500f)
				{
					Pawn pawn;
					bool flag = this.TrySpawnPawn(out pawn);
					if (flag && pawn.caller != null)
					{
						pawn.caller.DoCall();
					}
				}
				if (Rand.MTBEventOccurs(4f, 2500f, 250f))
				{
					this.TrySpawnSlimeFilth();
				}
			}
		}
		public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			if (dinfo.Def.externalViolence && dinfo.Instigator != null)
			{
				if (this.ticksToSpawnInitialPawns > 0)
				{
					this.SpawnInitialPawnsNow();
				}
				this.Notify_HiveOrPawnAttacked(dinfo.Instigator);
			}
			base.PostApplyDamage(dinfo, totalDamageDealt);
		}
		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			if (this.CanSpawnChildHive)
			{
				text = text + "HiveReproducesIn".Translate() + ": ";
				if (this.nextHiveSpawnTick < 0)
				{
					this.CalculateNextHiveSpawnTick();
				}
				int numTicks = this.nextHiveSpawnTick - Find.TickManager.TicksGame;
				text += numTicks.TicksToPeriodString(true);
			}
			return text;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<bool>(ref this.active, "active", false, false);
			Scribe_Values.LookValue<int>(ref this.nextHiveSpawnTick, "nextHiveSpawnTick", 0, false);
			Scribe_Values.LookValue<int>(ref this.nextPawnSpawnTick, "nextPawnSpawnTick", 0, false);
			Scribe_Collections.LookList<Pawn>(ref this.spawnedPawns, "spawnedPawns", LookMode.MapReference, new object[0]);
			Scribe_Values.LookValue<int>(ref this.lastHarmedTick, "lastHarmedTick", 0, false);
			Scribe_Values.LookValue<int>(ref this.ticksToSpawnInitialPawns, "ticksToSpawnInitialPawns", 0, false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.spawnedPawns.RemoveAll((Pawn x) => x == null);
			}
		}
		public void Notify_HiveOrPawnAttacked(Thing attacker)
		{
			this.lastHarmedTick = Find.TickManager.TicksGame;
		}
		private void Activate()
		{
			this.active = true;
			this.nextPawnSpawnTick = Find.TickManager.TicksGame + Rand.Range(200, 400);
		}
		private void CalculateNextHiveSpawnTick()
		{
			this.nextHiveSpawnTick = Find.TickManager.TicksGame + (int)(Hive.HiveSpawnIntervalDays.RandomInRange * 60000f);
		}
		private void CalculateNextPawnSpawnTick()
		{
			this.nextPawnSpawnTick = Find.TickManager.TicksGame + (int)(Hive.PawnSpawnIntervalDays.RandomInRange * 60000f);
		}
		private void FilterOutUnspawnedPawns()
		{
			this.spawnedPawns.RemoveAll((Pawn x) => !x.Spawned);
		}
		public bool TrySpawnChildHive(bool ignoreRoofedRequirement, out Hive newHive)
		{
			if (!this.CanSpawnChildHive)
			{
				newHive = null;
				return false;
			}
			this.CalculateNextHiveSpawnTick();
			IntVec3 invalid = IntVec3.Invalid;
			for (int i = 0; i < 3; i++)
			{
				float minDist = 3.5f;
				if (i == 1)
				{
					minDist = 0f;
				}
				else
				{
					if (i == 2)
					{
						newHive = null;
						return false;
					}
				}
				if (CellFinder.TryFindRandomReachableCellNear(base.Position, 10f, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), delegate(IntVec3 c)
				{
					if ((!ignoreRoofedRequirement && !c.Roofed()) || !c.Standable() || (minDist != 0f && c.DistanceToSquared(this.Position) < minDist * minDist))
					{
						return false;
					}
					for (int j = 0; j < 8; j++)
					{
						IntVec3 c2 = c + GenAdj.AdjacentCells[j];
						List<Thing> thingList = c2.GetThingList();
						for (int k = 0; k < thingList.Count; k++)
						{
							if (thingList[k] is Hive)
							{
								return false;
							}
						}
					}
					return true;
				}, null, out invalid, 999999))
				{
					break;
				}
			}
			newHive = (Hive)GenSpawn.Spawn(this.def, invalid);
			if (newHive.Faction != base.Faction)
			{
				newHive.SetFaction(base.Faction, null);
			}
			newHive.active = this.active;
			return true;
		}
		private bool TrySpawnPawn(out Pawn pawn)
		{
			this.CalculateNextPawnSpawnTick();
			List<PawnKindDef> list = new List<PawnKindDef>();
			list.Add(PawnKindDefOf.Megascarab);
			list.Add(PawnKindDefOf.Spelopede);
			list.Add(PawnKindDefOf.Megaspider);
			float curPoints = this.SpawnedPawnsPoints;
			IEnumerable<PawnKindDef> source = 
				from x in list
				where curPoints + x.combatPower <= 500f
				select x;
			PawnKindDef kindDef;
			if (!source.TryRandomElement(out kindDef))
			{
				pawn = null;
				return false;
			}
			pawn = PawnGenerator.GeneratePawn(kindDef, base.Faction);
			GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(base.Position, 5));
			pawn.mindState.hive = this;
			this.spawnedPawns.Add(pawn);
			return true;
		}
		public void TrySpawnSlimeFilth()
		{
			IntVec3 c;
			if (!CellFinder.TryFindRandomReachableCellNear(base.Position, 5f, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), (IntVec3 x) => x.Standable(), (Region x) => true, out c, 999999))
			{
				return;
			}
			FilthMaker.MakeFilth(c, ThingDefOf.FilthSlime, 1);
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Hive.<GetGizmos>c__IteratorE9 <GetGizmos>c__IteratorE = new Hive.<GetGizmos>c__IteratorE9();
			<GetGizmos>c__IteratorE.<>f__this = this;
			Hive.<GetGizmos>c__IteratorE9 expr_0E = <GetGizmos>c__IteratorE;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
