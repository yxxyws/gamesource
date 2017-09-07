using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class SiegeBlueprintPlacer
	{
		private const int MaxArtyCount = 2;
		public const float ArtyCost = 60f;
		private const int MinSandbagDistSquared = 36;
		private static IntVec3 center;
		private static Faction faction;
		private static List<IntVec3> placedSandbagLocs = new List<IntVec3>();
		private static readonly IntRange NumSandbagRange = new IntRange(2, 4);
		private static readonly IntRange SandbagLengthRange = new IntRange(2, 7);
		[DebuggerHidden]
		public static IEnumerable<Blueprint_Build> PlaceBlueprints(IntVec3 placeCenter, Faction placeFaction, float points)
		{
			SiegeBlueprintPlacer.<PlaceBlueprints>c__Iterator57 <PlaceBlueprints>c__Iterator = new SiegeBlueprintPlacer.<PlaceBlueprints>c__Iterator57();
			<PlaceBlueprints>c__Iterator.placeCenter = placeCenter;
			<PlaceBlueprints>c__Iterator.placeFaction = placeFaction;
			<PlaceBlueprints>c__Iterator.points = points;
			<PlaceBlueprints>c__Iterator.<$>placeCenter = placeCenter;
			<PlaceBlueprints>c__Iterator.<$>placeFaction = placeFaction;
			<PlaceBlueprints>c__Iterator.<$>points = points;
			SiegeBlueprintPlacer.<PlaceBlueprints>c__Iterator57 expr_31 = <PlaceBlueprints>c__Iterator;
			expr_31.$PC = -2;
			return expr_31;
		}
		private static bool CanPlaceBlueprintAt(IntVec3 root, Rot4 rot, ThingDef buildingDef)
		{
			return GenConstruct.CanPlaceBlueprintAt(buildingDef, root, rot, false).Accepted;
		}
		[DebuggerHidden]
		private static IEnumerable<Blueprint_Build> PlaceSandbagBlueprints()
		{
			SiegeBlueprintPlacer.<PlaceSandbagBlueprints>c__Iterator58 <PlaceSandbagBlueprints>c__Iterator = new SiegeBlueprintPlacer.<PlaceSandbagBlueprints>c__Iterator58();
			SiegeBlueprintPlacer.<PlaceSandbagBlueprints>c__Iterator58 expr_07 = <PlaceSandbagBlueprints>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		[DebuggerHidden]
		private static IEnumerable<Blueprint_Build> MakeSandbagLine(IntVec3 root, Rot4 growDir, int maxLength)
		{
			SiegeBlueprintPlacer.<MakeSandbagLine>c__Iterator59 <MakeSandbagLine>c__Iterator = new SiegeBlueprintPlacer.<MakeSandbagLine>c__Iterator59();
			<MakeSandbagLine>c__Iterator.root = root;
			<MakeSandbagLine>c__Iterator.maxLength = maxLength;
			<MakeSandbagLine>c__Iterator.growDir = growDir;
			<MakeSandbagLine>c__Iterator.<$>root = root;
			<MakeSandbagLine>c__Iterator.<$>maxLength = maxLength;
			<MakeSandbagLine>c__Iterator.<$>growDir = growDir;
			SiegeBlueprintPlacer.<MakeSandbagLine>c__Iterator59 expr_31 = <MakeSandbagLine>c__Iterator;
			expr_31.$PC = -2;
			return expr_31;
		}
		[DebuggerHidden]
		private static IEnumerable<Blueprint_Build> PlaceArtilleryBlueprints(float points)
		{
			SiegeBlueprintPlacer.<PlaceArtilleryBlueprints>c__Iterator5A <PlaceArtilleryBlueprints>c__Iterator5A = new SiegeBlueprintPlacer.<PlaceArtilleryBlueprints>c__Iterator5A();
			<PlaceArtilleryBlueprints>c__Iterator5A.points = points;
			<PlaceArtilleryBlueprints>c__Iterator5A.<$>points = points;
			SiegeBlueprintPlacer.<PlaceArtilleryBlueprints>c__Iterator5A expr_15 = <PlaceArtilleryBlueprints>c__Iterator5A;
			expr_15.$PC = -2;
			return expr_15;
		}
		private static IntVec3 FindSandbagRoot()
		{
			CellRect cellRect = CellRect.CenteredOn(SiegeBlueprintPlacer.center, 13);
			cellRect.ClipInsideMap();
			CellRect cellRect2 = CellRect.CenteredOn(SiegeBlueprintPlacer.center, 8);
			cellRect2.ClipInsideMap();
			int num = 0;
			while (true)
			{
				num++;
				if (num > 200)
				{
					break;
				}
				IntVec3 randomCell = cellRect.RandomCell;
				if (!cellRect2.Contains(randomCell))
				{
					if (randomCell.CanReach(SiegeBlueprintPlacer.center, PathEndMode.OnCell, TraverseMode.NoPassClosedDoors, Danger.Deadly))
					{
						if (SiegeBlueprintPlacer.CanPlaceBlueprintAt(randomCell, Rot4.North, ThingDefOf.Sandbags))
						{
							bool flag = false;
							for (int i = 0; i < SiegeBlueprintPlacer.placedSandbagLocs.Count; i++)
							{
								float lengthHorizontalSquared = (SiegeBlueprintPlacer.placedSandbagLocs[i] - randomCell).LengthHorizontalSquared;
								if (lengthHorizontalSquared < 36f)
								{
									flag = true;
								}
							}
							if (!flag)
							{
								return randomCell;
							}
						}
					}
				}
			}
			return IntVec3.Invalid;
		}
		private static IntVec3 FindArtySpot(ThingDef artyDef, Rot4 rot)
		{
			CellRect cellRect = CellRect.CenteredOn(SiegeBlueprintPlacer.center, 8);
			cellRect.ClipInsideMap();
			int num = 0;
			while (true)
			{
				num++;
				if (num > 200)
				{
					break;
				}
				IntVec3 randomCell = cellRect.RandomCell;
				if (randomCell.CanReach(SiegeBlueprintPlacer.center, PathEndMode.OnCell, TraverseMode.NoPassClosedDoors, Danger.Deadly))
				{
					if (!randomCell.Roofed())
					{
						if (SiegeBlueprintPlacer.CanPlaceBlueprintAt(randomCell, rot, artyDef))
						{
							return randomCell;
						}
					}
				}
			}
			return IntVec3.Invalid;
		}
	}
}
