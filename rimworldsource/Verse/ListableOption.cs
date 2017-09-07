using System;
using UnityEngine;
namespace Verse
{
	public class ListableOption
	{
		public string label;
		public Action action;
		public float minHeight = 45f;
		public ListableOption(string label, Action action)
		{
			this.label = label;
			this.action = action;
		}
		public virtual float DrawOption(Vector2 pos, float width)
		{
			float b = Text.CalcHeight(this.label, width);
			float num = Mathf.Max(this.minHeight, b);
			Rect rect = new Rect(pos.x, pos.y, width, num);
			if (Widgets.TextButton(rect, this.label, true, true))
			{
				this.action();
			}
			return num;
		}
	}
}
