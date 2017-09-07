using System;
using System.Collections;
using UnityEngine;
namespace Verse
{
	public static class Text
	{
		private const int NumFonts = 3;
		private static GameFont fontInt;
		private static TextAnchor alignmentInt;
		private static bool wordWrapInt;
		public static readonly GUIStyle[] fontStyles;
		public static readonly GUIStyle[] textFieldStyles;
		private static readonly float[] lineHeights;
		public static GameFont Font
		{
			get
			{
				return Text.fontInt;
			}
			set
			{
				if (value == GameFont.Tiny && !LongEventHandler.AnyEventNowOrWaiting && !LanguageDatabase.activeLanguage.info.canBeTiny)
				{
					Text.fontInt = GameFont.Small;
				}
				else
				{
					Text.fontInt = value;
				}
			}
		}
		public static TextAnchor Anchor
		{
			get
			{
				return Text.alignmentInt;
			}
			set
			{
				Text.alignmentInt = value;
			}
		}
		public static bool WordWrap
		{
			get
			{
				return Text.wordWrapInt;
			}
			set
			{
				Text.wordWrapInt = value;
			}
		}
		public static float LineHeight
		{
			get
			{
				return Text.lineHeights[(int)Text.Font];
			}
		}
		internal static GUIStyle CurFontStyle
		{
			get
			{
				GUIStyle gUIStyle = Text.fontStyles[1];
				switch (Text.fontInt)
				{
				case GameFont.Tiny:
					gUIStyle = Text.fontStyles[0];
					break;
				case GameFont.Small:
					gUIStyle = Text.fontStyles[1];
					break;
				case GameFont.Medium:
					gUIStyle = Text.fontStyles[2];
					break;
				default:
					Log.Error("CurFontStyle with undefined curFont.");
					break;
				}
				gUIStyle.alignment = Text.alignmentInt;
				gUIStyle.wordWrap = Text.wordWrapInt;
				return gUIStyle;
			}
		}
		public static GUIStyle CurTextFieldStyle
		{
			get
			{
				GUIStyle result = Text.textFieldStyles[1];
				switch (Text.fontInt)
				{
				case GameFont.Tiny:
					result = Text.textFieldStyles[0];
					break;
				case GameFont.Small:
					result = Text.textFieldStyles[1];
					break;
				case GameFont.Medium:
					result = Text.textFieldStyles[2];
					break;
				default:
					Log.Error("CurFontStyle with undefined curFont.");
					break;
				}
				return result;
			}
		}
		static Text()
		{
			Text.fontInt = GameFont.Small;
			Text.alignmentInt = TextAnchor.UpperLeft;
			Text.wordWrapInt = true;
			Text.fontStyles = new GUIStyle[3];
			Text.textFieldStyles = new GUIStyle[3];
			Text.lineHeights = new float[3];
			Font font = (Font)Resources.Load("Fonts/Calibri_tiny");
			Font font2 = (Font)Resources.Load("Fonts/Arial_small");
			Font font3 = (Font)Resources.Load("Fonts/Arial_medium");
			Text.fontStyles[0] = new GUIStyle(GUI.skin.label);
			Text.fontStyles[0].font = font;
			Text.fontStyles[0].contentOffset = new Vector2(0f, -1f);
			Text.fontStyles[1] = new GUIStyle(GUI.skin.label);
			Text.fontStyles[1].font = font2;
			Text.fontStyles[1].contentOffset = new Vector2(0f, -1f);
			Text.fontStyles[2] = new GUIStyle(GUI.skin.label);
			Text.fontStyles[2].font = font3;
			Text.fontStyles[2].contentOffset = new Vector2(0f, -1f);
			Text.textFieldStyles[0] = new GUIStyle(GUI.skin.textField);
			Text.textFieldStyles[0].font = font;
			Text.textFieldStyles[1] = new GUIStyle(GUI.skin.textField);
			Text.textFieldStyles[1].font = font2;
			Text.textFieldStyles[2] = new GUIStyle(GUI.skin.textField);
			Text.textFieldStyles[2].font = font3;
			GUIStyle[] array = Text.textFieldStyles;
			for (int i = 0; i < array.Length; i++)
			{
				GUIStyle gUIStyle = array[i];
				gUIStyle.alignment = TextAnchor.MiddleLeft;
				gUIStyle.contentOffset = new Vector2(2f, 0f);
			}
			GUI.skin.settings.doubleClickSelectsWord = true;
			int num = 0;
			IEnumerator enumerator = Enum.GetValues(typeof(GameFont)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					GameFont font4 = (GameFont)((byte)enumerator.Current);
					Text.Font = font4;
					float num2 = Text.CalcHeight("W", 999f);
					Text.lineHeights[num] = num2;
					num++;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			Text.Font = GameFont.Small;
		}
		public static float CalcHeight(string text, float width)
		{
			return Text.CurFontStyle.CalcHeight(new GUIContent(text), width);
		}
		public static Vector2 CalcSize(string text)
		{
			return Text.CurFontStyle.CalcSize(new GUIContent(text));
		}
		internal static void StartOfOnGUI()
		{
			if (!Text.WordWrap)
			{
				Log.ErrorOnce("Word wrap was false at end of frame.", 764362);
				Text.WordWrap = true;
			}
			if (Text.Anchor != TextAnchor.UpperLeft)
			{
				Log.ErrorOnce("Alignment was " + Text.Anchor + " at end of frame.", 15558);
				Text.Anchor = TextAnchor.UpperLeft;
			}
			Text.Font = GameFont.Small;
		}
	}
}
