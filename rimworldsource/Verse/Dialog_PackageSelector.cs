using System;
using System.IO;
using UnityEngine;
namespace Verse
{
	public class Dialog_PackageSelector : Window
	{
		private Action<DefPackage> setPackageCallback;
		private LoadedMod mod;
		private string relFolder;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(1000f, 700f);
			}
		}
		public Dialog_PackageSelector(Action<DefPackage> setPackageCallback, LoadedMod mod, string relFolder)
		{
			this.setPackageCallback = setPackageCallback;
			this.mod = mod;
			this.relFolder = relFolder;
			this.doCloseX = true;
			this.closeOnEscapeKey = true;
			this.onlyOneOfTypeAllowed = true;
			this.draggable = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			Listing_Standard listing_Standard = new Listing_Standard(inRect.AtZero());
			listing_Standard.OverrideColumnWidth = 240f;
			foreach (DefPackage current in this.mod.GetDefPackagesInFolder(this.relFolder))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(current.fileName);
				if (listing_Standard.DoTextButton(fileNameWithoutExtension))
				{
					this.setPackageCallback(current);
					this.Close(true);
				}
			}
			listing_Standard.End();
		}
	}
}
