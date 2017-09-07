using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public static class HuntJobUtility
	{
		public static Corpse TryFindCorpse(Pawn killedPawn)
		{
			List<Thing> thingList = killedPawn.Position.GetThingList();
			for (int i = 0; i < thingList.Count; i++)
			{
				Corpse corpse = thingList[i] as Corpse;
				if (corpse != null && corpse.innerPawn == killedPawn)
				{
					return corpse;
				}
			}
			return null;
		}
	}
}
