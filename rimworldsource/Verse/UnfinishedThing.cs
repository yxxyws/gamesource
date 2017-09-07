using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public class UnfinishedThing : ThingWithComps
	{
		private const float CancelIngredientRecoveryFraction = 0.75f;
		private Pawn creatorInt;
		private string creatorName = "ErrorCreatorName";
		private RecipeDef recipeInt;
		public List<Thing> ingredients = new List<Thing>();
		private Bill_ProductionWithUft boundBillInt;
		public float workLeft = -10000f;
		public Pawn Creator
		{
			get
			{
				return this.creatorInt;
			}
			set
			{
				if (value == null)
				{
					Log.Error("Cannot set creator to null.");
					return;
				}
				this.creatorInt = value;
				this.creatorName = value.NameStringShort;
			}
		}
		public RecipeDef Recipe
		{
			get
			{
				return this.recipeInt;
			}
		}
		public Bill_ProductionWithUft BoundBill
		{
			get
			{
				if (this.boundBillInt != null && (this.boundBillInt.DeletedOrDereferenced || this.boundBillInt.BoundUft != this))
				{
					this.boundBillInt = null;
				}
				return this.boundBillInt;
			}
			set
			{
				if (value == this.boundBillInt)
				{
					return;
				}
				Bill_ProductionWithUft bill_ProductionWithUft = this.boundBillInt;
				this.boundBillInt = value;
				if (bill_ProductionWithUft != null && bill_ProductionWithUft.BoundUft == this)
				{
					bill_ProductionWithUft.SetBoundUft(null, false);
				}
				if (value != null)
				{
					this.recipeInt = value.recipe;
					if (value.BoundUft != this)
					{
						value.SetBoundUft(this, false);
					}
				}
			}
		}
		public Thing BoundWorkTable
		{
			get
			{
				if (this.BoundBill == null)
				{
					return null;
				}
				IBillGiver billGiver = this.BoundBill.billStack.billGiver;
				Thing thing = billGiver as Thing;
				if (thing.Destroyed)
				{
					return null;
				}
				return thing;
			}
		}
		public override string LabelBase
		{
			get
			{
				if (base.Stuff == null)
				{
					return "UnfinishedItem".Translate(new object[]
					{
						this.Recipe.products[0].thingDef.label
					});
				}
				return "UnfinishedItemWithStuff".Translate(new object[]
				{
					base.Stuff.LabelAsStuff,
					this.Recipe.products[0].thingDef.label
				});
			}
		}
		public bool Initialized
		{
			get
			{
				return this.workLeft > -5000f;
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.Saving && this.boundBillInt != null && this.boundBillInt.DeletedOrDereferenced)
			{
				this.boundBillInt = null;
			}
			Scribe_References.LookReference<Pawn>(ref this.creatorInt, "creator", false);
			Scribe_Values.LookValue<string>(ref this.creatorName, "creatorName", null, false);
			Scribe_References.LookReference<Bill_ProductionWithUft>(ref this.boundBillInt, "bill", false);
			Scribe_Defs.LookDef<RecipeDef>(ref this.recipeInt, "recipe");
			Scribe_Values.LookValue<float>(ref this.workLeft, "workLeft", 0f, false);
			Scribe_Collections.LookList<Thing>(ref this.ingredients, "ingredients", LookMode.Deep, new object[0]);
		}
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			if (mode == DestroyMode.Cancel)
			{
				for (int i = 0; i < this.ingredients.Count; i++)
				{
					this.ingredients[i].stackCount = Mathf.CeilToInt((float)this.ingredients[i].stackCount * 0.75f);
					GenPlace.TryPlaceThing(this.ingredients[i], base.Position, ThingPlaceMode.Near);
				}
				this.ingredients.Clear();
			}
			base.Destroy(mode);
			this.BoundBill = null;
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			UnfinishedThing.<GetGizmos>c__Iterator18C <GetGizmos>c__Iterator18C = new UnfinishedThing.<GetGizmos>c__Iterator18C();
			<GetGizmos>c__Iterator18C.<>f__this = this;
			UnfinishedThing.<GetGizmos>c__Iterator18C expr_0E = <GetGizmos>c__Iterator18C;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public Bill_ProductionWithUft BillOnTableForMe(Thing workTable)
		{
			if (this.Recipe.AllRecipeUsers.Contains(workTable.def))
			{
				IBillGiver billGiver = (IBillGiver)workTable;
				for (int i = 0; i < billGiver.BillStack.Count; i++)
				{
					Bill_ProductionWithUft bill_ProductionWithUft = billGiver.BillStack[i] as Bill_ProductionWithUft;
					if (bill_ProductionWithUft != null)
					{
						if (bill_ProductionWithUft.ShouldDoNow())
						{
							if (bill_ProductionWithUft != null && bill_ProductionWithUft.recipe == this.Recipe)
							{
								return bill_ProductionWithUft;
							}
						}
					}
				}
			}
			return null;
		}
		public override void DrawExtraSelectionOverlays()
		{
			base.DrawExtraSelectionOverlays();
			if (this.BoundWorkTable != null)
			{
				GenDraw.DrawLineBetween(this.TrueCenter(), this.BoundWorkTable.TrueCenter());
			}
		}
		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			string text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"\n",
				"Author".Translate(),
				": ",
				this.creatorName
			});
			text2 = text;
			return string.Concat(new string[]
			{
				text2,
				"\n",
				"WorkLeft".Translate(),
				": ",
				this.workLeft.ToStringWorkAmount()
			});
		}
	}
}
