using System;
using System.Collections.Generic;
namespace Verse
{
	internal class ReachabilityCache
	{
		private Dictionary<ulong, bool> cacheDict = new Dictionary<ulong, bool>();
		public int Count
		{
			get
			{
				return this.cacheDict.Count;
			}
		}
		public BoolUnknown CachedResultFor(Room A, Room B, TraverseParms traverseParams)
		{
			bool flag;
			if (this.cacheDict.TryGetValue(ReachabilityCache.UniqueHashFor(A, B, traverseParams), out flag))
			{
				return (!flag) ? BoolUnknown.False : BoolUnknown.True;
			}
			return BoolUnknown.Unknown;
		}
		public void AddCachedResult(Room A, Room B, TraverseParms traverseParams, bool reachable)
		{
			ulong key = ReachabilityCache.UniqueHashFor(A, B, traverseParams);
			if (!this.cacheDict.ContainsKey(key))
			{
				this.cacheDict.Add(key, reachable);
			}
		}
		public void Clear()
		{
			this.cacheDict.Clear();
		}
		private static ulong UniqueHashFor(Room A, Room B, TraverseParms traverseParams)
		{
			ulong num = (ulong)((traverseParams.pawn == null) ? 0L : ((long)traverseParams.pawn.thingIDNumber));
			if (A.ID < B.ID)
			{
				return (ulong)(((long)A.ID << 44) + ((long)B.ID << 24) + (long)((long)num << 3) + (long)((ulong)traverseParams.mode));
			}
			return (ulong)(((long)B.ID << 44) + ((long)A.ID << 24) + (long)((long)num << 3) + (long)((ulong)traverseParams.mode));
		}
	}
}
