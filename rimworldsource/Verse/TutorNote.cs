using RimWorld;
using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public class TutorNote : TutorItem, IExposable
	{
		private const float RectWidth = 500f;
		private const float TextWidth = 432f;
		private const float MinDistFromScreenBottom = 60f;
		private const float FadeInDuration = 0.4f;
		private const float DoneButPad = 8f;
		private const float DoneButSize = 32f;
		private const float ExpiryDuration = 2.1f;
		private const float ExpiryFadeTime = 1.1f;
		public ConceptDef def;
		public int curPage;
		public bool doFadeIn = true;
		private float expiryTime = 3.40282347E+38f;
		public override bool Relevant
		{
			get
			{
				return true;
			}
		}
		private string CurText
		{
			get
			{
				return this.def.GetHelpText(this.curPage);
			}
		}
		private bool OnLastPage
		{
			get
			{
				return this.curPage < this.def.HelpTextsCount;
			}
		}
		public bool Expiring
		{
			get
			{
				return this.expiryTime < 3.40282347E+38f;
			}
		}
		private Rect MainRect
		{
			get
			{
				float num = Mathf.Max(60f, GizmoGridDrawer.HeightDrawnRecently + 20f);
				float num2 = Text.CalcHeight(this.CurText, 432f);
				float num3 = num2 + 20f;
				return new Rect((float)(Screen.width / 2) - 250f, (float)Screen.height - num - num3, 500f, num3);
			}
		}
		public TutorNote()
		{
		}
		public TutorNote(ConceptDef concept)
		{
			this.def = concept;
		}
		public void ExposeData()
		{
			Scribe_Defs.LookDef<ConceptDef>(ref this.def, "concept");
		}
		public void InitiateExpiry()
		{
			if (!this.Expiring)
			{
				this.expiryTime = Time.timeSinceLevelLoad + 2.1f;
			}
		}
		public override void TutorItemOnGUI()
		{
			if (Time.timeSinceLevelLoad < 0.01f)
			{
				return;
			}
			Rect mainRect = this.MainRect;
			float alpha = 1f;
			if (this.doFadeIn)
			{
				alpha = (Time.time - ActiveTutorNoteManager.activeNoteStartRealTime) / 0.4f;
				if (alpha > 1f)
				{
					alpha = 1f;
				}
			}
			if (this.Expiring)
			{
				float num = this.expiryTime - Time.timeSinceLevelLoad;
				if (num < 1.1f)
				{
					alpha = num / 1.1f;
				}
			}
			WindowStack arg_C8_0 = Find.WindowStack;
			float alpha2 = alpha;
			arg_C8_0.ImmediateWindow(134706, mainRect, WindowLayer.Super, delegate
			{
				Rect rect = mainRect.AtZero();
				Text.Font = GameFont.Small;
				if (!this.Expiring)
				{
					TutorUIHighlighter.HighlightTag(this.def.highlightTag);
				}
				if (this.doFadeIn || this.Expiring)
				{
					GUI.color = new Color(1f, 1f, 1f, alpha);
				}
				Widgets.DrawWindowBackgroundTutor(rect);
				Rect rect2 = rect.ContractedBy(10f);
				rect2.width = 432f;
				Widgets.Label(rect2, this.CurText);
				Rect butRect = new Rect(rect.xMax - 32f - 8f, rect.y + 8f, 32f, 32f);
				Texture2D tex;
				if (this.Expiring)
				{
					tex = Widgets.CheckboxOnTex;
				}
				else
				{
					tex = ((!this.OnLastPage) ? TexButton.NextBig : TexButton.CloseXBig);
				}
				if (Widgets.ImageButton(butRect, tex))
				{
					SoundDefOf.Click.PlayOneShotOnCamera();
					this.NoteDone();
				}
				if (Time.timeSinceLevelLoad > this.expiryTime)
				{
					this.NoteDone();
				}
				GUI.color = Color.white;
			}, false, false, alpha2);
		}
		private void NoteDone()
		{
			if (!this.OnLastPage)
			{
				this.curPage++;
			}
			else
			{
				KnowledgeAmount know = (!this.def.noteTeaches) ? KnowledgeAmount.NoteClosed : KnowledgeAmount.NoteTaught;
				ConceptDatabase.KnowledgeDemonstrated(this.def, know);
				ActiveTutorNoteManager.activeNote = null;
			}
		}
	}
}
