using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Dialog_ManageAreas : Window
	{
		private static Regex validNameRegex = new Regex("^[a-zA-Z0-9 '\\-]*$");
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(700f, 700f);
			}
		}
		public Dialog_ManageAreas()
		{
			this.forcePause = true;
			this.doCloseX = true;
			this.closeOnEscapeKey = true;
			this.doCloseButton = true;
			this.closeOnClickedOutside = true;
			this.absorbInputAroundWindow = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			Listing_Standard listing_Standard = new Listing_Standard(inRect);
			listing_Standard.OverrideColumnWidth = inRect.width;
			List<Area> allAreas = Find.AreaManager.AllAreas;
			for (int i = 0; i < allAreas.Count; i++)
			{
				if (allAreas[i].Mutable)
				{
					Rect rect = listing_Standard.GetRect(24f);
					Dialog_ManageAreas.DoAreaRow(rect, allAreas[i]);
					listing_Standard.DoGap(6f);
				}
			}
			listing_Standard.OverrideColumnWidth = inRect.width / 2f;
			if (Find.AreaManager.CanMakeNewAllowed(AllowedAreaMode.Humanlike) && listing_Standard.DoTextButton("NewArea".Translate()))
			{
				Area_Allowed area_Allowed;
				Find.AreaManager.TryMakeNewAllowed(AllowedAreaMode.Humanlike, out area_Allowed);
			}
			if (Find.AreaManager.CanMakeNewAllowed(AllowedAreaMode.Animal) && listing_Standard.DoTextButton("NewAreaAnimal".Translate()))
			{
				Area_Allowed area_Allowed2;
				Find.AreaManager.TryMakeNewAllowed(AllowedAreaMode.Animal, out area_Allowed2);
			}
			listing_Standard.End();
		}
		private static void DoAreaRow(Rect rect, Area area)
		{
			if (Mouse.IsOver(rect))
			{
				area.MarkForDraw();
				GUI.color = area.Color;
				Widgets.DrawHighlight(rect);
				GUI.color = Color.white;
			}
			GUI.BeginGroup(rect);
			WidgetRow widgetRow = new WidgetRow(0f, 0f, UIDirection.RightThenUp, 2000f, 29f);
			widgetRow.DoIcon(area.ColorTexture);
			widgetRow.DoGap(4f);
			widgetRow.DoLabel(area.Label, 220f);
			if (widgetRow.DoTextButton("Rename".Translate(), null, true, false))
			{
				Find.WindowStack.Add(new Dialog_RenameArea(area));
			}
			if (widgetRow.DoTextButton("InvertArea".Translate(), null, true, false))
			{
				area.Invert();
			}
			if (widgetRow.DoIconButton(TexButton.DeleteX, null))
			{
				area.Delete();
			}
			GUI.EndGroup();
		}
		public static void DoNameInputRect(Rect rect, ref string name, int maxLength)
		{
			string text = Widgets.TextField(rect, name);
			if (text.Length <= maxLength && Dialog_ManageAreas.validNameRegex.IsMatch(text))
			{
				name = text;
			}
		}
	}
}
