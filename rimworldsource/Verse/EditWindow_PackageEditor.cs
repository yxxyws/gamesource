using System;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public class EditWindow_PackageEditor<TNewDef> : EditWindow where TNewDef : Def, new()
	{
		private const float EditButSize = 24f;
		public LoadedMod curMod = LoadedModManager.LoadedMods.First<LoadedMod>();
		private DefPackage curPackage;
		private Vector2 scrollPosition = default(Vector2);
		private float viewHeight;
		private string relFolder;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(250f, 600f);
			}
		}
		public EditWindow_PackageEditor(string relFolder)
		{
			this.relFolder = relFolder;
			this.onlyOneOfTypeAllowed = true;
			this.optionalTitle = "Package Editor: " + relFolder;
		}
		public override void DoWindowContents(Rect selectorInner)
		{
			Text.Font = GameFont.Tiny;
			float width = (selectorInner.width - 4f) / 2f;
			Rect rect = new Rect(0f, 0f, width, 24f);
			string name = this.curMod.name;
			if (Widgets.TextButton(rect, "Editing: " + name, true, false))
			{
				Messages.Message("Mod changing not implemented - it's always Core for now.", MessageSound.RejectInput);
			}
			TooltipHandler.TipRegion(rect, "Change the mod being edited.");
			Rect rect2 = new Rect(rect.xMax + 4f, 0f, width, 24f);
			string label = "No package loaded";
			if (this.curPackage != null)
			{
				label = this.curPackage.fileName;
			}
			if (Widgets.TextButton(rect2, label, true, false))
			{
				Find.WindowStack.Add(new Dialog_PackageSelector(delegate(DefPackage pack)
				{
					if (pack != this.curPackage)
					{
						this.curPackage = pack;
					}
				}, this.curMod, this.relFolder));
			}
			TooltipHandler.TipRegion(rect2, "Open a Def package for editing.");
			WidgetRow widgetRow = new WidgetRow(0f, 28f, UIDirection.RightThenUp, 2000f, 29f);
			if (widgetRow.DoIconButton(TexButton.NewFile, "Create a new Def package."))
			{
				string name2 = DefPackage.UnusedPackageName(this.relFolder, this.curMod);
				DefPackage defPackage = new DefPackage(name2, this.relFolder);
				this.curMod.AddDefPackage(defPackage);
				this.curPackage = defPackage;
			}
			if (this.curPackage != null)
			{
				if (widgetRow.DoIconButton(TexButton.Save, "Save the current Def package."))
				{
					this.curPackage.SaveIn(this.curMod);
				}
				if (widgetRow.DoIconButton(TexButton.RenameDev, "Rename the current Def package."))
				{
					Find.WindowStack.Add(new Dialog_RenamePackage(this.curPackage));
				}
			}
			float num = 56f;
			Rect rect3 = new Rect(0f, num, selectorInner.width, selectorInner.height - num);
			Rect rect4 = new Rect(0f, 0f, rect3.width - 16f, this.viewHeight);
			Widgets.DrawMenuSection(rect3, true);
			Widgets.BeginScrollView(rect3, ref this.scrollPosition, rect4);
			Rect rect5 = rect4.ContractedBy(4f);
			rect5.height = 9999f;
			Listing_Standard listing_Standard = new Listing_Standard(rect5);
			Text.Font = GameFont.Tiny;
			if (this.curPackage == null)
			{
				listing_Standard.DoLabel("(no package open)");
			}
			else
			{
				if (this.curPackage.defs.Count == 0)
				{
					listing_Standard.DoLabel("(package is empty)");
				}
				else
				{
					EditWindow_PackageEditor<TNewDef>.<DoWindowContents>c__AnonStorey3B1 <DoWindowContents>c__AnonStorey3B = new EditWindow_PackageEditor<TNewDef>.<DoWindowContents>c__AnonStorey3B1();
					<DoWindowContents>c__AnonStorey3B.<>f__this = this;
					<DoWindowContents>c__AnonStorey3B.deletingDef = null;
					foreach (Def def in this.curPackage)
					{
						if (listing_Standard.DoSelectableDef(def.defName, false, delegate
						{
							<DoWindowContents>c__AnonStorey3B.deletingDef = def;
						}))
						{
							bool flag = false;
							WindowStack windowStack = Find.WindowStack;
							for (int i = 0; i < windowStack.Count; i++)
							{
								EditWindow_DefEditor editWindow_DefEditor = windowStack[i] as EditWindow_DefEditor;
								if (editWindow_DefEditor != null && editWindow_DefEditor.def == def)
								{
									flag = true;
								}
							}
							if (!flag)
							{
								Find.WindowStack.Add(new EditWindow_DefEditor(def));
							}
						}
					}
					if (<DoWindowContents>c__AnonStorey3B.deletingDef != null)
					{
						Find.WindowStack.Add(new Dialog_Confirm("Really delete Def " + <DoWindowContents>c__AnonStorey3B.deletingDef.defName + "?", delegate
						{
							<DoWindowContents>c__AnonStorey3B.<>f__this.curPackage.RemoveDef(<DoWindowContents>c__AnonStorey3B.deletingDef);
						}, true));
					}
				}
				if (listing_Standard.DoImageButton(TexButton.Add, 24f, 24f))
				{
					Def def2 = Activator.CreateInstance<TNewDef>();
					def2.defName = "New" + typeof(TNewDef).Name;
					this.curPackage.AddDef(def2);
				}
			}
			if (Event.current.type == EventType.Layout)
			{
				this.viewHeight = listing_Standard.CurHeight;
			}
			listing_Standard.End();
			Widgets.EndScrollView();
		}
	}
}
