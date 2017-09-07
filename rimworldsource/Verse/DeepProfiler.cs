using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace Verse
{
	public static class DeepProfiler
	{
		private class Watcher
		{
			private string label;
			private Stopwatch watch;
			private List<DeepProfiler.Watcher> children;
			public string Label
			{
				get
				{
					return this.label;
				}
			}
			public Stopwatch Watch
			{
				get
				{
					return this.watch;
				}
			}
			public List<DeepProfiler.Watcher> Children
			{
				get
				{
					return this.children;
				}
			}
			public Watcher(string label)
			{
				this.label = label;
				this.watch = Stopwatch.StartNew();
				this.children = null;
			}
			public void AddChildResult(DeepProfiler.Watcher w)
			{
				if (this.children == null)
				{
					this.children = new List<DeepProfiler.Watcher>();
				}
				this.children.Add(w);
			}
		}
		private const int MaxDepth = 50;
		private static Stack<DeepProfiler.Watcher> watchers;
		private static readonly string[] Prefixes;
		static DeepProfiler()
		{
			DeepProfiler.watchers = new Stack<DeepProfiler.Watcher>();
			DeepProfiler.Prefixes = new string[50];
			for (int i = 0; i < 50; i++)
			{
				DeepProfiler.Prefixes[i] = string.Empty;
				for (int j = 0; j < i; j++)
				{
					string[] expr_3B_cp_0 = DeepProfiler.Prefixes;
					int expr_3B_cp_1 = i;
					expr_3B_cp_0[expr_3B_cp_1] += " -";
				}
			}
		}
		public static void Start(string label = null)
		{
			if (!Prefs.LogVerbose)
			{
				return;
			}
			DeepProfiler.watchers.Push(new DeepProfiler.Watcher(label));
		}
		public static void End()
		{
			if (!Prefs.LogVerbose)
			{
				return;
			}
			if (DeepProfiler.watchers.Count == 0)
			{
				Log.Error("Ended deep profiling while not profiling.");
				return;
			}
			DeepProfiler.Watcher watcher = DeepProfiler.watchers.Pop();
			watcher.Watch.Stop();
			if (DeepProfiler.watchers.Count > 0)
			{
				DeepProfiler.watchers.Peek().AddChildResult(watcher);
			}
			else
			{
				DeepProfiler.Output(watcher);
			}
		}
		private static void Output(DeepProfiler.Watcher root)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DeepProfiler.AppendStringRecursive(stringBuilder, root, 0);
			Log.Message(stringBuilder.ToString());
		}
		private static void AppendStringRecursive(StringBuilder sb, DeepProfiler.Watcher w, int depth)
		{
			sb.AppendLine(string.Concat(new object[]
			{
				DeepProfiler.Prefixes[depth],
				" ",
				w.Watch.ElapsedMilliseconds,
				"ms ",
				w.Label
			}));
			if (w.Children != null)
			{
				for (int i = 0; i < w.Children.Count; i++)
				{
					DeepProfiler.AppendStringRecursive(sb, w.Children[i], depth + 1);
				}
			}
		}
	}
}
