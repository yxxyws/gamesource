using RimWorld;
using System;
namespace Verse.AI.Group
{
	public class Trigger_BecameColonyEnemy : Trigger
	{
		public override bool ActivateOn(Lord lord, TriggerSignal signal)
		{
			return signal.type == TriggerSignalType.Tick && lord.faction.HostileTo(Faction.OfColony);
		}
	}
}
