using System;
using Verse;
namespace RimWorld
{
	public static class BodyPartDefOf
	{
		public static BodyPartDef Heart;
		public static BodyPartDef LeftLeg;
		public static BodyPartDef RightLeg;
		public static BodyPartDef Liver;
		public static BodyPartDef LeftLung;
		public static BodyPartDef RightLung;
		public static BodyPartDef LeftKidney;
		public static BodyPartDef RightKidney;
		public static BodyPartDef Brain;
		public static BodyPartDef LeftEye;
		public static BodyPartDef RightEye;
		public static BodyPartDef LeftArm;
		public static BodyPartDef RightArm;
		public static BodyPartDef Jaw;
		public static BodyPartDef LeftHand;
		public static BodyPartDef RightHand;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<BodyPartDef>(typeof(BodyPartDefOf));
		}
	}
}
