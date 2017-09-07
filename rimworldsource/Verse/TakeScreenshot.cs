using RimWorld;
using System;
using System.IO;
using UnityEngine;
namespace Verse
{
	public class TakeScreenshot : MonoBehaviour
	{
		private int screenshotCount;
		private int lastShotFrame = -999;
		private string lastShotFilePath;
		private void Update()
		{
			if (LongEventHandler.ShouldWaitForEvent)
			{
				return;
			}
			if (Time.frameCount == this.lastShotFrame + 1)
			{
				Messages.Message("Screenshot saved as " + this.lastShotFilePath, MessageSound.Standard);
			}
			if (KeyBindingDefOf.TakeScreenshot.JustPressed)
			{
				this.TakeShot();
			}
		}
		private void TakeShot()
		{
			string screenshotFolderPath = GenFilePaths.ScreenshotFolderPath;
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(screenshotFolderPath);
				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}
				string text;
				do
				{
					this.screenshotCount++;
					text = string.Concat(new object[]
					{
						screenshotFolderPath,
						Path.DirectorySeparatorChar,
						"screenshot",
						this.screenshotCount,
						".png"
					});
				}
				while (File.Exists(text));
				Application.CaptureScreenshot(text);
				this.lastShotFrame = Time.frameCount;
				this.lastShotFilePath = text;
			}
			catch (Exception ex)
			{
				Log.Error("Failed to save screenshot in " + screenshotFolderPath + "\nException follows:\n" + ex.ToString());
			}
		}
	}
}
