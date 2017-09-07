using System;
using Verse;
namespace RimWorld
{
	public static class HediffDefOf
	{
		public static HediffDef Misc;
		public static HediffDef Burn;
		public static HediffDef Cut;
		public static HediffDef SurgicalCut;
		public static HediffDef Stab;
		public static HediffDef Gunshot;
		public static HediffDef Shredded;
		public static HediffDef Bruise;
		public static HediffDef Bite;
		public static HediffDef BrainDamage;
		public static HediffDef MissingBodyPart;
		public static HediffDef BloodLoss;
		public static HediffDef Hypothermia;
		public static HediffDef Heatstroke;
		public static HediffDef Malnutrition;
		public static HediffDef ToxicBuildup;
		public static HediffDef PsychicShock;
		public static HediffDef Anesthetic;
		public static HediffDef Concussion;
		public static HediffDef CryptosleepSickness;
		public static HediffDef FoodPoisoning;
		public static HediffDef Pregnant;
		public static HediffDef Flu;
		public static HediffDef Plague;
		public static HediffDef Malaria;
		public static HediffDef WoundInfection;
		public static HediffDef Alcohol;
		public static HediffDef Hangover;
		public static HediffDef BadBack;
		public static HediffDef Cataract;
		public static HediffDef Frail;
		public static HediffDef PegLeg;
		public static HediffDef Denture;
		public static HediffDef SimpleProstheticLeg;
		public static HediffDef SimpleProstheticArm;
		public static HediffDef BionicEye;
		public static HediffDef BionicArm;
		public static HediffDef BionicLeg;
		public static HediffDef PowerClaw;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<HediffDef>(typeof(HediffDefOf));
		}
	}
}
