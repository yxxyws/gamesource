using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse
{
	public static class GenRecipe
	{
		[DebuggerHidden]
		public static IEnumerable<Thing> MakeRecipeProducts(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient)
		{
			GenRecipe.<MakeRecipeProducts>c__Iterator1A7 <MakeRecipeProducts>c__Iterator1A = new GenRecipe.<MakeRecipeProducts>c__Iterator1A7();
			<MakeRecipeProducts>c__Iterator1A.recipeDef = recipeDef;
			<MakeRecipeProducts>c__Iterator1A.worker = worker;
			<MakeRecipeProducts>c__Iterator1A.dominantIngredient = dominantIngredient;
			<MakeRecipeProducts>c__Iterator1A.ingredients = ingredients;
			<MakeRecipeProducts>c__Iterator1A.<$>recipeDef = recipeDef;
			<MakeRecipeProducts>c__Iterator1A.<$>worker = worker;
			<MakeRecipeProducts>c__Iterator1A.<$>dominantIngredient = dominantIngredient;
			<MakeRecipeProducts>c__Iterator1A.<$>ingredients = ingredients;
			GenRecipe.<MakeRecipeProducts>c__Iterator1A7 expr_3F = <MakeRecipeProducts>c__Iterator1A;
			expr_3F.$PC = -2;
			return expr_3F;
		}
		private static Thing PostProcessProduct(Thing product, RecipeDef recipeDef, Pawn worker)
		{
			CompQuality compQuality = product.TryGetComp<CompQuality>();
			if (compQuality != null)
			{
				if (recipeDef.workSkill == null)
				{
					Log.Error(recipeDef + " needs workSkill because it creates a product with a quality.");
				}
				int level = worker.skills.GetSkill(recipeDef.workSkill).level;
				compQuality.SetQuality(QualityUtility.RandomCreationQuality(level), ArtGenerationContext.Colony);
			}
			CompArt compArt = product.TryGetComp<CompArt>();
			if (compArt != null)
			{
				compArt.JustCreatedBy(worker);
				if (compQuality.Quality >= QualityCategory.Excellent)
				{
					TaleRecorder.RecordTale(TaleDef.Named("CraftedArt"), new object[]
					{
						worker,
						product
					});
				}
			}
			if (product.def.Minifiable)
			{
				product = product.MakeMinified();
			}
			return product;
		}
	}
}
