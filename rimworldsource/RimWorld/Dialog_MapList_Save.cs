using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Dialog_MapList_Save : Dialog_MapList
	{
		protected const float NewSaveNameWidth = 400f;
		protected const float NewSaveHeight = 35f;
		protected const float NewSaveNameButtonSpace = 20f;
		private string savingName = string.Empty;
		private bool focusedMapNameArea;
		public Dialog_MapList_Save()
		{
			this.interactButLabel = "OverwriteButton".Translate();
			this.bottomAreaHeight = 85f;
			if (Find.Map.colonyInfo.ColonyHasName)
			{
				this.savingName = Find.Map.colonyInfo.ColonyName;
			}
			else
			{
				this.savingName = MapFilesUtility.UnusedDefaultName();
			}
		}
		protected override void DoMapEntryInteraction(string mapName)
		{
			LongEventHandler.QueueLongEvent(delegate
			{
				GameDataSaver.SaveGame(Find.Map, mapName);
			}, "SavingLongEvent", false, null);
			Messages.Message("SavedAs".Translate(new object[]
			{
				mapName
			}), MessageSound.Silent);
			this.Close(true);
		}
		protected override void DoSpecialSaveLoadGUI(Rect inRect)
		{
			GUI.BeginGroup(inRect);
			bool flag = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;
			float top = inRect.height - 52f;
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.SetNextControlName("MapNameField");
			Rect rect = new Rect(5f, top, 400f, 35f);
			string str = Widgets.TextField(rect, this.savingName);
			if (GenText.IsValidFilename(str))
			{
				this.savingName = str;
			}
			if (!this.focusedMapNameArea)
			{
				GUI.FocusControl("MapNameField");
				this.focusedMapNameArea = true;
			}
			Rect rect2 = new Rect(420f, top, inRect.width - 400f - 20f, 35f);
			if (Widgets.TextButton(rect2, "SaveGameButton".Translate(), true, false) || flag)
			{
				if (this.savingName.NullOrEmpty())
				{
					Messages.Message("NeedAName".Translate(), MessageSound.RejectInput);
				}
				else
				{
					LongEventHandler.QueueLongEvent(delegate
					{
						GameDataSaver.SaveGame(Find.Map, this.savingName);
					}, "SavingLongEvent", false, null);
					Messages.Message("SavedAs".Translate(new object[]
					{
						this.savingName
					}), MessageSound.Silent);
					this.Close(true);
				}
			}
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.EndGroup();
		}
	}
}
