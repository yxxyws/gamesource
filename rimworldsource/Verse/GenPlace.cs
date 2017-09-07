using RimWorld;
using System;
using System.Collections.Generic;
using Verse.AI;
namespace Verse
{
	public static class GenPlace
	{
		private enum PlaceSpotQuality : byte
		{
			Unusable,
			Awful,
			Bad,
			Okay,
			Perfect
		}
		private static readonly int PlaceNearMaxRadialCells = GenRadial.NumCellsInRadius(12.9f);
		private static readonly int PlaceNearMiddleRadialCells = GenRadial.NumCellsInRadius(3f);
		public static bool TryPlaceThing(Thing thing, IntVec3 center, ThingPlaceMode mode)
		{
			Thing thing2;
			return GenPlace.TryPlaceThing(thing, center, mode, out thing2);
		}
		public static bool TryPlaceThing(Thing thing, IntVec3 center, ThingPlaceMode mode, out Thing lastResultingThing)
		{
			if (thing.def.category == ThingCategory.Filth)
			{
				mode = ThingPlaceMode.Direct;
			}
			if (mode == ThingPlaceMode.Direct)
			{
				return GenPlace.TryPlaceDirect(thing, center, out lastResultingThing);
			}
			if (mode == ThingPlaceMode.Near)
			{
				lastResultingThing = null;
				while (true)
				{
					int stackCount = thing.stackCount;
					IntVec3 loc;
					if (!GenPlace.TryFindPlaceSpotNear(center, thing, out loc))
					{
						break;
					}
					if (GenPlace.TryPlaceDirect(thing, loc, out lastResultingThing))
					{
						return true;
					}
					if (thing.stackCount == stackCount)
					{
						goto Block_6;
					}
				}
				return false;
				Block_6:
				Log.Error(string.Concat(new object[]
				{
					"Failed to place ",
					thing,
					" at ",
					center,
					" in mode ",
					mode,
					"."
				}));
				lastResultingThing = null;
				return false;
			}
			throw new InvalidOperationException();
		}
		private static bool TryFindPlaceSpotNear(IntVec3 center, Thing thing, out IntVec3 bestSpot)
		{
			GenPlace.PlaceSpotQuality placeSpotQuality = GenPlace.PlaceSpotQuality.Unusable;
			bestSpot = center;
			for (int i = 0; i < 9; i++)
			{
				IntVec3 intVec = center + GenRadial.RadialPattern[i];
				GenPlace.PlaceSpotQuality placeSpotQuality2 = GenPlace.PlaceSpotQualityAt(intVec, thing, center);
				if (placeSpotQuality2 > placeSpotQuality)
				{
					bestSpot = intVec;
					placeSpotQuality = placeSpotQuality2;
				}
				if (placeSpotQuality == GenPlace.PlaceSpotQuality.Perfect)
				{
					break;
				}
			}
			if (placeSpotQuality >= GenPlace.PlaceSpotQuality.Okay)
			{
				return true;
			}
			for (int j = 0; j < GenPlace.PlaceNearMiddleRadialCells; j++)
			{
				IntVec3 intVec = center + GenRadial.RadialPattern[j];
				GenPlace.PlaceSpotQuality placeSpotQuality2 = GenPlace.PlaceSpotQualityAt(intVec, thing, center);
				if (placeSpotQuality2 > placeSpotQuality)
				{
					bestSpot = intVec;
					placeSpotQuality = placeSpotQuality2;
				}
				if (placeSpotQuality == GenPlace.PlaceSpotQuality.Perfect)
				{
					break;
				}
			}
			if (placeSpotQuality >= GenPlace.PlaceSpotQuality.Okay)
			{
				return true;
			}
			for (int k = 0; k < GenPlace.PlaceNearMaxRadialCells; k++)
			{
				IntVec3 intVec = center + GenRadial.RadialPattern[k];
				GenPlace.PlaceSpotQuality placeSpotQuality2 = GenPlace.PlaceSpotQualityAt(intVec, thing, center);
				if (placeSpotQuality2 > placeSpotQuality)
				{
					bestSpot = intVec;
					placeSpotQuality = placeSpotQuality2;
				}
				if (placeSpotQuality == GenPlace.PlaceSpotQuality.Perfect)
				{
					break;
				}
			}
			if (placeSpotQuality > GenPlace.PlaceSpotQuality.Unusable)
			{
				return true;
			}
			bestSpot = center;
			return false;
		}
		private static GenPlace.PlaceSpotQuality PlaceSpotQualityAt(IntVec3 c, Thing thing, IntVec3 center)
		{
			if (!c.InBounds() || !c.Walkable())
			{
				return GenPlace.PlaceSpotQuality.Unusable;
			}
			List<Thing> list = Find.ThingGrid.ThingsListAt(c);
			int i = 0;
			while (i < list.Count)
			{
				Thing thing2 = list[i];
				if (thing.def.saveCompressible && thing2.def.saveCompressible)
				{
					return GenPlace.PlaceSpotQuality.Unusable;
				}
				if (thing2.def.category == ThingCategory.Item)
				{
					if (thing2.def == thing.def && thing2.stackCount < thing.def.stackLimit)
					{
						return GenPlace.PlaceSpotQuality.Perfect;
					}
					return GenPlace.PlaceSpotQuality.Unusable;
				}
				else
				{
					i++;
				}
			}
			if (c.GetRoom() == center.GetRoom())
			{
				GenPlace.PlaceSpotQuality placeSpotQuality = GenPlace.PlaceSpotQuality.Perfect;
				for (int j = 0; j < list.Count; j++)
				{
					Thing thing3 = list[j];
					if (thing3.def.thingClass == typeof(Building_Door))
					{
						return GenPlace.PlaceSpotQuality.Bad;
					}
					Pawn pawn = thing3 as Pawn;
					if (pawn != null)
					{
						if (pawn.Downed)
						{
							return GenPlace.PlaceSpotQuality.Bad;
						}
						if (placeSpotQuality > GenPlace.PlaceSpotQuality.Okay)
						{
							placeSpotQuality = GenPlace.PlaceSpotQuality.Okay;
						}
					}
					if (thing3.def.category == ThingCategory.Plant && thing3.def.selectable && placeSpotQuality > GenPlace.PlaceSpotQuality.Okay)
					{
						placeSpotQuality = GenPlace.PlaceSpotQuality.Okay;
					}
				}
				return placeSpotQuality;
			}
			if (!center.CanReach(c, PathEndMode.OnCell, TraverseMode.PassDoors, Danger.Deadly))
			{
				return GenPlace.PlaceSpotQuality.Awful;
			}
			return GenPlace.PlaceSpotQuality.Bad;
		}
		private static bool TryPlaceDirect(Thing thing, IntVec3 loc, out Thing resultingThing)
		{
			bool flag = false;
			if (thing.stackCount > thing.def.stackLimit)
			{
				thing = thing.SplitOff(thing.def.stackLimit);
				flag = true;
			}
			if (thing.def.stackLimit > 1)
			{
				List<Thing> thingList = loc.GetThingList();
				int i = 0;
				while (i < thingList.Count)
				{
					Thing thing2 = thingList[i];
					if (!thing2.CanStackWith(thing))
					{
						i++;
					}
					else
					{
						if (thing2.TryAbsorbStack(thing, true))
						{
							resultingThing = thing2;
							return !flag;
						}
						resultingThing = null;
						return false;
					}
				}
			}
			resultingThing = GenSpawn.Spawn(thing, loc);
			SlotGroup slotGroup = loc.GetSlotGroup();
			if (slotGroup != null && slotGroup.parent != null)
			{
				slotGroup.parent.Notify_ReceivedThing(resultingThing);
			}
			return !flag;
		}
		public static Thing HaulPlaceBlockerIn(Thing haulThing, IntVec3 c, bool checkBlueprints)
		{
			List<Thing> list = Find.ThingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (checkBlueprints && thing.def.IsBlueprint)
				{
					return thing;
				}
				if ((thing.def.category != ThingCategory.Plant || thing.def.passability != Traversability.Standable) && thing.def.category != ThingCategory.Filth)
				{
					if (thing.def.EverHaulable)
					{
						return thing;
					}
					if (haulThing != null && GenSpawn.SpawningWipes(haulThing.def, thing.def))
					{
						return thing;
					}
					if (thing.def.passability != Traversability.Standable)
					{
						if (thing.def.surfaceType != SurfaceType.Item)
						{
							return thing;
						}
					}
				}
			}
			return null;
		}
	}
}
