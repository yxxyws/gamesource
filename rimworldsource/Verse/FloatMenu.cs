using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public class FloatMenu : Window
	{
		private const float ChoiceSpacing = -1f;
		private const float MaxScreenHeightPercent = 0.9f;
		public bool givesColonistOrders;
		public bool vanishIfMouseDistant = true;
		protected List<FloatMenuOption> options;
		private string title;
		private bool needSelection;
		private Color baseColor = Color.white;
		private static readonly Vector2 TitleOffset = new Vector2(30f, -25f);
		private static readonly Vector2 DefaultChoiceSize = new Vector2(300f, 30f);
		private float MaxScreenHeight
		{
			get
			{
				return (float)Screen.height * 0.9f;
			}
		}
		private float TotalHeight
		{
			get
			{
				float num = 0f;
				float num2 = 0f;
				for (int i = 0; i < this.options.Count; i++)
				{
					float optionHeight = this.GetOptionHeight(this.options[i]);
					if (num2 + optionHeight + -1f > this.MaxScreenHeight)
					{
						if (num2 > num)
						{
							num = num2;
						}
						num2 = optionHeight;
					}
					else
					{
						num2 += optionHeight + -1f;
					}
				}
				return Mathf.Max(num, num2);
			}
		}
		private float TotalWidth
		{
			get
			{
				return (float)this.ColumnsCount * FloatMenu.DefaultChoiceSize.x;
			}
		}
		private int ColumnsCount
		{
			get
			{
				if (this.options == null)
				{
					return 1;
				}
				Text.Font = GameFont.Small;
				int num = 1;
				float num2 = 0f;
				for (int i = 0; i < this.options.Count; i++)
				{
					float optionHeight = this.GetOptionHeight(this.options[i]);
					if (num2 + optionHeight + -1f > this.MaxScreenHeight)
					{
						num2 = optionHeight;
						num++;
					}
					else
					{
						num2 += optionHeight + -1f;
					}
				}
				return num;
			}
		}
		private Vector2 ListRoot
		{
			get
			{
				return new Vector2(4f, 0f);
			}
		}
		private Rect OverRect
		{
			get
			{
				return new Rect(this.ListRoot.x, this.ListRoot.y, this.TotalWidth, this.TotalHeight);
			}
		}
		public override Vector2 InitialWindowSize
		{
			get
			{
				if (this.options.NullOrEmpty<FloatMenuOption>())
				{
					return new Vector2(0f, 0f);
				}
				return new Vector2(this.TotalWidth, this.TotalHeight + 1f);
			}
		}
		protected override float WindowPadding
		{
			get
			{
				return 0f;
			}
		}
		protected override WindowInitialPosition InitialPosition
		{
			get
			{
				return WindowInitialPosition.OnMouse;
			}
		}
		public FloatMenu(List<FloatMenuOption> options, bool showAboveMouse = false)
		{
			this.options = options;
			this.layer = WindowLayer.Super;
			this.closeOnClickedOutside = true;
			this.doWindowBackground = false;
			this.drawShadow = false;
			this.currentWindowRect.size = this.InitialWindowSize;
			if (this.currentWindowRect.xMax > (float)Screen.width)
			{
				this.currentWindowRect.x = (float)Screen.width - this.currentWindowRect.width;
			}
			if (this.currentWindowRect.yMax > (float)Screen.height)
			{
				this.currentWindowRect.y = (float)Screen.height - this.currentWindowRect.height;
			}
			SoundInfo info = SoundInfo.OnCamera(MaintenanceType.None);
			info.SetParameter("NumChoices", (float)options.Count);
			SoundDefOf.FloatMenuOpen.PlayOneShot(info);
			MouseoverSounds.SilenceForNextFrame();
		}
		public FloatMenu(List<FloatMenuOption> options, string title, bool needSelection = false, bool showAbove = false) : this(options, showAbove)
		{
			this.title = title;
			this.needSelection = needSelection;
		}
		public override void ExtraOnGUI()
		{
			base.ExtraOnGUI();
			if (!this.title.NullOrEmpty())
			{
				Vector2 listRoot = this.ListRoot;
				listRoot.x += this.currentWindowRect.x;
				listRoot.y += this.currentWindowRect.y;
				Text.Font = GameFont.Small;
				float width = Mathf.Max(150f, 15f + Text.CalcSize(this.title).x);
				Rect titleRect = new Rect(listRoot.x + FloatMenu.TitleOffset.x, listRoot.y + FloatMenu.TitleOffset.y, width, 23f);
				Find.WindowStack.ImmediateWindow(6830963, titleRect, WindowLayer.Super, delegate
				{
					GUI.color = this.baseColor;
					Text.Font = GameFont.Small;
					Rect position = titleRect.AtZero();
					position.width = 150f;
					GUI.DrawTexture(position, TexUI.TextBGBlack);
					Rect rect = titleRect.AtZero();
					rect.x += 15f;
					Text.Anchor = TextAnchor.MiddleLeft;
					Widgets.Label(rect, this.title);
					Text.Anchor = TextAnchor.UpperLeft;
				}, false, false, 0f);
			}
		}
		public override void DoWindowContents(Rect inRect)
		{
			if (this.needSelection && Find.Selector.SingleSelectedThing == null)
			{
				Find.WindowStack.TryRemove(this, true);
				return;
			}
			this.UpdateBaseColor();
			GUI.color = this.baseColor;
			Vector2 listRoot = this.ListRoot;
			Text.Font = GameFont.Small;
			Vector2 vector = new Vector2(0f, 0f);
			foreach (FloatMenuOption current in 
				from op in this.options
				orderby op.priority descending
				select op)
			{
				float optionHeight = this.GetOptionHeight(current);
				if (vector.y + optionHeight + -1f > this.MaxScreenHeight)
				{
					vector.y = 0f;
					vector.x += FloatMenu.DefaultChoiceSize.x;
				}
				Rect rect = new Rect(listRoot.x + vector.x, listRoot.y + vector.y, FloatMenu.DefaultChoiceSize.x, optionHeight);
				vector.y += optionHeight + -1f;
				bool flag = current.OptionOnGUI(rect, this.baseColor);
				if (flag)
				{
					current.Chosen(this.givesColonistOrders);
					Find.WindowStack.TryRemove(this, true);
					return;
				}
			}
			if (Event.current.type == EventType.MouseDown)
			{
				Event.current.Use();
				this.Close(true);
			}
			GUI.color = Color.white;
		}
		public void Cancel()
		{
			SoundDefOf.FloatMenuCancel.PlayOneShotOnCamera();
			Find.WindowStack.TryRemove(this, true);
		}
		private float GetOptionHeight(FloatMenuOption opt)
		{
			Vector2 textRectSize = FloatMenuOption.GetTextRectSize(FloatMenu.DefaultChoiceSize);
			return Mathf.Max(Text.CalcHeight(opt.label.TrimEnd(new char[0]), textRectSize.x), FloatMenu.DefaultChoiceSize.y);
		}
		private void UpdateBaseColor()
		{
			this.baseColor = Color.white;
			if (this.vanishIfMouseDistant)
			{
				Rect r = this.OverRect.ContractedBy(-12f);
				if (!r.Contains(Event.current.mousePosition))
				{
					float num = GenUI.DistFromRect(r, Event.current.mousePosition);
					this.baseColor = new Color(1f, 1f, 1f, 1f - num / 200f);
					if (num > 200f)
					{
						this.Close(false);
						this.Cancel();
						return;
					}
				}
			}
		}
	}
}
