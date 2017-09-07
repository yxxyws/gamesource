using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public abstract class Dialog_MapList : Window
	{
		protected const float BoxMargin = 20f;
		protected const float MapEntrySpacing = 3f;
		protected const float MapEntryMargin = 1f;
		protected const float MapNameExtraLeftMargin = 15f;
		protected const float MapInfoExtraLeftMargin = 270f;
		protected const float DeleteButtonSpace = 5f;
		protected const float MapEntryHeight = 36f;
		protected string interactButLabel = "Error";
		protected float bottomAreaHeight;
		private List<SaveFileInfo> mapFiles = new List<SaveFileInfo>();
		private Vector2 scrollPosition = Vector2.zero;
		private static readonly Color ManualSaveTextColor = new Color(1f, 1f, 0.6f);
		private static readonly Color AutosaveTextColor = new Color(0.75f, 0.75f, 0.75f);
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(600f, 700f);
			}
		}
		public Dialog_MapList()
		{
			this.closeOnEscapeKey = true;
			this.doCloseButton = true;
			this.doCloseX = true;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.ReloadMapFiles();
		}
		protected void ReloadMapFiles()
		{
			this.mapFiles.Clear();
			foreach (FileInfo current in MapFilesUtility.AllMapFiles)
			{
				try
				{
					this.mapFiles.Add(new SaveFileInfo(current));
				}
				catch (Exception ex)
				{
					Log.Error("Exception loading " + current.Name + ": " + ex.ToString());
				}
			}
		}
		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new Vector2(inRect.width - 16f, 36f);
			Vector2 vector2 = new Vector2(100f, vector.y - 2f);
			inRect.height -= 45f;
			float num = vector.y + 3f;
			float height = (float)this.mapFiles.Count * num;
			Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, height);
			Rect outRect = new Rect(inRect.AtZero());
			outRect.height -= this.bottomAreaHeight;
			Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (SaveFileInfo current in this.mapFiles)
			{
				Rect rect = new Rect(0f, num2, vector.x, vector.y);
				if (num3 % 2 == 0)
				{
					Widgets.DrawAltRect(rect);
				}
				Rect position = rect.ContractedBy(1f);
				GUI.BeginGroup(position);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(current.FileInfo.Name);
				if (MapFilesUtility.IsAutoSave(fileNameWithoutExtension))
				{
					GUI.color = Dialog_MapList.AutosaveTextColor;
				}
				else
				{
					GUI.color = Dialog_MapList.ManualSaveTextColor;
				}
				Rect rect2 = new Rect(15f, 0f, position.width, position.height);
				Text.Anchor = TextAnchor.MiddleLeft;
				Text.Font = GameFont.Small;
				Widgets.Label(rect2, fileNameWithoutExtension);
				GUI.color = Color.white;
				Rect rect3 = new Rect(270f, 0f, 200f, position.height);
				Dialog_MapList.DrawDateAndVersion(current, rect3);
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperLeft;
				Text.Font = GameFont.Small;
				float num4 = vector.x - 2f - vector2.x - vector2.y;
				Rect rect4 = new Rect(num4, 0f, vector2.x, vector2.y);
				if (Widgets.TextButton(rect4, this.interactButLabel, true, false))
				{
					this.DoMapEntryInteraction(Path.GetFileNameWithoutExtension(current.FileInfo.Name));
				}
				Rect rect5 = new Rect(num4 + vector2.x + 5f, 0f, vector2.y, vector2.y);
				if (Widgets.ImageButton(rect5, TexButton.DeleteX))
				{
					FileInfo localFile = current.FileInfo;
					Find.WindowStack.Add(new Dialog_Confirm("ConfirmDelete".Translate(new object[]
					{
						localFile.Name
					}), delegate
					{
						localFile.Delete();
						this.ReloadMapFiles();
					}, true));
				}
				TooltipHandler.TipRegion(rect5, "DeleteThisSavegame".Translate());
				GUI.EndGroup();
				num2 += vector.y + 3f;
				num3++;
			}
			Widgets.EndScrollView();
			this.DoSpecialSaveLoadGUI(inRect.AtZero());
		}
		public override void PostClose()
		{
			if (Game.Mode == GameMode.MapPlaying)
			{
				Find.MainTabsRoot.SetCurrentTab(MainTabDefOf.Menu, true);
			}
		}
		protected virtual void DoSpecialSaveLoadGUI(Rect inRect)
		{
		}
		protected abstract void DoMapEntryInteraction(string mapName);
		public static void DrawDateAndVersion(SaveFileInfo sfi, Rect rect)
		{
			GUI.BeginGroup(rect);
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect2 = new Rect(0f, 2f, rect.width, rect.height / 2f);
			GUI.color = SaveFileInfo.UnimportantTextColor;
			Widgets.Label(rect2, sfi.FileInfo.LastWriteTime.ToString("g"));
			Rect rect3 = new Rect(0f, rect2.yMax, rect.width, rect.height / 2f);
			GUI.color = sfi.VersionColor;
			Widgets.Label(rect3, sfi.GameVersion);
			TooltipHandler.TipRegion(rect3, sfi.CompatibilityTip);
			GUI.EndGroup();
		}
	}
}
