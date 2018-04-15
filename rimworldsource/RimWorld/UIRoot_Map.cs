using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class UIRoot_Map : UIRoot
	{
		public ThingOverlays thingOverlays = new ThingOverlays();
		public Selector selector = new Selector();
		public Targeter targeter = new Targeter();
		public MainTabsRoot mainTabsRoot = new MainTabsRoot();
		private MouseoverReadout mouseoverReadout = new MouseoverReadout();
		public GlobalControls globalControls = new GlobalControls();
		protected ResourceReadout resourceReadout = new ResourceReadout();
		public AlertsReadout alerts = new AlertsReadout();
		public override void Init()
		{
			base.Init();
			Messages.Clear();
		}
		public override void UIRootOnGUI()
		{
			base.UIRootOnGUI();
			this.thingOverlays.ThingOverlaysOnGUI();
			for (int i = 0; i < Find.Map.components.Count; i++)
			{
				Find.Map.components[i].MapComponentOnGUI();
			}
			BeautyDrawer.BeautyOnGUI();
			this.selector.dragBox.DragBoxOnGUI();
			DesignatorManager.DesignationManagerOnGUI();
			this.targeter.TargeterOnGUI();
			Find.TooltipGiverList.DispenseAllThingTooltips();
			Find.ColonyInfo.ColonyInfoOnGUI();
			DebugTools.DebugToolsOnGUI();
			if (!this.screenshotMode.FiltersCurrentEvent)
			{
				this.globalControls.GlobalControlsOnGUI();
				this.resourceReadout.ResourceReadoutOnGUI();
				this.mainTabsRoot.MainTabsOnGUI();
				this.mouseoverReadout.MouseoverReadoutOnGUI();
				this.alerts.AlertsReadoutOnGUI();
				ActiveTutorNoteManager.ActiveLessonManagerOnGUI();
			}
			RoomStatsDrawer.RoomStatsOnGUI();
			Find.DebugDrawer.DebugDrawerOnGUI();
			this.windows.WindowStackOnGUI();
			DesignatorManager.ProcessInputEvents();
			this.targeter.ProcessInputEvents();
			this.mainTabsRoot.HandleLowPriorityShortcuts();
			this.selector.SelectorOnGUI();
			Find.LordManager.LordManagerOnGUI();
			this.OpenMainMenuShortcut();
		}
		public override void UIRootUpdate()
		{
			base.UIRootUpdate();
			try
			{
				Messages.Update();
				this.targeter.TargeterUpdate();
				SelectionDrawer.DrawSelectionOverlays();
				RoomStatsDrawer.DrawRoomOverlays();
				DesignatorManager.DesignatorManagerUpdate();
				this.alerts.AlertsReadoutUpdate();
				ConceptDecider.ConceptDeciderUpdate();
			}
			catch (Exception ex)
			{
				Log.Error("Exception in UIRootUpdate: " + ex.ToString());
			}
		}
		private void OpenMainMenuShortcut()
		{
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				Event.current.Use();
				this.mainTabsRoot.SetCurrentTab(MainTabDefOf.Menu, true);
			}
		}
	}
}
