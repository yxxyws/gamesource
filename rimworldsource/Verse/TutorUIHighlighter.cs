using System;
using UnityEngine;
namespace Verse
{
	[StaticConstructorOnStartup]
	public static class TutorUIHighlighter
	{
		private const float PulseFrequency = 1.2f;
		private const float PulseAmplitude = 0.7f;
		private static string lastHighlightTag = null;
		private static int lastHighlightFrame;
		private static readonly Texture2D TutorHighlightAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/TutorHighlightAtlas", true);
		public static void HighlightTag(string tag)
		{
			if (!Prefs.AdaptiveTrainingEnabled)
			{
				return;
			}
			if (tag.NullOrEmpty())
			{
				return;
			}
			TutorUIHighlighter.lastHighlightTag = tag;
			TutorUIHighlighter.lastHighlightFrame = Time.frameCount;
		}
		public static void HighlightOpportunity(string tag, Rect rect)
		{
			if (!Prefs.AdaptiveTrainingEnabled || Time.frameCount > TutorUIHighlighter.lastHighlightFrame + 1 || TutorUIHighlighter.lastHighlightTag != tag)
			{
				return;
			}
			Rect rect2 = rect.ContractedBy(-10f);
			GUI.color = new Color(1f, 1f, 1f, Pulser.PulseBrightness(1.2f, 0.7f));
			Widgets.DrawAtlas(rect2, TutorUIHighlighter.TutorHighlightAtlas);
			GUI.color = Color.white;
		}
	}
}
