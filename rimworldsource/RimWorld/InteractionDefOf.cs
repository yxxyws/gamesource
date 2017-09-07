using System;
namespace RimWorld
{
	public static class InteractionDefOf
	{
		public static InteractionDef Chitchat;
		public static InteractionDef DeepTalk;
		public static InteractionDef RomanceAttempt;
		public static InteractionDef MarriageProposal;
		public static InteractionDef PrisonerFriendlyChat;
		public static InteractionDef RecruitAttempt;
		public static InteractionDef AnimalChat;
		public static InteractionDef TrainAttempt;
		public static InteractionDef TameAttempt;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<InteractionDef>(typeof(InteractionDefOf));
		}
	}
}
