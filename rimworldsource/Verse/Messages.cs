using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	[StaticConstructorOnStartup]
	public static class Messages
	{
		private class LiveMessage
		{
			private const float DefaultMessageLifespan = 13f;
			private const float FadeoutDuration = 0.6f;
			private int ID;
			public string text;
			private float startingTime;
			public int startingFrame;
			public TargetInfo lookTarget;
			private static int uniqueID;
			protected float Age
			{
				get
				{
					return Time.time - this.startingTime;
				}
			}
			protected float TimeLeft
			{
				get
				{
					return 13f - this.Age;
				}
			}
			public bool Expired
			{
				get
				{
					return this.TimeLeft <= 0f;
				}
			}
			public LiveMessage(string text)
			{
				this.text = text;
				this.lookTarget = TargetInfo.Invalid;
				this.startingFrame = Time.frameCount;
				this.startingTime = Time.time;
				this.ID = Messages.LiveMessage.uniqueID++;
			}
			public LiveMessage(string text, TargetInfo lookTarget) : this(text)
			{
				this.lookTarget = lookTarget;
			}
			public void Draw(int xOffset, int yOffset)
			{
				Text.Font = GameFont.Small;
				Vector2 vector = Text.CalcSize(this.text);
				Rect rect = new Rect((float)xOffset, (float)yOffset, vector.x, vector.y);
				Find.WindowStack.ImmediateWindow(Gen.HashCombineInt(this.ID, 45574281), rect, WindowLayer.Super, delegate
				{
					Text.Font = GameFont.Small;
					Rect rect = rect.AtZero();
					if (this.TimeLeft < 0.6f)
					{
						float a = this.TimeLeft / 0.6f;
						GUI.color = new Color(1f, 1f, 1f, a);
					}
					else
					{
						GUI.color = Color.white;
					}
					if (this.lookTarget.IsValid)
					{
						TutorUIHighlighter.HighlightOpportunity("Messages", rect);
						Widgets.DrawHighlightIfMouseover(rect);
					}
					Widgets.Label(rect, this.text);
					if (Game.Mode == GameMode.MapPlaying && this.lookTarget.IsValid && Widgets.InvisibleButton(rect))
					{
						JumpToTargetUtility.TryJumpAndSelect(this.lookTarget);
						ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.ClickingMessages, KnowledgeAmount.Total);
					}
					GUI.color = Color.white;
				}, false, false, 0f);
			}
		}
		private const int MessageYInterval = 26;
		private const int MaxLiveMessages = 12;
		private static List<Messages.LiveMessage> liveMessages = new List<Messages.LiveMessage>();
		private static readonly Vector2 MessagesTopLeftStandard = new Vector2(140f, 16f);
		public static void Update()
		{
			Messages.liveMessages.RemoveAll((Messages.LiveMessage m) => m.Expired);
		}
		public static void Message(string text, TargetInfo lookTarget, MessageSound sound)
		{
			if (!Messages.AcceptsMessage(text, lookTarget))
			{
				return;
			}
			Messages.LiveMessage msg = new Messages.LiveMessage(text, lookTarget);
			Messages.Message(msg, sound);
		}
		public static void Message(string text, MessageSound sound)
		{
			if (!Messages.AcceptsMessage(text, TargetInfo.Invalid))
			{
				return;
			}
			Messages.LiveMessage msg = new Messages.LiveMessage(text);
			Messages.Message(msg, sound);
		}
		public static void MessagesDoGUI()
		{
			Text.Font = GameFont.Small;
			int xOffset = (int)Messages.MessagesTopLeftStandard.x;
			int num = (int)Messages.MessagesTopLeftStandard.y;
			for (int i = Messages.liveMessages.Count - 1; i >= 0; i--)
			{
				Messages.liveMessages[i].Draw(xOffset, num);
				num += 26;
			}
		}
		public static void Clear()
		{
			Messages.liveMessages.Clear();
		}
		public static void Notify_LoadedLevelChanged()
		{
			for (int i = 0; i < Messages.liveMessages.Count; i++)
			{
				Messages.liveMessages[i].lookTarget = TargetInfo.Invalid;
			}
		}
		private static bool AcceptsMessage(string text, TargetInfo lookTarget)
		{
			if (text.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < Messages.liveMessages.Count; i++)
			{
				if (Messages.liveMessages[i].text == text && Messages.liveMessages[i].lookTarget == lookTarget && Messages.liveMessages[i].startingFrame == Time.frameCount)
				{
					return false;
				}
			}
			return true;
		}
		private static void Message(Messages.LiveMessage msg, MessageSound sound)
		{
			Messages.liveMessages.Add(msg);
			while (Messages.liveMessages.Count > 12)
			{
				Messages.liveMessages.RemoveAt(0);
			}
			if (sound != MessageSound.Silent)
			{
				SoundDef soundDef = null;
				switch (sound)
				{
				case MessageSound.Standard:
					soundDef = SoundDefOf.MessageAlert;
					break;
				case MessageSound.RejectInput:
					soundDef = SoundDefOf.ClickReject;
					break;
				case MessageSound.Benefit:
					soundDef = SoundDefOf.MessageBenefit;
					break;
				case MessageSound.Negative:
					soundDef = SoundDefOf.MessageAlertNegative;
					break;
				case MessageSound.SeriousAlert:
					soundDef = SoundDefOf.MessageSeriousAlert;
					break;
				}
				soundDef.PlayOneShotOnCamera();
			}
		}
	}
}
