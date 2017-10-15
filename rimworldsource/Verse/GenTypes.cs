using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
namespace Verse
{
	public static class GenTypes
	{
		public static readonly List<string> IgnoredNamespaceNames = new List<string>
		{
			"RimWorld",
			"Verse",
			"Verse.AI",
			"Verse.Sound",
			"Verse.Grammar",
			"RimWorld.Planet"
		};
		private static IEnumerable<Assembly> AllActiveAssemblies
		{
			get
			{
				GenTypes.<>c__Iterator1A8 <>c__Iterator1A = new GenTypes.<>c__Iterator1A8();
				GenTypes.<>c__Iterator1A8 expr_07 = <>c__Iterator1A;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public static IEnumerable<Type> AllTypes
		{
			get
			{
				GenTypes.<>c__Iterator1A9 <>c__Iterator1A = new GenTypes.<>c__Iterator1A9();
				GenTypes.<>c__Iterator1A9 expr_07 = <>c__Iterator1A;
				expr_07.$PC = -2;
				return expr_07;
			}
		}
		public static IEnumerable<Type> AllTypesWithAttribute<TAttr>() where TAttr : Attribute
		{
			return 
				from x in GenTypes.AllTypes
				where x.HasAttribute<TAttr>()
				select x;
		}
		public static IEnumerable<Type> AllSubclasses(this Type baseType)
		{
			return 
				from x in GenTypes.AllTypes
				where x.IsSubclassOf(baseType)
				select x;
		}
		public static IEnumerable<Type> AllLeafSubclasses(this Type baseType)
		{
			return 
				from type in baseType.AllSubclasses()
				where !type.AllSubclasses().Any<Type>()
				select type;
		}
		[DebuggerHidden]
		public static IEnumerable<Type> InstantiableDescendantsAndSelf(this Type baseType)
		{
			GenTypes.<InstantiableDescendantsAndSelf>c__Iterator1AA <InstantiableDescendantsAndSelf>c__Iterator1AA = new GenTypes.<InstantiableDescendantsAndSelf>c__Iterator1AA();
			<InstantiableDescendantsAndSelf>c__Iterator1AA.baseType = baseType;
			<InstantiableDescendantsAndSelf>c__Iterator1AA.<$>baseType = baseType;
			GenTypes.<InstantiableDescendantsAndSelf>c__Iterator1AA expr_15 = <InstantiableDescendantsAndSelf>c__Iterator1AA;
			expr_15.$PC = -2;
			return expr_15;
		}
		public static Type GetTypeInAnyAssembly(string typeName)
		{
			Type typeInAnyAssemblyRaw = GenTypes.GetTypeInAnyAssemblyRaw(typeName);
			if (typeInAnyAssemblyRaw != null)
			{
				return typeInAnyAssemblyRaw;
			}
			for (int i = 0; i < GenTypes.IgnoredNamespaceNames.Count; i++)
			{
				string typeName2 = GenTypes.IgnoredNamespaceNames[i] + "." + typeName;
				typeInAnyAssemblyRaw = GenTypes.GetTypeInAnyAssemblyRaw(typeName2);
				if (typeInAnyAssemblyRaw != null)
				{
					return typeInAnyAssemblyRaw;
				}
			}
			return null;
		}
		private static Type GetTypeInAnyAssemblyRaw(string typeName)
		{
			foreach (Assembly current in GenTypes.AllActiveAssemblies)
			{
				Type type = current.GetType(typeName, false, true);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}
		public static string GetTypeNameWithoutIgnoredNamespaces(Type type)
		{
			for (int i = 0; i < GenTypes.IgnoredNamespaceNames.Count; i++)
			{
				if (type.Namespace == GenTypes.IgnoredNamespaceNames[i])
				{
					return type.Name;
				}
			}
			return type.FullName;
		}
	}
}
