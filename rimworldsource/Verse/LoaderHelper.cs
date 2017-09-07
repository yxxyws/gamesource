using System;
namespace Verse
{
	public static class LoaderHelper
	{
		public static Def GetDef(Type type, string targetDefName)
		{
			if (type == typeof(SoundDef))
			{
				return SoundDef.Named(targetDefName);
			}
			return (Def)GenGeneric.InvokeStaticMethodOnGenericType(typeof(DefDatabase<>), type, "GetNamedSilentFail", new object[]
			{
				targetDefName
			});
		}
		public static T TryResolveDef<T>(string defName, FailMode failReportMode)
		{
			T t = (T)((object)LoaderHelper.GetDef(typeof(T), defName));
			if (t != null)
			{
				return t;
			}
			if (failReportMode == FailMode.LogErrors)
			{
				Log.Error(string.Concat(new object[]
				{
					"Could not resolve cross-reference to ",
					typeof(T),
					" named ",
					defName
				}));
			}
			return default(T);
		}
	}
}
