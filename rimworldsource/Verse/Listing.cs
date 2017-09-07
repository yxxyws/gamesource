using System;
using UnityEngine;
namespace Verse
{
	public class Listing
	{
		public const float DefaultColumnWidth = 200f;
		public float lineHeight = 20f;
		public float lineSpacing;
		protected Rect listingRect;
		protected float curY;
		private float columnWidthInt = 200f;
		public float CurHeight
		{
			get
			{
				return this.curY;
			}
		}
		protected float ColumnWidth
		{
			get
			{
				return Mathf.Min(this.columnWidthInt, this.listingRect.width);
			}
		}
		public float OverrideColumnWidth
		{
			set
			{
				this.columnWidthInt = value;
			}
		}
		public Listing(Rect rect)
		{
			this.listingRect = rect;
			GUI.BeginGroup(rect);
		}
		protected void EndLine()
		{
			this.curY += this.lineHeight + this.lineSpacing;
		}
		public virtual void End()
		{
			GUI.EndGroup();
		}
		public virtual bool DoImageButton(Texture2D tex, float width, float height)
		{
			bool result = Widgets.ImageButton(new Rect(0f, this.curY, width, height), tex);
			this.curY += height + 4f;
			return result;
		}
	}
}
