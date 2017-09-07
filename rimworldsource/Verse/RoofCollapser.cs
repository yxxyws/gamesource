using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public static class RoofCollapser
	{
		private const float RoofMaxSupportDistance = 6.9f;
		private static List<IntVec3> collapsingCells = new List<IntVec3>();
		private static List<Thing> crushedThingsThisFrame = new List<Thing>();
		private static readonly int NumRadialCells = GenRadial.NumCellsInRadius(6.9f);
		private static readonly IntRange CrushDamageRange = new IntRange(50, 75);
		public static void RoofCollapseCheckerTick_First()
		{
			if (RoofCollapser.collapsingCells.Count > 0)
			{
				for (int i = 0; i < RoofCollapser.collapsingCells.Count; i++)
				{
					RoofCollapser.DropRoofInCell(RoofCollapser.collapsingCells[i]);
				}
				for (int j = 0; j < RoofCollapser.collapsingCells.Count; j++)
				{
					IntVec3 c = RoofCollapser.collapsingCells[j];
					RoofDef roofDef = Find.RoofGrid.RoofAt(c);
					if (roofDef != null)
					{
						if (roofDef.filthLeaving != null)
						{
							FilthMaker.MakeFilth(c, roofDef.filthLeaving, 1);
						}
						if (!roofDef.isThickRoof)
						{
							Find.RoofGrid.SetRoof(c, null);
						}
					}
				}
				if (RoofCollapser.crushedThingsThisFrame.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine("RoofCollapsed".Translate());
					stringBuilder.AppendLine("\n " + "TheseThingsCrushed".Translate());
					HashSet<string> hashSet = new HashSet<string>();
					foreach (Thing current in RoofCollapser.crushedThingsThisFrame)
					{
						string item = current.LabelBaseShort.CapitalizeFirst();
						if (current.def.category == ThingCategory.Pawn)
						{
							item = current.LabelCap;
						}
						if (!hashSet.Contains(item))
						{
							hashSet.Add(item);
						}
					}
					foreach (string current2 in hashSet)
					{
						stringBuilder.AppendLine("    -" + current2);
					}
					RoofCollapser.crushedThingsThisFrame.Clear();
					Letter let = new Letter("LetterLabelRoofCollapsed".Translate(), stringBuilder.ToString(), LetterType.BadNonUrgent, RoofCollapser.collapsingCells[0]);
					Find.LetterStack.ReceiveLetter(let, null);
				}
				else
				{
					string text = "RoofCollapsed".Translate();
					Messages.Message(text, RoofCollapser.collapsingCells[0], MessageSound.Negative);
				}
				SoundDef.Named("RoofCollapse").PlayOneShot(RoofCollapser.collapsingCells[0]);
				RoofCollapser.collapsingCells.Clear();
			}
		}
		private static bool CareAboutCrushed(Thing t)
		{
			if (!t.def.destroyable)
			{
				return false;
			}
			switch (t.def.category)
			{
			case ThingCategory.Pawn:
				return true;
			case ThingCategory.Item:
				return t.MarketValue > 0.01f;
			case ThingCategory.Building:
				return true;
			}
			return false;
		}
		private static void DropRoofInCell(IntVec3 c)
		{
			RoofDef roofDef = Find.RoofGrid.RoofAt(c);
			if (roofDef == null)
			{
				return;
			}
			if (roofDef.collapseLeavingThingDef != null && roofDef.collapseLeavingThingDef.passability == Traversability.Impassable)
			{
				List<Thing> thingList = c.GetThingList();
				for (int i = thingList.Count - 1; i >= 0; i--)
				{
					Thing thing = thingList[i];
					if (!RoofCollapser.crushedThingsThisFrame.Contains(thing) && RoofCollapser.CareAboutCrushed(thing))
					{
						RoofCollapser.crushedThingsThisFrame.Add(thing);
					}
					if (thing.def.destroyable)
					{
						thing.Destroy(DestroyMode.Vanish);
					}
				}
			}
			else
			{
				List<Thing> thingList2 = c.GetThingList();
				for (int j = thingList2.Count - 1; j >= 0; j--)
				{
					Thing thing2 = thingList2[j];
					if (!RoofCollapser.crushedThingsThisFrame.Contains(thing2) && RoofCollapser.CareAboutCrushed(thing2))
					{
						RoofCollapser.crushedThingsThisFrame.Add(thing2);
					}
					BodyPartDamageInfo value = new BodyPartDamageInfo(new BodyPartHeight?(BodyPartHeight.Top), new BodyPartDepth?(BodyPartDepth.Outside));
					DamageInfo dinfo = new DamageInfo(DamageDefOf.Crush, RoofCollapser.CrushDamageRange.RandomInRange, null, new BodyPartDamageInfo?(value), null);
					thing2.TakeDamage(dinfo);
				}
				Thing edifice = c.GetEdifice();
				if (edifice != null && !edifice.Destroyed)
				{
					if (!RoofCollapser.crushedThingsThisFrame.Contains(edifice) && RoofCollapser.CareAboutCrushed(edifice))
					{
						RoofCollapser.crushedThingsThisFrame.Add(edifice);
					}
					edifice.Destroy(DestroyMode.Vanish);
				}
			}
			if (roofDef.collapseLeavingThingDef != null)
			{
				Thing thing3 = GenSpawn.Spawn(roofDef.collapseLeavingThingDef, c);
				if (thing3.def.rotatable)
				{
					thing3.Rotation = Rot4.Random;
				}
			}
			for (int k = 0; k < 2; k++)
			{
				Vector3 vector = c.ToVector3Shifted();
				vector += Gen.RandomHorizontalVector(0.6f);
				MoteThrower.ThrowDustPuff(vector, 2f);
			}
		}
		public static void Notify_RoofHolderRemoved(Thing t)
		{
			if (Game.Mode != GameMode.MapPlaying)
			{
				return;
			}
			RoofGrid roofGrid = Find.RoofGrid;
			for (int i = 0; i < RoofCollapser.NumRadialCells; i++)
			{
				IntVec3 intVec = t.Position + GenRadial.RadialPattern[i];
				if (intVec.InBounds())
				{
					if (roofGrid.Roofed(intVec.x, intVec.z))
					{
						if (!RoofCollapser.IsSupported(intVec))
						{
							RoofCollapser.collapsingCells.Add(intVec);
						}
					}
				}
			}
		}
		public static bool IsSupported(IntVec3 roofLoc)
		{
			Building[] innerArray = Find.EdificeGrid.InnerArray;
			for (int i = 0; i < RoofCollapser.NumRadialCells; i++)
			{
				IntVec3 c = roofLoc + GenRadial.RadialPattern[i];
				if (c.InBounds())
				{
					Thing thing = innerArray[CellIndices.CellToIndex(c)];
					if (thing != null && thing.def.holdsRoof)
					{
						if (DebugViewSettings.drawRoofs)
						{
							Find.DebugDrawer.FlashCell(c, 0.5f, null);
							Find.DebugDrawer.FlashCell(roofLoc, 1f, null);
						}
						return true;
					}
				}
			}
			return false;
		}
	}
}
