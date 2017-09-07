using System;
namespace RimWorld
{
	public static class ThoughtDefOf
	{
		public static ThoughtDef GreenThumbHappy;
		public static ThoughtDef WasImprisoned;
		public static ThoughtDef NewColonyOptimism;
		public static ThoughtDef SleptOutside;
		public static ThoughtDef SleptOnGround;
		public static ThoughtDef SleptInCold;
		public static ThoughtDef SleptInHeat;
		public static ThoughtDef SleepDisturbed;
		public static ThoughtDef AteWithoutTable;
		public static ThoughtDef AteInImpressiveDiningRoom;
		public static ThoughtDef JoyActivityInImpressiveRecRoom;
		public static ThoughtDef Catharsis;
		public static ThoughtDef MyOrganHarvested;
		public static ThoughtDef KnowGuestOrganHarvested;
		public static ThoughtDef KnowColonistOrganHarvested;
		public static ThoughtDef KnowGuestExecuted;
		public static ThoughtDef KnowColonistExecuted;
		public static ThoughtDef KnowPrisonerSold;
		public static ThoughtDef BattleWounded;
		public static ThoughtDef WitnessedDeathStranger;
		public static ThoughtDef WitnessedDeathStrangerBloodlust;
		public static ThoughtDef WitnessedDeathAlly;
		public static ThoughtDef KilledHumanlikeBloodlust;
		public static ThoughtDef ButcheredHumanlikeCorpse;
		public static ThoughtDef KnowButcheredHumanlikeCorpse;
		public static ThoughtDef ObservedLayingRottingCorpse;
		public static ThoughtDef ObservedLayingCorpse;
		public static ThoughtDef PrisonerFriendlyChat;
		public static ThoughtDef AteHumanlikeMeatDirect;
		public static ThoughtDef AteHumanlikeMeatDirectCannibal;
		public static ThoughtDef AteHumanlikeMeatAsIngredient;
		public static ThoughtDef AteHumanlikeMeatAsIngredientCannibal;
		public static ThoughtDef AteCorpse;
		public static ThoughtDef Nuzzled;
		public static ThoughtDef ArtifactMoodBoost;
		public static ThoughtDef KnowBuriedInSarcophagus;
		public static ThoughtDef Chitchat;
		public static ThoughtDef PawnWithGoodOpinionDied;
		public static ThoughtDef PawnWithBadOpinionDied;
		public static ThoughtDef HadAngeringFight;
		public static ThoughtDef HadCatharticFight;
		public static ThoughtDef HarmedMe;
		public static ThoughtDef BotchedMySurgery;
		public static ThoughtDef KilledMyKin;
		public static ThoughtDef CheatedOnMe;
		public static ThoughtDef Rebuffed;
		public static ThoughtDef FailedRomanceAttempt;
		public static ThoughtDef BrokeUpWithMe;
		public static ThoughtDef DivorcedMe;
		public static ThoughtDef RejectedMyProposal;
		public static ThoughtDef IRejectedTheirProposal;
		public static ThoughtDef GotMarried;
		public static ThoughtDef HoneymoonPhase;
		public static ThoughtDef RescuedMe;
		public static ThoughtDef RecruitedMe;
		public static ThoughtDef AttendedWedding;
		public static ThoughtDef AttendedParty;
		public static ThoughtDef CrashedTogether;
		public static ThoughtDef Insulted;
		public static ThoughtDef GotSomeLovin;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<ThoughtDef>(typeof(ThoughtDefOf));
		}
	}
}
