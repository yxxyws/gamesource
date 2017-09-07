using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Page_SelectWorld : Window
	{
		private const float TitleAreaHeight = 50f;
		private const float BoxMargin = 20f;
		private const float EntrySpacing = 8f;
		private const float EntryMargin = 6f;
		private const float WorldNameExtraLeftMargin = 15f;
		private const float DateExtraLeftMargin = 400f;
		private const float DeleteButtonSpace = 5f;
		private List<SaveFileInfo> worldFiles = new List<SaveFileInfo>();
		private Vector2 scrollPosition = Vector2.zero;
		private Vector2 mapEntrySize;
		private Vector2 interactButSize;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(1020f, 764f);
			}
		}
		public Page_SelectWorld()
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.ScanWorldFiles();
		}
		private void ScanWorldFiles()
		{
			this.worldFiles.Clear();
			foreach (FileInfo current in SavedWorldsDatabase.AllWorldFiles)
			{
				try
				{
					this.worldFiles.Add(new SaveFileInfo(current));
				}
				catch (Exception ex)
				{
					Log.Error("Exception loading " + current.Name + ": " + ex.ToString());
				}
			}
		}
		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0f, 0f, 300f, 300f), "SelectWorld".Translate());
			float num = 50f;
			Rect rect = new Rect(0f, num, inRect.width, inRect.height - num - 50f);
			this.mapEntrySize = new Vector2(rect.width - 16f, 48f);
			this.interactButSize = new Vector2(100f, this.mapEntrySize.y - 12f);
			GUI.BeginGroup(rect);
			float num2 = this.mapEntrySize.y + 8f;
			float height = (float)this.worldFiles.Count * num2;
			Rect viewRect = new Rect(0f, 0f, rect.width - 16f, height);
			Rect outRect = new Rect(rect.AtZero());
			Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);
			float num3 = 0f;
			foreach (SaveFileInfo current in this.worldFiles)
			{
				this.DrawWorldFileEntry(current, num3);
				num3 += this.mapEntrySize.y + 8f;
			}
			if (this.worldFiles.Count == 0)
			{
				Rect rect2 = new Rect(0f, num3, this.mapEntrySize.x, this.mapEntrySize.y);
				Text.Font = GameFont.Small;
				Widgets.Label(rect2, "NoWorldsFilesFound".Translate());
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
			DialogUtility.DoNextBackButtons(inRect, null, null, delegate
			{
				Find.WindowStack.Add(new Page_SelectStoryteller());
				this.Close(true);
			});
		}
		private void DrawWorldFileEntry(SaveFileInfo wfi, float curY)
		{
			Rect rect = new Rect(0f, curY, this.mapEntrySize.x, this.mapEntrySize.y);
			Widgets.DrawMenuSection(rect, true);
			Rect position = rect.ContractedBy(6f);
			GUI.BeginGroup(position);
			Rect rect2 = new Rect(15f, 0f, position.width, position.height);
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.Font = GameFont.Small;
			Widgets.Label(rect2, Path.GetFileNameWithoutExtension(wfi.FileInfo.Name));
			GUI.color = Color.white;
			Rect rect3 = new Rect(400f, 0f, position.width, position.height);
			Dialog_MapList.DrawDateAndVersion(wfi, rect3);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			float num = this.mapEntrySize.x - 12f - this.interactButSize.x - this.interactButSize.y;
			Rect rect4 = new Rect(num, 0f, this.interactButSize.x, this.interactButSize.y);
			if (Widgets.TextButton(rect4, "WorldChooseButton".Translate(), true, false))
			{
				this.SelectWorldFile(wfi.FileInfo);
			}
			Rect rect5 = new Rect(num + this.interactButSize.x + 5f, 0f, this.interactButSize.y, this.interactButSize.y);
			if (Widgets.ImageButton(rect5, TexButton.DeleteX))
			{
				FileInfo localFile = wfi.FileInfo;
				Find.WindowStack.Add(new Dialog_Confirm("ConfirmDelete".Translate(new object[]
				{
					localFile.Name
				}), delegate
				{
					localFile.Delete();
					this.ScanWorldFiles();
					MainMenuDrawer.Notify_WorldFilesChanged();
				}, true));
			}
			TooltipHandler.TipRegion(rect5, "DeleteThisSavegame".Translate());
			GUI.EndGroup();
		}
		private void SelectWorldFile(FileInfo worldFile)
		{
			try
			{
				MapInitData.landingCoords = IntVec2.Invalid;
				WorldLoader.LoadWorldFromFile(worldFile.ToString());
			}
			catch (Exception ex)
			{
				Log.Error("Exception loading world from " + worldFile.Name + ":\n" + ex.ToString());
				Current.World = null;
			}
			if (Current.World != null)
			{
				Find.WindowStack.Add(new Page_SelectLandingSite());
				this.Close(true);
			}
		}
	}
}
