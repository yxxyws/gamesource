using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
namespace Verse
{
	public class RecipeDef : Def
	{
		public Type workerClass;
		public Type workerCounterClass = typeof(RecipeWorkerCounter);
		public string jobString = "Doing an unknown recipe.";
		public float workAmount = -1f;
		public StatDef workSpeedStat;
		public StatDef efficiencyStat;
		public List<IngredientCount> ingredients = new List<IngredientCount>();
		public ThingFilter fixedIngredientFilter = new ThingFilter();
		public ThingFilter defaultIngredientFilter;
		public bool allowMixingIngredients;
		private Type ingredientValueGetterClass = typeof(IngredientValueGetter_Volume);
		public List<ThingCount> products = new List<ThingCount>();
		public List<SpecialProductType> specialProducts;
		public bool productHasIngredientStuff;
		public int targetCountAdjustment = 1;
		public ThingDef unfinishedThingDef;
		public List<SkillRequirement> skillRequirements;
		public SkillDef workSkill;
		public float workSkillLearnFactor = 1f;
		public EffecterDef effectWorking;
		public SoundDef soundWorking;
		public List<ThingDef> recipeUsers;
		public List<BodyPartDef> appliedOnFixedBodyParts = new List<BodyPartDef>();
		public HediffDef addsHediff;
		public HediffDef removesHediff;
		public bool hideBodyPartNames;
		public bool isViolation;
		public string successfullyRemovedHediffMessage;
		public float surgeonSurgerySuccessChanceExponent = 1f;
		public float roomSurgerySuccessChanceFactorExponent = 1f;
		public float surgerySuccessChanceFactor = 1f;
		public float deathOnFailedSurgeryChance;
		public ResearchProjectDef researchPrerequisite;
		[Unsaved]
		private RecipeWorker workerInt;
		[Unsaved]
		private RecipeWorkerCounter workerCounterInt;
		[Unsaved]
		private IngredientValueGetter ingredientValueGetterInt;
		public RecipeWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (RecipeWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.recipe = this;
				}
				return this.workerInt;
			}
		}
		public RecipeWorkerCounter WorkerCounter
		{
			get
			{
				if (this.workerCounterInt == null)
				{
					this.workerCounterInt = (RecipeWorkerCounter)Activator.CreateInstance(this.workerCounterClass);
					this.workerCounterInt.recipe = this;
				}
				return this.workerCounterInt;
			}
		}
		public IngredientValueGetter IngredientValueGetter
		{
			get
			{
				if (this.ingredientValueGetterInt == null)
				{
					this.ingredientValueGetterInt = (IngredientValueGetter)Activator.CreateInstance(this.ingredientValueGetterClass);
				}
				return this.ingredientValueGetterInt;
			}
		}
		public bool AvailableNow
		{
			get
			{
				return this.researchPrerequisite == null || Find.ResearchManager.IsFinished(this.researchPrerequisite);
			}
		}
		public string MinSkillString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				if (this.skillRequirements != null)
				{
					for (int i = 0; i < this.skillRequirements.Count; i++)
					{
						SkillRequirement skillRequirement = this.skillRequirements[i];
						stringBuilder.AppendLine(string.Concat(new object[]
						{
							"   ",
							skillRequirement.skill.skillLabel,
							": ",
							skillRequirement.minLevel
						}));
						flag = true;
					}
				}
				if (!flag)
				{
					stringBuilder.AppendLine("   (" + "NoneLower".Translate() + ")");
				}
				return stringBuilder.ToString();
			}
		}
		public IEnumerable<ThingDef> AllRecipeUsers
		{
			get
			{
				RecipeDef.<>c__Iterator142 <>c__Iterator = new RecipeDef.<>c__Iterator142();
				<>c__Iterator.<>f__this = this;
				RecipeDef.<>c__Iterator142 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public bool UsesUnfinishedThing
		{
			get
			{
				return this.unfinishedThingDef != null;
			}
		}
		public float WorkAmountTotal(ThingDef stuffDef)
		{
			if (this.workAmount >= 0f)
			{
				return this.workAmount;
			}
			return this.products[0].thingDef.GetStatValueAbstract(StatDefOf.WorkToMake, stuffDef);
		}
		[DebuggerHidden]
		public IEnumerable<ThingDef> PotentiallyMissingIngredients(Pawn billDoer)
		{
			RecipeDef.<PotentiallyMissingIngredients>c__Iterator143 <PotentiallyMissingIngredients>c__Iterator = new RecipeDef.<PotentiallyMissingIngredients>c__Iterator143();
			<PotentiallyMissingIngredients>c__Iterator.billDoer = billDoer;
			<PotentiallyMissingIngredients>c__Iterator.<$>billDoer = billDoer;
			<PotentiallyMissingIngredients>c__Iterator.<>f__this = this;
			RecipeDef.<PotentiallyMissingIngredients>c__Iterator143 expr_1C = <PotentiallyMissingIngredients>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		public bool IsIngredient(ThingDef th)
		{
			if (!this.fixedIngredientFilter.Allows(th))
			{
				return false;
			}
			for (int i = 0; i < this.ingredients.Count; i++)
			{
				if (this.ingredients[i].filter.Allows(th))
				{
					return true;
				}
			}
			return false;
		}
		public override void ResolveReferences()
		{
			base.ResolveReferences();
			for (int i = 0; i < this.ingredients.Count; i++)
			{
				this.ingredients[i].ResolveReferences();
			}
			if (this.fixedIngredientFilter != null)
			{
				this.fixedIngredientFilter.ResolveReferences();
			}
			if (this.defaultIngredientFilter == null)
			{
				this.defaultIngredientFilter = new ThingFilter();
				if (this.fixedIngredientFilter != null)
				{
					this.defaultIngredientFilter.CopyFrom(this.fixedIngredientFilter);
				}
			}
			this.defaultIngredientFilter.ResolveReferences();
		}
		public bool PawnSatisfiesSkillRequirements(Pawn pawn)
		{
			return this.skillRequirements == null || !this.skillRequirements.Any((SkillRequirement req) => !req.PawnSatisfies(pawn));
		}
	}
}
