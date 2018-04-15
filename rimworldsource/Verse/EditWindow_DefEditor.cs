using System;
using UnityEngine;
namespace Verse
{
	internal class EditWindow_DefEditor : EditWindow
	{
		private const float TopAreaHeight = 16f;
		private const float ExtraScrollHeight = 200f;
		public Def def;
		private float viewHeight;
		private Vector2 scrollPosition = default(Vector2);
		private float labelColumnWidth = 140f;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(400f, 600f);
			}
		}
		public EditWindow_DefEditor(Def def)
		{
			this.def = def;
			this.optionalTitle = def.ToString();
		}
		public override void DoWindowContents(Rect inRect)
		{
			if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Escape))
			{
				GUI.FocusControl(string.Empty);
			}
			Rect position = new Rect(0f, 0f, inRect.width, 16f);
			this.labelColumnWidth = GUI.HorizontalSlider(position, this.labelColumnWidth, 0f, inRect.width);
			Rect outRect = inRect.AtZero();
			outRect.yMin += 16f;
			Rect rect = new Rect(0f, 0f, outRect.width - 16f, this.viewHeight);
			Widgets.BeginScrollView(outRect, ref this.scrollPosition, rect);
			Listing_TreeDefs listing_TreeDefs = new Listing_TreeDefs(rect, this.labelColumnWidth);
			TreeNode_Editor node = EditTreeNodeDatabase.RootOf(this.def);
			listing_TreeDefs.DoContentLines(node, 0);
			listing_TreeDefs.End();
			if (Event.current.type == EventType.Layout)
			{
				this.viewHeight = listing_TreeDefs.CurHeight + 200f;
			}
			Widgets.EndScrollView();
		}
	}
}
