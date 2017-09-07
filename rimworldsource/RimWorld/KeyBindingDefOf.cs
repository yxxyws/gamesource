using System;
using Verse;
namespace RimWorld
{
	public static class KeyBindingDefOf
	{
		public static KeyBindingDef MapDollyUp;
		public static KeyBindingDef MapDollyDown;
		public static KeyBindingDef MapDollyLeft;
		public static KeyBindingDef MapDollyRight;
		public static KeyBindingDef MapZoomIn;
		public static KeyBindingDef MapZoomOut;
		public static KeyBindingDef ToggleScreenshotMode;
		public static KeyBindingDef TakeScreenshot;
		public static KeyBindingDef SelectNextInCell;
		public static KeyBindingDef TogglePause;
		public static KeyBindingDef TimeSpeedNormal;
		public static KeyBindingDef TimeSpeedFast;
		public static KeyBindingDef TimeSpeedSuperfast;
		public static KeyBindingDef TimeSpeedUltrafast;
		public static KeyBindingDef PreviousColonist;
		public static KeyBindingDef NextColonist;
		public static KeyBindingDef Misc1;
		public static KeyBindingDef Misc2;
		public static KeyBindingDef Misc3;
		public static KeyBindingDef Misc4;
		public static KeyBindingDef Misc5;
		public static KeyBindingDef Misc6;
		public static KeyBindingDef Misc7;
		public static KeyBindingDef Misc8;
		public static KeyBindingDef Misc9;
		public static KeyBindingDef Misc10;
		public static KeyBindingDef Misc11;
		public static KeyBindingDef Misc12;
		public static KeyBindingDef CommandTogglePower;
		public static KeyBindingDef CommandItemForbid;
		public static KeyBindingDef CommandColonistDraft;
		public static KeyBindingDef DesignatorCancel;
		public static KeyBindingDef DesignatorDeconstruct;
		public static KeyBindingDef DesignatorRotateLeft;
		public static KeyBindingDef DesignatorRotateRight;
		public static KeyBindingDef ToggleLog;
		public static KeyBindingDef DebugTickOnce;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<KeyBindingDef>(typeof(KeyBindingDefOf));
		}
	}
}
