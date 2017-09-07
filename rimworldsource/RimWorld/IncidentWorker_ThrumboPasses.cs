using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_ThrumboPasses : IncidentWorker
	{
		protected override bool StorytellerCanUseNowSub()
		{
			return !Find.MapConditionManager.ConditionIsActive(MapConditionDefOf.ToxicFallout);
		}
		public override bool TryExecute(IncidentParms parms)
		{
			IntVec3 intVec;
			if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec))
			{
				return false;
			}
			PawnKindDef thrumbo = PawnKindDefOf.Thrumbo;
			float points = IncidentMakerUtility.DefaultParmsNow(Find.Storyteller.def, IncidentCategory.ThreatBig).points;
			int num = GenMath.RoundRandom(points / thrumbo.combatPower);
			int max = Rand.RangeInclusive(2, 4);
			num = Mathf.Clamp(num, 1, max);
			int num2 = Rand.RangeInclusive(90000, 150000);
			IntVec3 invalid = IntVec3.Invalid;
			if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(intVec, 10f, out invalid))
			{
				invalid = IntVec3.Invalid;
			}
			Pawn pawn = null;
			for (int i = 0; i < num; i++)
			{
				IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, 10);
				pawn = PawnGenerator.GeneratePawn(thrumbo, null);
				GenSpawn.Spawn(pawn, loc, Rot4.Random);
				pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num2;
				if (invalid.IsValid)
				{
					pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(invalid, 10);
				}
			}
			Find.LetterStack.ReceiveLetter("LetterLabelThrumboPasses".Translate(new object[]
			{
				thrumbo.label
			}).CapitalizeFirst(), "LetterThrumboPasses".Translate(new object[]
			{
				thrumbo.label
			}), LetterType.Good, pawn, null);
			return true;
		}
	}
}
