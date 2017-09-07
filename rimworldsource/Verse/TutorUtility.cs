using System;
namespace Verse
{
	public static class TutorUtility
	{
		public static void DoModalDialogIfNotKnown(ConceptDef conc)
		{
			if (!ConceptDatabase.IsComplete(conc))
			{
				string helpText = conc.GetHelpText(0);
				Find.WindowStack.Add(Dialog_NodeTree.SimpleNotifyDialog(helpText));
				ConceptDatabase.KnowledgeDemonstrated(conc, KnowledgeAmount.Total);
			}
		}
	}
}
