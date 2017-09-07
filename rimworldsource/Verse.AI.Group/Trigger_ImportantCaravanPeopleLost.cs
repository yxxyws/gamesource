using RimWorld;
using System;
namespace Verse.AI.Group
{
	public class Trigger_ImportantCaravanPeopleLost : Trigger
	{
		public override bool ActivateOn(Lord lord, TriggerSignal signal)
		{
			if (signal.type == TriggerSignalType.PawnLost && (signal.condition == PawnLostCondition.IncappedOrKilled || signal.condition == PawnLostCondition.MadePrisoner))
			{
				TraderCaravanRole caravanRole = signal.pawn.GetCaravanRole();
				if (caravanRole == TraderCaravanRole.Trader || caravanRole == TraderCaravanRole.Carrier)
				{
					return true;
				}
				if (lord.numPawnsLostViolently > 0 && (float)lord.numPawnsLostViolently / (float)lord.numPawnsEverGained >= 0.5f)
				{
					return true;
				}
			}
			return false;
		}
	}
}
