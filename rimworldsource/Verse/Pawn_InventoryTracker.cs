using RimWorld;
using System;
namespace Verse
{
	public class Pawn_InventoryTracker : IExposable, IThingContainerOwner
	{
		public Pawn pawn;
		public ThingContainer container;
		public bool Spawned
		{
			get
			{
				return this.pawn.Spawned;
			}
		}
		public Pawn_InventoryTracker(Pawn pawn)
		{
			this.pawn = pawn;
			this.container = new ThingContainer(this, false);
		}
		public void ExposeData()
		{
			Scribe_Deep.LookDeep<ThingContainer>(ref this.container, "container", new object[]
			{
				this
			});
		}
		public void InventoryTrackerTick()
		{
			this.container.ThingContainerTick();
		}
		public void DropAllNearPawn(IntVec3 pos, bool forbid = false)
		{
			while (this.container.Count > 0)
			{
				Thing thing;
				this.container.TryDrop(this.container[0], pos, ThingPlaceMode.Near, out thing);
				if (forbid && thing != null)
				{
					thing.SetForbiddenIfOutsideHomeArea();
				}
			}
		}
		public void DestroyAll(DestroyMode mode = DestroyMode.Vanish)
		{
			this.container.ClearAndDestroyContents(mode);
		}
		public ThingContainer GetContainer()
		{
			return this.container;
		}
		public IntVec3 GetPosition()
		{
			return this.pawn.Position;
		}
	}
}
