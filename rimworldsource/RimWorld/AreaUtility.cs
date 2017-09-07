using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace RimWorld
{
	public static class AreaUtility
	{
		public static void MakeAllowedAreaListFloatMenu(Action<Area> selAction, AllowedAreaMode mode, bool addNullAreaOption, bool addManageOption)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			if (addNullAreaOption)
			{
				list.Add(new FloatMenuOption("NoAreaAllowed".Translate(), delegate
				{
					selAction(null);
				}, MenuOptionPriority.High, null, null));
			}
			foreach (Area current in 
				from a in Find.AreaManager.AllAreas
				where a.AssignableAsAllowed(mode)
				select a)
			{
				Area localArea = current;
				Action mouseoverGuiAction = delegate
				{
					localArea.MarkForDraw();
				};
				FloatMenuOption item = new FloatMenuOption(localArea.Label, delegate
				{
					selAction(localArea);
				}, MenuOptionPriority.Medium, mouseoverGuiAction, null);
				list.Add(item);
			}
			if (addManageOption)
			{
				list.Add(new FloatMenuOption("ManageAreas".Translate(), delegate
				{
					Find.WindowStack.Add(new Dialog_ManageAreas());
				}, MenuOptionPriority.Low, null, null));
			}
			Find.WindowStack.Add(new FloatMenu(list, false));
		}
		public static string AreaAllowedLabel(Pawn pawn)
		{
			if (pawn.playerSettings != null)
			{
				return AreaUtility.AreaAllowedLabel_Area(pawn.playerSettings.AreaRestriction);
			}
			return AreaUtility.AreaAllowedLabel_Area(null);
		}
		public static string AreaAllowedLabel_Area(Area area)
		{
			if (area != null)
			{
				return area.Label;
			}
			return "NoAreaAllowed".Translate();
		}
	}
}
