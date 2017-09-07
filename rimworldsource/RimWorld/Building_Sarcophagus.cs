using System;
using Verse;
namespace RimWorld
{
	public class Building_Sarcophagus : Building_Grave
	{
		private bool everNonEmpty;
		private bool thisIsFirstBodyEver;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<bool>(ref this.everNonEmpty, "everNonEmpty", false, false);
		}
		public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
		{
			if (base.TryAcceptThing(thing, allowSpecialEffects))
			{
				this.thisIsFirstBodyEver = !this.everNonEmpty;
				this.everNonEmpty = true;
				return true;
			}
			return false;
		}
		public override void Notify_CorpseBuried(Pawn worker)
		{
			base.Notify_CorpseBuried(worker);
			if (this.thisIsFirstBodyEver && worker.IsColonist && base.Corpse.innerPawn.def.race.Humanlike)
			{
				foreach (Pawn current in Find.MapPawns.FreeColonists)
				{
					if (current.needs.mood != null)
					{
						current.needs.mood.thoughts.TryGainThought(ThoughtDefOf.KnowBuriedInSarcophagus);
					}
				}
			}
		}
	}
}
