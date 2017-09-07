using System;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	public abstract class Bill : ILoadReferenceable, IExposable
	{
		public const int MaxIngredientSearchRadius = 999;
		public const float ButSize = 24f;
		private const float InterfaceBaseHeight = 53f;
		private const float InterfaceStatusLineHeight = 17f;
		[Unsaved]
		public BillStack billStack;
		private int loadID = -1;
		public RecipeDef recipe;
		public bool suspended;
		public ThingFilter ingredientFilter;
		public float ingredientSearchRadius = 999f;
		public int minSkillLevel;
		public bool deleted;
		public int lastIngredientSearchFailTicks = -99999;
		public virtual string Label
		{
			get
			{
				return this.recipe.label;
			}
		}
		public virtual string LabelCap
		{
			get
			{
				return this.Label.CapitalizeFirst();
			}
		}
		public virtual bool CheckIngredientsIfSociallyProper
		{
			get
			{
				return true;
			}
		}
		public virtual bool ShouldBeRemovedBecauseInvalid
		{
			get
			{
				return false;
			}
		}
		protected virtual string StatusString
		{
			get
			{
				return null;
			}
		}
		public bool DeletedOrDereferenced
		{
			get
			{
				if (this.deleted)
				{
					return true;
				}
				Thing thing = this.billStack.billGiver as Thing;
				return thing != null && thing.Destroyed;
			}
		}
		public Bill()
		{
		}
		public Bill(RecipeDef recipe)
		{
			this.recipe = recipe;
			this.ingredientFilter = new ThingFilter();
			this.ingredientFilter.CopyFrom(recipe.defaultIngredientFilter);
			this.loadID = Find.World.uniqueIDsManager.GetNextBillID();
		}
		public virtual void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.loadID, "loadID", 0, false);
			Scribe_Defs.LookDef<RecipeDef>(ref this.recipe, "recipe");
			Scribe_Values.LookValue<bool>(ref this.suspended, "suspended", false, false);
			Scribe_Values.LookValue<float>(ref this.ingredientSearchRadius, "ingredientSearchRadius", 999f, false);
			Scribe_Values.LookValue<int>(ref this.minSkillLevel, "minSkillLevel", 0, false);
			Scribe_Deep.LookDeep<ThingFilter>(ref this.ingredientFilter, "ingredientFilter", new object[0]);
		}
		public virtual bool PawnAllowedToStartAnew(Pawn p)
		{
			return this.recipe.workSkill == null || p.skills.GetSkill(this.recipe.workSkill).level >= this.minSkillLevel;
		}
		public virtual void Notify_PawnDidWork(Pawn p)
		{
		}
		public virtual void Notify_IterationCompleted(Pawn billDoer)
		{
		}
		public abstract bool ShouldDoNow();
		public virtual void Notify_DoBillStarted()
		{
		}
		protected virtual void DrawConfigInterface(Rect rect, Color baseColor)
		{
		}
		public Rect DrawInterface(float x, float y, float width, int index)
		{
			Rect rect = new Rect(x, y, width, 53f);
			if (!this.StatusString.NullOrEmpty())
			{
				rect.height += 17f;
			}
			Color white = Color.white;
			if (!this.ShouldDoNow())
			{
				white = new Color(1f, 0.7f, 0.7f, 0.7f);
			}
			GUI.color = white;
			Text.Font = GameFont.Small;
			if (index % 2 == 0)
			{
				Widgets.DrawAltRect(rect);
			}
			GUI.BeginGroup(rect);
			Rect butRect = new Rect(0f, 0f, 24f, 24f);
			if (this.billStack.IndexOf(this) > 0 && Widgets.ImageButton(butRect, TexButton.ReorderUp, white))
			{
				this.billStack.Reorder(this, -1);
				SoundDefOf.TickHigh.PlayOneShotOnCamera();
			}
			if (this.billStack.IndexOf(this) < this.billStack.Count - 1)
			{
				Rect butRect2 = new Rect(0f, 24f, 24f, 24f);
				if (Widgets.ImageButton(butRect2, TexButton.ReorderDown, white))
				{
					this.billStack.Reorder(this, 1);
					SoundDefOf.TickLow.PlayOneShotOnCamera();
				}
			}
			Rect rect2 = new Rect(28f, 0f, rect.width - 48f - 20f, 48f);
			Widgets.Label(rect2, this.LabelCap);
			this.DrawConfigInterface(rect.AtZero(), white);
			Rect rect3 = new Rect(rect.width - 24f, 0f, 24f, 24f);
			if (Widgets.ImageButton(rect3, TexButton.DeleteX, white))
			{
				this.billStack.Delete(this);
			}
			Rect butRect3 = new Rect(rect3);
			butRect3.x -= butRect3.width + 4f;
			if (Widgets.ImageButton(butRect3, TexButton.Suspend, white))
			{
				this.suspended = !this.suspended;
			}
			if (!this.StatusString.NullOrEmpty())
			{
				Text.Font = GameFont.Tiny;
				Rect rect4 = new Rect(24f, rect.height - 17f, rect.width - 24f, 17f);
				Widgets.Label(rect4, this.StatusString);
			}
			GUI.EndGroup();
			if (this.suspended)
			{
				Text.Font = GameFont.Medium;
				Text.Anchor = TextAnchor.MiddleCenter;
				Rect rect5 = new Rect(rect.x + rect.width / 2f - 70f, rect.y + rect.height / 2f - 20f, 140f, 40f);
				GUI.DrawTexture(rect5, TexUI.GrayTextBG);
				Widgets.Label(rect5, "SuspendedCaps".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
				Text.Font = GameFont.Small;
			}
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			return rect;
		}
		public static void CreateNoPawnsWithSkillDialog(RecipeDef recipe)
		{
			string text = "RecipeRequiresSkills".Translate(new object[]
			{
				recipe.LabelCap
			});
			text += "\n\n";
			text += recipe.MinSkillString;
			Find.WindowStack.Add(Dialog_NodeTree.SimpleNotifyDialog(text));
		}
		public virtual BillStoreMode GetStoreMode()
		{
			return BillStoreMode.BestStockpile;
		}
		public string GetUniqueLoadID()
		{
			return string.Concat(new object[]
			{
				"Bill_",
				this.recipe.defName,
				"_",
				this.loadID
			});
		}
		public override string ToString()
		{
			return this.GetUniqueLoadID();
		}
	}
}
