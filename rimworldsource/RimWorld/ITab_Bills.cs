using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class ITab_Bills : ITab
	{
		private float viewHeight = 1000f;
		private Vector2 scrollPosition = default(Vector2);
		private Bill mouseoverBill;
		private static readonly Vector2 WinSize = new Vector2(370f, 480f);
		protected Building_WorkTable SelTable
		{
			get
			{
				return (Building_WorkTable)base.SelThing;
			}
		}
		public ITab_Bills()
		{
			this.size = ITab_Bills.WinSize;
			this.labelKey = "TabBills";
			this.tutorHighlightTag = "TabBills";
		}
		protected override void FillTab()
		{
			ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.GuiFrame);
			Rect rect = new Rect(0f, 0f, ITab_Bills.WinSize.x, ITab_Bills.WinSize.y).ContractedBy(10f);
			Func<List<FloatMenuOption>> recipeOptionsMaker = delegate
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				for (int i = 0; i < this.SelTable.def.AllRecipes.Count; i++)
				{
					if (this.SelTable.def.AllRecipes[i].AvailableNow)
					{
						RecipeDef recipe = this.SelTable.def.AllRecipes[i];
						list.Add(new FloatMenuOption(recipe.LabelCap, delegate
						{
							if (!Find.MapPawns.FreeColonists.Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
							{
								Bill.CreateNoPawnsWithSkillDialog(recipe);
							}
							Bill bill = recipe.MakeNewBill();
							this.SelTable.billStack.AddBill(bill);
						}, MenuOptionPriority.Medium, null, null));
					}
				}
				return list;
			};
			this.mouseoverBill = this.SelTable.billStack.DrawListing(rect, recipeOptionsMaker, ref this.scrollPosition, ref this.viewHeight);
		}
		public override void TabUpdate()
		{
			if (this.mouseoverBill != null)
			{
				this.mouseoverBill.TryDrawIngredientSearchRadiusOnMap(this.SelTable.Position);
				this.mouseoverBill = null;
			}
		}
	}
}
