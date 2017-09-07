using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Dialog_ManageOutfits : Window
	{
		private const float TopAreaHeight = 40f;
		private const float TopButtonHeight = 35f;
		private const float TopButtonWidth = 150f;
		private Vector2 scrollPosition;
		private Outfit selOutfitInt;
		private static ThingFilter apparelGlobalFilter;
		private static Regex validNameRegex = new Regex("^[a-zA-Z0-9 '\\-]*$");
		private Outfit SelectedOutfit
		{
			get
			{
				return this.selOutfitInt;
			}
			set
			{
				this.CheckSelectedOutfitHasName();
				this.selOutfitInt = value;
			}
		}
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(700f, 700f);
			}
		}
		public Dialog_ManageOutfits(Outfit selectedOutfit)
		{
			this.forcePause = true;
			this.doCloseX = true;
			this.closeOnEscapeKey = true;
			this.doCloseButton = true;
			this.closeOnClickedOutside = true;
			this.absorbInputAroundWindow = true;
			if (Dialog_ManageOutfits.apparelGlobalFilter == null)
			{
				Dialog_ManageOutfits.apparelGlobalFilter = new ThingFilter();
				Dialog_ManageOutfits.apparelGlobalFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
			}
			this.SelectedOutfit = selectedOutfit;
		}
		private void CheckSelectedOutfitHasName()
		{
			if (this.SelectedOutfit != null && this.SelectedOutfit.label.NullOrEmpty())
			{
				this.SelectedOutfit.label = "Unnamed";
			}
		}
		public override void DoWindowContents(Rect inRect)
		{
			float num = 0f;
			Rect rect = new Rect(0f, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.TextButton(rect, "SelectOutfit".Translate(), true, false))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (Outfit current in Find.Map.outfitDatabase.AllOutfits)
				{
					Outfit localOut = current;
					list.Add(new FloatMenuOption(localOut.label, delegate
					{
						this.SelectedOutfit = localOut;
					}, MenuOptionPriority.Medium, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			}
			num += 10f;
			Rect rect2 = new Rect(num, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.TextButton(rect2, "NewOutfit".Translate(), true, false))
			{
				this.SelectedOutfit = Find.Map.outfitDatabase.MakeNewOutfit();
			}
			num += 10f;
			Rect rect3 = new Rect(num, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.TextButton(rect3, "DeleteOutfit".Translate(), true, false))
			{
				List<FloatMenuOption> list2 = new List<FloatMenuOption>();
				foreach (Outfit current2 in Find.Map.outfitDatabase.AllOutfits)
				{
					Outfit localOut = current2;
					list2.Add(new FloatMenuOption(localOut.label, delegate
					{
						AcceptanceReport acceptanceReport = Find.Map.outfitDatabase.TryDelete(localOut);
						if (!acceptanceReport.Accepted)
						{
							Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
						}
						else
						{
							if (localOut == this.SelectedOutfit)
							{
								this.SelectedOutfit = null;
							}
						}
					}, MenuOptionPriority.Medium, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list2, false));
			}
			Rect rect4 = new Rect(0f, 40f, inRect.width, inRect.height - 40f - this.CloseButSize.y).ContractedBy(10f);
			if (this.SelectedOutfit == null)
			{
				GUI.color = Color.grey;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect4, "NoOutfitSelected".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
				return;
			}
			GUI.BeginGroup(rect4);
			Rect rect5 = new Rect(0f, 0f, 200f, 30f);
			Dialog_ManageOutfits.DoNameInputRect(rect5, ref this.SelectedOutfit.label, 30);
			Rect rect6 = new Rect(0f, 40f, 300f, rect4.height - 45f - 10f);
			ThingFilterUI.DoThingFilterConfigWindow(rect6, ref this.scrollPosition, this.SelectedOutfit.filter, Dialog_ManageOutfits.apparelGlobalFilter, 16);
			GUI.EndGroup();
		}
		public override void PreClose()
		{
			base.PreClose();
			this.CheckSelectedOutfitHasName();
		}
		public static void DoNameInputRect(Rect rect, ref string name, int maxLength)
		{
			string text = Widgets.TextField(rect, name);
			if (text.Length <= maxLength && Dialog_ManageOutfits.validNameRegex.IsMatch(text))
			{
				name = text;
			}
		}
	}
}
