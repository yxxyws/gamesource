using System;
using UnityEngine;
namespace Verse
{
	public class ImmediateWindow : Window
	{
		public Action doWindowFunc;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return this.currentWindowRect.size;
			}
		}
		protected override float WindowPadding
		{
			get
			{
				return 0f;
			}
		}
		public ImmediateWindow()
		{
			this.doCloseButton = false;
			this.doCloseX = false;
			this.soundAppear = null;
			this.soundClose = null;
			this.closeOnClickedOutside = false;
			this.closeOnEscapeKey = false;
			this.focusWhenOpened = false;
		}
		public override void DoWindowContents(Rect inRect)
		{
			this.doWindowFunc();
		}
	}
}
