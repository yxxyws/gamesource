using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Verse
{
	public static class GenText
	{
		private const int SaveNameMaxLength = 28;
		public static string Possessive(this Pawn p)
		{
			if (p.gender == Gender.Male)
			{
				return "Prohis".Translate();
			}
			return "Proher".Translate();
		}
		public static string PossessiveCap(this Pawn p)
		{
			if (p.gender == Gender.Male)
			{
				return "ProhisCap".Translate();
			}
			return "ProherCap".Translate();
		}
		public static string ProObj(this Pawn p)
		{
			if (p.gender == Gender.Male)
			{
				return "ProhimObj".Translate();
			}
			return "ProherObj".Translate();
		}
		public static string ProObjCap(this Pawn p)
		{
			if (p.gender == Gender.Male)
			{
				return "ProhimObjCap".Translate();
			}
			return "ProherObjCap".Translate();
		}
		public static string ProSubj(this Pawn p)
		{
			if (p.gender == Gender.Male)
			{
				return "Prohe".Translate();
			}
			return "Proshe".Translate();
		}
		public static string ProSubjCap(this Pawn p)
		{
			if (p.gender == Gender.Male)
			{
				return "ProheCap".Translate();
			}
			return "ProsheCap".Translate();
		}
		public static string AdjustedFor(this string text, Pawn p)
		{
			return text.Replace("NAME", p.NameStringShort).Replace("HISCAP", p.PossessiveCap()).Replace("HIMCAP", p.ProObjCap()).Replace("HECAP", p.ProSubjCap()).Replace("HIS", p.Possessive()).Replace("HIM", p.ProObj()).Replace("HE", p.ProSubj());
		}
		public static string LabelIndefinite(this Pawn pawn)
		{
			if (pawn.Name != null && !pawn.Name.Numerical)
			{
				return pawn.LabelBaseShort;
			}
			string str = Find.ActiveLanguageWorker.WithIndefiniteArticle(GenLabel.BestKindLabel(pawn, false, false));
			return Find.ActiveLanguageWorker.PostProcessed(str);
		}
		public static string LabelDefinite(this Pawn pawn)
		{
			if (pawn.Name != null && !pawn.Name.Numerical)
			{
				return pawn.LabelBaseShort;
			}
			string str = Find.ActiveLanguageWorker.WithDefiniteArticle(GenLabel.BestKindLabel(pawn, false, false));
			return Find.ActiveLanguageWorker.PostProcessed(str);
		}
		public static void SetTextSizeToFit(string text, Rect r)
		{
			Text.Font = GameFont.Small;
			float num = Text.CalcHeight(text, r.width);
			if (num > r.height)
			{
				Text.Font = GameFont.Tiny;
			}
		}
		public static string TrimEndNewlines(this string s)
		{
			return s.TrimEnd(new char[]
			{
				'\r',
				'\n'
			});
		}
		public static string Indented(this string s)
		{
			if (s.NullOrEmpty())
			{
				return s;
			}
			return "    " + s.Replace("\r", string.Empty).Replace("\n", "\n    ");
		}
		public static string ReplaceFirst(this string source, string key, string replacement)
		{
			int num = source.IndexOf(key);
			if (num < 0)
			{
				return source;
			}
			return source.Substring(0, num) + replacement + source.Substring(num + key.Length);
		}
		public static int StableStringHash(string str)
		{
			if (str == null)
			{
				return 0;
			}
			int num = 23;
			int length = str.Length;
			for (int i = 0; i < length; i++)
			{
				num = num * 31 + (int)str[i];
			}
			return num;
		}
		public static string StringFromEnumerable(IEnumerable source)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object current in source)
			{
				stringBuilder.AppendLine("� " + current.ToString());
			}
			return stringBuilder.ToString();
		}
		[DebuggerHidden]
		public static IEnumerable<string> LinesFromString(string text)
		{
			GenText.<LinesFromString>c__Iterator1AB <LinesFromString>c__Iterator1AB = new GenText.<LinesFromString>c__Iterator1AB();
			<LinesFromString>c__Iterator1AB.text = text;
			<LinesFromString>c__Iterator1AB.<$>text = text;
			GenText.<LinesFromString>c__Iterator1AB expr_15 = <LinesFromString>c__Iterator1AB;
			expr_15.$PC = -2;
			return expr_15;
		}
		public static bool IsValidFilename(string str)
		{
			if (str.Length > 28)
			{
				return false;
			}
			string str2 = new string(Path.GetInvalidFileNameChars());
			Regex regex = new Regex("[" + Regex.Escape(str2) + "]");
			return !regex.IsMatch(str);
		}
		public static string AsPercent(float pct)
		{
			return Mathf.RoundToInt(100f * pct) + "%";
		}
		public static bool NullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}
		public static string SplitCamelCase(string Str)
		{
			return Regex.Replace(Str, "(?<a>(?<!^)((?:[A-Z][a-z])|(?:(?<!^[A-Z]+)[A-Z0-9]+(?:(?=[A-Z][a-z])|$))|(?:[0-9]+)))", " ${a}");
		}
		public static string CapitalizedNoSpaces(string s)
		{
			string[] array = s.Split(new char[]
			{
				' '
			});
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (text.Length > 0)
				{
					stringBuilder.Append(char.ToUpper(text[0]));
				}
				if (text.Length > 1)
				{
					stringBuilder.Append(text.Substring(1));
				}
			}
			return stringBuilder.ToString();
		}
		public static bool EqualsIgnoreCase(this string A, string B)
		{
			return string.Compare(A, B, true) == 0;
		}
		public static string WithoutByteOrderMark(this string str)
		{
			return str.Trim().Trim(new char[]
			{
				'﻿'
			});
		}
		public static bool NamesOverlap(string A, string B)
		{
			A = A.ToLower();
			B = B.ToLower();
			string[] array = A.Split(new char[]
			{
				' '
			});
			string[] source = B.Split(new char[]
			{
				' '
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (TitleCaseHelper.IsUppercaseTitleWord(text))
				{
					if (source.Contains(text))
					{
						return true;
					}
				}
			}
			return false;
		}
		public static string CapitalizeFirst(this string str)
		{
			if (str.NullOrEmpty())
			{
				return str;
			}
			if (str.Length == 1)
			{
				return str.ToUpper();
			}
			return str[0].ToString().ToUpper() + str.Substring(1);
		}
		public static string ToNewsCase(string str)
		{
			string[] array = str.Split(new char[]
			{
				' '
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.Length >= 2)
				{
					if (i == 0)
					{
						array[i] = text[0].ToString().ToUpper() + text.Substring(1);
					}
					else
					{
						array[i] = text.ToLower();
					}
				}
			}
			return string.Join(" ", array);
		}
		public static string ToTitleCaseSmart(string str)
		{
			string[] array = str.Split(new char[]
			{
				' '
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (i == 0 || i == array.Length - 1 || TitleCaseHelper.IsUppercaseTitleWord(text))
				{
					string str2 = text[0].ToString().ToUpper();
					string str3 = text.Substring(1);
					array[i] = str2 + str3;
				}
			}
			return string.Join(" ", array);
		}
		public static string CapitalizeSentences(string input)
		{
			if (input.NullOrEmpty())
			{
				return input;
			}
			if (input.Length == 1)
			{
				return input.ToUpper();
			}
			input = Regex.Replace(input, "\\s+", " ");
			input = input.Trim();
			input = char.ToUpper(input[0]) + input.Substring(1);
			string[] array = new string[]
			{
				". ",
				"! ",
				"? "
			};
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				int length = text.Length;
				for (int j = input.IndexOf(text, 0); j > -1; j = input.IndexOf(text, j + 1))
				{
					input = input.Substring(0, j + length) + input[j + length].ToString().ToUpper() + input.Substring(j + length + 1);
				}
			}
			return input;
		}
		public static string ToCommaList(IEnumerable<object> items)
		{
			return GenText.ToCommaList(
				from it in items
				select it.ToString());
		}
		public static string ToCommaList(IEnumerable<string> items)
		{
			string text = null;
			string text2 = null;
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			IList<string> list = items as IList<string>;
			if (list != null)
			{
				num = list.Count;
				for (int i = 0; i < num; i++)
				{
					string text3 = list[i];
					if (!text3.NullOrEmpty())
					{
						if (text2 == null)
						{
							text2 = text3;
						}
						if (text != null)
						{
							stringBuilder.Append(text + ", ");
						}
						text = text3;
					}
				}
			}
			else
			{
				foreach (string current in items)
				{
					if (!current.NullOrEmpty())
					{
						if (text2 == null)
						{
							text2 = current;
						}
						if (text != null)
						{
							stringBuilder.Append(text + ", ");
						}
						text = current;
						num++;
					}
				}
			}
			if (num == 0)
			{
				return "NoneLower".Translate();
			}
			if (num == 1)
			{
				return text;
			}
			if (num == 2)
			{
				return string.Concat(new string[]
				{
					text2,
					" ",
					"AndLower".Translate(),
					" ",
					text
				});
			}
			stringBuilder.Append("AndLower".Translate() + " " + text);
			return stringBuilder.ToString();
		}
		public static string ToSpaceList(IEnumerable<string> strs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string current in strs)
			{
				stringBuilder.Append(current + " ");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}
		public static string ToCamelCase(string str)
		{
			str = GenText.ToTitleCaseSmart(str);
			return str.Replace(" ", null);
		}
		public static string Truncate(this string str, float width, Dictionary<string, string> cache = null)
		{
			if (Text.CalcSize(str).x <= width)
			{
				return str;
			}
			string text;
			if (cache != null && cache.TryGetValue(str, out text))
			{
				return text;
			}
			text = str;
			do
			{
				text = text.Substring(0, text.Length - 1);
			}
			while (text.Length > 0 && Text.CalcSize(text + "...").x > width);
			text += "...";
			if (cache != null)
			{
				cache.Add(str, text);
			}
			return text;
		}
		public static string ToStringByStyle(this float f, ToStringStyle style, ToStringNumberSense numberSense = ToStringNumberSense.Absolute)
		{
			if (style == ToStringStyle.Temperature && numberSense == ToStringNumberSense.Offset)
			{
				style = ToStringStyle.TemperatureOffset;
			}
			string text;
			switch (style)
			{
			case ToStringStyle.Integer:
				text = f.ToString("F0");
				break;
			case ToStringStyle.FloatOne:
				text = f.ToString("F1");
				break;
			case ToStringStyle.FloatTwo:
				text = f.ToString("F2");
				break;
			case ToStringStyle.PercentZero:
				text = f.ToStringPercent();
				break;
			case ToStringStyle.PercentOne:
				text = f.ToStringPercent("F1");
				break;
			case ToStringStyle.PercentTwo:
				text = f.ToStringPercent("F2");
				break;
			case ToStringStyle.Temperature:
				text = f.ToStringTemperature("F1");
				break;
			case ToStringStyle.TemperatureOffset:
				text = f.ToStringTemperatureOffset("F1");
				break;
			case ToStringStyle.WorkAmount:
				text = f.ToStringWorkAmount();
				break;
			default:
				Log.Error("Unknown ToStringStyle " + style);
				text = f.ToString();
				break;
			}
			if (numberSense == ToStringNumberSense.Offset)
			{
				if (f >= 0f)
				{
					text = "+" + text;
				}
			}
			else
			{
				if (numberSense == ToStringNumberSense.Factor)
				{
					text = "x" + text;
				}
			}
			return text;
		}
		public static string ToStringDecimalIfSmall(this float f)
		{
			if (Mathf.Abs(f) < 1f)
			{
				return Math.Round((double)f, 2).ToString("0.##");
			}
			if (Mathf.Abs(f) < 10f)
			{
				return Math.Round((double)f, 1).ToString("0.#");
			}
			return Mathf.RoundToInt(f).ToStringCached();
		}
		public static string ToStringPercent(this float f)
		{
			return (f * 100f).ToStringDecimalIfSmall() + "%";
		}
		public static string ToStringPercent(this float f, string format)
		{
			return ((f + 1E-05f) * 100f).ToString(format) + "%";
		}
		public static string ToStringMoney(this float f)
		{
			return "$" + f.ToString("F2");
		}
		public static string ToStringWithSign(this int i)
		{
			return i.ToString("+#;-#;0");
		}
		public static string ToStringTemperature(this float celsiusTemp, string format = "F1")
		{
			celsiusTemp = GenTemperature.CelsiusTo(celsiusTemp, Prefs.TemperatureMode);
			return celsiusTemp.ToStringTemperatureRaw(format);
		}
		public static string ToStringTemperatureOffset(this float celsiusTemp, string format = "F1")
		{
			celsiusTemp = GenTemperature.CelsiusToOffset(celsiusTemp, Prefs.TemperatureMode);
			return celsiusTemp.ToStringTemperatureRaw(format);
		}
		public static string ToStringTemperatureRaw(this float temp, string format = "F1")
		{
			switch (Prefs.TemperatureMode)
			{
			case TemperatureDisplayMode.Celsius:
				return temp.ToString(format) + "C";
			case TemperatureDisplayMode.Fahrenheit:
				return temp.ToString(format) + "F";
			case TemperatureDisplayMode.Kelvin:
				return temp.ToString(format) + "K";
			default:
				throw new InvalidOperationException();
			}
		}
		public static string ToStringTwoDigits(this Vector2 v)
		{
			return string.Concat(new string[]
			{
				"(",
				v.x.ToString("F2"),
				", ",
				v.y.ToString("F2"),
				")"
			});
		}
		public static string ToStringWorkAmount(this float workAmount)
		{
			return Mathf.CeilToInt(workAmount / 60f).ToString();
		}
		public static string ToStringBytes(this int b, string format = "F2")
		{
			return ((float)b / 8f / 1024f).ToString(format) + "kb";
		}
		public static string ToStringBytes(this uint b, string format = "F2")
		{
			return (b / 8f / 1024f).ToString(format) + "kb";
		}
	}
}
