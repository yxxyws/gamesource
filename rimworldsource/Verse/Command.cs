using RimWorld;
using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	[StaticConstructorOnStartup]
	public abstract class Command : Gizmo
	{
		public string defaultLabel;
		public string defaultDesc = "No description.";
		public Texture2D icon;
		public Vector2 iconProportions = Vector2.one;
		public Rect iconTexCoords = new Rect(0f, 0f, 1f, 1f);
		public float iconDrawScale = 1f;
		public KeyBindingDef hotKey;
		public SoundDef activateSound;
		public int groupKey;
		public string tutorHighlightTag;
		private static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG", true);
		public virtual string Label
		{
			get
			{
				return this.defaultLabel;
			}
		}
		public virtual string LabelCap
		{
			get
			{
				return this.Label.CapitalizeFirst();
			}
		}
		public virtual string Desc
		{
			get
			{
				return this.defaultDesc;
			}
		}
		protected virtual Color IconDrawColor
		{
			get
			{
				return Color.white;
			}
		}
		public virtual SoundDef CurActivateSound
		{
			get
			{
				return this.activateSound;
			}
		}
		protected virtual bool DoTooltip
		{
			get
			{
				return true;
			}
		}
		public override float Width
		{
			get
			{
				return 75f;
			}
		}
		public override GizmoResult GizmoOnGUI(Vector2 topLeft)
		{
			Rect rect = new Rect(topLeft.x, topLeft.y, this.Width, 75f);
			bool flag = false;
			if (Mouse.IsOver(rect))
			{
				flag = true;
				GUI.color = GenUI.MouseoverColor;
			}
			Texture2D badTex = this.icon;
			if (badTex == null)
			{
				badTex = BaseContent.BadTex;
			}
			GUI.DrawTexture(rect, Command.BGTex);
			MouseoverSounds.DoRegion(rect, SoundDefOf.MouseoverButtonCommand);
			GUI.color = this.IconDrawColor;
			Widgets.DrawTextureFitted(new Rect(rect), badTex, this.iconDrawScale * 0.85f, this.iconProportions, this.iconTexCoords);
			GUI.color = Color.white;
			bool flag2 = false;
			KeyCode keyCode = (this.hotKey != null) ? this.hotKey.MainKey : KeyCode.None;
			if (keyCode != KeyCode.None && !GizmoGridDrawer.drawnHotKeys.Contains(keyCode))
			{
				Rect rect2 = new Rect(rect.x + 5f, rect.y + 5f, 16f, 18f);
				Widgets.Label(rect2, keyCode.ToString());
				GizmoGridDrawer.drawnHotKeys.Add(keyCode);
				if (this.hotKey.KeyDownEvent)
				{
					flag2 = true;
					Event.current.Use();
				}
			}
			if (Widgets.InvisibleButton(rect))
			{
				flag2 = true;
			}
			string labelCap = this.LabelCap;
			if (!labelCap.NullOrEmpty())
			{
				float num = Text.CalcHeight(labelCap, rect.width);
				num -= 2f;
				Rect rect3 = new Rect(rect.x, rect.yMax - num + 12f, rect.width, num);
				GUI.DrawTexture(rect3, TexUI.GrayTextBG);
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperCenter;
				Widgets.Label(rect3, labelCap);
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
			}
			GUI.color = Color.white;
			if (this.DoTooltip)
			{
				TipSignal tip = this.Desc;
				if (this.disabled && !this.disabledReason.NullOrEmpty())
				{
					tip.text = tip.text + "\n\nDISABLED: " + this.disabledReason;
				}
				TooltipHandler.TipRegion(rect, tip);
			}
			if (!this.tutorHighlightTag.NullOrEmpty())
			{
				TutorUIHighlighter.HighlightOpportunity(this.tutorHighlightTag, rect);
			}
			if (flag2)
			{
				if (this.disabled)
				{
					if (!this.disabledReason.NullOrEmpty())
					{
						Messages.Message(this.disabledReason, MessageSound.RejectInput);
					}
					return new GizmoResult(GizmoState.Mouseover, null);
				}
				return new GizmoResult(GizmoState.Interacted, Event.current);
			}
			else
			{
				if (flag)
				{
					return new GizmoResult(GizmoState.Mouseover, null);
				}
				return new GizmoResult(GizmoState.Clear, null);
			}
		}
		public override bool GroupsWith(Gizmo other)
		{
			Command command = other as Command;
			return command != null && ((this.hotKey == command.hotKey && this.defaultLabel == command.defaultLabel && this.icon == command.icon) || (this.groupKey != 0 && command.groupKey != 0 && this.groupKey == command.groupKey));
		}
		public override void ProcessInput(Event ev)
		{
			if (this.CurActivateSound != null)
			{
				this.CurActivateSound.PlayOneShotOnCamera();
			}
		}
		public override int GetHashCode()
		{
			int seed = 0;
			seed = Gen.HashCombine<KeyBindingDef>(seed, this.hotKey);
			seed = Gen.HashCombine<Texture2D>(seed, this.icon);
			return Gen.HashCombine<string>(seed, this.defaultDesc);
		}
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Command(label=",
				this.defaultLabel,
				", defaultDesc=",
				this.defaultDesc,
				")"
			});
		}
	}
}
