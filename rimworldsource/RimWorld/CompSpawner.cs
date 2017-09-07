using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class CompSpawner : ThingComp
	{
		private int ticksUntilSpawn;
		private CompProperties_Spawner PropsSpawner
		{
			get
			{
				return (CompProperties_Spawner)this.props;
			}
		}
		public override void PostSpawnSetup()
		{
			this.ResetCountdown();
		}
		public override void CompTick()
		{
			this.ticksUntilSpawn--;
			this.CheckShouldSpawn();
		}
		public override void CompTickRare()
		{
			this.ticksUntilSpawn -= 250;
			this.CheckShouldSpawn();
		}
		private void CheckShouldSpawn()
		{
			if (this.ticksUntilSpawn <= 0)
			{
				this.TryDoSpawn();
				this.ResetCountdown();
			}
		}
		public bool TryDoSpawn()
		{
			if (this.PropsSpawner.spawnMaxAdjacent >= 0)
			{
				int num = 0;
				for (int i = 0; i < 9; i++)
				{
					List<Thing> thingList = (this.parent.Position + GenAdj.AdjacentCellsAndInside[i]).GetThingList();
					for (int j = 0; j < thingList.Count; j++)
					{
						if (thingList[j].def == this.PropsSpawner.thingToSpawn)
						{
							num += thingList[j].stackCount;
							if (num >= this.PropsSpawner.spawnMaxAdjacent)
							{
								return false;
							}
						}
					}
				}
			}
			IntVec3 center;
			if (this.TryFindSpawnCell(out center))
			{
				Thing thing = ThingMaker.MakeThing(this.PropsSpawner.thingToSpawn, null);
				thing.stackCount = this.PropsSpawner.spawnCount;
				Thing t;
				GenPlace.TryPlaceThing(thing, center, ThingPlaceMode.Direct, out t);
				if (this.PropsSpawner.spawnForbidden)
				{
					t.SetForbidden(true, true);
				}
				return true;
			}
			return false;
		}
		private bool TryFindSpawnCell(out IntVec3 result)
		{
			foreach (IntVec3 current in GenAdj.CellsAdjacent8Way(this.parent).InRandomOrder(null))
			{
				if (current.Walkable() && GenSight.LineOfSight(this.parent.Position, current, false) && (current.GetFirstItem() == null || (current.GetFirstItem().def == this.PropsSpawner.thingToSpawn && current.GetFirstItem().stackCount <= this.PropsSpawner.thingToSpawn.stackLimit - this.PropsSpawner.spawnCount)))
				{
					result = current;
					return true;
				}
			}
			result = IntVec3.Invalid;
			return false;
		}
		private void ResetCountdown()
		{
			this.ticksUntilSpawn = this.PropsSpawner.spawnInterval;
		}
		public override void PostExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.ticksUntilSpawn, "ticksUntilSpawn", 0, false);
		}
		[DebuggerHidden]
		public override IEnumerable<Command> CompGetGizmosExtra()
		{
			CompSpawner.<CompGetGizmosExtra>c__IteratorF2 <CompGetGizmosExtra>c__IteratorF = new CompSpawner.<CompGetGizmosExtra>c__IteratorF2();
			<CompGetGizmosExtra>c__IteratorF.<>f__this = this;
			CompSpawner.<CompGetGizmosExtra>c__IteratorF2 expr_0E = <CompGetGizmosExtra>c__IteratorF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
