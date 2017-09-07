using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	[StaticConstructorOnStartup]
	public static class WidgetsSubtle
	{
		private const int MouseoverContentOffset = 2;
		private static readonly Texture2D ButtonSubtleAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonSubtleAtlas", true);
		private static readonly Texture2D ButtonBarTex;
		static WidgetsSubtle()
		{
			// Note: this type is marked as 'beforefieldinit'.
			ColorInt colorInt = new ColorInt(78, 109, 129, 130);
			WidgetsSubtle.ButtonBarTex = SolidColorMaterials.NewSolidColorTexture(colorInt.ToColor);
		}
		public static bool ButtonSubtle(Rect rect, string label, float barPercent = 0f, float textLeftMargin = -1f, SoundDef mouseoverSound = null)
		{
			bool flag = false;
			if (Mouse.IsOver(rect))
			{
				flag = true;
				GUI.color = GenUI.MouseoverColor;
			}
			if (mouseoverSound != null)
			{
				MouseoverSounds.DoRegion(rect, mouseoverSound);
			}
			Widgets.DrawAtlas(rect, WidgetsSubtle.ButtonSubtleAtlas);
			GUI.color = Color.white;
			if (barPercent > 0.001f)
			{
				Rect rect2 = rect.ContractedBy(1f);
				Widgets.FillableBar(rect2, barPercent, WidgetsSubtle.ButtonBarTex, null, false);
			}
			Rect rect3 = new Rect(rect);
			if (textLeftMargin < 0f)
			{
				textLeftMargin = rect.width * 0.15f;
			}
			rect3.x += textLeftMargin;
			if (flag)
			{
				rect3.x += 2f;
				rect3.y -= 2f;
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;
			Text.Font = GameFont.Small;
			Widgets.Label(rect3, label);
			Text.Anchor = TextAnchor.UpperLeft;
			Text.WordWrap = true;
			return Widgets.InvisibleButton(rect);
		}
	}
}
