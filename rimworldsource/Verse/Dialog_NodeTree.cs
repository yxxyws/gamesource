using System;
using UnityEngine;
namespace Verse
{
	public class Dialog_NodeTree : Window
	{
		private const int OptionHeight = 31;
		private Vector2 scrollPosition;
		protected DiaNode curNode;
		public Action closeAction;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(620f, 480f);
			}
		}
		public Dialog_NodeTree(DiaNode nodeRoot)
		{
			this.GotoNode(nodeRoot);
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
		}
		public override void PreClose()
		{
			base.PreClose();
			this.curNode.PreClose();
		}
		public override void PostClose()
		{
			base.PostClose();
			if (this.closeAction != null)
			{
				this.closeAction();
			}
		}
		public override void DoWindowContents(Rect inRect)
		{
			this.DrawNode(inRect.AtZero());
		}
		protected void DrawNode(Rect drawRect)
		{
			GUI.BeginGroup(drawRect);
			Text.Font = GameFont.Small;
			int num = Mathf.Max(3, this.curNode.options.Count);
			float num2 = drawRect.height - (float)(31 * num);
			Rect outRect = new Rect(0f, 0f, drawRect.width, num2 - 8f);
			float width = drawRect.width - 16f;
			Rect rect = new Rect(0f, 0f, width, Text.CalcHeight(this.curNode.text, width));
			Widgets.BeginScrollView(outRect, ref this.scrollPosition, rect);
			Widgets.Label(rect, this.curNode.text);
			Widgets.EndScrollView();
			foreach (DiaOption current in this.curNode.options)
			{
				current.OptOnGUI(new Rect(15f, num2, drawRect.width - 30f, 31f));
				num2 += 31f;
			}
			GUI.EndGroup();
		}
		public void GotoNode(DiaNode node)
		{
			foreach (DiaOption current in node.options)
			{
				current.dialog = this;
			}
			this.curNode = node;
		}
		public static Window SimpleNotifyDialog(string msg)
		{
			DiaNode diaNode = new DiaNode(msg);
			diaNode.options.Add(new DiaOption("OK".Translate()));
			diaNode.options[0].resolveTree = true;
			return new Dialog_NodeTree(diaNode);
		}
	}
}
