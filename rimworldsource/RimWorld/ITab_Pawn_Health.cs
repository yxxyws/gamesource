using System;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class ITab_Pawn_Health : ITab
	{
		private const float TopPadding = 20f;
		private const int HideBloodLossTicksThreshold = 60000;
		public ITab_Pawn_Health()
		{
			this.size = new Vector2(630f, 430f);
			this.labelKey = "TabHealth";
		}
		protected override void FillTab()
		{
			Pawn pawn = null;
			if (base.SelPawn != null)
			{
				pawn = base.SelPawn;
			}
			else
			{
				Corpse corpse = base.SelThing as Corpse;
				if (corpse != null)
				{
					pawn = corpse.innerPawn;
				}
			}
			if (pawn == null)
			{
				Log.Error("Health tab found no selected pawn to display.");
				return;
			}
			Corpse corpse2 = base.SelThing as Corpse;
			bool showBloodLoss = corpse2 == null || corpse2.Age < 60000;
			bool flag = base.SelThing.def.AllRecipes.Any((RecipeDef x) => x.AvailableNow);
			bool flag2 = !pawn.RaceProps.Humanlike && pawn.Downed;
			bool allowOperations = flag && !pawn.Dead && (pawn.Faction == Faction.OfColony || pawn.IsPrisonerOfColony || flag2);
			Rect outRect = new Rect(0f, 20f, this.size.x, this.size.y - 20f);
			HealthCardUtility.DrawPawnHealthCard(outRect, pawn, allowOperations, showBloodLoss, base.SelThing);
		}
	}
}
