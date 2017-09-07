using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public static class DefDatabase<T> where T : Def, new()
	{
		private static List<T> defsList = new List<T>();
		private static Dictionary<string, T> defsByName = new Dictionary<string, T>();
		public static IEnumerable<T> AllDefs
		{
			get
			{
				return DefDatabase<T>.defsList;
			}
		}
		public static List<T> AllDefsListForReading
		{
			get
			{
				return DefDatabase<T>.defsList;
			}
		}
		public static int DefCount
		{
			get
			{
				return DefDatabase<T>.defsList.Count;
			}
		}
		public static void AddAllInMods()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (LoadedMod current in 
				from m in LoadedModManager.LoadedMods
				orderby m.OverwritePriority
				select m)
			{
				hashSet.Clear();
				foreach (T current2 in current.AllDefs.OfType<T>())
				{
					if (hashSet.Contains(current2.defName))
					{
						Log.Error(string.Concat(new object[]
						{
							"Mod ",
							current,
							" has multiple ",
							typeof(T),
							"s named ",
							current2.defName,
							". Skipping."
						}));
					}
					else
					{
						if (current2.defName == "UnnamedDef")
						{
							string text = "Unnamed" + typeof(T).Name + Rand.Range(1, 100000).ToString() + "A";
							Log.Error(string.Concat(new string[]
							{
								typeof(T).Name,
								" in ",
								current.name,
								" with label ",
								current2.label,
								" lacks a defName. Giving name ",
								text
							}));
							current2.defName = text;
						}
						T def;
						if (DefDatabase<T>.defsByName.TryGetValue(current2.defName, out def))
						{
							DefDatabase<T>.Remove(def);
						}
						DefDatabase<T>.Add(current2);
					}
				}
			}
		}
		public static void Add(IEnumerable<T> defs)
		{
			foreach (T current in defs)
			{
				DefDatabase<T>.Add(current);
			}
		}
		public static void Add(T def)
		{
			while (DefDatabase<T>.defsByName.ContainsKey(def.defName))
			{
				Log.Error(string.Concat(new object[]
				{
					"Adding duplicate ",
					typeof(T),
					" name: ",
					def.defName
				}));
				T expr_46 = def;
				expr_46.defName += Mathf.RoundToInt(Rand.Value * 1000f);
			}
			DefDatabase<T>.defsList.Add(def);
			DefDatabase<T>.defsByName.Add(def.defName, def);
		}
		private static void Remove(T def)
		{
			DefDatabase<T>.defsByName.Remove(def.defName);
			DefDatabase<T>.defsList.Remove(def);
		}
		public static void Clear()
		{
			DefDatabase<T>.defsList.Clear();
			DefDatabase<T>.defsByName.Clear();
		}
		public static void ClearCachedData()
		{
			for (int i = 0; i < DefDatabase<T>.defsList.Count; i++)
			{
				T t = DefDatabase<T>.defsList[i];
				t.ClearCachedData();
			}
		}
		public static void ResolveAllReferences()
		{
			for (int i = 0; i < DefDatabase<T>.defsList.Count; i++)
			{
				DefDatabase<T>.defsList[i].index = (ushort)i;
			}
			for (int j = 0; j < DefDatabase<T>.defsList.Count; j++)
			{
				T t = DefDatabase<T>.defsList[j];
				t.ResolveReferences();
			}
		}
		public static void ErrorCheckAllDefs()
		{
			foreach (T current in DefDatabase<T>.AllDefs)
			{
				foreach (string current2 in current.ConfigErrors())
				{
					Log.Warning(string.Concat(new object[]
					{
						"Config error in ",
						current,
						": ",
						current2
					}));
				}
			}
		}
		public static T GetNamed(string defName, bool errorOnFail = true)
		{
			if (errorOnFail)
			{
				T result;
				if (DefDatabase<T>.defsByName.TryGetValue(defName, out result))
				{
					return result;
				}
				Log.Error(string.Concat(new object[]
				{
					"Failed to find ",
					typeof(T),
					" named ",
					defName,
					". There are ",
					DefDatabase<T>.defsList.Count,
					" defs loaded."
				}));
				return (T)((object)null);
			}
			else
			{
				T result2;
				if (DefDatabase<T>.defsByName.TryGetValue(defName, out result2))
				{
					return result2;
				}
				return (T)((object)null);
			}
		}
		public static T GetNamedSilentFail(string defName)
		{
			return DefDatabase<T>.GetNamed(defName, false);
		}
		public static T GetByShortHash(ushort shortHash)
		{
			for (int i = 0; i < DefDatabase<T>.defsList.Count; i++)
			{
				if (DefDatabase<T>.defsList[i].shortHash == shortHash)
				{
					return DefDatabase<T>.defsList[i];
				}
			}
			return (T)((object)null);
		}
		public static T GetRandom()
		{
			return DefDatabase<T>.defsList.RandomElement<T>();
		}
	}
}
