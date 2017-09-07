using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	public class IncidentWorker_AnimalInsanityMass : IncidentWorker
	{
		public static bool AnimalUsable(Pawn p)
		{
			return p.Spawned && !p.Position.Fogged() && (!p.InMentalState || !p.MentalStateDef.isAggro) && !p.Downed && p.Faction == null;
		}
		public override bool TryExecute(IncidentParms parms)
		{
			if (parms.points <= 0f)
			{
				Log.Error("AnimalInsanity running without points.");
				parms.points = (float)((int)(Find.StoryWatcher.watcherStrength.StrengthRating * 50f));
			}
			float adjustedPoints = parms.points;
			if (adjustedPoints > 250f)
			{
				adjustedPoints -= 250f;
				adjustedPoints *= 0.5f;
				adjustedPoints += 250f;
			}
			IEnumerable<PawnKindDef> source = 
				from def in DefDatabase<PawnKindDef>.AllDefs
				where def.RaceProps.Animal && def.combatPower <= adjustedPoints && (
					from p in Find.MapPawns.AllPawnsSpawned
					where p.kindDef == def && IncidentWorker_AnimalInsanityMass.AnimalUsable(p)
					select p).Count<Pawn>() >= 3
				select def;
			PawnKindDef animalDef;
			if (!source.TryRandomElement(out animalDef))
			{
				return false;
			}
			List<Pawn> list = (
				from p in Find.MapPawns.AllPawnsSpawned
				where p.kindDef == animalDef && IncidentWorker_AnimalInsanityMass.AnimalUsable(p)
				select p).ToList<Pawn>();
			float combatPower = animalDef.combatPower;
			float num = 0f;
			int num2 = 0;
			Pawn t = null;
			list.Shuffle<Pawn>();
			foreach (Pawn current in list)
			{
				if (num + combatPower > adjustedPoints)
				{
					break;
				}
				current.mindState.mentalStateHandler.StartMentalState(MentalStateDefOf.Manhunter);
				num += combatPower;
				num2++;
				t = current;
			}
			if (num == 0f)
			{
				return false;
			}
			string label;
			string text;
			if (num2 == 1)
			{
				label = "LetterLabelAnimalInsanitySingle".Translate();
				text = "AnimalInsanitySingle".Translate(new object[]
				{
					animalDef.label
				});
			}
			else
			{
				label = "LetterLabelAnimalInsanityMultiple".Translate();
				text = "AnimalInsanityMultiple".Translate(new object[]
				{
					animalDef.label
				});
			}
			Find.LetterStack.ReceiveLetter(label, text, LetterType.BadUrgent, t, null);
			SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera();
			Find.CameraMap.shaker.DoShake(1f);
			return true;
		}
	}
}
