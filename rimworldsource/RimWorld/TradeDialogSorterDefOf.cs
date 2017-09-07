using System;
namespace RimWorld
{
	public static class TradeDialogSorterDefOf
	{
		public static TradeDialogSorterDef Category;
		public static TradeDialogSorterDef MarketValue;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<TradeDialogSorterDef>(typeof(TradeDialogSorterDefOf));
		}
	}
}
