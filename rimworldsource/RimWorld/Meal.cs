using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Meal : ThingWithComps
	{
		private const int MaxNumIngredients = 3;
		private List<ThingDef> ingredients = new List<ThingDef>();
		private float poisonPct;
		public override bool IngestibleNow
		{
			get
			{
				return true;
			}
		}
		public float PoisonPercent
		{
			get
			{
				return this.poisonPct;
			}
			set
			{
				this.poisonPct = Mathf.Clamp01(value);
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.poisonPct, "poisonPct", 0f, false);
			Scribe_Collections.LookList<ThingDef>(ref this.ingredients, "ingredients", LookMode.DefReference, new object[0]);
		}
		public void RegisterIngredient(ThingDef def)
		{
			if (!this.ingredients.Contains(def))
			{
				this.ingredients.Add(def);
			}
		}
		public override void Ingested(Pawn ingester, float nutritionWanted)
		{
			if (ingester.needs.mood != null)
			{
				for (int i = 0; i < this.ingredients.Count; i++)
				{
					ThingDef thingDef = this.ingredients[i];
					if (thingDef.ingestible != null)
					{
						if (thingDef.ingestible.ingestedAsIngredientThought != null)
						{
							ingester.needs.mood.thoughts.TryGainThought(thingDef.ingestible.ingestedAsIngredientThought);
						}
						if (ingester.RaceProps.Humanlike && thingDef.ingestible.sourceDef != null && thingDef.ingestible.sourceDef.race != null && thingDef.ingestible.sourceDef.race.Humanlike)
						{
							if (!ingester.story.traits.HasTrait(TraitDefOf.Cannibal))
							{
								ingester.needs.mood.thoughts.TryGainThought(ThoughtDefOf.AteHumanlikeMeatAsIngredient);
							}
							else
							{
								ingester.needs.mood.thoughts.TryGainThought(ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal);
							}
						}
					}
				}
			}
			base.Ingested(ingester, nutritionWanted);
		}
		public override bool TryAbsorbStack(Thing other, bool respectStackLimit)
		{
			int stackCount = other.stackCount;
			if (!base.TryAbsorbStack(other, respectStackLimit))
			{
				return false;
			}
			List<ThingDef> list = ((Meal)other).ingredients;
			for (int i = 0; i < list.Count; i++)
			{
				if (!this.ingredients.Contains(list[i]))
				{
					this.ingredients.Add(list[i]);
				}
			}
			if (this.ingredients.Count > 3)
			{
				this.ingredients.Shuffle<ThingDef>();
				while (this.ingredients.Count > 3)
				{
					this.ingredients.Remove(this.ingredients[this.ingredients.Count - 1]);
				}
			}
			this.poisonPct = GenMath.WeightedAverage(this.poisonPct, (float)this.stackCount, ((Meal)other).poisonPct, (float)stackCount);
			return true;
		}
		public override Thing SplitOff(int count)
		{
			Meal meal = (Meal)base.SplitOff(count);
			if (meal == this)
			{
				return meal;
			}
			for (int i = 0; i < this.ingredients.Count; i++)
			{
				meal.ingredients.Add(this.ingredients[i]);
			}
			meal.poisonPct = this.poisonPct;
			return meal;
		}
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			stringBuilder.Append("Ingredients".Translate() + ": ");
			for (int i = 0; i < this.ingredients.Count; i++)
			{
				stringBuilder.Append(this.ingredients[i].LabelCap);
				if (i < this.ingredients.Count - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
