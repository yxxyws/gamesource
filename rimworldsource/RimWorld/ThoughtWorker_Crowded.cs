using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_Crowded : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.holder != null)
			{
				return ThoughtState.Inactive;
			}
			Room room = p.GetRoom();
			if (room == null || room.PsychologicallyOutdoors || room.IsHuge)
			{
				return ThoughtState.Inactive;
			}
			int num = 0;
			List<Thing> allContainedThings = room.AllContainedThings;
			for (int i = 0; i < allContainedThings.Count; i++)
			{
				if (allContainedThings[i] is Pawn)
				{
					num++;
				}
			}
			if (num == 0)
			{
				Log.Error("Pawns count in this room is zero. Where am I?");
				return ThoughtState.Inactive;
			}
			float num2 = (float)room.CellCount / (float)num;
			if (num == 1 || num2 > 12.4f)
			{
				return ThoughtState.Inactive;
			}
			if (num == 2 || num2 > 10f)
			{
				return ThoughtState.ActiveAtStage(0);
			}
			if (num2 > 6.55f)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			return ThoughtState.ActiveAtStage(2);
		}
	}
}
