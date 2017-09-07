using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_DropUnusedInventory : ThinkNode_JobGiver
	{
		private const int RawFoodDropDelay = 150000;
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			if (pawn.inventory == null)
			{
				return null;
			}
			if (Find.TickManager.TicksGame > pawn.mindState.lastInventoryRawFoodUseTick + 150000)
			{
				if (!Find.AreaHome[pawn.Position])
				{
					return null;
				}
				if (pawn.Faction != Faction.OfColony)
				{
					return null;
				}
				for (int i = pawn.inventory.container.Count - 1; i >= 0; i--)
				{
					Thing thing = pawn.inventory.container[i];
					if (thing.def.ingestible != null && thing.def.ingestible.preferability <= FoodPreferability.Raw)
					{
						Thing thing2;
						pawn.inventory.container.TryDrop(thing, pawn.Position, ThingPlaceMode.Near, out thing2);
					}
				}
			}
			return null;
		}
	}
}
