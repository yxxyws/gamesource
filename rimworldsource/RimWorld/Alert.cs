using System;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public abstract class Alert
	{
		public const float Width = 154f;
		private const float TextWidth = 148f;
		public const float Height = 28f;
		private const float ItemPeekWidth = 30f;
		public const float InfoRectWidth = 330f;
		protected string baseLabel;
		protected string baseExplanation;
		protected float lastBellTime = -1000f;
		private AlertArrow alertArrow;
		private static readonly Texture2D AlertBGTex = SolidColorMaterials.NewSolidColorTexture(Color.white);
		private static readonly Texture2D AlertBGTexHighlight = TexUI.HighlightTex;
		public abstract AlertPriority Priority
		{
			get;
		}
		public abstract AlertReport Report
		{
			get;
		}
		public virtual Color ArrowColor
		{
			get
			{
				return Color.white;
			}
		}
		protected virtual Color BGColor
		{
			get
			{
				return Color.clear;
			}
		}
		public bool Active
		{
			get
			{
				return this.Report.active;
			}
		}
		public virtual string FullLabel
		{
			get
			{
				return this.baseLabel;
			}
		}
		public virtual string FullExplanation
		{
			get
			{
				return this.baseExplanation;
			}
		}
		public void Notify_Started()
		{
			if (this.Priority >= AlertPriority.High)
			{
				if (this.alertArrow == null)
				{
					this.alertArrow = new AlertArrow(this);
				}
				this.alertArrow.DoAlertStartEffect();
				if (Time.timeSinceLevelLoad > 1f && Time.realtimeSinceStartup > this.lastBellTime + 0.5f)
				{
					SoundDefOf.TinyBell.PlayOneShotOnCamera();
					this.lastBellTime = Time.realtimeSinceStartup;
				}
			}
		}
		public virtual void AlertActiveUpdate()
		{
		}
		public virtual Rect DrawAt(float topY, bool minimized)
		{
			Text.Font = GameFont.Small;
			string fullLabel = this.FullLabel;
			float height = Text.CalcHeight(fullLabel, 148f);
			Rect rect = new Rect((float)Screen.width - 154f, topY, 154f, height);
			if (this.alertArrow != null)
			{
				this.alertArrow.AlertArrowOnGUI(rect.x, rect.y);
			}
			GUI.color = this.BGColor;
			GUI.DrawTexture(rect, Alert.AlertBGTex);
			GUI.color = Color.white;
			GUI.BeginGroup(rect);
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(new Rect(0f, 0f, 148f, height), fullLabel);
			GUI.EndGroup();
			if (Mouse.IsOver(rect))
			{
				GUI.DrawTexture(rect, Alert.AlertBGTexHighlight);
			}
			if (this.Report.culprit != null && Widgets.InvisibleButton(rect))
			{
				JumpToTargetUtility.TryJumpAndSelect(this.Report.culprit);
			}
			Text.Anchor = TextAnchor.UpperLeft;
			return rect;
		}
		public void DrawInfoPane()
		{
			Alert.<DrawInfoPane>c__AnonStorey1EB <DrawInfoPane>c__AnonStorey1EB = new Alert.<DrawInfoPane>c__AnonStorey1EB();
			<DrawInfoPane>c__AnonStorey1EB.<>f__this = this;
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			<DrawInfoPane>c__AnonStorey1EB.expString = this.FullExplanation;
			if (this.Report.culprit != null)
			{
				<DrawInfoPane>c__AnonStorey1EB.expString = <DrawInfoPane>c__AnonStorey1EB.expString + "\n\n(" + "ClickToJumpToProblem".Translate() + ")";
			}
			float num = Text.CalcHeight(<DrawInfoPane>c__AnonStorey1EB.expString, 310f);
			num += 20f;
			<DrawInfoPane>c__AnonStorey1EB.infoRect = new Rect((float)Screen.width - 154f - 330f - 8f, Mathf.Max(Mathf.Min(Event.current.mousePosition.y, (float)Screen.height - num), 0f), 330f, num);
			if (<DrawInfoPane>c__AnonStorey1EB.infoRect.yMax > (float)Screen.height)
			{
				Alert.<DrawInfoPane>c__AnonStorey1EB expr_E4_cp_0 = <DrawInfoPane>c__AnonStorey1EB;
				expr_E4_cp_0.infoRect.y = expr_E4_cp_0.infoRect.y - ((float)Screen.height - <DrawInfoPane>c__AnonStorey1EB.infoRect.yMax);
			}
			if (<DrawInfoPane>c__AnonStorey1EB.infoRect.y < 0f)
			{
				<DrawInfoPane>c__AnonStorey1EB.infoRect.y = 0f;
			}
			Find.WindowStack.ImmediateWindow(138956, <DrawInfoPane>c__AnonStorey1EB.infoRect, WindowLayer.GameUI, delegate
			{
				Text.Font = GameFont.Small;
				Rect rect = <DrawInfoPane>c__AnonStorey1EB.infoRect.AtZero();
				if (<DrawInfoPane>c__AnonStorey1EB.<>f__this.Priority == AlertPriority.Tutorial)
				{
					Widgets.DrawWindowBackgroundTutor(rect);
				}
				else
				{
					Widgets.DrawWindowBackground(rect);
				}
				Rect position = rect.ContractedBy(10f);
				GUI.BeginGroup(position);
				Widgets.Label(new Rect(0f, 0f, position.width, position.height), <DrawInfoPane>c__AnonStorey1EB.expString);
				GUI.EndGroup();
			}, false, false, 1f);
		}
	}
}
