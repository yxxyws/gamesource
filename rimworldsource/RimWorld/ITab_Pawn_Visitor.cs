using System;
using System.Collections;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public abstract class ITab_Pawn_Visitor : ITab
	{
		private const float CheckboxInterval = 30f;
		private const float CheckboxMargin = 50f;
		public ITab_Pawn_Visitor()
		{
			this.size = new Vector2(280f, 450f);
		}
		protected override void FillTab()
		{
			ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.PrisonerTab, KnowledgeAmount.GuiFrame);
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 0f, this.size.x, this.size.y);
			Rect rect2 = rect.ContractedBy(10f);
			bool isPrisonerOfColony = base.SelPawn.IsPrisonerOfColony;
			Listing_Standard listing_Standard = new Listing_Standard(rect2);
			bool getsFood = base.SelPawn.guest.GetsFood;
			listing_Standard.DoLabelCheckbox("GetsFood".Translate(), ref getsFood, null);
			base.SelPawn.guest.GetsFood = getsFood;
			Rect rect3 = listing_Standard.GetRect(28f);
			rect3.width = 140f;
			MedicalCareUtility.MedicalCareSetter(rect3, ref base.SelPawn.playerSettings.medCare);
			listing_Standard.DoGap(4f);
			if (isPrisonerOfColony)
			{
				listing_Standard.DoLabel("RecruitmentDifficulty".Translate() + ": " + base.SelPawn.guest.RecruitDifficulty.ToString("##0"));
				if (Prefs.DevMode)
				{
					listing_Standard.DoLabel("Dev: Prison break MTB days: " + (int)PrisonBreakUtility.InitiatePrisonBreakMtbDays(base.SelPawn));
				}
				Rect rect4 = listing_Standard.GetRect(200f);
				Widgets.DrawMenuSection(rect4, true);
				Rect position = rect4.ContractedBy(10f);
				GUI.BeginGroup(position);
				Rect rect5 = new Rect(0f, 0f, position.width, 30f);
				IEnumerator enumerator = Enum.GetValues(typeof(PrisonerInteractionMode)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						PrisonerInteractionMode prisonerInteractionMode = (PrisonerInteractionMode)((byte)enumerator.Current);
						if (Widgets.LabelRadioButton(rect5, prisonerInteractionMode.GetLabel(), base.SelPawn.guest.interactionMode == prisonerInteractionMode))
						{
							base.SelPawn.guest.interactionMode = prisonerInteractionMode;
						}
						rect5.y += 28f;
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				GUI.EndGroup();
			}
			listing_Standard.End();
		}
	}
}
