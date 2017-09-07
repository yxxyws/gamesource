using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public abstract class MainTabWindow : Window
	{
		public MainTabDef def;
		public virtual Vector2 RequestedTabSize
		{
			get
			{
				return new Vector2(1010f, 684f);
			}
		}
		public virtual MainTabWindowAnchor Anchor
		{
			get
			{
				return MainTabWindowAnchor.Left;
			}
		}
		public override Vector2 InitialWindowSize
		{
			get
			{
				Vector2 requestedTabSize = this.RequestedTabSize;
				if (requestedTabSize.y > (float)(Screen.height - 35))
				{
					requestedTabSize.y = (float)(Screen.height - 35);
				}
				if (requestedTabSize.x > (float)Screen.width)
				{
					requestedTabSize.x = (float)Screen.width;
				}
				return requestedTabSize;
			}
		}
		public virtual float TabButtonBarPercent
		{
			get
			{
				return 0f;
			}
		}
		public MainTabWindow()
		{
			this.layer = WindowLayer.GameUI;
			this.soundAppear = null;
			this.soundClose = null;
			this.doCloseButton = false;
			this.doCloseX = false;
			this.closeOnEscapeKey = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			if (this.Anchor == MainTabWindowAnchor.Left)
			{
				this.currentWindowRect.x = 0f;
			}
			else
			{
				this.currentWindowRect.x = (float)Screen.width - this.currentWindowRect.width;
			}
			this.currentWindowRect.y = (float)(Screen.height - 35) - this.currentWindowRect.height;
			if (this.def.concept != null)
			{
				ConceptDatabase.KnowledgeDemonstrated(this.def.concept, KnowledgeAmount.GuiFrame);
			}
		}
	}
}
