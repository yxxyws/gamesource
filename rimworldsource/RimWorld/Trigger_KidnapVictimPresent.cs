using System;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class Trigger_KidnapVictimPresent : Trigger
	{
		private const int CheckInterval = 75;
		private const float ColonistCheckRadius = 12f;
		private TriggerData_PawnCycleInd Data
		{
			get
			{
				return (TriggerData_PawnCycleInd)this.data;
			}
		}
		public Trigger_KidnapVictimPresent()
		{
			this.data = new TriggerData_PawnCycleInd();
		}
		public override bool ActivateOn(Lord lord, TriggerSignal signal)
		{
			if (signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % 75 == 0)
			{
				TriggerData_PawnCycleInd data = this.Data;
				data.pawnCycleInd++;
				if (data.pawnCycleInd >= lord.ownedPawns.Count)
				{
					data.pawnCycleInd = 0;
				}
				Pawn pawn = lord.ownedPawns[data.pawnCycleInd];
				if (pawn.Spawned && !pawn.Downed && pawn.MentalStateDef == null && KidnapAIUtility.ClosestKidnapVictim(pawn, 8f, true) != null)
				{
					foreach (Pawn current in Find.MapPawns.FreeColonistsSpawned)
					{
						if (!current.Downed && current.GetPosture() == PawnPosture.Standing && (current.Position - pawn.Position).LengthHorizontalSquared < 144f)
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}
	}
}
