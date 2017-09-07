using RimWorld;
using System;
using System.Collections.Generic;
namespace Verse.AI.Group
{
	public static class LordMaker
	{
		public static Lord MakeNewLord(Faction faction, LordJob lordJob, IEnumerable<Pawn> startingPawns = null)
		{
			Lord lord = new Lord();
			lord.loadID = Find.World.uniqueIDsManager.GetNextLordID();
			lord.faction = faction;
			Find.Map.lordManager.AddLord(lord);
			lord.SetJob(lordJob);
			lord.GotoToil(lord.Graph.StartingToil);
			if (startingPawns != null)
			{
				foreach (Pawn current in startingPawns)
				{
					lord.AddPawn(current);
				}
			}
			return lord;
		}
	}
}
