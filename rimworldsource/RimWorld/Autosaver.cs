using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public sealed class Autosaver
	{
		private const int NumAutosaves = 5;
		public const float MaxPermadeathModeAutosaveInterval = 1f;
		private int ticksSinceSave;
		private float AutosaveIntervalDays
		{
			get
			{
				float num = Prefs.AutosaveIntervalDays;
				if (Find.Map.info.permadeathMode && num > 1f)
				{
					num = 1f;
				}
				return num;
			}
		}
		public void AutosaverTick()
		{
			this.ticksSinceSave++;
			if (this.ticksSinceSave / 60000 > 0 && (float)(this.ticksSinceSave / 60000) % this.AutosaveIntervalDays == 0f)
			{
				LongEventHandler.QueueLongEvent(new Action(this.DoAutosave), "Autosaving", false, null);
				this.ticksSinceSave = 0;
			}
		}
		private void DoAutosave()
		{
			ProfilerThreadCheck.BeginSample("DoAutosave");
			MapInfo info = Find.Map.info;
			string fileName;
			if (info.permadeathMode)
			{
				fileName = info.permadeathModeUniqueName;
			}
			else
			{
				fileName = this.NewAutosaveFileName();
			}
			GameDataSaver.SaveGame(Find.Map, fileName);
			ProfilerThreadCheck.EndSample();
		}
		private void DoMemoryCleanup()
		{
			Resources.UnloadUnusedAssets();
		}
		private string NewAutosaveFileName()
		{
			string text = (
				from name in this.AutoSaveNames()
				where !MapFilesUtility.HaveMapNamed(name)
				select name).FirstOrDefault<string>();
			if (text != null)
			{
				return text;
			}
			return this.AutoSaveNames().MinBy((string name) => new FileInfo(GenFilePaths.FilePathForSavedGame(name)).LastWriteTime);
		}
		[DebuggerHidden]
		private IEnumerable<string> AutoSaveNames()
		{
			Autosaver.<AutoSaveNames>c__Iterator11A <AutoSaveNames>c__Iterator11A = new Autosaver.<AutoSaveNames>c__Iterator11A();
			Autosaver.<AutoSaveNames>c__Iterator11A expr_07 = <AutoSaveNames>c__Iterator11A;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
