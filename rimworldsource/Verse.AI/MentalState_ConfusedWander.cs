using RimWorld;
using System;
namespace Verse.AI
{
	public class MentalState_ConfusedWander : MentalState
	{
		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}
	}
}
