using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class MainTabDef : Def
	{
		public const int TabButtonHeight = 35;
		public Type windowClass;
		public ConceptDef concept;
		public string tutorHighlightTag;
		public bool showTabButton = true;
		public int order;
		public KeyCode defaultToggleKey;
		[Unsaved]
		public KeyBindingDef toggleHotKey;
		[Unsaved]
		private MainTabWindow windowInt;
		public MainTabWindow Window
		{
			get
			{
				if (this.windowInt == null)
				{
					this.windowInt = (MainTabWindow)Activator.CreateInstance(this.windowClass);
					this.windowInt.def = this;
				}
				return this.windowInt;
			}
		}
	}
}
