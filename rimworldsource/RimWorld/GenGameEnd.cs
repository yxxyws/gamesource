using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public static class GenGameEnd
	{
		public static void EndGameDialogMessage(string msg)
		{
			DiaNode diaNode = new DiaNode(msg);
			DiaOption diaOption = new DiaOption("GameOverKeepPlaying".Translate());
			diaOption.resolveTree = true;
			diaNode.options.Add(diaOption);
			DiaOption diaOption2 = new DiaOption("GameOverMainMenu".Translate());
			diaOption2.action = delegate
			{
				Application.LoadLevel("Entry");
			};
			diaOption2.resolveTree = true;
			diaNode.options.Add(diaOption2);
			if (HistoryUpload.CanUploadWithNoDialog())
			{
				HistoryUpload.SaveAndUpload();
			}
			else
			{
				diaOption.action = delegate
				{
					HistoryUpload.TrySaveAndUpload();
				};
			}
			Find.WindowStack.Add(new Dialog_NodeTree(diaNode));
		}
	}
}
