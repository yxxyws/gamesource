using RimWorld;
using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public abstract class Window
	{
		protected const float StandardMargin = 18f;
		public WindowLayer layer = WindowLayer.Dialog;
		public string optionalTitle;
		public bool doCloseX;
		public bool doCloseButton;
		public bool closeOnEscapeKey = true;
		public bool closeOnClickedOutside;
		public bool forcePause;
		public bool preventCameraMotion;
		public bool preventDrawTutor;
		public bool doWindowBackground = true;
		public bool onlyOneOfTypeAllowed = true;
		public bool absorbInputAroundWindow;
		public bool resizeable;
		public bool draggable;
		public bool drawShadow = true;
		public bool focusWhenOpened = true;
		public float shadowAlpha = 1f;
		public SoundDef soundAppear;
		public SoundDef soundClose;
		public SoundDef soundAmbient;
		protected readonly Vector2 CloseButSize = new Vector2(120f, 40f);
		public int ID;
		public Rect currentWindowRect;
		private Sustainer sustainerAmbient;
		private WindowResizer windowResizer;
		private bool resizeLater;
		private Rect resizeLaterRect;
		public virtual Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(500f, 500f);
			}
		}
		protected virtual float WindowPadding
		{
			get
			{
				return 18f;
			}
		}
		protected virtual WindowInitialPosition InitialPosition
		{
			get
			{
				return WindowInitialPosition.Center;
			}
		}
		public Window()
		{
			if (this.InitialPosition == WindowInitialPosition.Center)
			{
				this.currentWindowRect = new Rect(((float)Screen.width - this.InitialWindowSize.x) / 2f, ((float)Screen.height - this.InitialWindowSize.y) / 2f, this.InitialWindowSize.x, this.InitialWindowSize.y);
			}
			else
			{
				if (this.InitialPosition == WindowInitialPosition.OnMouse)
				{
					Vector2 vector = GenUI.AbsMousePosition();
					if (vector.x + this.InitialWindowSize.x > (float)Screen.width)
					{
						vector.x = (float)Screen.width - this.InitialWindowSize.x;
					}
					if (vector.y + this.InitialWindowSize.y > (float)Screen.height)
					{
						vector.y = (float)Screen.height - this.InitialWindowSize.y;
					}
					this.currentWindowRect = new Rect(vector.x, vector.y, this.InitialWindowSize.x, this.InitialWindowSize.y);
				}
			}
			this.soundAppear = SoundDefOf.DialogBoxAppear;
			this.soundClose = SoundDefOf.Click;
		}
		public virtual void WindowUpdate()
		{
			if (this.sustainerAmbient != null)
			{
				this.sustainerAmbient.Maintain();
			}
		}
		public abstract void DoWindowContents(Rect inRect);
		public virtual void ExtraOnGUI()
		{
		}
		public virtual void PreOpen()
		{
			if (Game.Mode == GameMode.MapPlaying && this.layer == WindowLayer.Dialog)
			{
				DesignatorManager.Dragger.EndDrag();
				DesignatorManager.Deselect();
				Find.Selector.Notify_DialogOpened();
			}
		}
		public virtual void PostOpen()
		{
			if (this.soundAppear != null)
			{
				this.soundAppear.PlayOneShotOnCamera();
			}
			if (this.soundAmbient != null)
			{
				this.sustainerAmbient = this.soundAmbient.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerFrame));
			}
		}
		public virtual void PreClose()
		{
		}
		public virtual void PostClose()
		{
		}
		public void WindowOnGUI()
		{
			if (this.resizeable)
			{
				if (this.windowResizer == null)
				{
					this.windowResizer = new WindowResizer();
				}
				if (this.resizeLater)
				{
					this.resizeLater = false;
					this.currentWindowRect = this.resizeLaterRect;
				}
			}
			Rect winRect = this.currentWindowRect.AtZero();
			this.currentWindowRect = GUI.Window(this.ID, this.currentWindowRect, delegate(int x)
			{
				Find.WindowStack.currentlyDrawnWindow = this;
				if (this.doWindowBackground)
				{
					Widgets.DrawWindowBackground(winRect);
				}
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
				{
					Find.WindowStack.Notify_PressedEscape();
				}
				if (Event.current.type == EventType.MouseDown)
				{
					Find.WindowStack.Notify_ClickedInsideWindow(this);
				}
				if (Event.current.type == EventType.KeyDown && !Find.WindowStack.GetsInput(this))
				{
					Event.current.Use();
				}
				if (!this.optionalTitle.NullOrEmpty())
				{
					GUI.Label(new Rect(this.WindowPadding, this.WindowPadding, this.currentWindowRect.width, 25f), this.optionalTitle);
				}
				if (this.doCloseX && Widgets.CloseButtonFor(winRect))
				{
					this.Close(true);
				}
				if (this.resizeable && Event.current.type != EventType.Repaint)
				{
					Rect lhs = this.windowResizer.DoResizeControl(this.currentWindowRect);
					if (lhs != this.currentWindowRect)
					{
						this.resizeLater = true;
						this.resizeLaterRect = lhs;
					}
				}
				Rect rect = winRect.ContractedBy(this.WindowPadding);
				if (!this.optionalTitle.NullOrEmpty())
				{
					rect.yMin += this.WindowPadding + 25f;
				}
				GUI.BeginGroup(rect);
				try
				{
					this.DoWindowContents(rect.AtZero());
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
						"Exception filling window for ",
						this.GetType().ToString(),
						": ",
						ex
					}));
				}
				GUI.EndGroup();
				if (this.resizeable && Event.current.type == EventType.Repaint)
				{
					this.windowResizer.DoResizeControl(this.currentWindowRect);
				}
				if (this.doCloseButton)
				{
					Text.Font = GameFont.Small;
					Rect rect2 = new Rect(winRect.width / 2f - this.CloseButSize.x / 2f, winRect.height - 55f, this.CloseButSize.x, this.CloseButSize.y);
					if (Widgets.TextButton(rect2, "CloseButton".Translate(), true, false))
					{
						this.Close(true);
					}
				}
				if (this.closeOnEscapeKey && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Escape || Event.current.keyCode == KeyCode.Return))
				{
					this.Close(true);
					Event.current.Use();
				}
				if (this.draggable)
				{
					GUI.DragWindow();
				}
				else
				{
					if (Event.current.type == EventType.MouseDown)
					{
						Event.current.Use();
					}
				}
				Find.WindowStack.currentlyDrawnWindow = null;
			}, string.Empty, Widgets.EmptyStyle);
		}
		public virtual void Close(bool doCloseSound = true)
		{
			Find.WindowStack.TryRemove(this, doCloseSound);
		}
	}
}
