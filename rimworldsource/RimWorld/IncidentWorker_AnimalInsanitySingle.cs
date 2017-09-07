using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_AnimalInsanitySingle : IncidentWorker
	{
		private const int FixedPoints = 30;
		public override bool TryExecute(IncidentParms parms)
		{
			int maxPoints = 150;
			if (GenDate.DaysPassed < 7)
			{
				maxPoints = 40;
			}
			List<Pawn> list = (
				from p in Find.MapPawns.AllPawnsSpawned
				where p.RaceProps.Animal && p.kindDef.combatPower <= (float)maxPoints && IncidentWorker_AnimalInsanityMass.AnimalUsable(p)
				select p).ToList<Pawn>();
			if (list.Count == 0)
			{
				return false;
			}
			Pawn pawn = list.RandomElement<Pawn>();
			pawn.mindState.mentalStateHandler.StartMentalState(MentalStateDefOf.Manhunter);
			string text = "AnimalInsanitySingle".Translate(new object[]
			{
				pawn.Label
			});
			Find.LetterStack.ReceiveLetter("LetterLabelAnimalInsanitySingle".Translate(), text, LetterType.BadUrgent, pawn, null);
			return true;
		}
	}
}
