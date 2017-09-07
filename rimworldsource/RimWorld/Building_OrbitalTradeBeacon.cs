using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class Building_OrbitalTradeBeacon : Building
	{
		private const float TradeRadius = 7.9f;
		private static List<IntVec3> tradeableCells = new List<IntVec3>();
		public IEnumerable<IntVec3> TradeableCells
		{
			get
			{
				return Building_OrbitalTradeBeacon.TradeableCellsAround(base.Position);
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_OrbitalTradeBeacon.<GetGizmos>c__IteratorE3 <GetGizmos>c__IteratorE = new Building_OrbitalTradeBeacon.<GetGizmos>c__IteratorE3();
			<GetGizmos>c__IteratorE.<>f__this = this;
			Building_OrbitalTradeBeacon.<GetGizmos>c__IteratorE3 expr_0E = <GetGizmos>c__IteratorE;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private void MakeMatchingStockpile()
		{
			Designator_ZoneAddStockpile_Resources des = new Designator_ZoneAddStockpile_Resources();
			des.DesignateMultiCell(
				from c in this.TradeableCells
				where des.CanDesignateCell(c).Accepted
				select c);
		}
		public static List<IntVec3> TradeableCellsAround(IntVec3 pos)
		{
			Building_OrbitalTradeBeacon.tradeableCells.Clear();
			if (!pos.InBounds())
			{
				return Building_OrbitalTradeBeacon.tradeableCells;
			}
			Region region = pos.GetRegion();
			if (region == null)
			{
				return Building_OrbitalTradeBeacon.tradeableCells;
			}
			RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.portal == null, delegate(Region r)
			{
				foreach (IntVec3 current in r.Cells)
				{
					if (current.InHorDistOf(pos, 7.9f))
					{
						Building_OrbitalTradeBeacon.tradeableCells.Add(current);
					}
				}
				return false;
			}, 12);
			return Building_OrbitalTradeBeacon.tradeableCells;
		}
		[DebuggerHidden]
		public static IEnumerable<Building_OrbitalTradeBeacon> AllPowered()
		{
			Building_OrbitalTradeBeacon.<AllPowered>c__IteratorE4 <AllPowered>c__IteratorE = new Building_OrbitalTradeBeacon.<AllPowered>c__IteratorE4();
			Building_OrbitalTradeBeacon.<AllPowered>c__IteratorE4 expr_07 = <AllPowered>c__IteratorE;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
