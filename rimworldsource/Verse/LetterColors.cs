using System;
using UnityEngine;
namespace Verse
{
	public static class LetterColors
	{
		private static readonly Color LetterColorGood;
		private static readonly Color LetterColorBadNonUrgent;
		private static readonly Color LetterColorBadUrgent;
		static LetterColors()
		{
			// Note: this type is marked as 'beforefieldinit'.
			ColorInt colorInt = new ColorInt(120, 176, 216);
			LetterColors.LetterColorGood = colorInt.ToColor;
			LetterColors.LetterColorBadNonUrgent = new Color(0.8f, 0.77f, 0.53f);
			LetterColors.LetterColorBadUrgent = new Color(0.8f, 0.45f, 0.45f);
		}
		public static Color GetColor(this LetterType g)
		{
			switch (g)
			{
			case LetterType.Good:
				return LetterColors.LetterColorGood;
			case LetterType.BadNonUrgent:
				return LetterColors.LetterColorBadNonUrgent;
			case LetterType.BadUrgent:
				return LetterColors.LetterColorBadUrgent;
			default:
				return Color.white;
			}
		}
	}
}
