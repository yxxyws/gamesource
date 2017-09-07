using System;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public abstract class EditWindow : Window
	{
		private const float SuperimposeAvoidThreshold = 8f;
		private const float SuperimposeAvoidOffset = 16f;
		private const float SuperimposeAvoidOffsetMinEdge = 200f;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(500f, 500f);
			}
		}
		protected override float WindowPadding
		{
			get
			{
				return 8f;
			}
		}
		public EditWindow()
		{
			this.resizeable = true;
			this.draggable = true;
			this.preventCameraMotion = false;
			this.doCloseX = true;
			this.currentWindowRect.x = 5f;
			this.currentWindowRect.y = 5f;
		}
		public override void PostOpen()
		{
			while (this.currentWindowRect.x <= (float)Screen.width - 200f && this.currentWindowRect.y <= (float)Screen.height - 200f)
			{
				bool flag = false;
				foreach (EditWindow current in (
					from di in Find.WindowStack.Windows
					where di is EditWindow
					select di).Cast<EditWindow>())
				{
					if (current != this)
					{
						if (Mathf.Abs(current.currentWindowRect.x - this.currentWindowRect.x) < 8f && Mathf.Abs(current.currentWindowRect.y - this.currentWindowRect.y) < 8f)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					return;
				}
				this.currentWindowRect.x = this.currentWindowRect.x + 16f;
				this.currentWindowRect.y = this.currentWindowRect.y + 16f;
			}
		}
	}
}
