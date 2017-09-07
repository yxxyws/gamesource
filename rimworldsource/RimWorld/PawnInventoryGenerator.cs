using System;
using Verse;
namespace RimWorld
{
	public static class PawnInventoryGenerator
	{
		public static void GenerateInventoryFor(Pawn p)
		{
			p.inventory.DestroyAll(DestroyMode.Vanish);
			for (int i = 0; i < p.kindDef.fixedInventory.Count; i++)
			{
				ThingCount thingCount = p.kindDef.fixedInventory[i];
				Thing thing = ThingMaker.MakeThing(thingCount.thingDef, null);
				thing.stackCount = thingCount.count;
				p.inventory.container.TryAdd(thing);
			}
			if (p.kindDef.inventoryOptions != null)
			{
				foreach (Thing current in p.kindDef.inventoryOptions.GenerateThings())
				{
					p.inventory.container.TryAdd(current);
				}
			}
			if (p.kindDef.invMealCount > 0)
			{
				float value = Rand.Value;
				ThingDef def;
				if (value < 0.5f)
				{
					def = ThingDefOf.MealSimple;
				}
				else
				{
					if ((double)value < 0.75)
					{
						def = ThingDefOf.MealFine;
					}
					else
					{
						def = ThingDefOf.MealSurvivalPack;
					}
				}
				Thing thing2 = ThingMaker.MakeThing(def, null);
				thing2.stackCount = p.kindDef.invMealCount;
				p.inventory.container.TryAdd(thing2);
			}
		}
	}
}
