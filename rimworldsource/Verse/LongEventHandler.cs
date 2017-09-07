using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
namespace Verse
{
	public static class LongEventHandler
	{
		private class QueuedLongEvent
		{
			public Action eventAction;
			public string levelToLoad;
			public string eventTextKey = string.Empty;
			public string eventText = string.Empty;
			public bool doAsynchronously;
			public Action<Exception> exceptionHandler;
			public bool alreadyDisplayed;
		}
		private static Queue<LongEventHandler.QueuedLongEvent> eventQueue = new Queue<LongEventHandler.QueuedLongEvent>();
		private static LongEventHandler.QueuedLongEvent currentEvent = null;
		private static Thread eventThread = null;
		private static AsyncOperation levelLoadOp = null;
		private static List<Action> toExecuteWhenFinished = new List<Action>();
		private static readonly object CurrentEventTextLock = new object();
		private static readonly Vector2 GUIRectSize = new Vector2(240f, 75f);
		public static bool ShouldWaitForEvent
		{
			get
			{
				return LongEventHandler.AnyEventNowOrWaiting && ((LongEventHandler.currentEvent != null && LongEventHandler.currentEvent.doAsynchronously) || (LongEventHandler.currentEvent == null && LongEventHandler.eventQueue.Any<LongEventHandler.QueuedLongEvent>() && LongEventHandler.eventQueue.Peek().doAsynchronously) || (Find.UIRoot == null || Find.WindowStack == null));
			}
		}
		public static bool AnyEventNowOrWaiting
		{
			get
			{
				return LongEventHandler.currentEvent != null || LongEventHandler.eventQueue.Any<LongEventHandler.QueuedLongEvent>();
			}
		}
		public static void QueueLongEvent(Action action, string textKey, bool doAsynchronously, Action<Exception> exceptionHandler)
		{
			LongEventHandler.QueuedLongEvent queuedLongEvent = new LongEventHandler.QueuedLongEvent();
			queuedLongEvent.eventAction = action;
			queuedLongEvent.eventTextKey = textKey;
			queuedLongEvent.doAsynchronously = doAsynchronously;
			queuedLongEvent.exceptionHandler = exceptionHandler;
			LongEventHandler.eventQueue.Enqueue(queuedLongEvent);
		}
		public static void QueueLongEvent(Action preLoadLevelAction, string levelToLoad, string textKey, bool doAsynchronously, Action<Exception> exceptionHandler)
		{
			LongEventHandler.QueuedLongEvent queuedLongEvent = new LongEventHandler.QueuedLongEvent();
			queuedLongEvent.eventAction = preLoadLevelAction;
			queuedLongEvent.levelToLoad = levelToLoad;
			queuedLongEvent.eventTextKey = textKey;
			queuedLongEvent.doAsynchronously = doAsynchronously;
			queuedLongEvent.exceptionHandler = exceptionHandler;
			LongEventHandler.eventQueue.Enqueue(queuedLongEvent);
		}
		public static void ClearQueuedEvents()
		{
			LongEventHandler.eventQueue.Clear();
		}
		public static void LongEventsOnGUI()
		{
			if (LongEventHandler.currentEvent == null)
			{
				return;
			}
			if (Event.current.type == EventType.Repaint)
			{
				LongEventHandler.currentEvent.alreadyDisplayed = true;
			}
			Rect rect = new Rect(((float)Screen.width - LongEventHandler.GUIRectSize.x) / 2f, ((float)Screen.height - LongEventHandler.GUIRectSize.y) / 2f, LongEventHandler.GUIRectSize.x, LongEventHandler.GUIRectSize.y);
			if (LongEventHandler.currentEvent.doAsynchronously || Find.UIRoot == null || Find.WindowStack == null)
			{
				if (UIMenuBackgroundManager.background == null)
				{
					UIMenuBackgroundManager.background = new UI_BackgroundMain();
				}
				UIMenuBackgroundManager.background.BackgroundOnGUI();
				Widgets.DrawShadowAround(rect);
				Widgets.DrawWindowBackground(rect);
				LongEventHandler.DrawLongEventWindowContents(rect);
			}
			else
			{
				Find.WindowStack.ImmediateWindow(62893994, rect, WindowLayer.Super, delegate
				{
					LongEventHandler.DrawLongEventWindowContents(rect.AtZero());
				}, true, false, 1f);
			}
		}
		public static void LongEventsUpdate()
		{
			if (LongEventHandler.currentEvent != null)
			{
				if (LongEventHandler.currentEvent.doAsynchronously)
				{
					LongEventHandler.UpdateCurrentAsynchronousEvent();
				}
				else
				{
					LongEventHandler.UpdateCurrentSynchronousEvent();
				}
			}
			if (LongEventHandler.currentEvent == null && LongEventHandler.eventQueue.Count > 0)
			{
				LongEventHandler.currentEvent = LongEventHandler.eventQueue.Dequeue();
				if (LongEventHandler.currentEvent.eventTextKey == null)
				{
					LongEventHandler.currentEvent.eventText = string.Empty;
				}
				else
				{
					LongEventHandler.currentEvent.eventText = LongEventHandler.currentEvent.eventTextKey.Translate();
				}
			}
		}
		public static void ExecuteWhenFinished(Action action)
		{
			if (LongEventHandler.currentEvent == null)
			{
				action();
			}
			else
			{
				LongEventHandler.toExecuteWhenFinished.Add(action);
			}
		}
		public static void SetCurrentEventText(string newText)
		{
			object currentEventTextLock = LongEventHandler.CurrentEventTextLock;
			Monitor.Enter(currentEventTextLock);
			try
			{
				if (LongEventHandler.currentEvent != null)
				{
					LongEventHandler.currentEvent.eventText = newText;
				}
			}
			finally
			{
				Monitor.Exit(currentEventTextLock);
			}
		}
		private static void UpdateCurrentAsynchronousEvent()
		{
			if (LongEventHandler.eventThread == null)
			{
				LongEventHandler.eventThread = new Thread(delegate
				{
					LongEventHandler.RunEventFromAnotherThread(LongEventHandler.currentEvent.eventAction);
				});
				LongEventHandler.eventThread.Start();
			}
			else
			{
				if (!LongEventHandler.eventThread.IsAlive)
				{
					bool flag = false;
					if (!LongEventHandler.currentEvent.levelToLoad.NullOrEmpty())
					{
						if (LongEventHandler.levelLoadOp == null)
						{
							LongEventHandler.levelLoadOp = Application.LoadLevelAsync(LongEventHandler.currentEvent.levelToLoad);
						}
						else
						{
							if (LongEventHandler.levelLoadOp.isDone)
							{
								flag = true;
							}
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						LongEventHandler.currentEvent = null;
						LongEventHandler.eventThread = null;
						LongEventHandler.levelLoadOp = null;
						LongEventHandler.ExecuteToExecuteWhenFinished();
					}
				}
			}
		}
		private static void UpdateCurrentSynchronousEvent()
		{
			if (!LongEventHandler.currentEvent.alreadyDisplayed)
			{
				return;
			}
			try
			{
				if (LongEventHandler.currentEvent.eventAction != null)
				{
					LongEventHandler.currentEvent.eventAction();
				}
				if (!LongEventHandler.currentEvent.levelToLoad.NullOrEmpty())
				{
					Application.LoadLevel(LongEventHandler.currentEvent.levelToLoad);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Exception from long event: " + ex);
				if (LongEventHandler.currentEvent.exceptionHandler != null)
				{
					LongEventHandler.currentEvent.exceptionHandler(ex);
				}
			}
			finally
			{
				LongEventHandler.currentEvent = null;
				LongEventHandler.eventThread = null;
				LongEventHandler.levelLoadOp = null;
				LongEventHandler.ExecuteToExecuteWhenFinished();
			}
		}
		private static void RunEventFromAnotherThread(Action action)
		{
			try
			{
				if (action != null)
				{
					action();
				}
			}
			catch (Exception ex)
			{
				Log.Error("Exception from asynchronous event: " + ex);
				try
				{
					if (LongEventHandler.currentEvent.exceptionHandler != null)
					{
						LongEventHandler.currentEvent.exceptionHandler(ex);
					}
				}
				catch (Exception arg)
				{
					Log.Error("Exception was thrown while trying to handle exception. Exception: " + arg);
				}
			}
		}
		private static void ExecuteToExecuteWhenFinished()
		{
			for (int i = 0; i < LongEventHandler.toExecuteWhenFinished.Count; i++)
			{
				try
				{
					LongEventHandler.toExecuteWhenFinished[i]();
				}
				catch (Exception arg)
				{
					Log.Error("Could not execute post-long-event action. Exception: " + arg);
				}
			}
			LongEventHandler.toExecuteWhenFinished.Clear();
		}
		private static void DrawLongEventWindowContents(Rect rect)
		{
			if (LongEventHandler.currentEvent == null)
			{
				return;
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			float num = 0f;
			if (LongEventHandler.levelLoadOp != null)
			{
				float f = 1f;
				if (!LongEventHandler.levelLoadOp.isDone)
				{
					f = LongEventHandler.levelLoadOp.progress;
				}
				string text = "LoadingAssets".Translate() + " " + f.ToStringPercent();
				num = Text.CalcSize(text).x;
				Widgets.Label(rect, text);
			}
			else
			{
				object currentEventTextLock = LongEventHandler.CurrentEventTextLock;
				Monitor.Enter(currentEventTextLock);
				try
				{
					num = Text.CalcSize(LongEventHandler.currentEvent.eventText).x;
					Widgets.Label(rect, LongEventHandler.currentEvent.eventText);
				}
				finally
				{
					Monitor.Exit(currentEventTextLock);
				}
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			rect.xMin = rect.center.x + num / 2f;
			Widgets.Label(rect, LongEventHandler.GetAnimatedDots());
			Text.Anchor = TextAnchor.UpperLeft;
		}
		private static string GetAnimatedDots()
		{
			if (!LongEventHandler.currentEvent.doAsynchronously)
			{
				return "...";
			}
			int num = (int)(Time.realtimeSinceStartup * 2f) % 4;
			string text = string.Empty;
			for (int i = 0; i < num; i++)
			{
				text += ".";
			}
			return text;
		}
		public static int Debug_GetToExecuteWhenFinishedActionsCount()
		{
			return LongEventHandler.toExecuteWhenFinished.Count;
		}
	}
}
