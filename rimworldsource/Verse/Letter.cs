using RimWorld;
using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	[StaticConstructorOnStartup]
	public class Letter : IExposable
	{
		public const float DrawWidth = 38f;
		public const float DrawHeight = 30f;
		private const float FallTime = 1f;
		private const float FallDistance = 200f;
		public string label = string.Empty;
		public string text = string.Empty;
		public TargetInfo lookTarget = TargetInfo.Invalid;
		private LetterType letterType;
		public float arrivalTime;
		private static readonly Texture2D LetterIcon = ContentFinder<Texture2D>.Get("UI/Letters/LetterUnopened", true);
		private static readonly Texture2D LetterFlashTex = ContentFinder<Texture2D>.Get("UI/Letters/LetterFlash", true);
		private Texture2D CurIcon
		{
			get
			{
				return Letter.LetterIcon;
			}
		}
		public bool Valid
		{
			get
			{
				return this.lookTarget.Thing == null || !this.lookTarget.Thing.Destroyed;
			}
		}
		public LetterType LetterType
		{
			get
			{
				return this.letterType;
			}
		}
		public Letter()
		{
		}
		public Letter(string label, string text, LetterType gameEventType)
		{
			this.label = label;
			this.text = text;
			this.letterType = gameEventType;
			this.lookTarget = TargetInfo.Invalid;
		}
		public Letter(string label, string text, LetterType gameEventType, TargetInfo lookTarget) : this(label, text, gameEventType)
		{
			this.lookTarget = lookTarget;
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<string>(ref this.label, "label", null, false);
			Scribe_Values.LookValue<string>(ref this.text, "text", null, false);
			Scribe_Values.LookValue<LetterType>(ref this.letterType, "letterType", LetterType.Good, false);
			if (Scribe.mode == LoadSaveMode.Saving && this.lookTarget.HasThing && this.lookTarget.Thing.Destroyed)
			{
				this.lookTarget = this.lookTarget.Cell;
			}
			Scribe_TargetInfo.LookTargetInfo(ref this.lookTarget, "lookTarget");
		}
		public void DrawButtonAt(float topY)
		{
			float num = (float)Screen.width - 38f - 12f;
			Rect rect = new Rect(num, topY, 38f, 30f);
			Rect rect2 = new Rect(rect);
			float num2 = Time.time - this.arrivalTime;
			Color color = this.LetterType.GetColor();
			if (num2 < 1f)
			{
				rect2.y -= (1f - num2) * 200f;
				color.a = num2 / 1f;
			}
			if (!Mouse.IsOver(rect) && this.LetterType == LetterType.BadUrgent && num2 > 15f && Time.time % 5f < 1f)
			{
				float num3 = (float)Screen.width * 0.06f;
				float num4 = 2f * (Time.time % 1f) - 1f;
				rect2.x -= num3 * (1f - num4 * num4);
			}
			if (this.LetterType == LetterType.BadUrgent)
			{
				float num5 = (float)Screen.width * 0.3f * 2f;
				float num6 = Time.time - (this.arrivalTime + 1f);
				if (num6 > 0f && num6 % 16f < 1f)
				{
					Rect position = new Rect(num - num5, topY - num5, num5 * 2f, num5 * 2f);
					float a = Pulser.PulseBrightness(1f, 1f, num6) * 0.55f;
					Color color2 = new Color(1f, 0f, 0f, a);
					GUI.color = color2;
					GUI.DrawTexture(position, Letter.LetterFlashTex);
					GUI.color = Color.white;
				}
			}
			GUI.color = color;
			Widgets.DrawShadowAround(rect2);
			GUI.DrawTexture(rect2, this.CurIcon);
			GUI.color = Color.white;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.UpperCenter;
			Vector2 vector = Text.CalcSize(this.label);
			float x = vector.x;
			float y = vector.y;
			Vector2 vector2 = new Vector2(rect2.x + rect2.width / 2f, rect2.center.y - y / 4f);
			float num7 = vector2.x + x / 2f - (float)(Screen.width - 2);
			if (num7 > 0f)
			{
				vector2.x -= num7;
			}
			Rect position2 = new Rect(vector2.x - x / 2f - 4f - 1f, vector2.y, x + 8f, 12f);
			GUI.DrawTexture(position2, TexUI.GrayTextBG);
			GUI.color = new Color(1f, 1f, 1f, 0.75f);
			Rect rect3 = new Rect(vector2.x - x / 2f, vector2.y - 3f, x, 999f);
			Widgets.Label(rect3, this.label);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.IsOver(rect))
			{
				SoundDefOf.Click.PlayOneShotOnCamera();
				Find.LetterStack.RemoveLetter(this);
				Event.current.Use();
				ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.RightClickClearLetter, KnowledgeAmount.Total);
			}
			if (Widgets.InvisibleButton(rect2))
			{
				this.OpenLetter();
				Event.current.Use();
			}
		}
		public void CheckForMouseOverTextAt(float topY)
		{
			float num = (float)Screen.width - 38f - 12f;
			Rect rect = new Rect(num, topY, 38f, 30f);
			if (Mouse.IsOver(rect))
			{
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.UpperLeft;
				float num2 = Text.CalcHeight(this.text, 310f);
				num2 += 20f;
				Rect infoRect = new Rect(num - 330f - 10f, topY - num2 / 2f, 330f, num2);
				Find.WindowStack.ImmediateWindow(2768333, infoRect, WindowLayer.GameUI, delegate
				{
					Text.Font = GameFont.Small;
					Rect position = infoRect.AtZero().ContractedBy(10f);
					GUI.BeginGroup(position);
					Widgets.Label(new Rect(0f, 0f, position.width, position.height), this.text);
					GUI.EndGroup();
				}, true, false, 1f);
			}
		}
		private void OpenLetter()
		{
			DiaNode diaNode = new DiaNode(this.text);
			DiaOption diaOption = new DiaOption("OK".Translate());
			diaOption.action = delegate
			{
				Find.LetterStack.RemoveLetter(this);
			};
			diaOption.resolveTree = true;
			diaNode.options.Add(diaOption);
			if (this.lookTarget.IsValid)
			{
				DiaOption diaOption2 = new DiaOption("JumpToLocation".Translate());
				diaOption2.action = delegate
				{
					Find.CameraMap.JumpTo(this.lookTarget.Cell);
					Find.LetterStack.RemoveLetter(this);
				};
				diaOption2.resolveTree = true;
				diaNode.options.Add(diaOption2);
			}
			Find.WindowStack.Add(new Dialog_NodeTree(diaNode));
		}
	}
}
