using System;
using System.Reflection;
using Verse;
namespace RimWorld
{
	public static class DefOfHelper
	{
		private static bool earlyTry = true;
		public static void RebindAllDefOfs(bool earlyTryMode)
		{
			DefOfHelper.earlyTry = earlyTryMode;
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				MethodInfo method = type.GetMethod("RebindDefs");
				if (method != null)
				{
					method.Invoke(null, null);
				}
			}
		}
		public static void BindDefsFor<T>(Type type) where T : Def, new()
		{
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (typeof(T) == typeof(SoundDef))
				{
					if (!DefOfHelper.earlyTry)
					{
						fieldInfo.SetValue(null, SoundDef.Named(fieldInfo.Name));
					}
				}
				else
				{
					fieldInfo.SetValue(null, DefDatabase<T>.GetNamed(fieldInfo.Name, !DefOfHelper.earlyTry));
				}
			}
		}
	}
}
