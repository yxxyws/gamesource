using System;
namespace RimWorld
{
	public static class IncidentDefOf
	{
		public static IncidentDef ShipChunkDrop;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<IncidentDef>(typeof(IncidentDefOf));
		}
	}
}
