using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_Nuzzle : ThinkNode_JobGiver
	{
		private const float MaxNuzzleDistance = 15f;
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			if (pawn.RaceProps.nuzzlePower <= 0.001f)
			{
				return null;
			}
			List<Pawn> source = Find.MapPawns.SpawnedPawnsInFaction(pawn.Faction);
			Pawn t;
			if (!(
				from p in source
				where p.RaceProps.Humanlike && p.Position.InHorDistOf(pawn.Position, 15f) && pawn.GetRoom() == p.GetRoom() && !p.Position.IsForbidden(pawn) && pawn.CasualInterruptibleNow() && !pawn.InBed()
				select p).TryRandomElement(out t))
			{
				return null;
			}
			return new Job(JobDefOf.Nuzzle, t)
			{
				locomotionUrgency = LocomotionUrgency.Walk,
				expiryInterval = 3000
			};
		}
	}
}
