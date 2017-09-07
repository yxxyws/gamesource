using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	public class MainTabsRoot
	{
		private List<MainTabDef> allTabs;
		private int TabButtonsCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.allTabs.Count; i++)
				{
					if (this.allTabs[i].showTabButton)
					{
						num++;
					}
				}
				return num;
			}
		}
		public MainTabDef OpenTab
		{
			get
			{
				MainTabWindow mainTabWindow = Find.WindowStack.WindowOfType<MainTabWindow>();
				if (mainTabWindow == null)
				{
					return null;
				}
				return mainTabWindow.def;
			}
		}
		public MainTabsRoot()
		{
			this.allTabs = (
				from x in DefDatabase<MainTabDef>.AllDefs
				orderby x.order
				select x).ToList<MainTabDef>();
		}
		public void MainTabsOnGUI()
		{
			this.DoTabButtons();
			for (int i = 0; i < this.allTabs.Count; i++)
			{
				if (this.allTabs[i].toggleHotKey != null && this.allTabs[i].toggleHotKey.KeyDownEvent)
				{
					this.ToggleTab(this.allTabs[i], true);
					Event.current.Use();
					break;
				}
			}
		}
		public void HandleLowPriorityShortcuts()
		{
			if (this.OpenTab == MainTabDefOf.Inspect && Find.Selector.NumSelected == 0)
			{
				this.EscapeCurrentTab(false);
			}
			if (Find.Selector.NumSelected == 0 && Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				Event.current.Use();
				this.ToggleTab(MainTabDefOf.Architect, true);
			}
			if (this.OpenTab != MainTabDefOf.Inspect && Event.current.type == EventType.MouseDown && Event.current.button != 2)
			{
				this.EscapeCurrentTab(true);
				Find.Selector.ClearSelection();
			}
		}
		public void EscapeCurrentTab(bool playSound = true)
		{
			this.SetCurrentTab(null, playSound);
		}
		public void SetCurrentTab(MainTabDef tab, bool playSound = true)
		{
			if (tab == this.OpenTab)
			{
				return;
			}
			this.ToggleTab(tab, playSound);
		}
		public void ToggleTab(MainTabDef newTab, bool playSound = true)
		{
			MainTabDef openTab = this.OpenTab;
			if (openTab == null && newTab == null)
			{
				return;
			}
			if (openTab == newTab)
			{
				Find.WindowStack.TryRemove(openTab.Window, true);
				if (playSound)
				{
					SoundDefOf.TabClose.PlayOneShotOnCamera();
				}
			}
			else
			{
				if (openTab != null)
				{
					Find.WindowStack.TryRemove(openTab.Window, true);
				}
				if (newTab != null)
				{
					Find.WindowStack.Add(newTab.Window);
				}
				if (playSound)
				{
					if (newTab == null)
					{
						SoundDefOf.TabClose.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.TabOpen.PlayOneShotOnCamera();
					}
				}
			}
		}
		private void DoTabButtons()
		{
			GUI.color = Color.white;
			int tabButtonsCount = this.TabButtonsCount;
			int num = (int)((float)Screen.width / (float)tabButtonsCount);
			int num2 = this.allTabs.FindLastIndex((MainTabDef x) => x.showTabButton);
			int num3 = 0;
			for (int i = 0; i < this.allTabs.Count; i++)
			{
				if (this.allTabs[i].showTabButton)
				{
					int num4 = num;
					if (i == num2)
					{
						num4 = Screen.width - num3;
					}
					this.DoTabButton(this.allTabs[i], (float)num3, (float)num4);
					num3 += num;
				}
			}
		}
		private void DoTabButton(MainTabDef def, float posX, float width)
		{
			Text.Font = GameFont.Small;
			Rect rect = new Rect(posX, (float)(Screen.height - 35), width, 35f);
			SoundDef mouseoverButtonCategory = SoundDefOf.MouseoverButtonCategory;
			if (WidgetsSubtle.ButtonSubtle(rect, def.LabelCap, def.Window.TabButtonBarPercent, -1f, mouseoverButtonCategory))
			{
				this.ToggleTab(def, true);
			}
			if (!def.tutorHighlightTag.NullOrEmpty())
			{
				TutorUIHighlighter.HighlightOpportunity(def.tutorHighlightTag, rect);
			}
			if (!def.description.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, def.description);
			}
		}
	}
}
