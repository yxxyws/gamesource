using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse
{
	public static class Log
	{
		private static LogMessageQueue messageQueue = new LogMessageQueue();
		private static HashSet<int> usedKeys = new HashSet<int>();
		public static IEnumerable<LogMessage> Messages
		{
			get
			{
				return Log.messageQueue.Messages;
			}
		}
		public static void Message(string text)
		{
			Debug.Log(text);
			Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Message, text, StackTraceUtility.ExtractStackTrace()));
		}
		public static void Warning(string text)
		{
			Debug.LogWarning(text);
			Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Warning, text, StackTraceUtility.ExtractStackTrace()));
		}
		public static void Error(string text)
		{
			if (Prefs.PauseOnError && Game.Mode == GameMode.MapPlaying)
			{
				Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
			}
			Debug.LogError(text);
			Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Error, text, StackTraceUtility.ExtractStackTrace()));
			if (Prefs.DevMode || Time.timeSinceLevelLoad < 1f)
			{
				Log.OpenLogWindow();
			}
		}
		public static void ErrorOnce(string text, int key)
		{
			if (Log.usedKeys.Contains(key))
			{
				return;
			}
			Log.usedKeys.Add(key);
			Log.Error(text);
		}
		public static void Notify_Exception(Exception e)
		{
			Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Error, e.Message, e.StackTrace));
			if (Time.timeSinceLevelLoad < 1f)
			{
				Log.OpenLogWindow();
			}
		}
		internal static void Clear()
		{
			Log.messageQueue.Clear();
			EditWindow_Log.ClearSelectedMessage();
		}
		public static void OpenLogWindow()
		{
			EditWindow_Log.TryAutoOpen();
		}
	}
}
