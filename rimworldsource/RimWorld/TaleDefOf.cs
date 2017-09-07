using System;
namespace RimWorld
{
	public static class TaleDefOf
	{
		public static TaleDef FinishedResearchProject;
		public static TaleDef ExecutedPrisoner;
		public static TaleDef KidnappedColonist;
		public static TaleDef SoldPrisoner;
		public static TaleDef TamedAnimal;
		public static TaleDef TrainedAnimal;
		public static TaleDef AteRawHumanlikeMeat;
		public static TaleDef WalkedNaked;
		public static TaleDef ButcheredHumanlikeCorpse;
		public static TaleDef WasOnFire;
		public static TaleDef KilledBy;
		public static TaleDef KilledColonist;
		public static TaleDef KilledColonyAnimal;
		public static TaleDef KilledMajorColonyEnemy;
		public static TaleDef BondedWithAnimal;
		public static TaleDef BecameLover;
		public static TaleDef Marriage;
		public static TaleDef Breakup;
		public static TaleDef RaidArrived;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<TaleDef>(typeof(TaleDefOf));
		}
	}
}
