using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class Blueprint : ThingWithComps, IConstructible
	{
		public override string Label
		{
			get
			{
				return this.def.entityDefToBuild.label + "BlueprintLabelExtra".Translate();
			}
		}
		protected abstract float WorkTotal
		{
			get;
		}
		public override void Tick()
		{
			base.Tick();
			if (!GenConstruct.CanBuildOnTerrain(this.def.entityDefToBuild, base.Position, base.Rotation))
			{
				this.Destroy(DestroyMode.Cancel);
			}
		}
		public override void Draw()
		{
			if (this.def.drawerType == DrawerType.RealtimeOnly)
			{
				base.Draw();
			}
			else
			{
				base.Comps_PostDraw();
			}
		}
		public virtual bool TryReplaceWithSolidThing(Pawn workerPawn, out Thing createdThing, out bool jobEnded)
		{
			jobEnded = false;
			if (this.FirstBlockingThing(workerPawn, false) != null)
			{
				workerPawn.jobs.EndCurrentJob(JobCondition.Incompletable);
				jobEnded = true;
				createdThing = null;
				return false;
			}
			Thing thing = this.FirstBlockingThing(null, false);
			if (thing != null)
			{
				Log.Error(string.Concat(new object[]
				{
					workerPawn,
					" tried to replace blueprint ",
					this.ToString(),
					" at ",
					base.Position,
					" with solid thing, but it is blocked by ",
					thing,
					" at ",
					thing.Position
				}));
				if (thing != workerPawn)
				{
					createdThing = null;
					return false;
				}
			}
			createdThing = this.MakeSolidThing();
			GenSpawn.WipeExistingThings(base.Position, base.Rotation, createdThing.def, true, new CellRect?(workerPawn.OccupiedRect()));
			if (!base.Destroyed)
			{
				this.Destroy(DestroyMode.Vanish);
			}
			createdThing.SetFactionDirect(workerPawn.Faction);
			GenSpawn.Spawn(createdThing, base.Position, base.Rotation);
			return true;
		}
		protected abstract Thing MakeSolidThing();
		public abstract List<ThingCount> MaterialsNeeded();
		public abstract ThingDef UIStuff();
		public Thing BlockingHaulableOnTop()
		{
			if (this.def.entityDefToBuild.passability == Traversability.Standable)
			{
				return null;
			}
			CellRect.CellRectIterator iterator = this.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				List<Thing> thingList = iterator.Current.GetThingList();
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if (thing.def.EverHaulable)
					{
						return thing;
					}
				}
				iterator.MoveNext();
			}
			return null;
		}
		public Thing FirstBlockingThing(Thing thingToIgnore = null, bool haulableOnly = false)
		{
			CellRect.CellRectIterator iterator = this.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				List<Thing> thingList = iterator.Current.GetThingList();
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if (!haulableOnly || thing.def.EverHaulable)
					{
						if (GenSpawn.BlocksFramePlacement(this, thing) && thing != thingToIgnore)
						{
							return thing;
						}
					}
				}
				iterator.MoveNext();
			}
			return null;
		}
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			stringBuilder.Append("WorkLeft".Translate() + ": " + this.WorkTotal.ToStringWorkAmount());
			return stringBuilder.ToString();
		}
	}
}
