using System;
using Verse;
namespace RimWorld
{
	public static class RecipeDefOf
	{
		public static RecipeDef RemoveBodyPart;
		public static RecipeDef RemoveMechanoidBodyPart;
		public static RecipeDef CookMealSimple;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<RecipeDef>(typeof(RecipeDefOf));
		}
	}
}
