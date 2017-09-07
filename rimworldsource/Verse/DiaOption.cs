using System;
using UnityEngine;
namespace Verse
{
	public class DiaOption
	{
		public Window dialog;
		protected string text;
		public DiaNode link;
		public Func<DiaNode> linkLateBind;
		public bool resolveTree;
		public Action action;
		public bool disabled;
		public string disabledReason = string.Empty;
		public bool playClickSound = true;
		protected readonly Color DisabledOptionColor = new Color(1f, 0.5f, 0.5f);
		public static DiaOption DefaultOK
		{
			get
			{
				return new DiaOption("OK".Translate())
				{
					resolveTree = true
				};
			}
		}
		protected Dialog_NodeTree OwningDialog
		{
			get
			{
				return (Dialog_NodeTree)this.dialog;
			}
		}
		public DiaOption()
		{
			this.text = "OK".Translate();
		}
		public DiaOption(string text)
		{
			this.text = text;
		}
		public DiaOption(DiaOptionMold def)
		{
			this.text = def.Text;
			DiaNodeMold diaNodeMold = def.RandomLinkNode();
			if (diaNodeMold != null)
			{
				this.link = new DiaNode(diaNodeMold);
			}
		}
		public void Disable(string newDisabledReason)
		{
			this.disabled = true;
			this.disabledReason = newDisabledReason;
		}
		public void OptOnGUI(Rect drawRect)
		{
			string text = this.text;
			if (this.disabled)
			{
				GUI.color = this.DisabledOptionColor;
				text = text + " (" + this.disabledReason + ")";
			}
			if (Widgets.TextButton(drawRect, text, false, false))
			{
				this.Activate();
			}
		}
		protected void Activate()
		{
			if (this.resolveTree)
			{
				this.OwningDialog.Close(true);
			}
			if (this.action != null)
			{
				this.action();
			}
			if (this.linkLateBind != null)
			{
				this.OwningDialog.GotoNode(this.linkLateBind());
			}
			else
			{
				if (this.link != null)
				{
					this.OwningDialog.GotoNode(this.link);
				}
			}
		}
	}
}
