using System;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class PlaceWorker_WindTurbine : PlaceWorker
	{
		public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
		{
			GenDraw.DrawFieldEdges(Building_WindTurbine.CalculateWindCells(center, rot, def.size).ToList<IntVec3>());
		}
	}
}
