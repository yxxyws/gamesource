using System;
using System.Collections.Generic;
namespace Verse
{
	public static class Dijkstra<T>
	{
		private class DistanceComparer : IComparer<KeyValuePair<T, float>>
		{
			public int Compare(KeyValuePair<T, float> a, KeyValuePair<T, float> b)
			{
				return a.Value.CompareTo(b.Value);
			}
		}
		private static Dictionary<T, float> distances = new Dictionary<T, float>();
		private static FastPriorityQueue<KeyValuePair<T, float>> queue = new FastPriorityQueue<KeyValuePair<T, float>>(new Dijkstra<T>.DistanceComparer());
		public static void Run(IEnumerable<T> startingNodes, Func<T, IEnumerable<T>> neighborsGetter, Func<T, T, float> distanceGetter, ref Dictionary<T, float> outDistances)
		{
			outDistances.Clear();
			Dijkstra<T>.distances.Clear();
			Dijkstra<T>.queue.Clear();
			foreach (T current in startingNodes)
			{
				Dijkstra<T>.distances.Add(current, 0f);
				Dijkstra<T>.queue.Push(new KeyValuePair<T, float>(current, 0f));
			}
			while (Dijkstra<T>.queue.Count != 0)
			{
				KeyValuePair<T, float> keyValuePair = Dijkstra<T>.queue.Pop();
				float num = Dijkstra<T>.distances[keyValuePair.Key];
				if (keyValuePair.Value == num)
				{
					foreach (T current2 in neighborsGetter(keyValuePair.Key))
					{
						float num2 = num + distanceGetter(keyValuePair.Key, current2);
						float num3;
						if (Dijkstra<T>.distances.TryGetValue(current2, out num3))
						{
							if (num2 < num3)
							{
								Dijkstra<T>.distances[current2] = num2;
								Dijkstra<T>.queue.Push(new KeyValuePair<T, float>(current2, num2));
							}
						}
						else
						{
							Dijkstra<T>.distances.Add(current2, num2);
							Dijkstra<T>.queue.Push(new KeyValuePair<T, float>(current2, num2));
						}
					}
				}
			}
			foreach (KeyValuePair<T, float> current3 in Dijkstra<T>.distances)
			{
				outDistances.Add(current3.Key, current3.Value);
			}
			Dijkstra<T>.distances.Clear();
		}
	}
}
