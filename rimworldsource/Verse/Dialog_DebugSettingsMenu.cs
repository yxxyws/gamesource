using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace Verse
{
	public class Dialog_DebugSettingsMenu : Dialog_DebugOptionLister
	{
		public Dialog_DebugSettingsMenu()
		{
			this.forcePause = true;
		}
		protected override void DoListingItems()
		{
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.RightBracket)
			{
				Event.current.Use();
				this.Close(true);
			}
			foreach (FieldInfo current in typeof(DebugSettings).GetFields().Concat(typeof(DebugViewSettings).GetFields()))
			{
				if (!current.IsLiteral)
				{
					string label = GenText.SplitCamelCase(current.Name).CapitalizeFirst();
					bool flag = (bool)current.GetValue(null);
					bool flag2 = flag;
					base.DrawDebugLabelCheckbox(label, ref flag);
					if (flag != flag2)
					{
						current.SetValue(null, flag);
					}
				}
			}
		}
	}
}
