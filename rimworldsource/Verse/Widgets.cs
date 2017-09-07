using RimWorld;
using System;
using System.Reflection;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	[StaticConstructorOnStartup]
	public static class Widgets
	{
		private enum RangeEnd : byte
		{
			None,
			Min,
			Max
		}
		public const int CheckboxSize = 24;
		private const float RadioButtonSize = 32f;
		private const int FillableBarBorderWidth = 3;
		private const int MaxFillChangeArrowHeight = 16;
		private const int FillChangeArrowWidth = 8;
		private const float CloseButtonSize = 18f;
		private const float CloseButtonMargin = 4f;
		private const float SeparatorLabelHeight = 20f;
		public const float InfoCardButtonSize = 24f;
		public static readonly GUIStyle EmptyStyle;
		private static readonly Color InactiveColor;
		private static readonly Texture2D DefaultBarBgTex;
		private static readonly Texture2D BarFullTexHor;
		public static readonly Texture2D CheckboxOnTex;
		public static readonly Texture2D CheckboxOffTex;
		public static readonly Texture2D CheckboxPartialTex;
		private static readonly Texture2D RadioButOnTex;
		private static readonly Texture2D RadioButOffTex;
		private static readonly Texture2D FillArrowTexRight;
		private static readonly Texture2D FillArrowTexLeft;
		private static readonly Texture2D MenuBGAtlas;
		private static readonly Texture2D TrainingBGAtlas;
		private static readonly Texture2D ShadowAtlas;
		private static readonly Texture2D SubmenuBGAtlas;
		private static readonly Texture2D ButtonBGAtlas;
		private static readonly Texture2D ButtonBGAtlasMouseover;
		private static readonly Texture2D ButtonBGAtlasClick;
		private static readonly Texture2D FloatRangeSliderTex;
		private static Texture2D LineTexAA;
		private static readonly Rect LineRect;
		private static readonly Material LineMat;
		private static readonly Texture2D AltTexture;
		private static readonly Color SeparatorLabelColor;
		private static readonly Color SeparatorLineColor;
		public static readonly Color NormalOptionColor;
		private static readonly Color MouseoverOptionColor;
		private static Widgets.RangeEnd frDragEnd;
		private static int draggingId;
		private static Widgets.RangeEnd qrDragEnd;
		private static float FillableBarChangeRateDisplayRatio;
		public static int MaxFillableBarChangeRate;
		static Widgets()
		{
			Widgets.EmptyStyle = new GUIStyle();
			Widgets.InactiveColor = new Color(0.37f, 0.37f, 0.37f, 0.8f);
			Widgets.DefaultBarBgTex = BaseContent.BlackTex;
			Widgets.BarFullTexHor = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.8f, 0.85f));
			Widgets.CheckboxOnTex = ContentFinder<Texture2D>.Get("UI/Widgets/CheckOn", true);
			Widgets.CheckboxOffTex = ContentFinder<Texture2D>.Get("UI/Widgets/CheckOff", true);
			Widgets.CheckboxPartialTex = ContentFinder<Texture2D>.Get("UI/Widgets/CheckPartial", true);
			Widgets.RadioButOnTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOn", true);
			Widgets.RadioButOffTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOff", true);
			Widgets.FillArrowTexRight = ContentFinder<Texture2D>.Get("UI/Widgets/FillChangeArrowRight", true);
			Widgets.FillArrowTexLeft = ContentFinder<Texture2D>.Get("UI/Widgets/FillChangeArrowLeft", true);
			Widgets.MenuBGAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/MenuBG", true);
			Widgets.TrainingBGAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/TutorBG", true);
			Widgets.ShadowAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/DropShadow", true);
			Widgets.SubmenuBGAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/MenuSectionBG", true);
			Widgets.ButtonBGAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBG", true);
			Widgets.ButtonBGAtlasMouseover = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGMouseover", true);
			Widgets.ButtonBGAtlasClick = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGClick", true);
			Widgets.FloatRangeSliderTex = ContentFinder<Texture2D>.Get("UI/Widgets/RangeSlider", true);
			Widgets.LineTexAA = null;
			Widgets.LineRect = new Rect(0f, 0f, 1f, 1f);
			Widgets.LineMat = null;
			Widgets.AltTexture = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.05f));
			Widgets.SeparatorLabelColor = new Color(0.8f, 0.8f, 0.8f, 1f);
			Widgets.SeparatorLineColor = new Color(0.3f, 0.3f, 0.3f, 1f);
			Widgets.NormalOptionColor = new Color(0.8f, 0.85f, 1f);
			Widgets.MouseoverOptionColor = Color.yellow;
			Widgets.frDragEnd = Widgets.RangeEnd.None;
			Widgets.draggingId = 0;
			Widgets.qrDragEnd = Widgets.RangeEnd.None;
			Widgets.FillableBarChangeRateDisplayRatio = 1E+08f;
			Widgets.MaxFillableBarChangeRate = 3;
			Color color = new Color(1f, 1f, 1f, 0f);
			Widgets.LineTexAA = new Texture2D(1, 3, TextureFormat.ARGB32, false);
			Widgets.LineTexAA.name = "LineTexAA";
			Widgets.LineTexAA.SetPixel(0, 0, color);
			Widgets.LineTexAA.SetPixel(0, 1, Color.white);
			Widgets.LineTexAA.SetPixel(0, 2, color);
			Widgets.LineTexAA.Apply();
			Widgets.LineMat = (Material)typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
		}
		public static void ThingIcon(Rect rect, Thing thing)
		{
			GUI.color = thing.DrawColor;
			Texture resolvedIcon;
			if (!thing.def.uiIconPath.NullOrEmpty())
			{
				resolvedIcon = thing.def.uiIcon;
			}
			else
			{
				if (thing is Pawn)
				{
					Pawn pawn = (Pawn)thing;
					if (!pawn.Drawer.renderer.graphics.AllResolved)
					{
						pawn.Drawer.renderer.graphics.ResolveAllGraphics();
					}
					Material matSingle = pawn.Drawer.renderer.graphics.nakedGraphic.MatSingle;
					resolvedIcon = matSingle.mainTexture;
					GUI.color = matSingle.color;
				}
				else
				{
					resolvedIcon = thing.Graphic.ExtractInnerGraphicFor(thing).MatSingle.mainTexture;
				}
			}
			Widgets.ThingIconWorker(rect, thing.def, resolvedIcon);
			GUI.color = Color.white;
		}
		public static void ThingIcon(Rect rect, ThingDef thingDef)
		{
			GUI.color = thingDef.graphicData.color;
			Widgets.ThingIconWorker(rect, thingDef, thingDef.uiIcon);
			GUI.color = Color.white;
		}
		private static void ThingIconWorker(Rect rect, ThingDef thingDef, Texture resolvedIcon)
		{
			float num = GenUI.IconDrawScale(thingDef);
			if (num != 1f)
			{
				Vector2 center = rect.center;
				rect.width *= num;
				rect.height *= num;
				rect.center = center;
			}
			GUI.DrawTexture(rect, resolvedIcon);
		}
		public static void DrawAltRect(Rect rect)
		{
			GUI.DrawTexture(rect, Widgets.AltTexture);
		}
		public static void ListSeparator(ref float curY, float width, string label)
		{
			Color color = GUI.color;
			curY += 3f;
			GUI.color = Widgets.SeparatorLabelColor;
			Rect rect = new Rect(0f, curY, width, 20f);
			Widgets.Label(rect, label);
			curY += 20f;
			GUI.color = Widgets.SeparatorLineColor;
			Widgets.DrawLineHorizontal(0f, curY, width);
			curY += 2f;
			GUI.color = color;
		}
		public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
		{
			float num = end.x - start.x;
			float num2 = end.y - start.y;
			float num3 = Mathf.Sqrt(num * num + num2 * num2);
			if (num3 < 0.01f)
			{
				return;
			}
			width *= 3f;
			float num4 = width * num2 / num3;
			float num5 = width * num / num3;
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m00 = num;
			identity.m01 = -num4;
			identity.m03 = start.x + 0.5f * num4;
			identity.m10 = num2;
			identity.m11 = num5;
			identity.m13 = start.y - 0.5f * num5;
			GL.PushMatrix();
			GL.MultMatrix(identity);
			Graphics.DrawTexture(Widgets.LineRect, Widgets.LineTexAA, Widgets.LineRect, 0, 0, 0, 0, color, Widgets.LineMat);
			GL.PopMatrix();
		}
		public static void DrawLineHorizontal(float x, float y, float length)
		{
			Rect position = new Rect(x, y, length, 1f);
			GUI.DrawTexture(position, BaseContent.WhiteTex);
		}
		public static void DrawLineVertical(float x, float y, float length)
		{
			Rect position = new Rect(x, y, 1f, length);
			GUI.DrawTexture(position, BaseContent.WhiteTex);
		}
		public static void DrawBox(Rect drawRect, int thickness = 1)
		{
			Vector2 b = new Vector2(drawRect.x, drawRect.y);
			Vector2 a = new Vector2(drawRect.x + drawRect.width, drawRect.y + drawRect.height);
			if (b.x > a.x)
			{
				float x = b.x;
				b.x = a.x;
				a.x = x;
			}
			if (b.y > a.y)
			{
				float y = b.y;
				b.y = a.y;
				a.y = y;
			}
			Vector3 vector = a - b;
			Texture2D whiteTex = BaseContent.WhiteTex;
			GUI.DrawTexture(new Rect(b.x, b.y, (float)thickness, vector.y), whiteTex);
			GUI.DrawTexture(new Rect(a.x - (float)thickness, b.y, (float)thickness, vector.y), whiteTex);
			GUI.DrawTexture(new Rect(b.x + (float)thickness, b.y, vector.x - (float)(thickness * 2), (float)thickness), whiteTex);
			GUI.DrawTexture(new Rect(b.x + (float)thickness, a.y - (float)thickness, vector.x - (float)(thickness * 2), (float)thickness), whiteTex);
		}
		public static void Label(Rect rect, GUIContent content)
		{
			GUI.Label(rect, content, Text.CurFontStyle);
		}
		public static void Label(Rect rect, string label)
		{
			GUI.Label(rect, label, Text.CurFontStyle);
		}
		public static void Checkbox(Vector2 topLeft, ref bool checkOn, float size = 24f, bool disabled = false)
		{
			if (disabled)
			{
				GUI.color = Widgets.InactiveColor;
			}
			Rect rect = new Rect(topLeft.x, topLeft.y, size, size);
			Widgets.CheckboxDraw(topLeft, checkOn, disabled, size);
			MouseoverSounds.DoRegion(rect);
			if (!disabled && Widgets.InvisibleButton(rect))
			{
				checkOn = (checkOn == 0);
				if (checkOn)
				{
					SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
				}
				else
				{
					SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
				}
			}
			if (disabled)
			{
				GUI.color = Color.white;
			}
		}
		public static void LabelCheckbox(Rect rect, string label, ref bool checkOn, bool disabled = false)
		{
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect, label);
			if (!disabled && Widgets.InvisibleButton(rect))
			{
				checkOn = (checkOn == 0);
				if (checkOn)
				{
					SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
				}
				else
				{
					SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
				}
			}
			Vector2 topLeft = new Vector2(rect.x + rect.width - 24f, rect.y);
			Widgets.CheckboxDraw(topLeft, checkOn, disabled, 24f);
			Text.Anchor = anchor;
		}
		public static bool LabelCheckboxSelectable(Rect rect, string label, ref bool selected, ref bool checkOn)
		{
			if (selected)
			{
				Widgets.DrawHighlight(rect);
			}
			Widgets.Label(rect, label);
			bool flag = selected;
			Rect butRect = rect;
			butRect.width -= 24f;
			if (!selected && Widgets.InvisibleButton(butRect))
			{
				SoundDefOf.TickTiny.PlayOneShotOnCamera();
				selected = true;
			}
			Vector2 topLeft = new Vector2(rect.xMax - 24f, rect.y);
			Widgets.CheckboxDraw(topLeft, checkOn, false, 24f);
			Rect butRect2 = new Rect(topLeft.x, topLeft.y, 24f, 24f);
			if (Widgets.InvisibleButton(butRect2))
			{
				checkOn = (checkOn == 0);
				if (checkOn)
				{
					SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
				}
				else
				{
					SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
				}
			}
			return selected && !flag;
		}
		private static void CheckboxDraw(Vector2 topLeft, bool active, bool disabled, float size = 24f)
		{
			Color color = GUI.color;
			if (disabled)
			{
				GUI.color = Widgets.InactiveColor;
			}
			Texture2D image;
			if (active)
			{
				image = Widgets.CheckboxOnTex;
			}
			else
			{
				image = Widgets.CheckboxOffTex;
			}
			Rect position = new Rect(topLeft.x, topLeft.y, size, size);
			GUI.DrawTexture(position, image);
			if (disabled)
			{
				GUI.color = color;
			}
		}
		public static bool MultiCheckbox(Vector2 topLeft, MultiCheckboxState state, float size)
		{
			Texture2D tex;
			if (state == MultiCheckboxState.On)
			{
				tex = Widgets.CheckboxOnTex;
			}
			else
			{
				if (state == MultiCheckboxState.Off)
				{
					tex = Widgets.CheckboxOffTex;
				}
				else
				{
					tex = Widgets.CheckboxPartialTex;
				}
			}
			Rect rect = new Rect(topLeft.x, topLeft.y, size, size);
			MouseoverSounds.DoRegion(rect);
			if (Widgets.ImageButton(rect, tex))
			{
				if (state == MultiCheckboxState.Off || state == MultiCheckboxState.Partial)
				{
					SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
				}
				else
				{
					SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
				}
				return true;
			}
			return false;
		}
		public static bool LabelRadioButton(Rect rect, string labelText, bool chosen)
		{
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect, labelText);
			Text.Anchor = anchor;
			bool flag = Widgets.InvisibleButton(rect);
			if (flag && !chosen)
			{
				SoundDefOf.RadioButtonClicked.PlayOneShotOnCamera();
			}
			Vector2 topLeft = new Vector2(rect.x + rect.width - 32f, rect.y + rect.height / 2f - 16f);
			Widgets.RadioButtonDraw(topLeft, chosen);
			return flag;
		}
		public static bool RadioButton(Vector2 topLeft, bool chosen)
		{
			Rect rect = new Rect(topLeft.x, topLeft.y, 24f, 24f);
			MouseoverSounds.DoRegion(rect);
			Widgets.RadioButtonDraw(topLeft, chosen);
			bool flag = Widgets.InvisibleButton(rect);
			if (flag && !chosen)
			{
				SoundDefOf.RadioButtonClicked.PlayOneShotOnCamera();
			}
			return flag;
		}
		private static void RadioButtonDraw(Vector2 topLeft, bool chosen)
		{
			Texture2D image;
			if (chosen)
			{
				image = Widgets.RadioButOnTex;
			}
			else
			{
				image = Widgets.RadioButOffTex;
			}
			Rect position = new Rect(topLeft.x, topLeft.y, 24f, 24f);
			GUI.DrawTexture(position, image);
		}
		public static bool CloseButtonFor(Rect rectToClose)
		{
			Rect butRect = new Rect(rectToClose.x + rectToClose.width - 18f - 4f, rectToClose.y + 4f, 18f, 18f);
			return Widgets.ImageButton(butRect, TexButton.CloseXSmall);
		}
		public static bool InvisibleButton(Rect ButRect)
		{
			return GUI.Button(ButRect, string.Empty, Widgets.EmptyStyle);
		}
		public static bool TextButton(Rect rect, string label, bool drawBackground = true, bool doMouseoverSound = false)
		{
			return Widgets.TextButton(rect, label, drawBackground, doMouseoverSound, Widgets.NormalOptionColor);
		}
		public static bool TextButton(Rect rect, string label, bool drawBackground, bool doMouseoverSound, Color textColor)
		{
			TextAnchor anchor = Text.Anchor;
			Color color = GUI.color;
			if (drawBackground)
			{
				Texture2D atlas = Widgets.ButtonBGAtlas;
				if (Mouse.IsOver(rect))
				{
					atlas = Widgets.ButtonBGAtlasMouseover;
					if (Input.GetMouseButton(0))
					{
						atlas = Widgets.ButtonBGAtlasClick;
					}
				}
				Widgets.DrawAtlas(rect, atlas);
			}
			if (doMouseoverSound)
			{
				MouseoverSounds.DoRegion(rect);
			}
			if (!drawBackground)
			{
				GUI.color = textColor;
				if (Mouse.IsOver(rect))
				{
					GUI.color = Widgets.MouseoverOptionColor;
				}
			}
			if (drawBackground)
			{
				Text.Anchor = TextAnchor.MiddleCenter;
			}
			else
			{
				Text.Anchor = TextAnchor.MiddleLeft;
			}
			Widgets.Label(rect, label);
			Text.Anchor = anchor;
			GUI.color = color;
			return Widgets.InvisibleButton(rect);
		}
		public static bool ImageButton(Rect butRect, Texture2D tex)
		{
			return Widgets.ImageButton(butRect, tex, Color.white);
		}
		public static bool ImageButton(Rect butRect, Texture2D tex, Color baseColor)
		{
			if (Mouse.IsOver(butRect))
			{
				GUI.color = GenUI.MouseoverColor;
			}
			GUI.DrawTexture(butRect, tex);
			GUI.color = baseColor;
			return Widgets.InvisibleButton(butRect);
		}
		public static void FloatRangeWithTypeIn(Rect rect, int id, ref FloatRange fRange, float sliderMin = 0f, float sliderMax = 1f, ToStringStyle valueStyle = ToStringStyle.FloatTwo, string labelKey = null)
		{
			Rect rect2 = new Rect(rect);
			rect2.width = rect.width / 4f;
			Rect rect3 = new Rect(rect);
			rect3.width = rect.width / 2f;
			rect3.x = rect.x + rect.width / 4f;
			rect3.height = rect.height / 2f;
			rect3.width -= rect.height;
			Rect butRect = new Rect(rect3);
			butRect.x = rect3.xMax;
			butRect.height = rect.height;
			butRect.width = rect.height;
			Rect rect4 = new Rect(rect);
			rect4.x = rect.x + rect.width * 0.75f;
			rect4.width = rect.width / 4f;
			Widgets.FloatRange(rect3, id, ref fRange, sliderMin, sliderMax, valueStyle, labelKey);
			if (Widgets.ImageButton(butRect, TexButton.RangeMatch))
			{
				fRange.max = fRange.min;
			}
			float.TryParse(Widgets.TextField(rect2, fRange.min.ToString()), out fRange.min);
			float.TryParse(Widgets.TextField(rect4, fRange.max.ToString()), out fRange.max);
		}
		public static void FloatRange(Rect rect, int id, ref FloatRange fRange, float sliderMin = 0f, float sliderMax = 1f, ToStringStyle valueStyle = ToStringStyle.FloatTwo, string labelKey = null)
		{
			rect.xMin += 8f;
			rect.xMax -= 8f;
			GUI.color = new Color(0.4f, 0.4f, 0.4f);
			string text = fRange.min.ToStringByStyle(valueStyle, ToStringNumberSense.Absolute) + " - " + fRange.max.ToStringByStyle(valueStyle, ToStringNumberSense.Absolute);
			if (labelKey != null)
			{
				text = labelKey.Translate(new object[]
				{
					text
				});
			}
			Text.Font = GameFont.Tiny;
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, 19f);
			Text.Anchor = TextAnchor.UpperCenter;
			Widgets.Label(rect2, text);
			Text.Anchor = TextAnchor.UpperLeft;
			Rect position = new Rect(rect.x, rect2.yMax, rect.width, 2f);
			GUI.DrawTexture(position, BaseContent.WhiteTex);
			GUI.color = Color.white;
			float num = rect.x + (rect.width * fRange.min - sliderMin / (sliderMax - sliderMin));
			float num2 = rect.x + (rect.width * fRange.max - sliderMin / (sliderMax - sliderMin));
			Rect position2 = new Rect(num - 16f, position.center.y - 8f, 16f, 16f);
			GUI.DrawTexture(position2, Widgets.FloatRangeSliderTex);
			Rect position3 = new Rect(num2 + 16f, position.center.y - 8f, -16f, 16f);
			GUI.DrawTexture(position3, Widgets.FloatRangeSliderTex);
			Rect rect3 = rect;
			rect3.xMin -= 8f;
			rect3.xMax += 8f;
			bool flag = false;
			if (Mouse.IsOver(rect3) || Widgets.draggingId == id)
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && id != Widgets.draggingId)
				{
					Widgets.draggingId = id;
					float x = Event.current.mousePosition.x;
					if (x < position2.xMax)
					{
						Widgets.frDragEnd = Widgets.RangeEnd.Min;
					}
					else
					{
						if (x > position3.xMin)
						{
							Widgets.frDragEnd = Widgets.RangeEnd.Max;
						}
						else
						{
							float num3 = Mathf.Abs(x - position2.xMax);
							float num4 = Mathf.Abs(x - (position3.x - 16f));
							Widgets.frDragEnd = ((num3 >= num4) ? Widgets.RangeEnd.Max : Widgets.RangeEnd.Min);
						}
					}
					flag = true;
					Event.current.Use();
				}
				if (flag || (Widgets.frDragEnd != Widgets.RangeEnd.None && Event.current.type == EventType.MouseDrag))
				{
					float num5 = (Event.current.mousePosition.x - rect.x) / rect.width * sliderMax + sliderMin;
					num5 = Mathf.Clamp(num5, sliderMin, sliderMax);
					if (Widgets.frDragEnd == Widgets.RangeEnd.Min)
					{
						fRange.min = num5;
						if (fRange.max < fRange.min)
						{
							fRange.max = fRange.min;
						}
					}
					else
					{
						if (Widgets.frDragEnd == Widgets.RangeEnd.Max)
						{
							fRange.max = num5;
							if (fRange.min > fRange.max)
							{
								fRange.min = fRange.max;
							}
						}
					}
					Event.current.Use();
				}
			}
			if (Widgets.frDragEnd != Widgets.RangeEnd.None && (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown))
			{
				Widgets.draggingId = 0;
				Widgets.frDragEnd = Widgets.RangeEnd.None;
			}
		}
		public static void QualityRange(Rect rect, int id, ref QualityRange qRange)
		{
			rect.xMin += 8f;
			rect.xMax -= 8f;
			GUI.color = new Color(0.4f, 0.4f, 0.4f);
			string label;
			if (qRange == RimWorld.QualityRange.All)
			{
				label = "AnyQuality".Translate();
			}
			else
			{
				if (qRange.max == qRange.min)
				{
					label = "OnlyQuality".Translate(new object[]
					{
						qRange.min.GetLabel()
					});
				}
				else
				{
					label = qRange.min.GetLabel() + " - " + qRange.max.GetLabel();
				}
			}
			Text.Font = GameFont.Tiny;
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, 19f);
			Text.Anchor = TextAnchor.UpperCenter;
			Widgets.Label(rect2, label);
			Text.Anchor = TextAnchor.UpperLeft;
			Rect position = new Rect(rect.x, rect2.yMax, rect.width, 2f);
			GUI.DrawTexture(position, BaseContent.WhiteTex);
			GUI.color = Color.white;
			int length = Enum.GetValues(typeof(QualityCategory)).Length;
			float num = rect.x + rect.width / (float)(length - 1) * (float)qRange.min;
			float num2 = rect.x + rect.width / (float)(length - 1) * (float)qRange.max;
			Rect position2 = new Rect(num - 16f, position.center.y - 8f, 16f, 16f);
			GUI.DrawTexture(position2, Widgets.FloatRangeSliderTex);
			Rect position3 = new Rect(num2 + 16f, position.center.y - 8f, -16f, 16f);
			GUI.DrawTexture(position3, Widgets.FloatRangeSliderTex);
			Rect rect3 = rect;
			rect3.xMin -= 8f;
			rect3.xMax += 8f;
			bool flag = false;
			if (Mouse.IsOver(rect3) || id == Widgets.draggingId)
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && id != Widgets.draggingId)
				{
					Widgets.draggingId = id;
					float x = Event.current.mousePosition.x;
					if (x < position2.xMax)
					{
						Widgets.qrDragEnd = Widgets.RangeEnd.Min;
					}
					else
					{
						if (x > position3.xMin)
						{
							Widgets.qrDragEnd = Widgets.RangeEnd.Max;
						}
						else
						{
							float num3 = Mathf.Abs(x - position2.xMax);
							float num4 = Mathf.Abs(x - (position3.x - 16f));
							Widgets.qrDragEnd = ((num3 >= num4) ? Widgets.RangeEnd.Max : Widgets.RangeEnd.Min);
						}
					}
					flag = true;
					Event.current.Use();
				}
				if (flag || (Widgets.qrDragEnd != Widgets.RangeEnd.None && Event.current.type == EventType.MouseDrag))
				{
					float num5 = (Event.current.mousePosition.x - rect.x) / rect.width;
					int num6 = Mathf.RoundToInt(num5 * (float)(length - 1));
					num6 = Mathf.Clamp(num6, 0, length - 1);
					if (Widgets.qrDragEnd == Widgets.RangeEnd.Min)
					{
						qRange.min = (QualityCategory)num6;
						if (qRange.max < qRange.min)
						{
							qRange.max = qRange.min;
						}
					}
					else
					{
						if (Widgets.qrDragEnd == Widgets.RangeEnd.Max)
						{
							qRange.max = (QualityCategory)num6;
							if (qRange.min > qRange.max)
							{
								qRange.min = qRange.max;
							}
						}
					}
					Event.current.Use();
				}
			}
			if (Widgets.qrDragEnd != Widgets.RangeEnd.None && (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown))
			{
				Widgets.draggingId = 0;
				Widgets.qrDragEnd = Widgets.RangeEnd.None;
			}
		}
		public static void FillableBar(Rect rect, float fillPercent)
		{
			Widgets.FillableBar(rect, fillPercent, Widgets.BarFullTexHor);
		}
		public static void FillableBar(Rect rect, float fillPercent, Texture2D fillTex)
		{
			bool doBorder = rect.height > 15f && rect.width > 20f;
			Widgets.FillableBar(rect, fillPercent, fillTex, Widgets.DefaultBarBgTex, doBorder);
		}
		public static void FillableBar(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder)
		{
			if (doBorder)
			{
				GUI.DrawTexture(rect, BaseContent.BlackTex);
				rect = rect.ContractedBy(3f);
			}
			if (bgTex != null)
			{
				GUI.DrawTexture(rect, bgTex);
			}
			rect.width *= fillPercent;
			GUI.DrawTexture(rect, fillTex);
		}
		public static void FillableBarLabeled(Rect rect, float fillPercent, int labelWidth, string label)
		{
			if (fillPercent < 0f)
			{
				fillPercent = 0f;
			}
			if (fillPercent > 1f)
			{
				fillPercent = 1f;
			}
			Rect rect2 = rect;
			rect2.width = (float)labelWidth;
			Widgets.Label(rect2, label);
			Rect rect3 = rect;
			rect3.x += (float)labelWidth;
			rect3.width -= (float)labelWidth;
			Widgets.FillableBar(rect3, fillPercent);
		}
		public static void FillableBarChangeArrows(Rect barRect, float changeRate)
		{
			int changeRate2 = (int)(changeRate * Widgets.FillableBarChangeRateDisplayRatio);
			Widgets.FillableBarChangeArrows(barRect, changeRate2);
		}
		public static void FillableBarChangeArrows(Rect barRect, int changeRate)
		{
			if (changeRate == 0)
			{
				return;
			}
			if (changeRate > Widgets.MaxFillableBarChangeRate)
			{
				changeRate = Widgets.MaxFillableBarChangeRate;
			}
			if (changeRate < -Widgets.MaxFillableBarChangeRate)
			{
				changeRate = -Widgets.MaxFillableBarChangeRate;
			}
			float num = barRect.height;
			if (num > 16f)
			{
				num = 16f;
			}
			int num2 = Mathf.Abs(changeRate);
			float top = barRect.y + barRect.height / 2f - num / 2f;
			float num3;
			float num4;
			Texture2D image;
			if (changeRate > 0)
			{
				num3 = barRect.x + barRect.width + 2f;
				num4 = 8f;
				image = Widgets.FillArrowTexRight;
			}
			else
			{
				num3 = barRect.x - 8f - 2f;
				num4 = -8f;
				image = Widgets.FillArrowTexLeft;
			}
			for (int i = 0; i < num2; i++)
			{
				Rect position = new Rect(num3, top, 8f, num);
				GUI.DrawTexture(position, image);
				num3 += num4;
			}
		}
		public static void DrawWindowBackground(Rect rect)
		{
			Widgets.DrawAtlas(rect, Widgets.MenuBGAtlas);
		}
		public static void DrawWindowBackgroundTutor(Rect rect)
		{
			Widgets.DrawAtlas(rect, Widgets.TrainingBGAtlas);
		}
		public static void DrawShadowAround(Rect rect)
		{
			Rect rect2 = rect.ContractedBy(-9f);
			rect2.x += 2f;
			rect2.y += 2f;
			Widgets.DrawAtlas(rect2, Widgets.ShadowAtlas);
		}
		public static void DrawMenuSection(Rect rect, bool drawTop = true)
		{
			Widgets.DrawAtlas(rect, Widgets.SubmenuBGAtlas, drawTop);
		}
		public static void DrawAtlas(Rect rect, Texture2D atlas)
		{
			Widgets.DrawAtlas(rect, atlas, true);
		}
		public static void DrawAtlas(Rect rect, Texture2D atlas, bool drawTop)
		{
			rect.x = (float)Mathf.RoundToInt(rect.x);
			rect.y = (float)Mathf.RoundToInt(rect.y);
			rect.width = (float)Mathf.RoundToInt(rect.width);
			rect.height = (float)Mathf.RoundToInt(rect.height);
			float num = (float)atlas.width * 0.25f;
			GUI.BeginGroup(rect);
			Rect drawRect;
			Rect texRect;
			if (drawTop)
			{
				drawRect = new Rect(0f, 0f, num, num);
				texRect = new Rect(0f, 0f, 0.25f, 0.25f);
				Widgets.DrawTexturePart(drawRect, texRect, atlas);
				drawRect = new Rect(rect.width - num, 0f, num, num);
				texRect = new Rect(0.75f, 0f, 0.25f, 0.25f);
				Widgets.DrawTexturePart(drawRect, texRect, atlas);
			}
			drawRect = new Rect(0f, rect.height - num, num, num);
			texRect = new Rect(0f, 0.75f, 0.25f, 0.25f);
			Widgets.DrawTexturePart(drawRect, texRect, atlas);
			drawRect = new Rect(rect.width - num, rect.height - num, num, num);
			texRect = new Rect(0.75f, 0.75f, 0.25f, 0.25f);
			Widgets.DrawTexturePart(drawRect, texRect, atlas);
			drawRect = new Rect(num, num, rect.width - num * 2f, rect.height - num * 2f);
			if (!drawTop)
			{
				drawRect.height += num;
				drawRect.y -= num;
			}
			texRect = new Rect(0.25f, 0.25f, 0.5f, 0.5f);
			Widgets.DrawTexturePart(drawRect, texRect, atlas);
			if (drawTop)
			{
				drawRect = new Rect(num, 0f, rect.width - num * 2f, num);
				texRect = new Rect(0.25f, 0f, 0.5f, 0.25f);
				Widgets.DrawTexturePart(drawRect, texRect, atlas);
			}
			drawRect = new Rect(num, rect.height - num, rect.width - num * 2f, num);
			texRect = new Rect(0.25f, 0.75f, 0.5f, 0.25f);
			Widgets.DrawTexturePart(drawRect, texRect, atlas);
			drawRect = new Rect(0f, num, num, rect.height - num * 2f);
			if (!drawTop)
			{
				drawRect.height += num;
				drawRect.y -= num;
			}
			texRect = new Rect(0f, 0.25f, 0.25f, 0.5f);
			Widgets.DrawTexturePart(drawRect, texRect, atlas);
			drawRect = new Rect(rect.width - num, num, num, rect.height - num * 2f);
			if (!drawTop)
			{
				drawRect.height += num;
				drawRect.y -= num;
			}
			texRect = new Rect(0.75f, 0.25f, 0.25f, 0.5f);
			Widgets.DrawTexturePart(drawRect, texRect, atlas);
			GUI.EndGroup();
		}
		public static Rect ToUVRect(this Rect r, Vector2 texSize)
		{
			return new Rect(r.x / texSize.x, r.y / texSize.y, r.width / texSize.x, r.height / texSize.y);
		}
		public static void DrawTexturePart(Rect drawRect, Rect texRect, Texture2D Tex)
		{
			GUI.BeginGroup(drawRect);
			Rect position = new Rect(-texRect.x * (drawRect.width / texRect.width), -texRect.y * (drawRect.height / texRect.height), drawRect.width / texRect.width, drawRect.height / texRect.height);
			GUI.DrawTexture(position, Tex);
			GUI.EndGroup();
		}
		public static void BeginScrollView(Rect outRect, ref Vector2 scrollPosition, Rect viewRect)
		{
			Vector2 vector = scrollPosition;
			Vector2 vector2 = GUI.BeginScrollView(outRect, scrollPosition, viewRect);
			Vector2 vector3;
			if (Event.current.type == EventType.MouseDown)
			{
				vector3 = vector;
			}
			else
			{
				vector3 = vector2;
			}
			if (Event.current.type == EventType.ScrollWheel && Mouse.IsOver(outRect))
			{
				vector3 += Event.current.delta * 40f;
				Event.current.Use();
			}
			scrollPosition = vector3;
		}
		public static void EndScrollView()
		{
			GUI.EndScrollView();
		}
		public static void DrawHighlightSelected(Rect rect)
		{
			GUI.DrawTexture(rect, TexUI.HighlightSelectedTex);
		}
		public static void DrawHighlightIfMouseover(Rect rect)
		{
			if (Mouse.IsOver(rect))
			{
				GUI.DrawTexture(rect, TexUI.HighlightTex);
			}
		}
		public static void DrawHighlight(Rect rect)
		{
			GUI.DrawTexture(rect, TexUI.HighlightTex);
		}
		public static string TextField(Rect rect, string text)
		{
			return GUI.TextField(rect, text, Text.CurTextFieldStyle);
		}
		public static void InfoCardButton(float x, float y, Thing thing)
		{
			IConstructible constructible = thing as IConstructible;
			if (constructible != null)
			{
				ThingDef thingDef = thing.def.entityDefToBuild as ThingDef;
				if (thingDef != null)
				{
					Widgets.InfoCardButton(x, y, thingDef, constructible.UIStuff());
				}
				else
				{
					Widgets.InfoCardButton(x, y, thing.def.entityDefToBuild);
				}
				return;
			}
			if (Widgets.InfoCardButtonWorker(x, y))
			{
				Find.WindowStack.Add(new Dialog_InfoCard(thing));
			}
		}
		public static void InfoCardButton(float x, float y, Def def)
		{
			if (Widgets.InfoCardButtonWorker(x, y))
			{
				Find.WindowStack.Add(new Dialog_InfoCard(def));
			}
		}
		public static void InfoCardButton(float x, float y, ThingDef thingDef, ThingDef stuffDef)
		{
			if (Widgets.InfoCardButtonWorker(x, y))
			{
				Find.WindowStack.Add(new Dialog_InfoCard(thingDef, stuffDef));
			}
		}
		private static bool InfoCardButtonWorker(float x, float y)
		{
			Rect rect = new Rect(x, y, 24f, 24f);
			TooltipHandler.TipRegion(rect, "DefInfoTip".Translate());
			bool result = Widgets.ImageButton(rect, TexButton.Info);
			TutorUIHighlighter.HighlightOpportunity("InfoCard", rect);
			return result;
		}
		public static void DrawTextureFitted(Rect outerRect, Texture2D tex, float scale)
		{
			Widgets.DrawTextureFitted(outerRect, tex, scale, new Vector2((float)tex.width, (float)tex.height), new Rect(0f, 0f, 1f, 1f));
		}
		public static void DrawTextureFitted(Rect outerRect, Texture2D tex, float scale, Vector2 texProportions, Rect texCoords)
		{
			Rect position = new Rect(0f, 0f, texProportions.x, texProportions.y);
			float num;
			if (position.width / position.height < outerRect.width / outerRect.height)
			{
				num = outerRect.height / position.height;
			}
			else
			{
				num = outerRect.width / position.width;
			}
			num *= scale;
			position.width *= num;
			position.height *= num;
			position.x = outerRect.x + outerRect.width / 2f - position.width / 2f;
			position.y = outerRect.y + outerRect.height / 2f - position.height / 2f;
			GUI.DrawTextureWithTexCoords(position, tex, texCoords);
		}
	}
}
