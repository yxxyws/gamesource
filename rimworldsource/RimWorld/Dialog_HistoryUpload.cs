using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Dialog_HistoryUpload : Window
	{
		private const float optionHeight = 31f;
		private List<DiaOption> options = new List<DiaOption>();
		private string userName = string.Empty;
		private int uploadFrequency;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(620f, 480f);
			}
		}
		public Dialog_HistoryUpload(int uploadFrequency)
		{
			this.uploadFrequency = uploadFrequency;
			this.forcePause = true;
			this.closeOnEscapeKey = true;
			this.doCloseX = true;
			this.absorbInputAroundWindow = true;
			this.userName = Prefs.UserName;
			DiaOption diaOption = new DiaOption("HistoryUploadYes".Translate());
			diaOption.action = delegate
			{
				Prefs.HistoryUpload = HistoryUploadPref.Always;
				Prefs.UserName = this.userName;
				Prefs.Save();
				HistoryUpload.SaveAndUpload();
				this.Close(true);
			};
			diaOption.resolveTree = false;
			this.options.Add(diaOption);
			DiaOption diaOption2 = new DiaOption("HistoryUploadYesOnce".Translate());
			diaOption2.action = delegate
			{
				Prefs.UserName = this.userName;
				Prefs.Save();
				HistoryUpload.SaveAndUpload();
				this.Close(true);
			};
			diaOption2.resolveTree = false;
			this.options.Add(diaOption2);
			DiaOption diaOption3 = new DiaOption("HistoryUploadNo".Translate());
			diaOption3.action = delegate
			{
				Prefs.HistoryUpload = HistoryUploadPref.Never;
				Prefs.Save();
				this.Close(true);
			};
			diaOption3.resolveTree = false;
			this.options.Add(diaOption3);
			DiaOption diaOption4 = new DiaOption("HistoryUploadNoOnce".Translate());
			diaOption4.action = delegate
			{
				this.Close(true);
			};
			diaOption4.resolveTree = false;
			this.options.Add(diaOption4);
		}
		public override void DoWindowContents(Rect inRect)
		{
			Widgets.Label(new Rect(0f, 0f, inRect.width, 220f), new GUIContent("HistoryUploadMessage".Translate(new object[]
			{
				this.uploadFrequency
			})));
			Widgets.Label(new Rect(0f, 220f, inRect.width, 30f), new GUIContent("HistoryUploadMessagePlayerName".Translate()));
			this.userName = Widgets.TextField(new Rect(0f, 260f, 200f, 30f), this.userName);
			float num = 325f;
			foreach (DiaOption current in this.options)
			{
				current.OptOnGUI(new Rect(15f, num, inRect.width - 30f, 31f));
				num += 31f;
			}
		}
	}
}
