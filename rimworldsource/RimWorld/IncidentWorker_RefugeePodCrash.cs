using System;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_RefugeePodCrash : IncidentWorker
	{
		private const float FogClearRadius = 4.5f;
		private const float RelationWithColonistWeight = 20f;
		public override bool TryExecute(IncidentParms parms)
		{
			IntVec3 intVec = DropCellFinder.RandomDropSpot();
			Faction faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.Spacer);
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest
			{
				kindDef = PawnKindDefOf.SpaceRefugee,
				faction = faction,
				colonistRelationChanceFactor = 20f
			});
			HealthUtility.GiveInjuriesToForceDowned(pawn);
			string label = "LetterLabelRefugeePodCrash".Translate();
			string text = "RefugeePodCrash".Translate();
			PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref label, pawn);
			Find.LetterStack.ReceiveLetter(label, text, LetterType.BadNonUrgent, intVec, null);
			DropPodUtility.MakeDropPodAt(intVec, new DropPodInfo
			{
				SingleContainedThing = pawn,
				openDelay = 180,
				leaveSlag = true
			});
			return true;
		}
	}
}
